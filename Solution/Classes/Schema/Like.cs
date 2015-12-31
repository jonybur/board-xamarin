using System;
using Newtonsoft.Json;

namespace Solution
{
	public class Like
	{
		public Like(){
		}

		public Like(string userID, string contentID)
		{
			UserId = userID;
			ContentId = contentID;
		}


		public Like(string id, string userID, string contentID)
		{
			Id = id;
			UserId = userID;
			ContentId = contentID;
		}

		public string Id { get; set; }

		[JsonProperty(PropertyName = "userid")]
		public string UserId { get; set; }

		[JsonProperty(PropertyName = "contentid")]
		public string ContentId { get; set; }
	}
}

