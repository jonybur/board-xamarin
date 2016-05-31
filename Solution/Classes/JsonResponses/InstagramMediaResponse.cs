using System.Collections.Generic;
using Foundation;

namespace Board.JsonResponses
{
	[Preserve(AllMembers = true)]
	public class InstagramMediaResponse
	{
		[Preserve(AllMembers = true)]
		public class Comments
		{
			public int count { get; set; }
		}
		[Preserve(AllMembers = true)]
		public class From
		{
			public string username { get; set; }
			public string full_name { get; set; }
			public string type { get; set; }
			public string id { get; set; }
		}
		[Preserve(AllMembers = true)]
		public class Caption
		{
			public string created_time { get; set; }
			public string text { get; set; }
			public From from { get; set; }
			public string id { get; set; }
		}
		[Preserve(AllMembers = true)]
		public class Likes
		{
			public int count { get; set; }
		}
		[Preserve(AllMembers = true)]
		public class User
		{
			public string username { get; set; }
			public string profile_picture { get; set; }
			public string id { get; set; }
		}
		[Preserve(AllMembers = true)]
		public class LowResolution
		{
			public string url { get; set; }
			public int width { get; set; }
			public int height { get; set; }
		}
		[Preserve(AllMembers = true)]
		public class Thumbnail
		{
			public string url { get; set; }
			public int width { get; set; }
			public int height { get; set; }
		}
		[Preserve(AllMembers = true)]
		public class StandardResolution
		{
			public string url { get; set; }
			public int width { get; set; }
			public int height { get; set; }
		}
		[Preserve(AllMembers = true)]
		public class Images
		{
			public LowResolution low_resolution { get; set; }
			public Thumbnail thumbnail { get; set; }
			public StandardResolution standard_resolution { get; set; }
		}
		[Preserve(AllMembers = true)]
		public class Location
		{
			public double latitude { get; set; }
			public string id { get; set; }
			public double longitude { get; set; }
			public string name { get; set; }
		}
		[Preserve(AllMembers = true)]
		public class Datum
		{
			public string type { get; set; }
			public List<object> users_in_photo { get; set; }
			public string filter { get; set; }
			public List<string> tags { get; set; }
			public Comments comments { get; set; }
			public Caption caption { get; set; }
			public Likes likes { get; set; }
			public string link { get; set; }
			public User user { get; set; }
			public string created_time { get; set; }
			public Images images { get; set; }
			public bool user_has_liked { get; set; }
			public string id { get; set; }
			public Location location { get; set; }
		}

		public List<Datum> data { get; set; }
	}
}

