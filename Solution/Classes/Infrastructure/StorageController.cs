using System.IO;
using SQLite;
using Foundation;
using System.Collections.Generic;
using Board.Schema;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Board.Infrastructure
{
	public static class StorageController
	{
		[Table("Announcements")]
		private class AnnouncementL {
			[PrimaryKey]
			public string Id { get; set; }

			public AnnouncementL(){}

			public AnnouncementL(string id){
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

			database = new SQLiteConnection (dbPath);
			database.CreateTable<AnnouncementL> ();
		}

		public static void InsertContent(Content content)
		{
			if (content is Announcement) {
				BinarySerialize ((Announcement)content);

				AnnouncementL anl = new AnnouncementL (content.Id);
				database.Insert (anl);
			}
		}

		public static void RemoveContent(Content content)
		{
			if (content is Announcement) {
				// searches in DB, if uid found then kill .bin

				File.Delete (docsPath + content.Id + ".bin");
			}
		}

		// returns all stored content dictionary
		public static Dictionary<string, Content> ReturnAllStoredContent()
		{
			Dictionary<string, Content> dicContent = new Dictionary<string, Content>();
			var table = database.Table<AnnouncementL> ();

			foreach (AnnouncementL anl in table) {
				
				string filename = Path.Combine (docsPath, anl.Id + ".bin"); 

				Announcement an = BinaryDeserialize (filename);

				dicContent.Add (anl.Id, an);
			}

			return dicContent;
		}

		// serializes object and saves it in file
		private static void BinarySerialize(Announcement obj){
			IFormatter formatter = new BinaryFormatter();

			string filename = Path.Combine (docsPath, obj.Id + ".bin"); 

			using (Stream stream = new FileStream (filename, FileMode.Create, FileAccess.Write, FileShare.None)) {
				formatter.Serialize (stream, obj);
				stream.Close ();
			}
		}

		// deserializes, returns object
		private static Announcement BinaryDeserialize(string url)
		{
			IFormatter formatter = new BinaryFormatter();

			Announcement obj;
			using (Stream stream = new FileStream (url, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				obj = (Announcement)formatter.Deserialize (stream);
				stream.Close ();
			}
			return obj;
		}
	}
}