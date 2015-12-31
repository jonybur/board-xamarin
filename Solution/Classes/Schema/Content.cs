using System;
using Newtonsoft.Json;

namespace Solution
{
	public class Content
	{
		[JsonProperty(PropertyName = "imgx")]
		public float ImgX { get; set; }

		[JsonProperty(PropertyName = "imgy")]
		public float ImgY { get; set; }

		[JsonProperty(PropertyName = "imgw")]
		public float ImgW { get; set; }

		[JsonProperty(PropertyName = "imgh")]
		public float ImgH { get; set; }

		[JsonProperty(PropertyName = "userID")]
		public string UserId { get; set; }
	}
}

