using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Foundation;

namespace Board.JsonResponses	
{	
	[Preserve(AllMembers = true)]
	public class MagazineResponse
	{
		public DateTime UpdatedTime;

		[Preserve(AllMembers = true)]
		public class Entry
		{
			public string magazineID { get; set; }
			public string uuid { get; set; }
			public string boardID { get; set; }
			public string section { get; set; }
			public BoardResponse.Datum board { get; set; }
		}

		[Preserve(AllMembers = true)]
		public class Data
		{
			public string uuid { get; set; }
			public double latitude { get; set; }
			public double longitude { get; set; }
			public List<Entry> entries { get; set; }
		}

		public MagazineResponse(){
			UpdatedTime = DateTime.Now;
		}

		public Data data { get; set; }

		public static MagazineResponse Deserialize (string json)
		{
			try {
				return JsonConvert.DeserializeObject<MagazineResponse>(json);
			} catch {
				return null;
			}
		}

		public static bool IsValidMagazine(MagazineResponse magazine){
			if (magazine != null) {
				if (magazine.data != null) {
					if (magazine.data.entries != null) {
						if (magazine.data.entries.Count > 0) {
							return true;
						}
					}
				}
			}
			return false;
		}

	}
}

