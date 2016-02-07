using System;
using System.IO;
using SQLite;

using Foundation;
using UIKit;

using CoreGraphics;
using CoreAnimation;
using CoreText;

using System.Net.Http;

using System.Threading.Tasks;
using System.Threading;

using System.Collections.Generic;


namespace Solution
{
	public static class StorageController
	{
		[Table("Messages")]
		public class MessageL {
			[PrimaryKey, Column("id"), MaxLength(128)]
			public string Id { get; set; }
			[MaxLength(128)]
			public string UserID { get; set; }
			[Column("contentId")]
			public string ContentID { get; set; }
			public string Text { get; set; }
			[Column("createdAt")]
			public DateTimeOffset CreatedAt { get; set; }

			public MessageL(Message m)
			{
				Id = m.Id;
				UserID = m.UserId;
				ContentID = m.ContentId;
				Text = m.Text;
				CreatedAt = m.CreatedAt;
			}

			public MessageL(){}
		}

		[Table("Pictures")]
		public class PictureL {
			[PrimaryKey, MaxLength(128)]
			public string Id { get; set; }

			[MaxLength(128)]
			public string UserID { get; set; }
			public float ImgX{ get; set;}
			public float ImgY{ get; set;}
			public float ImgW{ get; set;}
			public float ImgH{ get; set;}
			public float Rotation{ get; set;}

			[Column ("ongallery")]
			public bool OnGallery { get; set;}

			public PictureL(){}

			public PictureL(Picture p, bool ongallery)
			{
				Id = p.Id;
				UserID = p.UserId;
				ImgX = p.ImgX;
				ImgY = p.ImgY;
				ImgW = p.ImgW;
				ImgH = p.ImgH;
				Rotation = p.Rotation;
				OnGallery = ongallery;
			}
		}

		[Table("Textboxes")]
		public class TextBoxL {
			[PrimaryKey, Column("id"), MaxLength(128)]
			public string Id { get; set; }

			[MaxLength(128)]
			public string UserID { get; set; }
			public string Text { get; set; }
			public float ImgX{ get; set;}
			public float ImgY{ get; set;}
			public float ImgW{ get; set;}
			public float ImgH{ get; set;}
			public float Rotation{ get; set;}

			[Column ("ongallery")]
			public bool OnGallery { get; set;}

			public TextBoxL(){}

			public TextBoxL(TextBox tb, bool ongallery)
			{
				Text = tb.Text;
				Id = tb.Id;
				UserID = tb.UserId;
				Rotation = tb.Rotation;
				ImgX = tb.ImgX;
				ImgY = tb.ImgY;
				ImgW = tb.ImgW;
				ImgH = tb.ImgH;
				OnGallery = ongallery;
			}
		}

		[Table("Likes")]
		public class LikeL {
			[PrimaryKey, Column("id"), MaxLength(128)]
			public string Id { get; set; }
			[Column("userid"), MaxLength(128)]
			public string UserId { get; set; }
			[Column("contentid"), MaxLength(128)]
			public string ContentId { get; set;}

			public LikeL(){}

			public LikeL(string id, string userid, string contentid)
			{
				Id = id; UserId = userid; ContentId = contentid;
			}
		}

		private static string dbPath;

		private static SQLiteConnection database;

		public static void Initialize () {
			var docs = (NSFileManager.DefaultManager.GetUrls (
				NSSearchPathDirectory.LibraryDirectory, 
				NSSearchPathDomain.User) [0]).Path;

			dbPath = Path.Combine (docs, "localdb.db3");

			database = new SQLiteConnection (dbPath);
			database.CreateTable<PictureL> ();
			database.CreateTable<LikeL> ();
			database.CreateTable<TextBoxL> ();
			database.CreateTable<MessageL> ();
		}

		public static List<Message> ReturnConversation(string contentId)
		{
			List<MessageL> query = database.Query<MessageL> ("SELECT * FROM Messages WHERE contentId = ? ORDER BY createdAt", contentId);
			List<Message> lstMessages = new List<Message> ();

			foreach (MessageL msgl in query) {
				Message msg = new Message (msgl.Id, msgl.UserID, msgl.ContentID, msgl.Text, msgl.CreatedAt);
				lstMessages.Add (msg);
			}

			return lstMessages;
		}

		public static async Task UpdateLocalDB()
		{
			// downloads new content
		}

		public static void UpdateLikeTable(IEnumerable<Like> enumLikes)
		{
			// TODO: improve the performance of this method
			database.Query<bool> ("DELETE FROM Likes");

			foreach (Like like in enumLikes) {
				LikeL likeL = new LikeL (like.Id, like.UserId, like.ContentId);
				database.Insert (likeL);
			}
		}

		public static void InsertMessages(List<Message> msgs)
		{
			foreach (Message m in msgs) {
				InsertMessage (m);
			}
		}

		public static void InsertPicture(Picture p)
		{
			// first I create a PictureL which will go to the DB, there I save all the image metadata
			PictureL picL = new PictureL (p, false);

			// then I store the imagefile on the directory, with an unique name which is the same as the ID
			var docs = (NSFileManager.DefaultManager.GetUrls (
				           NSSearchPathDirectory.DocumentDirectory, 
				           NSSearchPathDomain.User) [0]).Path;
			string imgFilename = System.IO.Path.Combine (docs, p.Id + ".jpg"); 
			string thumbFilename = System.IO.Path.Combine (docs, p.Id + "-thumb.jpg");
			UIImage imgData = p.Image;
			database.Insert (picL);
		}

		public static DateTimeOffset GetLastMessage(string contentID)
		{
			List<DateTimeOffset> aux = database.Query<DateTimeOffset> ("SELECT MAX(createdAt) FROM Messages WHERE contentId = ?", contentID);

			if (aux.Count > 0) {
				return aux [0];
			} else {
				return new DateTimeOffset ();
			}
		}

		public static void InsertTextBox(TextBox tb)
		{
			try{
				TextBoxL tbl = new TextBoxL (tb, false);
				database.Insert (tbl);
			} catch (Exception e){
				Console.WriteLine ("ERROR: " + e.Message);
			}
		}

		public static void InsertMessage(Message msg)
		{
			List<MessageL> aux = database.Query<MessageL> ("SELECT * FROM Messages WHERE id = ?", msg.Id);

			if (aux.Count == 0) {
				MessageL msgl = new MessageL (msg);
				database.Insert (msgl);
			}
		}

		public static void SendTextBoxToGallery(string id)
		{
			database.Query<bool>("UPDATE Textboxes SET ongallery = ? WHERE id = ?", true, id);
		}

		public static void SendPictureToGallery(string id)
		{
			database.Query<bool>("UPDATE Pictures SET ongallery = ? WHERE id = ?", true, id);
		}

		// returns all pictures
		public static List<Picture> ReturnAllStoredPictures()
		{
			List<Picture> lstPictures = new List<Picture>();

			var table = database.Table<PictureL> ();
			foreach (var p in table) {

				Picture aux = new Picture ();
				aux.Id = p.Id;
				aux.ImgH = p.ImgH;
				aux.ImgW = p.ImgW;
				aux.ImgX = p.ImgX;
				aux.ImgY = p.ImgY;
				aux.Rotation = p.Rotation;
				aux.UserId = p.UserID;

				var docs = (NSFileManager.DefaultManager.GetUrls (
					NSSearchPathDirectory.DocumentDirectory, 
					NSSearchPathDomain.User) [0]).Path;
				string jpgFilename = System.IO.Path.Combine (docs, p.Id + ".jpg"); 

				aux.Image = UIImage.FromBundle (jpgFilename);

				lstPictures.Add (aux);
			}

			return lstPictures;
		}

		// returns all textboxes
		public static List<TextBox> ReturnAllStoredTextboxes()
		{
			List<TextBox> lstTextboxes = new List<TextBox>();
			List<TextBoxL> table = database.Query<TextBoxL> ("SELECT * FROM Textboxes");
			foreach (var p in table) {

				TextBox aux = new TextBox (p.Id, p.UserID, p.Text, new CGRect(p.ImgX, p.ImgY, p.ImgW, p.ImgH), p.Rotation);
				lstTextboxes.Add (aux);
			}

			return lstTextboxes;
		}

		// returns all textboxes with ongallery discrimination
		public static List<TextBox> ReturnAllStoredTextboxes(bool onGallery)
		{
			List<TextBox> lstTextboxes = new List<TextBox>();
			List<TextBoxL> table = database.Query<TextBoxL> ("SELECT * FROM Textboxes WHERE ongallery = ?", onGallery);
			foreach (var p in table) {

				TextBox aux = new TextBox (p.Id, p.UserID, p.Text, new CGRect(p.ImgX, p.ImgY, p.ImgW, p.ImgH), p.Rotation);
				lstTextboxes.Add (aux);
			}

			return lstTextboxes;
		}

		public static int ReturnNumberOfLikes(string contentid)
		{
			try {
				// TODO: fix, always returns 0
				// count = database.Query<int> ("SELECT COUNT(*) FROM Likes WHERE contentid = ?", contentid)[0];
				List<LikeL> lst = database.Query<LikeL> ("SELECT * FROM Likes WHERE contentid = ?", contentid);
				if (lst != null)
				{ return lst.Count; }
				else { return 0; }
			}
			catch{
				return -1;
			}
		}

		// returns all pictures discriminating ongallery status
		public static List<Picture> ReturnAllStoredPictures(bool onGallery)
		{
			List<Picture> lstPictures = new List<Picture>();
			List<PictureL> table = database.Query<PictureL> ("SELECT * FROM Pictures WHERE ongallery = ?", onGallery);
			foreach (PictureL p in table) {

				Picture aux = new Picture ();
				aux.Id = p.Id;
				aux.ImgH = p.ImgH;
				aux.ImgW = p.ImgW;
				aux.ImgX = p.ImgX;
				aux.ImgY = p.ImgY;
				aux.Rotation = p.Rotation;
				aux.UserId = p.UserID;

				var docs = (NSFileManager.DefaultManager.GetUrls (
					NSSearchPathDirectory.DocumentDirectory, 
					NSSearchPathDomain.User) [0]).Path;
				string imgFilename = System.IO.Path.Combine (docs, p.Id + ".jpg"); 

				aux.Image = UIImage.FromBundle (imgFilename);

				string thumbFilename = System.IO.Path.Combine (docs, p.Id + "-thumb.jpg"); 
				aux.Thumbnail = UIImage.FromBundle (thumbFilename);

				lstPictures.Add (aux);
			}

			return lstPictures;
		}

		// returns all picture's id's from the local storage
		public static List<string> ReturnAllPictureIDs()
		{
			List<string> lstIDs = new List<string> ();
			List<PictureL> pictureTable = database.Query<PictureL> ("SELECT id FROM Pictures");

			foreach (PictureL p in pictureTable){
				lstIDs.Add(p.Id);
			}

			return lstIDs;
		}

		// returns all messages's id's from the local storage
		public static List<string> ReturnAllMessageIDs()
		{
			List<string> lstIDs = new List<string> ();
			List<MessageL> messageTable = database.Query<MessageL> ("SELECT id FROM Messages");

			foreach (MessageL p in messageTable){
				lstIDs.Add(p.Id);
			}

			return lstIDs;
		}

		// returns all textbox's id's from the local storage
		public static List<string> ReturnAllTextBoxIDs()
		{
			List<string> lstIDs = new List<string> ();
			List<TextBoxL> textboxTable = database.Query<TextBoxL> ("SELECT id FROM Textboxes");

			foreach (TextBoxL tb in textboxTable){
				lstIDs.Add(tb.Id);
			}

			return lstIDs;
		}

		// returns all picture id's depending on it's gallery status
		public static List<string> ReturnAllPictureIDs(bool onGallery)
		{
			List<string> lstIDs = new List<string> ();
			List<PictureL> pictureTable = database.Query<PictureL> ("SELECT id FROM Pictures WHERE ongallery = ?", onGallery);

			foreach (PictureL p in pictureTable){
				lstIDs.Add(p.Id);
			}

			return lstIDs;
		}

		// returns all picture id's depending on it's gallery status
		public static List<string> ReturnAllTextBoxIDs(bool onGallery)
		{
			List<string> lstIDs = new List<string> ();
			List<TextBoxL> textboxTable = database.Query<TextBoxL> ("SELECT id FROM Textboxes WHERE ongallery = ?", onGallery);

			foreach (TextBoxL tb in textboxTable){
				lstIDs.Add(tb.Id);
			}

			return lstIDs;
		}

		public static Like LookupLike(string userid, string contentid)
		{
			Like like; LikeL likeL;
			try{
				likeL = database.Query<LikeL> ("SELECT * FROM Likes WHERE userid = ? AND contentid = ?", userid, contentid)[0];
				like = new Like(likeL.Id, likeL.UserId, likeL.ContentId);
			}
			catch{
				like = new Like ();
			}
			return like;
		}
	}
}