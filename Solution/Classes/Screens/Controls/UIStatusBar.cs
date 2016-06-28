using System;
using UIKit;
using CoreGraphics;

namespace Board.Screens
{
	public class UIStatusBar : UIView
	{
		public const int Height = 20;

		public UIStatusBar ()
		{
			Frame = new CGRect (0, 0, AppDelegate.ScreenWidth, 20);
			Alpha = .95f;
			BackgroundColor = UIColor.FromRGB(249, 249, 249);
		}
	}
}

