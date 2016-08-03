using System;
using System.Diagnostics;
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

		public async System.Threading.Tasks.Task LoadFacebookDatum(FacebookImportedPage importedVenue){
			var sw = new Stopwatch();
			sw.Start();

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


			sw.Stop();
			Console.WriteLine("Crear un venue toma: {0}",sw.Elapsed);
		}

		public Venue(string id){
			Id = id;
		}
	}
}

