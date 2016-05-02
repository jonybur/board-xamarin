using System;
using UIKit;
using CoreGraphics;

namespace Board.Screens
{
	public class UILocationLabel : UILabel{

		public UILocationLabel(float yposition, string location)
		{
			Frame = new CGRect(10, yposition, AppDelegate.ScreenWidth - 20, 24);
			Font = AppDelegate.Narwhal20;
			TextAlignment = UITextAlignment.Center;
			TextColor = AppDelegate.BoardOrange;
			Text = location;
		}
	}

}

