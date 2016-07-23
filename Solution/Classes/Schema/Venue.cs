using System;
using System.Collections.Generic;
using Clubby.Facebook;
using Clubby.Infrastructure;
using Clubby.JsonResponses;
using Clubby.Utilities;

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
		public List<string> CategoryList;
		public string CoverImageUrl;
		public string Name;
		public string About;
		public string Phone;
		public int FriendLikes;

		// lat+long from facebook, object from google
		public GoogleGeolocatorObject GeolocatorObject;

		public List<Content> ContentList;

		private void InitFromIds(string facebookId, string instagramId){

			InstagramId = instagramId;
			FacebookId = facebookId;

			//get lat and long from facebook

			Name = string.Empty;
			CategoryList = new List<string>();
			LogoUrl = string.Empty;
			CoverImageUrl = string.Empty;
			About = string.Empty;
			Phone = string.Empty;
			GeolocatorObject = new GoogleGeolocatorObject ();
		}

		public Venue(string facebookId, string instagramId) {
			Id = CommonUtils.GenerateGuid ();
			InitFromIds (facebookId, instagramId);
		}

		public Venue(string facebookId, string instagramId, string id) {
			Id = id;
			InitFromIds (facebookId, instagramId);
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

		public static Content GenerateContent(InstagramPageResponse.Item item){
			
			var content = new Content ();

			if (item.videos != null) {

				content = new Video (item.id);

				((Video)content).VideoUrl = item.videos.standard_resolution.url;
				((Video)content).ImageUrl = RemoveParametersFromURL(item.images.standard_resolution.url);

			} else {

				content = new Picture (item.id);

				((Picture)content).ThumbnailImageUrl = item.images.low_resolution.url;
				((Picture)content).ImageUrl = RemoveParametersFromURL(item.images.standard_resolution.url);

			}

			content.Likes = item.likes.count;
			content.InstagramId = item.user.username;
			content.CreationDate = CommonUtils.UnixTimeStampToDateTime (Int32.Parse (item.created_time));
			if (item.caption != null) {
				content.Description = item.caption.text;
			}

			return content;
		}

		private static string RemoveParametersFromURL(string url){
			
			int indexOf = url.IndexOf ('?');
			if (indexOf != -1) {
				return url.Substring (0, indexOf);
			} else {
				return url;
			}

		}

		private void GenerateContentList(){
			ContentList = new List<Content> ();

			var allItems = new List<InstagramPageResponse.Item>();
			allItems.AddRange (InstagramPage.items);

			foreach (var item in allItems) {
				var picture = GenerateContent (item);
				ContentList.Add (picture);
			}

		}

		public async System.Threading.Tasks.Task LoadFacebookDatum(FacebookImportedPage importedVenue){
			Name = importedVenue.Name;
			About = importedVenue.About;
			Phone = importedVenue.Phone;
			CategoryList = importedVenue.Category;
			LogoUrl = importedVenue.PictureUrl;
			CoverImageUrl = importedVenue.CoverUrl;
			FriendLikes = importedVenue.FriendLikes;

			GeolocatorObject = StorageController.TryGettingGeolocatorObject (FacebookId);

			if (GeolocatorObject == null) {
				string jsonObj = await CloudController.GetGeolocatorJson (importedVenue.Location);
				StorageController.StoreGeolocation (FacebookId, jsonObj);
				GeolocatorObject = JsonHandler.DeserializeObject (jsonObj);
			}
		}

		public Venue(string id){
			Id = id;
		}
	}
}

