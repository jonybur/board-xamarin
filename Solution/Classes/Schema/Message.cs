using System;
using Newtonsoft.Json;

namespace Solution
{
	public class Message
	{
		public Message(){
		}

		public Message(string userID, string contentID, string text, DateTimeOffset createdAt)
		{
			UserId = userID;
			ContentId = contentID;
			Text = text;
			CreatedAt = createdAt;
		}

		public Message(string id, string userID, string contentID, string text, DateTimeOffset createdAt)
		{
			Id = id;
			UserId = userID;
			ContentId = contentID;
			Text = text;
			CreatedAt = createdAt;
		}

		public string Id { get; set; }

		[JsonProperty(PropertyName = "userID")]
		public string UserId { get; set; }

		[JsonProperty(PropertyName = "contentID")]
		public string ContentId { get; set; }

		[JsonProperty(PropertyName = "text")]
		public string Text { get; set; }

		[JsonProperty(PropertyName = "__createdAt")]
		public DateTimeOffset CreatedAt { get; set; }
	}
}

