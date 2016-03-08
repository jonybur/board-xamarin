using System;
using Facebook.CoreKit;
using Facebook.LoginKit;
using Foundation;
using UIKit;
using System.Collections.Generic;

namespace Board.Facebook
{
	public static class FacebookUtils
	{
		public static EventHandler Callback;
		public static List<FacebookElement> ElementList;
		public static string Element;
		public static string Id;

		// TODO: modify to accept permission params
		public static async System.Threading.Tasks.Task GetReadPermission(UIViewController uiv, string permission)
		{
			if (!HasPermission(permission))
			{
				LoginManager manager = new LoginManager ();
				await manager.LogInWithReadPermissionsAsync (new string[]{ permission }, uiv);
			}
		}

		public static async System.Threading.Tasks.Task GetPublishPermission(UIViewController uiv, string permission)
		{
			if (!HasPermission(permission))
			{
				LoginManager manager = new LoginManager ();
				await manager.LogInWithPublishPermissionsAsync (new string[]{ permission }, uiv);
			}
		}

		public static bool HasPermission(string permission)
		{
			return AccessToken.CurrentAccessToken.HasGranted (permission);
		}

		public static void MakeGraphRequest(string id, string element, EventHandler onCompletionHandler)
		{
			Callback = onCompletionHandler;
			Element = element;
			Id = id;

			GraphRequest graph = new GraphRequest (id + "/" + element, null, AccessToken.CurrentAccessToken.TokenString, "v2.5", "GET");
			graph.Start (LoadList);
		}

		private static void LoadList(GraphRequestConnection connection, NSObject obj, NSError err)
		{
			ElementList = new List<FacebookElement> ();

			if (Element == "events") {
				string[,] objects = NSObjectToElement (obj, "data.id", "data.name", "data.description", "data.start_time", "data.end_time");

				for (int i = 0; i < objects.GetLength (0); i++) {
					FacebookEvent fbevent = new FacebookEvent (objects [i, 0], objects [i, 1], objects [i, 2], objects [i, 3], objects [i, 4]);
					ElementList.Add (fbevent);
				}

				/*GraphRequest graph = new GraphRequest (Id + "/" + Element + "?fields=cover", null, AccessToken.CurrentAccessToken.TokenString, "v2.5", "GET");
				graph.Start ();*/

			} else if (Element == "posts") {
				string[,] objects = NSObjectToElement (obj, "data.id", "data.message", "data.story", "data.created_time");

				for (int i = 0; i < objects.GetLength (0); i++) {
					FacebookPost fbpost = new FacebookPost (objects [i, 0], objects [i, 1], objects [i, 2], objects [i, 3]);
					ElementList.Add (fbpost);
				}
			} else if (Element == "accounts") {
				string[,] objects = NSObjectToElement (obj, "data.id", "data.name", "data.category");

				for (int i = 0; i < objects.GetLength (0); i++) {
					FacebookPage fbpage = new FacebookPage (objects [i, 0], objects [i, 1], objects [i, 2]);
					ElementList.Add (fbpage);
				}
			}

			if (Callback != null) {
				Callback (null, null);
			}
		}

		// first parameter must be primary key
		public static string[,] NSObjectToElement(NSObject obj, params string[] fetch)
		{
			NSString idString = new NSString (fetch[0]);

			NSArray ids = (NSArray)obj.ValueForKeyPath (idString);
			NSArray[] attributes = new NSArray[fetch.Length - 1];

			for (int i = 1; i < fetch.Length; i++) {
				NSString nsString = new NSString (fetch [i]);
				attributes[i - 1] = (NSArray)obj.ValueForKeyPath (nsString);
			}

			// instancias x atributos
			string[,] result = new string[ids.Count, fetch.Length];

			for (int i = 0; i < (int)ids.Count; i++) {
				
				var item = ids.GetItem<NSObject> ((nuint)i);
				result [i, 0] = item.ToString ();

				for (int j = 0; j < attributes.Length; j++) {
					var att = attributes [j].GetItem<NSObject> ((nuint)i);
					result [i, j + 1] = att.ToString ();
				}

			}

			return result;
		}
	}
}

