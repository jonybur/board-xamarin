using System;
using CoreLocation;
using Foundation;
using Newtonsoft.Json.Linq;
using Clubby.Infrastructure;
using Facebook.CoreKit;
using System.Collections.Generic;

namespace Clubby.Facebook
{
	public static class FacebookUtils
	{
		private static string facebookAccessToken;

		/*
		private static async System.Threading.Tasks.Task<string> FetchFacebookAccessToken(){
			string accessToken = await WebAPI.GetJsonAsync ("https://graph.facebook.com/oauth/access_token?%20" +
				"client_id=1614192198892777&" +
				"client_secret=b2d869cdd379966deda4f6d3031aedac" +
				"&%20grant_type=client_credentials");

			return accessToken.Substring(accessToken.IndexOf ('=') + 1);
		}

		public static async System.Threading.Tasks.Task<string> GetFacebookAccessToken(){
			if (string.IsNullOrEmpty(facebookAccessToken)) {
				facebookAccessToken = await FetchFacebookAccessToken ();
			}
			return facebookAccessToken;
		}
		*/

		public static string GetFacebookAccessToken(){
			return AccessToken.CurrentAccessToken != null ? AccessToken.CurrentAccessToken.TokenString : string.Empty;
		}

		public static async System.Threading.Tasks.Task MakeGraphRequest(string id, string element, Action<List<FacebookElement>> callback)
		{
			string query = id + "/" + element;
			var accessToken = GetFacebookAccessToken ();
			var graph = new GraphRequest (query, null, accessToken, "v2.6", "GET");
			graph.Start (delegate (GraphRequestConnection connection, NSObject obj, NSError error) {

				var ElementList = new List<FacebookElement> ();

				if (obj == null) {
					callback (ElementList);
				}

				if (element.StartsWith("events", StringComparison.Ordinal)) {
					string[,] objects = NSObjectToElement (obj, "data.id", "data.name", "data.description", "data.start_time", "data.end_time");

					for (int i = 0; i < objects.GetLength (0); i++) {
						var fbevent = new FacebookEvent (objects [i, 0], objects [i, 1], objects [i, 2], objects [i, 3], objects [i, 4]);
						ElementList.Add (fbevent);
					}
				} else if (element == "?fields=fan_count"){
					string[,] objects = NSObjectToElement (obj, "id", "fan_count");

					for (int i = 0; i < objects.GetLength (0); i++) {
						var fbfancount = new FacebookFanCount (objects [i, 0], objects [i, 1]);
						ElementList.Add (fbfancount);
					}

				} else if (element.StartsWith("videos?fields=source,description,updated_time,thumbnails", StringComparison.Ordinal)) {

					string[,] objects = NSObjectToElement (obj, "data.description", "data.updated_time", "data.id", "data.source", "data.thumbnails.data.uri");

					for (int i = 0; i < objects.GetLength (0); i++) {
						var fbvideo = new FacebookVideo (objects [i, 0], objects [i, 1], objects [i, 2], objects [i, 3], objects [i, 4]);
						ElementList.Add (fbvideo);
					}

				} else if (element.StartsWith("?fields=photos", StringComparison.Ordinal)) {
					string[,] objects = NSObjectToElement (obj, "photos.data.id", "photos.data.name", "photos.data.created_time");

					for (int i = 0; i < objects.GetLength (0); i++) {
						var fbphoto = new FacebookPhoto (objects [i, 0], objects [i, 1], objects [i, 2]);
						ElementList.Add (fbphoto);
					}
				} else if (element == "accounts") {
					string[,] objects = NSObjectToElement (obj, "data.id", "data.name", "data.category");

					for (int i = 0; i < objects.GetLength (0); i++) {
						var fbpage = new FacebookPage (objects [i, 0], objects [i, 1], objects [i, 2]);
						ElementList.Add (fbpage);
					}
				} else if (element.StartsWith("?fields=albums", StringComparison.Ordinal)) {
					string[,] objects = NSObjectToElement (obj, "albums.data.id", "albums.data.name", "albums.data.created_time");

					for (int i = 0; i < objects.GetLength (0); i++) {
						var fbalbum = new FacebookAlbum (objects [i, 0], objects [i, 1], objects [i, 2]);
						ElementList.Add (fbalbum);
					}
				} else if (element == "?fields=likes") {
					string[,] objects = NSObjectToElement (obj, "id", "likes");

					for (int i = 0; i < objects.GetLength (0); i++) {
						var fbimage = new FacebookLikes (objects [i, 0], objects [i, 1]);
						ElementList.Add (fbimage);
					}
				} else if (element == "?fields=images") {
					string[,] objects = NSObjectToElement (obj, "id", "images.height", "images.source", "images.width");

					for (int i = 0; i < objects.GetLength (0); i++) {
						var fbimage = new FacebookImage (objects [i, 0], objects [i, 1], objects [i, 2], objects[i, 3]);
						ElementList.Add (fbimage);
					}
				} else if (element == "?fields=cover") {
					string[,] objects = NSObjectToElement (obj, "cover.id", "cover.source");

					for (int i = 0; i < objects.GetLength (0); i++) {
						var fbcover = new FacebookCover (objects [i, 0], objects [i, 1]);
						ElementList.Add (fbcover);
					}

				} else if (element == "?fields=cover,updated_time") {
					string[,] objects = NSObjectToElement (obj, "cover.id", "cover.source", "updated_time");

					for (int i = 0; i < objects.GetLength (0); i++) {
						var fbcover = new FacebookCoverUpdatedTime (objects [i, 0], objects [i, 1], objects[i, 2]);
						ElementList.Add (fbcover);
					}

				} else if (element == "?fields=hours") {
					string[,] objects = NSObjectToElement (obj, "id", "hours");

					for (int i = 0; i < objects.GetLength (0); i++) {
						var fbhours = new FacebookHours (objects [i, 0], objects [i, 1]);
						ElementList.Add (fbhours);
					}

				} else if (element.StartsWith("?fields=posts", StringComparison.Ordinal)) {
					string[,] objects = NSObjectToElement (obj, "posts.data.id", "posts.data.message", "posts.data.story", "posts.data.created_time", "posts.data.full_picture");

					for (int i = 0; i < objects.GetLength (0); i++) {
						var fbposts = new FacebookPost (objects [i, 0], objects [i, 1], objects [i, 2], objects [i, 3], objects [i, 4]);
						ElementList.Add (fbposts);
					}
				} else if (element.StartsWith("?fields=full_picture", StringComparison.Ordinal)) {
					string[,] objects = NSObjectToElement (obj, "id", "full_picture");

					for (int i = 0; i < objects.GetLength (0); i++) {
						var fbposts = new FacebookFullPicture (objects [i, 0], objects [i, 1]);
						ElementList.Add (fbposts);
					}
				}else if (element.StartsWith("?fields=events", StringComparison.Ordinal)) {
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

		public static FacebookImportedPage ReadFacebookResponse(string json){
			var jobject = JObject.Parse (json);

			//"id", "name", "location.latitude", "location.longitude", "about", "cover.source", "picture.data.url", "phone", "category_list.name"

			string id = 	     TryGetJsonValue((JValue)jobject ["id"]);
			string name = 	     TryGetJsonValue((JValue)jobject ["name"]);
			string latitude =    TryGetJsonValue((JValue)jobject ["location"]["latitude"]);
			string longitude =   TryGetJsonValue((JValue)jobject ["location"]["longitude"]);
			string about = 	     TryGetJsonValue((JValue)jobject ["about"]);
			string cover = 	     TryGetJsonValue((JValue)jobject ["cover"]["source"]);
			string picture =     TryGetJsonValue((JValue)jobject ["picture"]["data"]["url"]);
			string phone =       TryGetJsonValue((JValue)jobject ["phone"]);
			string friendLikes = TryGetJsonValue((JValue)jobject["context"]["friends_who_like"]["summary"]["total_count"]);

			var categoryObject = JArray.FromObject (jobject["category_list"]);

			var categoryList = new List<string> ();
			foreach (var category in categoryObject) {
				var catName = TryGetJsonValue((JValue)category["name"]);
				if (!string.IsNullOrEmpty (catName)) {
					categoryList.Add (catName);
				}

			}

			CLLocationCoordinate2D location;
			if (latitude != null && longitude != null) {
				location = new CLLocationCoordinate2D (Double.Parse (latitude), Double.Parse (longitude));
			} else {
				location = new CLLocationCoordinate2D ();
			}

			return new FacebookImportedPage (id, name, location, about, cover, picture, phone, categoryList, Int32.Parse(friendLikes));
		}

		private static string TryGetJsonValue(JValue jvalue){
			try{
				return jvalue.ToString();
			}catch{
				return string.Empty;
			}
		}

		// first parameter must be primary key
		private static string[,] NSObjectToElement(NSObject obj, params string[] fetch)
		{
			if (obj == null) {
				return new string[0,0];
			}

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
						if (att != null) {
							result [i, j + 1] = att.ToString ();
						}
					}
				}

			}

			return result;
		}
	}
}

