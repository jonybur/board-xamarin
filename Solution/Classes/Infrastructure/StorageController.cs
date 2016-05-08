using System;
using System.Collections.Generic;
using System.IO;
using Board.JsonResponses;
using Board.Schema;
using Board.Utilities;
using System.Threading.Tasks;
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
		[Table("Users")]
		private class UserL {
			[PrimaryKey, Column("id")]
			public string Id { get; set; }

			public UserL(){}

			public UserL(string id){
				Id = id;
			}
		}


		private static string dbPath;
		private static string docsPath;
		private static SQLiteConnection database;

		public static void Initialize () {
			
			docsPath = (NSFileManager.DefaultManager.GetUrls (
				NSSearchPathDirectory.LibraryDirectory, 
				NSSearchPathDomain.User) [0]).Path;

			dbPath = Path.Combine (docsPath, "localdb.db3");
			 
			//File.Delete(dbPath);

			database = new SQLiteConnection (dbPath);
			database.CreateTable<BoardL> ();
			database.CreateTable<UserL> ();
		}

		public static Board.Schema.Board BoardIsStored(string id){
			var boardL = database.Query<BoardL> ("SELECT * FROM Boards WHERE id = ?", id);

			if (boardL.Count > 0) {
				
				// gets image and location from storage
				string imgPath = Path.Combine (docsPath, id + ".jpg"); 
				var image = UIImage.FromBundle (imgPath);

				var board = new Board.Schema.Board(id);

				board.ImageView = new UIImageView(image);
				board.GeolocatorObject = JsonHandler.DeserializeObject (boardL[0].GeolocatorJson);

				return board;
			} else {
				return null;
			}
		}

		public static User UserIsStored(string id = "11111"){
			var userL = database.Query<BoardL> ("SELECT * FROM Users WHERE id = ?", id);

			if (userL.Count > 0) {

				// gets image and location from storage
				string imgPath = Path.Combine (docsPath, id + ".jpg"); 
				var image = UIImage.FromBundle (imgPath);

				var user = new User();

				user.ProfilePictureUIImage = image;

				return user;
			} else {
				return null;
			}
		}

		public static void StoreUser(User user){
			string uid = "11111";

			var userL = new UserL (uid);
			string imgFilename = Path.Combine (docsPath, uid + ".jpg"); 
			NSData imgData = user.ProfilePictureUIImage.AsJPEG();

			NSError err = null;

			if (imgData.Save (imgFilename, false, out err) && imgData.Save (imgFilename, false, out err)) {
				database.Insert (userL);
			} else {
				Console.WriteLine ("ERROR : picture hasnt been saved " + err.LocalizedDescription);
			}
		}

		public static void StoreBoard(Board.Schema.Board board, string geolocationJson){
			var boardL = new BoardL (board.Id, geolocationJson);
				
			string imgFilename = Path.Combine (docsPath, board.Id + ".jpg"); 
			NSData imgData = board.ImageView.Image.AsJPEG ();

			NSError err = null;
			if (imgData.Save (imgFilename, false, out err) && imgData.Save (imgFilename, false, out err)) {
				database.Insert (boardL);
			} else {
				Console.WriteLine ("ERROR : picture hasnt been saved " + err.LocalizedDescription);
			}

		}
	}
}