using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Solution
{
	public class BoardsResponse
	{
		public static BoardsResponse Deserialize (string json)
		{
			try {
				return JsonConvert.DeserializeObject<BoardsResponse>(json);
			}catch{
				return null;
			}
		}

		public List<Datum> data { get; set; }

		public class Datum
		{
			public string uuid { get; set; }
			public string address { get; set; }
			public string name { get; set; }
			public string userId { get; set; }
			public string mainColor { get; set; }
			public string secondaryColor { get; set; }
			public string mainColorCode { get; set; }
			public string secondaryColorCode { get; set; }
			public string logoURL { get; set; }
		}
	}
}

