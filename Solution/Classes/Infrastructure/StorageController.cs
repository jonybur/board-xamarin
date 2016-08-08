using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Clubby.JsonResponses;
using Clubby.Utilities;
using Foundation;
using Newtonsoft.Json.Linq;
using SQLite;

namespace Clubby.Infrastructure
{
	[Preserve(AllMembers = true)]
	public static class StorageController
	{	
		// caches google's geolocator json
		[Preserve(AllMembers = true)]
		[Table("Venues")]
		private class StoredFacebookPage {
			
			[PrimaryKey, Column("id")]
			public string FbId { get; set; }

			[Column("geolocatorJson")]
			public string GeolocatorJson { get; set; }

			public StoredFacebookPage(){}

			public StoredFacebookPage(string fbid, string geolocatorJson){
				FbId = fbid;
				GeolocatorJson = geolocatorJson;
			}
		}

		private class LikeL{

			[PrimaryKey, Column("id")]
			public string Id { get; set; }

			public LikeL(){}

			public LikeL(string id){
				Id = id;
			}

		}

		private class FacebookPages{

			[PrimaryKey, Column("id")]
			public string Id { get; set; }

			public string Json { get; set; }

			public int Timestamp { get; set;}

			public FacebookPages(){}

			public FacebookPages(string id, string json, int timestamp){
				Id = id;
				Json = json;
				Timestamp = timestamp;
			}

		}

		private static string dbPath;
		private static string docsPathLibrary;
		private static SQLiteConnection database;
		private static string timelinePath;
		private static string userPath;

		public static void Initialize () {
			
			docsPathLibrary = (NSFileManager.DefaultManager.GetUrls (
				NSSearchPathDirectory.LibraryDirectory, 
				NSSearchPathDomain.User) [0]).Path;
			
			dbPath = Path.Combine (docsPathLibrary, "localdb.db3");
			timelinePath = Path.Combine (docsPathLibrary, "timeline.txt");
			userPath = Path.Combine (docsPathLibrary, "user.txt");
			//File.Delete(dbPath);

			database = new SQLiteConnection (dbPath);
			database.CreateTable<StoredFacebookPage> ();
			database.CreateTable<LikeL> ();
			database.CreateTable<FacebookPages> ();
		}

		public static List<string> GetUserLikes(){
			return database.Query<LikeL> ("SELECT * FROM LikeL").Select(x=>x.Id).ToList();
		}

		public static Dictionary<string, dynamic> GetFacebookPage(string facebookId){
			var facePages = database.Query<FacebookPages> ("SELECT * FROM FacebookPages WHERE id = ?", facebookId);

			if (facePages.Count > 0) {

				var facePage = facePages [0];

				var storedDateTime = CommonUtils.UnixTimeStampToDateTime (facePage.Timestamp);

				if ((DateTime.Now - storedDateTime).TotalMinutes < 10080) {
					
					var dic = new Dictionary<string, dynamic> ();
					dic.Add (facePage.Id, JObject.Parse (facePage.Json));
					return dic;

				} else {

					Console.WriteLine ("deletes");
					database.Delete (facePage);
					return null;

				}
			}
			return null;
		}

		public static void DeleteAllFacebookPages(){
			database.DeleteAll<FacebookPages> ();
		}

		public static void StoreFacebookPage(string facebookId, string json){
			database.Insert (new FacebookPages (facebookId.ToLower(), json, CommonUtils.GetUnixTimeStamp ()));
		}

		public static void ActionLike(string id){
			if (GetLike (id)) {
				RemoveLike (id);
			} else {
				StoreLike (id);
			}
		}

		public static bool GetLike(string id){
			var likeL = database.Query<LikeL> ("SELECT * FROM LikeL WHERE id = ?", id);

			if (likeL.Count > 0) {
				return true;
			}
			return false;
		}

		private static void StoreLike(string id){
			database.Insert (new LikeL(id));
		}

		private static void RemoveLike(string id){
			database.Delete <LikeL>(id);
		}

		public static GoogleGeolocatorObject TryGettingGeolocatorObject(string fbid){
			var venueL = database.Query<StoredFacebookPage> ("SELECT * FROM Venues WHERE id = ?", fbid);

			if (venueL.Count > 0) {
				// gets image and location from storage
				return JsonHandler.DeserializeObject (venueL [0].GeolocatorJson);
			}

			return null;
		}

		public static DateTime GetTimelineLastWriteTime(){
			return File.GetLastWriteTime (timelinePath);
		}

		public static string GetInstagramTimeline(){
			try {
				return File.ReadAllText(timelinePath);
			}catch{
				return string.Empty;
			}
		}

		public static void StoreInstagramTimeline(string instagramJson){
			File.WriteAllText(timelinePath, instagramJson);
		}


		public static void StoreUserId(string userId){
			File.WriteAllText(userPath, userId);
		}

		public static string GetCurrentUserId(){
			try {
				return File.ReadAllText(userPath);
			}catch{
				return string.Empty;
			}
		}


		public static void StoreGeolocation(string fbId, string geolocationJson){
			var storedFBPage = new StoredFacebookPage (fbId, geolocationJson);
			database.Insert (storedFBPage);
		}

		public static string GetImagePath(string id){
			return Path.Combine (docsPathLibrary, id + ".jpg"); 
		}

		public static void DeleteLocalImage(string id){
			string imagePath = GetImagePath (id + ".jpg");
			File.Delete (imagePath);
		}
	}
}