using CoreGraphics;
using UIKit;

namespace Clubby.Screens.Controls
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

			var Label = new UILabel ();
			Label.Frame = new CGRect (20, 0, AppDelegate.ScreenWidth - 50, FontHeight + 3);
			Label.Center = new CGPoint (Label.Center.X, Frame.Height / 2);
			Label.Font = nameFont;
			Label.AdjustsFontSizeToFitWidth = true;

			ListLabels.Add (Label);

			var nextLabel = new UILabel ();
			nextLabel.Frame = new CGRect (0, 0, 25, 20);
			nextLabel.Center = new CGPoint (AppDelegate.ScreenWidth - 20, 30);
			nextLabel.Font = nameFont;
			nextLabel.Text = ">";

			ListLabels.Add (nextLabel);

			SetUnpressedColors ();

			AddSubviews (Label, nextLabel);
		}

		public void SetLabel(string text)
		{
			ListLabels[0].Text = text;
		}
	}
}

