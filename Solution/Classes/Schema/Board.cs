using System;
using Newtonsoft.Json;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;
using System.Threading.Tasks;

using System.Collections.Generic;


namespace Solution
{
	public class Board
	{
		public UIImage Image;
		public UIColor MainColor;
		public UIColor SecondaryColor;
		public GoogleGeolocatorObject GeolocatorObject;
		public string Location;
		public string Name;
		public string CreatorId;

		public Board()
		{
		}

		public Board (string name, UIImage image, UIColor mainColor, UIColor secondaryColor, string location, string creatorId)
		{
			Image = image;
			MainColor = mainColor;
			SecondaryColor = secondaryColor;
			Location = location;
			Name = name;
			CreatorId = creatorId;
		}
	}
}

