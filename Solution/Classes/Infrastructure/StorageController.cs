using System;
using System.IO;
using SQLite;
using Foundation;
using UIKit;
using System.Threading.Tasks;
using CoreGraphics;
using System.Collections.Generic;
using Board.Schema;

namespace Board.Infrastructure
{
	public static class StorageController
	{
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

				p.Frame = new CGRect(ImgX, ImgY, ImgW, ImgH);

				Rotation = p.Rotation;
				OnGallery = ongallery;
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
		}

		public static async Task UpdateLocalDB()
		{
			// downloads new content
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
			UIImage imgData = p.ImageView.Image;
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
				aux.Frame = new CGRect (p.ImgX, p.ImgY, p.ImgW, p.ImgH);
				aux.Rotation = p.Rotation;
				aux.UserId = p.UserID;

				var docs = (NSFileManager.DefaultManager.GetUrls (
					NSSearchPathDirectory.DocumentDirectory, 
					NSSearchPathDomain.User) [0]).Path;
				string jpgFilename = System.IO.Path.Combine (docs, p.Id + ".jpg"); 

				aux.ImageView = new UIImageView(UIImage.FromBundle (jpgFilename));

				lstPictures.Add (aux);
			}

			return lstPictures;
		}

		// returns all pictures discriminating ongallery status
		public static List<Picture> ReturnAllStoredPictures(bool onGallery)
		{
			List<Picture> lstPictures = new List<Picture>();
			List<PictureL> table = database.Query<PictureL> ("SELECT * FROM Pictures WHERE ongallery = ?", onGallery);
			foreach (PictureL p in table) {

				Picture aux = new Picture ();
				aux.Id = p.Id;
				aux.Frame = new CGRect (p.ImgX, p.ImgY, p.ImgW, p.ImgH);
				aux.Rotation = p.Rotation;
				aux.UserId = p.UserID;

				var docs = (NSFileManager.DefaultManager.GetUrls (
					NSSearchPathDirectory.DocumentDirectory, 
					NSSearchPathDomain.User) [0]).Path;
				string imgFilename = System.IO.Path.Combine (docs, p.Id + ".jpg"); 

				aux.ImageView = new UIImageView(UIImage.FromBundle (imgFilename));

				string thumbFilename = System.IO.Path.Combine (docs, p.Id + "-thumb.jpg"); 
				aux.ThumbnailView = new UIImageView (UIImage.FromBundle (thumbFilename));

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
	}
}