using System;
using UIKit;
using CoreGraphics;

namespace Board.Screens
{
	public class LocationLabel : UILabel{
		public static UIFont font;

		public LocationLabel(float yposition, string location)
		{
			Frame = new CGRect(10, yposition, AppDelegate.ScreenWidth - 20, 24);
			Font = font;
			TextAlignment = UITextAlignment.Center;
			TextColor = AppDelegate.BoardOrange;
			Text = location;
		}
	}

}

