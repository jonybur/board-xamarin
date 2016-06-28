using System;
using UIKit;
using CoreGraphics;
using Board.Utilities;

namespace Board.Screens
{
	public class UILocationLabel : UILabel{

		public const int Height = 20;

		public UILocationLabel(string text, float indent = 10 ,UITextAlignment alignment = UITextAlignment.Left)
		{
			Frame = new CGRect(indent, 0, AppDelegate.ScreenWidth - indent, Height);

			SetProperties (text, alignment);
		}

		public UILocationLabel(string text, CGPoint point, UITextAlignment alignment = UITextAlignment.Left)
		{
			Frame = new CGRect(point.X, point.Y, AppDelegate.ScreenWidth - point.X * 2, Height);

			SetProperties (text, alignment);
		}

		private void SetProperties(string text, UITextAlignment alignment){
			Font = UIFont.SystemFontOfSize (16, UIFontWeight.Medium);//AppDelegate.Narwhal18;
			TextColor = AppDelegate.BoardBlack;//AppDelegate.BoardOrange;
			AdjustsFontSizeToFitWidth = true;
			Text = CommonUtils.FirstLetterOfEveryWordToUpper(text);
			TextAlignment = alignment;
		}
	}

}

