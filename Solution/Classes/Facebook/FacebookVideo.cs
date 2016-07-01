using System;
using System.Collections.Generic;
using Clubby.Utilities;

namespace Clubby.Facebook
{
	public class FacebookVideo : FacebookElement
	{
		public string Description;
		public string UpdatedTime;
		public int Timestamp;
		public string Source;
		public List<string> ThumbnailUris;

		public FacebookVideo (string description, string updatedTime, string id, string source, string thumbnailuri)
		{
			Description = description;
			UpdatedTime = updatedTime;
			Id = id;
			Source = source; 
			Timestamp = CommonUtils.GetUnixTimeStamp(DateTime.Parse (updatedTime));

			var splitted = thumbnailuri.Split (new []{ "\"" }, System.StringSplitOptions.None);

			ThumbnailUris = new List<string> ();
			for (int i = 1; i < splitted.Length; i += 2) {
				ThumbnailUris.Add(splitted [i]);
			}
		}
	}
}

