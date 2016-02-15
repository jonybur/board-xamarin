using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Board.Schema
{
	public class Content
	{
		public float ImgX { get; set; }

		public float ImgY { get; set; }

		public float ImgW { get; set; }

		public float ImgH { get; set; }

		public string UserId { get; set; }

		public List<int> SocialChannel { get; set; }
	}
}

