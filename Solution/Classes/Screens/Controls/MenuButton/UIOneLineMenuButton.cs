using CoreGraphics;
using UIKit;

namespace Board.Screens.Controls
{
	public class UIOneLineMenuButton : UIMenuButton
	{
		public UIOneLineMenuButton(){}

		public const float Height = 80;

		public UIOneLineMenuButton(float yPosition)
		{
			Frame = new CGRect (0, yPosition, AppDelegate.ScreenWidth, Height);
			UserInteractionEnabled = true;

			UIFont nameFont = AppDelegate.SystemFontOfSize18;
			UILabel Label = new UILabel (new CGRect (42, 30, AppDelegate.ScreenWidth - 50, 20));
			Label.Font = nameFont;
			Label.AdjustsFontSizeToFitWidth = true;

			ListLabels.Add (Label);

			AddSubviews (Label);
		}

		public void SetLabel(string text)
		{
			ListLabels[0].Text = text;
		}
	}
}

