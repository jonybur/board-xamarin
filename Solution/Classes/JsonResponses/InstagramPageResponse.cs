using System.Collections.Generic;
using Board.Schema;
using Board.Utilities;
using System;

namespace Board.JsonResponses
{
	public class InstagramPageResponse
	{
		public class Location
		{
			public string name { get; set; }
		}

		public class LowResolution
		{
			public string url { get; set; }
			public int width { get; set; }
			public int height { get; set; }
		}

		public class Thumbnail
		{
			public string url { get; set; }
			public int width { get; set; }
			public int height { get; set; }
		}

		public class StandardResolution
		{
			public string url { get; set; }
			public int width { get; set; }
			public int height { get; set; }
		}

		public class Images
		{
			public LowResolution low_resolution { get; set; }
			public Thumbnail thumbnail { get; set; }
			public StandardResolution standard_resolution { get; set; }
		}

		public class Comments
		{
			public int count { get; set; }
			public List<object> data { get; set; }
		}

		public class From
		{
			public string username { get; set; }
			public string profile_picture { get; set; }
			public string id { get; set; }
			public string full_name { get; set; }
		}

		public class Caption
		{
			public string created_time { get; set; }
			public string text { get; set; }
			public From from { get; set; }
			public string id { get; set; }
		}

		public class Datum
		{
			public string username { get; set; }
			public string profile_picture { get; set; }
			public string id { get; set; }
			public string full_name { get; set; }
		}

		public class Likes
		{
			public int count { get; set; }
			public List<Datum> data { get; set; }
		}

		public class User
		{
			public string username { get; set; }
			public string profile_picture { get; set; }
			public string id { get; set; }
			public string full_name { get; set; }
		}

		public class LowResolution2
		{
			public string url { get; set; }
			public int width { get; set; }
			public int height { get; set; }
		}

		public class StandardResolution2
		{
			public string url { get; set; }
			public int width { get; set; }
			public int height { get; set; }
		}

		public class LowBandwidth
		{
			public string url { get; set; }
			public int width { get; set; }
			public int height { get; set; }
		}

		public class Videos
		{
			public LowResolution2 low_resolution { get; set; }
			public StandardResolution2 standard_resolution { get; set; }
			public LowBandwidth low_bandwidth { get; set; }
		}

		public class Item
		{
			public bool can_delete_comments { get; set; }
			public string code { get; set; }
			public Location location { get; set; }
			public Images images { get; set; }
			public bool can_view_comments { get; set; }
			public Comments comments { get; set; }
			public string alt_media_url { get; set; }
			public Caption caption { get; set; }
			public string link { get; set; }
			public Likes likes { get; set; }
			public string created_time { get; set; }
			public bool user_has_liked { get; set; }
			public string type { get; set; }
			public string id { get; set; }
			public User user { get; set; }
			public int? video_views { get; set; }
			public Videos videos { get; set; }
		}

		public string status { get; set; }
		public List<Item> items { get; set; }
		public bool more_available { get; set; }



		public static Content GenerateContent(InstagramPageResponse.Item item){

			var content = new Content ();

			if (item.videos != null) {

				content = new Video (item.id);

				((Video)content).VideoUrl = item.videos.standard_resolution.url;
				((Video)content).ImageUrl = RemoveParametersFromURL(item.images.standard_resolution.url);

			} else {

				content = new Picture (item.id);

				((Picture)content).ThumbnailImageUrl = item.images.low_resolution.url;
				((Picture)content).ImageUrl = RemoveParametersFromURL(item.images.standard_resolution.url);

			}

			content.Likes = item.likes.count;
			content.InstagramId = item.user.username;
			content.CreationDate = CommonUtils.UnixTimeStampToDateTime (Int32.Parse (item.created_time));
			if (item.caption != null) {
				content.Description = item.caption.text;
			}

			return content;
		}

		private static string RemoveParametersFromURL(string url){

			int indexOf = url.IndexOf ('?');
			if (indexOf != -1) {
				return url.Substring (0, indexOf);
			} else {
				return url;
			}

		}

		public static List<Content> GenerateContentList(List<InstagramPageResponse.Item> items){
			var ContentList = new List<Content> ();

			var allItems = new List<InstagramPageResponse.Item>();
			allItems.AddRange (items);

			foreach (var item in allItems) {
				var picture = GenerateContent (item);
				ContentList.Add (picture);
			}

			return ContentList;
		}
	}
}
