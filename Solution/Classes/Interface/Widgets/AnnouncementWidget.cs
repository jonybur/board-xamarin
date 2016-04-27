using Board.Schema;
using CoreGraphics;
using UIKit;

namespace Board.Interface.Widgets
{
	public class AnnouncementWidget : Widget
	{
		// UIView contains ScrollView and BackButton
		// ScrollView contains LookUpImage
		private UITextView textview;

		public Announcement announcement
		{
			get { return (Announcement)content; }
		}

		public AnnouncementWidget()
		{
		}

		public AnnouncementWidget(Announcement ann)
		{
			content = ann;

			UITextView insideText = CreateText ();

			// mounting
			CreateMounting (insideText.Frame);
			View = new UIView (MountingView.Frame);

			View.AddSubviews (MountingView, insideText);

			EyeOpen = false;

			CreateGestures ();
		}

		public void ScrollEnabled(bool value)
		{
			textview.ScrollEnabled = value;
		}

		private UITextView CreateText()
		{
			textview = new UITextView ();
			textview.Editable = false;
			textview.Selectable = false;
			textview.ScrollEnabled = true;
			textview.DataDetectorTypes = UIDataDetectorType.Link;
			textview.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
			textview.AttributedText = announcement.AttributedText;
			textview.TextColor = Widget.HighlightColor;
			textview.SizeToFit ();

			if (textview.Frame.Width < 160) {
				textview.Frame = new CGRect (SideMargin, TopMargin, 160, textview.Frame.Height);
			} else if (textview.Frame.Width > 250) {
				textview.Frame = new CGRect (SideMargin, TopMargin, 250, textview.Frame.Height);
			}

			if (textview.Frame.Height < 80) {
				textview.Frame = new CGRect (SideMargin, TopMargin, textview.Frame.Width, 80);
			} else if (textview.Frame.Height > 180) {
				textview.Frame = new CGRect (SideMargin, TopMargin, textview.Frame.Width, 180);
			}

			if (textview.Frame.Height < 81 && textview.Text.Length > 90) {
				float height = ((textview.Text.Length - 90) / 30) * 20 + 80;
				textview.Frame = new CGRect (SideMargin, TopMargin, textview.Frame.Width, height);
			}

			textview.ContentOffset = new CGPoint (0, 0);

			return textview;
		}

		public void SetFrame(CGRect frame)
		{
			View.Frame = frame;
		}

	}
}

