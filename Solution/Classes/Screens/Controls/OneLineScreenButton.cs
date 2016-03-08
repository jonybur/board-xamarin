using CoreGraphics;
using UIKit;

namespace Board.Screens.Controls
{
	public class OneLineScreenButton : ScreenButton
	{
		public OneLineScreenButton(){}

		public OneLineScreenButton(float yPosition)
		{
			Frame = new CGRect (0, yPosition, AppDelegate.ScreenWidth, 80);
			UserInteractionEnabled = true;

			UIFont nameFont = UIFont.SystemFontOfSize (18);
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

