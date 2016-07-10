using System.IO;
using Clubby.Schema;
using Clubby.JsonResponses;
using Foundation;
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

		private static string dbPath;
		private static string docsPathLibrary;
		private static SQLiteConnection database;

		public static void Initialize () {
			
			docsPathLibrary = (NSFileManager.DefaultManager.GetUrls (
				NSSearchPathDirectory.LibraryDirectory, 
				NSSearchPathDomain.User) [0]).Path;
			
			dbPath = Path.Combine (docsPathLibrary, "localdb.db3");
			 
			//File.Delete(dbPath);

			database = new SQLiteConnection (dbPath);
			database.CreateTable<StoredFacebookPage> ();
		}

		/*public static NSUrl StoreVideoInCache(NSData data, string id){
			string path = Path.Combine (docsPathCaches, id, ".mp4");

			NSError error;
			data.Save (path, NSDataWritingOptions.Atomic, out error);

			return NSUrl.FromFilename (path);
		}*/

		public static GoogleGeolocatorObject TryGettingGeolocatorObject(string fbid){
			var venueL = database.Query<StoredFacebookPage> ("SELECT * FROM Venues WHERE id = ?", fbid);

			if (venueL.Count > 0) {
				// gets image and location from storage
				return JsonHandler.DeserializeObject (venueL [0].GeolocatorJson);
			}

			return null;
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