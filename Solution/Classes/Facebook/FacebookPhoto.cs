using System.Collections.Generic;
using System;

namespace Board.Facebook
{
	public class FacebookPhoto : FacebookElement
	{
		public string Name;
		public string CreatedTime;

		public FacebookPhoto(string id, string name, string createdtime)
		{
			if (name != "<null>") {
				Name = name;
			}
			CreatedTime = createdtime;
			Id = id;
		}
	}

	public class FacebookImage : FacebookElement {
		public int Height;
		public string Source;
		public int Width;

		public FacebookImage(){}

		public FacebookImage (string id, string height, string source, string width){
			Height = Int32.Parse(height);
			Source = source;
			Width = Int32.Parse(width);
			Id = id;
		}
	}
}

