using CoreGraphics;
using UIKit;

namespace Board.Screens.Controls
{
	public class UIOneLineMenuButton : UIMenuButton
	{
		public UIOneLineMenuButton(){}

		public const float Height = 60;
		public const int FontHeight = 16;

		public UIOneLineMenuButton(float yPosition)
		{
			Frame = new CGRect (0, yPosition, AppDelegate.ScreenWidth, Height);
			UserInteractionEnabled = true;

			var nameFont = UIFont.SystemFontOfSize(FontHeight, UIFontWeight.Light);
			var Label = new UILabel (new CGRect (20, 0, AppDelegate.ScreenWidth - 50, FontHeight));
			Label.Center = new CGPoint (Label.Center.X, Frame.Height / 2);
			Label.Font = nameFont;
			Label.AdjustsFontSizeToFitWidth = true;

			ListLabels.Add (Label);
			SetUnpressedColors ();

			AddSubviews (Label);
		}

		public void SetLabel(string text)
		{
			ListLabels[0].Text = text;
		}
	}
}

