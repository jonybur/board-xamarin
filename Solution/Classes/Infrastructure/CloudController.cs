using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure;
using UIKit;

using Newtonsoft.Json.Linq;

namespace Solution
{
	public class CloudController : DelegatingHandler
	{
		// BoardClient is used for interaction with the Azure DB (cannot store FBUser in the DB)
		public static User BoardUser;
		// User is the Azure user client that works for authentication using Facebook credentials
		public MobileServiceUser AzureUser { get { return azureUser; } }

		private MobileServiceUser azureUser;
		static CloudController instance = new CloudController ();
		const string applicationURL = @"https://board.azure-mobile.net/";
		const string applicationKey = @"AtVndLIPKSjlNTSpJbteAtkPaeYnNZ70";
		public MobileServiceClient msclient;
		int busyCount = 0;
		public event Action<bool> BusyUpdate;

		IMobileServiceTable<User> userTable;
		IMobileServiceTable<Picture> pictureTable;
		IMobileServiceTable<Like> likeTable;
		IMobileServiceTable<TextBox> textboxTable;
		IMobileServiceTable<Message> messageTable;

		public List<User> ListUsers { get; private set; }

		CloudController ()
		{
			CurrentPlatform.Init ();

			// Initialize the Mobile Service client with your URL and key
			msclient = new MobileServiceClient (applicationURL, applicationKey, this);
			// Creates MSTable instances to allow us to work with the tables
			userTable = msclient.GetTable <User> ();
			pictureTable = msclient.GetTable <Picture> ();
			likeTable = msclient.GetTable<Like> ();
			textboxTable = msclient.GetTable<TextBox> ();
			messageTable = msclient.GetTable<Message> ();
		}

		public static CloudController DefaultService {
			get {
				return instance;
			}
		}

		// TODO: REFRESHXDATAASYNC populates the local db with ALL the new content on ONE board
		//		 ALL the REFRESHXDATAASYNC will have to change to work with multiple boards

		// Populates the storagecontroller with new pictures
		async public Task RefreshPictureDataAsync ()
		{
			try {
				// This code checks if there are any new pictures to download and proceeds to do that
				IMobileServiceTableQuery<string> query = pictureTable.Select(picture => picture.Id);

				// compares two ID lists, local and cloud stored
				List<string> localItems = StorageController.ReturnAllPictureIDs(false);
				List<string> cloudItems = await query.ToListAsync();

				// only checks on items that are not on gallery (on current board)
				foreach (string item in localItems)
				{
					// there's an ID that's not in the cloud storage (it has been deleted)
					if (!cloudItems.Contains(item))
					{
						// sets bool "ongallery" value to true
						StorageController.SendPictureToGallery(item);
					}
				}

				foreach (string item in cloudItems)
				{
					// there's an ID that's not in the local storage
					if (!localItems.Contains(item))
					{
						// a new picture is found, download it and store it
						Picture pic = await pictureTable.LookupAsync (item);
						StorageController.InsertPicture(pic);
					}
				}

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
			}
		}

		// will download new (and not previosuly downloaded) messages for the content in the list
		async public Task RefreshMessageDataAsync ()
		{
			try {
				// get the messages id's that i do have
				List<string> lstContentIDs = StorageController.ReturnAllPictureIDs(false);
				lstContentIDs.AddRange(StorageController.ReturnAllTextBoxIDs(false));
				List<Message> cloudItems;

				foreach (string pID in lstContentIDs)
				{
					DateTimeOffset lastMessage = StorageController.GetLastMessage(pID);

					// get messages that ...

					// condition 1: they belong to this particular picture (contentID == picture.ID)
					// TODO: condition 2: they are newer than the last message that I saved
				
					IMobileServiceTableQuery<Message> query = messageTable.Where(msg => (msg.ContentId == pID) && (lastMessage < msg.CreatedAt));
							
					// run the query
					cloudItems = await query.ToListAsync();

					StorageController.InsertMessages(cloudItems);
				}


			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
			}
		}

		async public Task RefreshTextBoxDataAsync ()
		{
			try {
				// This code checks if there are any new pictures to download and proceeds to do that
				IMobileServiceTableQuery<string> query = textboxTable.Select(textbox => textbox.Id);

				List<string> localItems = StorageController.ReturnAllTextBoxIDs(true);
				List<string> cloudItems = await query.ToListAsync();

				// only checks on items that are not on gallery (on current board)
			
				foreach (string item in localItems)
				{
					// there's an ID that's not in the cloud storage (it has been deleted)
					if (!cloudItems.Contains(item))
					{
						// sets bool "ongallery" value to true
						StorageController.SendPictureToGallery(item);
					}
				}

				foreach (string item in cloudItems)
				{
					// there's an ID that's not in the local storage
					if (!localItems.Contains(item))
					{
						// a new picture is found, download it and store it
						TextBox tex = await textboxTable.LookupAsync (item);
						StorageController.InsertTextBox(tex);
					}
				}

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
			}
		}


		async public Task RefreshLikeDataAsync()
		{
			try {
				IEnumerable<Like> listLikes = await likeTable.ReadAsync();

				StorageController.UpdateLikeTable(listLikes);

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
			}
		}

		async public Task<User> LookupUser(string id)
		{
			User user;
			try{
				user = await userTable.LookupAsync(id);
			}
			catch{
				user = new User();
			}
			return user;
		}

		async public Task InsertUserAsync (User user)
		{
			try {
				// This code inserts a new Client into the database. When the operation completes
				// and Mobile Services has assigned an Id, the item is added to the CollectionView
				try {
					await userTable.LookupAsync(user.Id);
				} catch {
					// LookupAsync returns an error if it doesn't find the Client
					// the user is not registered to the DB, register him
					await userTable.InsertAsync (user);
				}
				BoardUser = user;
			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
			}
		}


		public async Task InsertMessageAsync (Message message)
		{
			try {
				// stores the message on the cloud
				await messageTable.InsertAsync (message);
				// stores the message on the local filesystem
				StorageController.InsertMessage(message);

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
			}
		}

		public async Task InsertTextBoxAsync (TextBox textbox)
		{
			try {
				// stores the textbox on the cloud
				await textboxTable.InsertAsync (textbox);
				// stores the textbox on the local filesystem
				StorageController.InsertTextBox(textbox);

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
			}
		}

		public bool IsLoggedIn()
		{
			return (AzureUser != null && BoardUser != null);
		}

		public async Task<Like> InsertLikeAsync(string contentID)
		{
			try {
				Like like = new Like(BoardUser.Id, contentID);
				await likeTable.InsertAsync(like);
				await RefreshLikeDataAsync();
				return like;
			}
			catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
				return new Like ();
			}
		}

		public async Task<Like> RemoveLikeAsync(Like like)
		{
			try{
				await likeTable.DeleteAsync (like);
				await RefreshLikeDataAsync();
			} catch (MobileServiceODataException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
			}
			return like;
		}

		public async Task InsertPictureAsync (Picture picture)
		{
			try {
				// stores the picture in the cloud
				await pictureTable.InsertAsync (picture);
				// stores the picture in the local filesystem
				StorageController.InsertPicture(picture);
			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
			}
		}

		public async Task RemovePictureAsync(Picture picture)
		{
			try{
				await pictureTable.DeleteAsync (picture);
			} catch (MobileServiceODataException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
			}
		}

		public async Task RemoveTextBoxAsync(TextBox textbox)
		{
			try{
				await textboxTable.DeleteAsync (textbox);
				Console.WriteLine(textbox.Id);
			} catch (MobileServiceODataException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
			}
		}

		void Busy (bool busy)
		{
			// assumes always executes on UI thread
			if (busy) {
				if (busyCount++ == 0 && BusyUpdate != null)
					BusyUpdate (true);

			} else {
				if (--busyCount == 0 && BusyUpdate != null)
					BusyUpdate (false);

			}
		}

		// Facebook Authentication
		public async Task Authenticate(UIViewController view)
		{
			try
			{
				/*JObject token = JObject.FromObject(new
					{
						access_token = FBUser.GetId()
					});
				user = await msclient.LoginAsync("facebook", token);

				FBAccessTokenData.CreateToken(??);
				*/

				// TODO: find a way to msclient.LoginAsync using a FB token from the FB login
				azureUser = await msclient.LoginAsync(view, MobileServiceAuthenticationProvider.Facebook);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine (@"ERROR - AUTHENTICATION FAILED {0}", ex.Message);
			}
		}

		// Facebook Authentication
		public void Deauthenticate()
		{
			try
			{
				azureUser = null;
				msclient.Logout();
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine (@"ERROR - DEAUTHENTICATION FAILED {0}", ex.Message);
			}
		}

		#region implemented abstract members of HttpMessageHandler

		protected override async Task<System.Net.Http.HttpResponseMessage> SendAsync (System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
		{
			Busy (true);
			var response = await base.SendAsync (request, cancellationToken);

			Busy (false);
			return response;
		}

		#endregion
	}
}