using CoreGraphics;
using UIKit;

namespace Board.Screens.Controls
{
	public class TwoLinesMenuButton : MenuButton
	{
		public TwoLinesMenuButton(float yPosition)
		{
			Frame = new CGRect (0, yPosition, AppDelegate.ScreenWidth, 80);
			UserInteractionEnabled = true;

			UIFont nameFont = AppDelegate.SystemFontOfSize18;
			UILabel Label = new UILabel (new CGRect (40, 22, AppDelegate.ScreenWidth - 50, 18));
			Label.Font = nameFont;
			Label.AdjustsFontSizeToFitWidth = true;

			UIFont categoryFont = UIFont.SystemFontOfSize(12);
			UILabel SubtitleLabel = new UILabel (new CGRect (40, Label.Frame.Bottom + 5, AppDelegate.ScreenWidth - 50, 14));
			SubtitleLabel.Font = categoryFont;
			SubtitleLabel.AdjustsFontSizeToFitWidth = true;

			ListLabels.Add (Label);
			ListLabels.Add (SubtitleLabel);

			AddSubviews (Label, SubtitleLabel);
		}

		public void SetLabels(string text, string subtitletext)
		{
			ListLabels[0].Text = text;
			ListLabels[1].Text = subtitletext;
		}

	}
}

