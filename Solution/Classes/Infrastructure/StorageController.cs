using System;
using System.Collections.Generic;
using System.IO;
using Board.JsonResponses;
using Board.Schema;
using Board.Utilities;
using Foundation;
using SQLite;
using UIKit;

namespace Board.Infrastructure
{
	[Preserve(AllMembers = true)]
	public static class StorageController
	{	
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

		[Preserve(AllMembers = true)]
		[Table("ProfilePictures")]
		private class ProfilePictureL {
			[PrimaryKey, Column("pictureurl")]
			public string PictureURL{ get; set; }

			public string Id{ get; set; }

			public ProfilePictureL(string pictureurl){
				PictureURL = pictureurl;
				Id = CommonUtils.GenerateGuid();
			}

			public ProfilePictureL(){}
		}

		private static string dbPath;
		private static string docsPathLibrary;
		private static string docsPathCaches;
		private static SQLiteConnection database;

		public static void Initialize () {
			
			docsPathLibrary = (NSFileManager.DefaultManager.GetUrls (
				NSSearchPathDirectory.LibraryDirectory, 
				NSSearchPathDomain.User) [0]).Path;

			docsPathCaches = (NSFileManager.DefaultManager.GetUrls (
				NSSearchPathDirectory.CachesDirectory, 
				NSSearchPathDomain.User) [0]).Path;
			
			dbPath = Path.Combine (docsPathLibrary, "localdb.db3");
			 
			//File.Delete(dbPath);

			database = new SQLiteConnection (dbPath);
			database.CreateTable<BoardL> ();
			database.CreateTable<ProfilePictureL> ();
		}

		public static NSUrl StoreVideoInCache(NSData data, string id){
			string path = Path.Combine (docsPathCaches, id, ".mp4");

			NSError error;
			data.Save (path, NSDataWritingOptions.Atomic, out error);

			return NSUrl.FromFilename (path);
		}

		public static Board.Schema.Board BoardIsStored(string id){
			var boardL = database.Query<BoardL> ("SELECT * FROM Boards WHERE id = ?", id);

			if (boardL.Count > 0) {
			
				// gets image and location from storage
				string imgPath = Path.Combine (docsPathLibrary, id + ".jpg"); 
				UIImage image = UIImage.FromBundle (imgPath);

				var board = new Board.Schema.Board (id);
				board.Image = image;
				board.GeolocatorObject = JsonHandler.DeserializeObject (boardL [0].GeolocatorJson);

				return board;
			} else {
				return null;
			}
		}

		public static UIImage GetProfilePicture(string pictureURL){
			var ppL = database.Query<ProfilePictureL> ("SELECT * FROM ProfilePictures WHERE pictureurl = ?", pictureURL);

			if (ppL.Count > 0) {
				string imgPath = Path.Combine (docsPathLibrary, ppL[0].Id + ".jpg"); 
				var image = UIImage.FromBundle (imgPath);
				return image;
			} else {
				return null;
			}
		}

		public static void StoreProfilePicture(User user){
			var ppL = new ProfilePictureL (user.ProfilePictureURL);

			if (StoreImage(user.ProfilePictureUIImage, ppL.Id)) {
				database.Insert (ppL);
			} else {
				Console.WriteLine ("ERROR : picture hasnt been saved");
			}
		}

		public static void StoreBoard(Board.Schema.Board board, string geolocationJson){
			var boardL = new BoardL (board.Id, geolocationJson);
				
			if (StoreImage(board.Image, board.Id)) {
				database.Insert (boardL);
			} else {
				Console.WriteLine ("ERROR: picture hasnt been saved");
			}
		}

		public static bool StoreImage(UIImage image, string id){
			string imagePath = GetImagePath (id);
			NSData imgData = image.AsJPEG ();
			NSError err = null;

			bool result = imgData.Save (imagePath, false, out err);

			return result;
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