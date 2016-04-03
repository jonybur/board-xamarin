using System;
using UIKit;
using CoreGraphics;
using Board.Schema;

namespace Board.Interface
{
	public class InfoBox : UIView
	{
		UILabel NameLabel;

		public InfoBox (Board.Schema.Board board)
		{
			Frame = new CGRect (30, 0, AppDelegate.ScreenWidth - 60, AppDelegate.ScreenHeight - 200);

			NameLabel = new UILabel (new CGRect (10, 20, Frame.Width-20, 24));
			NameLabel.Font = AppDelegate.Narwhal24;
			NameLabel.TextColor = UIColor.White;
			NameLabel.TextAlignment = UITextAlignment.Center;
			NameLabel.AdjustsFontSizeToFitWidth = true;
			NameLabel.Text = board.Name;

			Center = new CGPoint (BoardInterface.ScrollViewWidthSize / 2 + (BoardInterface.BannerHeight - BoardInterface.ButtonBarHeight) / 2, AppDelegate.ScreenHeight / 2);

			BackgroundColor = UIColor.FromRGBA (0, 0, 0, 100);

			AddSubview (NameLabel);
		}
	}
}

