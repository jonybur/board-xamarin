using System.Collections.Generic;
using Newtonsoft.Json;
using Foundation;

namespace Board.JsonResponses
{
	[Preserve(AllMembers = true)]
	public class BoardResponse
	{
		public static BoardResponse Deserialize (string json)
		{
			try {
				return JsonConvert.DeserializeObject<BoardResponse>(json);
			}catch{
				return null;
			}
		}

		[Preserve(AllMembers = true)]
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

		public List<Datum> data { get; set; }
	}
}

