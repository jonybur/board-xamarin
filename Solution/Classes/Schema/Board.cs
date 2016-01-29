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
		public string Location;
		public string Name;

		public Board()
		{
		}

		public Board (string name, string imageAddress, UIColor mainColor, UIColor secondaryColor, string location)
		{
			Image = UIImage.FromFile (imageAddress);
			MainColor = mainColor;
			SecondaryColor = secondaryColor;
			Location = location;
			Name = name;
		}
	}
}

