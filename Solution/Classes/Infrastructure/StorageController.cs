using System;
using System.IO;
using SQLite;
using Foundation;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using Board.Schema;
using Board.Interface.Widgets;

namespace Board.Infrastructure
{
	public static class StorageController
	{
		[Table("Announcements")]
		public class AnnouncementL {
			[PrimaryKey]
			public string Id { get; set; }

			public string UserId { get; set; }

			public float ImgX{ get; set;}
			public float ImgY{ get; set;}
			public float ImgW{ get; set;}
			public float ImgH{ get; set;}

			public float Rotation{ get; set;}

			public AnnouncementL(){}

			public AnnouncementL(Announcement ann)
			{
				Id = ann.Id;
				UserId = ann.UserId;

				ImgX = (float)ann.Frame.X;
				ImgY = (float)ann.Frame.Y;
				ImgW = (float)ann.Frame.Width;
				ImgH = (float)ann.Frame.Height;

				Rotation = ann.Rotation;
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
			database.CreateTable<AnnouncementL> ();
		}

		public static void InsertContent(Content content)
		{
			if (content is Announcement) {
				AnnouncementL anl = new AnnouncementL ((Announcement)content);
				database.Insert (anl);
			}
		}

		// returns all pictures
		public static List<Content> ReturnAllStoredContent()
		{
			List<Content> lstContent = new List<Content>();

			var table = database.Table<AnnouncementL> ();
			foreach (AnnouncementL anl in table) {

				Announcement ann = new Announcement ();
				ann.Id = anl.Id;
				ann.Frame = new CGRect (anl.ImgX, anl.ImgY, anl.ImgW, anl.ImgH);
				ann.Rotation = anl.Rotation;
				ann.UserId = anl.UserId;
				lstContent.Add (ann);
			}

			return lstContent;
		}
	}
}