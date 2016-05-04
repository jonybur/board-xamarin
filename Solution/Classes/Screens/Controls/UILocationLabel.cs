using System;
using UIKit;
using CoreGraphics;

namespace Board.Screens
{
	public class UILocationLabel : UILabel{

		public const int Height = 20;

		public UILocationLabel(string text, float indent = 10 ,UITextAlignment alignment = UITextAlignment.Left)
		{
			Frame = new CGRect(indent, 0, AppDelegate.ScreenWidth - indent, Height);
			Font = AppDelegate.Narwhal18;
			TextColor = AppDelegate.BoardOrange;
			AdjustsFontSizeToFitWidth = true;
			Text = text;
			TextAlignment = alignment;
		}

		public UILocationLabel(string text, CGPoint point, UITextAlignment alignment = UITextAlignment.Left)
		{
			Frame = new CGRect(point.X, point.Y, AppDelegate.ScreenWidth - point.X * 2, Height);
			Font = AppDelegate.Narwhal18;
			TextColor = AppDelegate.BoardOrange;
			AdjustsFontSizeToFitWidth = true;
			Text = text;
			TextAlignment = alignment;
		}
	}

}

