using System;
using System.Collections.Generic;
using CoreLocation;
using Clubby.Facebook;
using Clubby.Infrastructure;
using Clubby.JsonResponses;
using Clubby.Utilities;
using UIKit;

namespace Clubby.Schema
{
	public class Venue
	{
		public string Id;

		public double Distance;
		public string InstagramId;
		public string FacebookId;

		// gets from instagram
		public string LogoUrl;
		public InstagramPageResponse InstagramPage;

		// gets from facebook
		public string CoverImageUrl;
		public string Name;
		public List<string> CategoryList;
		public string About;
		public string Phone;

		// lat+long from facebook, object from google
		public GoogleGeolocatorObject GeolocatorObject;

		public List<Content> ContentList;

		public Venue(string facebookId, string instagramId) {
			Id = CommonUtils.GenerateGuid ();

			InstagramId = instagramId;
			FacebookId = facebookId;

			//get lat and long from facebook

			Name = string.Empty;
			CategoryList = new List<string>();
			LogoUrl = string.Empty;
			CoverImageUrl = string.Empty;
			About = string.Empty;
			Phone = string.Empty;
		}

		public string GetAllCategories(){
			string allCategories = string.Empty;
			for (int i = 0; i < CategoryList.Count; i++) {
				allCategories += CategoryList [i];

				if (i < CategoryList.Count - 1) {
					allCategories += " · ";		
				}
			}
			return allCategories;
		}

		public async System.Threading.Tasks.Task Initialize(){
			
			Console.Write ("Goes to Facebook ... ");
			string json = await CloudController.AsyncGraphAPIRequest (FacebookId, "?fields=name,location,about,cover,phone,category_list,picture.type(large),context");

			if (json == "400" || json == "404") {
				Console.WriteLine("failed");
				return;
			}
			Console.WriteLine("done");

			Console.Write ("Reads json response ... ");
			var facebookElements = FacebookUtils.ReadFacebookResponse (json);
		 	await LoadFacebookDatum (facebookElements);
			Console.WriteLine("done");

			Console.Write ("Goes to Instagram ... ");
			InstagramPage = await CloudController.GetInstagramPage (InstagramId);
			Console.WriteLine ("done");

			GenerateContentList ();

			BigTed.BTProgressHUD.Dismiss ();
		}

		private void GenerateContentList(){
			ContentList = new List<Content> ();

			var allItems = new List<Clubby.JsonResponses.InstagramPageResponse.Item>();
			allItems.AddRange (InstagramPage.items);

			foreach (var item in allItems) {
				var picture = new Picture ();
				picture.InstagramId = item.user.username;
				if (item.caption != null) {
					picture.Description = item.caption.text;
				}
				picture.CreationDate = CommonUtils.UnixTimeStampToDateTime(Int32.Parse(item.created_time));
				picture.Likes = item.likes.count;

				var imageurl = item.images.standard_resolution.url;
				int indexOf = imageurl.IndexOf ('?');
				if (indexOf != -1) {
					picture.ImageUrl = imageurl.Substring (0, indexOf);
				} else {
					picture.ImageUrl = imageurl;
				}
				picture.ThumbnailImageUrl = item.images.low_resolution.url;
				ContentList.Add (picture);
			}

		}

		private async System.Threading.Tasks.Task LoadFacebookDatum(FacebookImportedPage importedVenue){
			BigTed.BTProgressHUD.Show ("Getting " + importedVenue.Name + "...");

			Name = importedVenue.Name;
			About = importedVenue.About;
			Phone = importedVenue.Phone;
			CategoryList = importedVenue.Category;
			LogoUrl = importedVenue.PictureUrl;
			CoverImageUrl = importedVenue.CoverUrl;

			GeolocatorObject = await CloudController.LoadGeolocatorObject (importedVenue.Location);
		}

		public Venue(string id){
			Id = id;
		}
	}
}

