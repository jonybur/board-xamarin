using System;
using CoreLocation;
using Facebook.CoreKit;
using Facebook.LoginKit;
using Foundation;
using UIKit;
using System.Collections.Generic;

namespace Board.Facebook
{
	public static class FacebookUtils
	{
		private static string Element;

		// TODO: modify to accept permission params
		public static async System.Threading.Tasks.Task GetReadPermission(UIViewController uiv, string permission)
		{
			if (!HasPermission(permission))
			{
				LoginManager manager = new LoginManager ();
				await manager.LogInWithReadPermissionsAsync (new []{ permission }, uiv);
			}
		}

		public static async System.Threading.Tasks.Task GetPublishPermission(UIViewController uiv, string permission)
		{
			if (!HasPermission(permission))
			{
				LoginManager manager = new LoginManager ();
				await manager.LogInWithPublishPermissionsAsync (new []{ permission }, uiv);
			}
		}

		public static bool HasPermission(string permission)
		{
			return AccessToken.CurrentAccessToken.HasGranted (permission);
		}

		public static void MakeGraphRequest(string id, string element, Action<List<FacebookElement>> callback)
		{
			Element = element;

			var graph = new GraphRequest (id + "/" + element, null, AccessToken.CurrentAccessToken.TokenString, "v2.5", "GET");
			graph.Start (async delegate(GraphRequestConnection connection, NSObject obj, NSError error) {

				var ElementList = new List<FacebookElement> ();

				if (obj == null) {
					callback (ElementList);
				}

				if (Element.StartsWith("events", StringComparison.Ordinal)) {
					string[,] objects = NSObjectToElement (obj, "data.id", "data.name", "data.description", "data.start_time", "data.end_time");

					for (int i = 0; i < objects.GetLength (0); i++) {
						var fbevent = new FacebookEvent (objects [i, 0], objects [i, 1], objects [i, 2], objects [i, 3], objects [i, 4]);
						ElementList.Add (fbevent);
					}
				} else if (Element.StartsWith("posts", StringComparison.Ordinal)) {
					string[,] objects = NSObjectToElement (obj, "data.id", "data.message", "data.story", "data.created_time");

					for (int i = 0; i < objects.GetLength (0); i++) {
						var fbpost = new FacebookPost (objects [i, 0], objects [i, 1], objects [i, 2], objects [i, 3]);
						ElementList.Add (fbpost);
					}
				} else if (Element.StartsWith("videos?fields=source,description,updated_time,thumbnails", StringComparison.Ordinal)) {

					string[,] objects = NSObjectToElement (obj, "data.description", "data.updated_time", "data.id", "data.source", "data.thumbnails.data.uri");

					for (int i = 0; i < objects.GetLength (0); i++) {
						var fbvideo = new FacebookVideo (objects [i, 0], objects [i, 1], objects [i, 2], objects [i, 3], objects [i, 4]);
						ElementList.Add (fbvideo);
					}

				} else if (Element.StartsWith("photos", StringComparison.Ordinal)) {
					string[,] objects = NSObjectToElement (obj, "data.id", "data.name", "data.created_time", "data.name");

					for (int i = 0; i < objects.GetLength (0); i++) {
						var fbphoto = new FacebookPhoto (objects [i, 0], objects [i, 1], objects [i, 2], objects[i, 3]);
						ElementList.Add (fbphoto);
					}
				} else if (Element == "accounts") {
					string[,] objects = NSObjectToElement (obj, "data.id", "data.name", "data.category");

					for (int i = 0; i < objects.GetLength (0); i++) {
						var fbpage = new FacebookPage (objects [i, 0], objects [i, 1], objects [i, 2]);
						ElementList.Add (fbpage);
					}
				} else if (Element.StartsWith("albums", StringComparison.Ordinal)) {
					string[,] objects = NSObjectToElement (obj, "data.id", "data.name", "data.created_time");

					for (int i = 0; i < objects.GetLength (0); i++) {
						var fbalbum = new FacebookAlbum (objects [i, 0], objects [i, 1], objects [i, 2]);
						ElementList.Add (fbalbum);
					}
				} else if (Element == "?fields=images") {
					string[,] objects = NSObjectToElement (obj, "id", "images.height", "images.source", "images.width");

					for (int i = 0; i < objects.GetLength (0); i++) {
						var fbimage = new FacebookImage (objects [i, 0], objects [i, 1], objects [i, 2], objects[i, 3]);
						ElementList.Add (fbimage);
					}
				} else if (Element == "?fields=cover") {
					string[,] objects = NSObjectToElement (obj, "cover.id", "cover.source");

					for (int i = 0; i < objects.GetLength (0); i++) {
						var fbcover = new FacebookCover (objects [i, 0], objects [i, 1]);
						ElementList.Add (fbcover);
					}
				} else if (Element.StartsWith ("?fields=name,location,about,cover,picture", StringComparison.Ordinal)) {
					string[,] objects = NSObjectToElement (obj, "id", "name", "location.latitude", "location.longitude", "about", "cover.source", "picture.data.url");

					for (int i = 0; i < objects.GetLength (0); i++) {

						CLLocationCoordinate2D location;
						if (objects [i, 2] != null && objects [i, 3] != null) {
							location = new CLLocationCoordinate2D (Double.Parse (objects [i, 2]), Double.Parse (objects [i, 3]));
						} else {
							location = new CLLocationCoordinate2D ();
						}

						var fbimportedpage = new FacebookImportedPage (objects [i, 0], objects [i, 1], location,
							objects [i, 4], objects [i, 5], objects [i, 6]);

						ElementList.Add (fbimportedpage);
					}
				} else if (Element.StartsWith("?fields=posts", StringComparison.Ordinal)) {
					string[,] objects = NSObjectToElement (obj, "posts.data.id", "posts.data.message", "posts.data.story", "posts.data.created_time");

					for (int i = 0; i < objects.GetLength (0); i++) {
						var fbposts = new FacebookPost (objects [i, 0], objects [i, 1], objects [i, 2], objects [i, 3]);
						ElementList.Add (fbposts);
					}
				} else if (Element.StartsWith("?fields=events", StringComparison.Ordinal)) {
					string[,] objects = NSObjectToElement (obj, "events.data.id", "events.data.name", "events.data.description",
						"events.data.start_time", "events.data.end_time");

					for (int i = 0; i < objects.GetLength (0); i++) {
						var fbevent = new FacebookEvent (objects [i, 0], objects [i, 1], objects [i, 2], objects [i, 3], objects [i, 4]);
						ElementList.Add (fbevent);
					}
				}

				if (callback != null){
					callback (ElementList);
				}
			});
		}

		enum GraphRequestType{
			Events = 1, Posts, Videos, Photos, Accounts, Albums, FieldsImages, FieldsCover, FieldsIdSourceThumbnails, FieldsNameLocationAboutCoverPicture
		}

		// first parameter must be primary key
		private static string[,] NSObjectToElement(NSObject obj, params string[] fetch)
		{
			NSString idString = new NSString (fetch[0]);
			NSArray ids = obj.ValueForKeyPath (idString) as NSArray;

			if (ids == null) {
				NSMutableArray array = new NSMutableArray (1);
				var valueForKeyPath = obj.ValueForKeyPath (idString);
				if (valueForKeyPath != null) {
					array.Add (valueForKeyPath);
				}
				ids = array;
			}

			NSArray[] attributes = new NSArray[fetch.Length - 1];

			for (int i = 1; i < fetch.Length; i++) {
				var nsString = new NSString (fetch [i]);
				attributes [i - 1] = obj.ValueForKeyPath (nsString) as NSArray;

				if (attributes [i - 1] == null) {
					var array = new NSMutableArray (1);
					var valueForKeyPath = obj.ValueForKeyPath (nsString);
					if (valueForKeyPath != null) {
						array.Add (valueForKeyPath);
					}
					attributes[i - 1] = array;
				}
			}

			// instancias x atributos
			string[,] result = new string[ids.Count, fetch.Length];

			for (int i = 0; i < (int)ids.Count; i++) {
				
				var item = ids.GetItem<NSObject> ((nuint)i);
				result [i, 0] = item.ToString ();

				for (int j = 0; j < attributes.Length; j++) {
					if (attributes [j].Count > 0) {
						var att = attributes [j].GetItem<NSObject> ((nuint)i);
						result [i, j + 1] = att.ToString ();
					}
				}

			}

			return result;
		}
	}
}

