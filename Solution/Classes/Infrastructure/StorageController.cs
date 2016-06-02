using System.Collections.Generic;
using System.IO;
using Board.JsonResponses;
using Board.Schema;
using Foundation;
using SQLite;

namespace Board.Infrastructure
{
	[Preserve(AllMembers = true)]
	public static class StorageController
	{	
		[Preserve(AllMembers = true)]
		[Table("SeenContents")]
		private class SeenContent {
			[PrimaryKey, Column("id")]
			public string Id { get; set; }

			public SeenContent(string id){
				Id = id;
			}

			public SeenContent(){}
		}

		// caches google's geolocator json
		[Preserve(AllMembers = true)]
		[Table("Boards")]
		private class BoardL {
			[PrimaryKey, Column("id")]
			public string Id { get; set; }

			[Column("geolocatorJson")]
			public string GeolocatorJson { get; set; }

			public BoardL(){}

			public BoardL(string id, string geolocatorJson){
				Id = id;
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
			database.CreateTable<BoardL> ();
			database.CreateTable<SeenContent> ();
		}

		/*public static NSUrl StoreVideoInCache(NSData data, string id){
			string path = Path.Combine (docsPathCaches, id, ".mp4");

			NSError error;
			data.Save (path, NSDataWritingOptions.Atomic, out error);

			return NSUrl.FromFilename (path);
		}*/

		public static bool WasContentSeen(string id){
			var seenContent = database.Query<SeenContent> ("SELECT * FROM SeenContents WHERE id = ?", id);

			if (seenContent.Count > 0) {
				// gets image and location from storage
				return true;
			}

			return false;
		}

		public static void SetContentAsSeen(string id){
			var seenContent = new SeenContent(id);
			database.Insert(seenContent);
		}

		public static Board.Schema.Board BoardIsStored(string id){
			var boardL = database.Query<BoardL> ("SELECT * FROM Boards WHERE id = ?", id);

			if (boardL.Count > 0) {
				// gets image and location from storage
				var board = new Board.Schema.Board (id);
				board.GeolocatorObject = JsonHandler.DeserializeObject (boardL [0].GeolocatorJson);
				return board;
			}

			return null;
		}

		public static void StoreBoard(Board.Schema.Board board, string geolocationJson){
			var boardL = new BoardL (board.Id, geolocationJson);
			database.Insert (boardL);
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