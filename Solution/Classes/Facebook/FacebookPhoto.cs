using System.Collections.Generic;
using System;

namespace Board.Facebook
{
	public class FacebookPhoto : FacebookElement
	{
		public string Name;
		public string CreatedTime;

		List<FacebookImage> Images;

		public FacebookPhoto(string id, string name, string createdtime)
		{
			Name = name;
			CreatedTime = createdtime;
			Id = id;
		}
	}

	public class FacebookImage : FacebookElement {
		public int Height;
		public string Source;
		public int Width;

		public FacebookImage(){}

		public FacebookImage (string height, string source, string width){
			Height = Int32.Parse(height);
			Source = source;
			Width = Int32.Parse(width);
		}
	}
}

