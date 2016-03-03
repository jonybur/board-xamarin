using Board.Schema;
using System;
using CoreGraphics;
using UIKit;

namespace Board.Interface.Widgets
{
	public class AnnouncementWidget : Widget
	{
		// UIView contains ScrollView and BackButton
		// ScrollView contains LookUpImage
		private Announcement announcement;
		private UITextView textview;

		public Announcement Announcement
		{
			get { return announcement; }
		}

		public AnnouncementWidget()
		{

		}

		public AnnouncementWidget(Announcement ann)
		{
			announcement = ann;

			UITextView insideText = CreateText ();

			// mounting
			UIImageView mounting = CreateMounting (insideText.Frame);
			View = new UIView(mounting.Frame);
			View.AddSubviews (mounting, insideText);

			// like
			UIImageView like = CreateLike (mounting.Frame);
			View.AddSubview (like);

			// like label

			UILabel likeLabel = CreateLikeLabel (like.Frame);
			View.AddSubview (likeLabel);

			// eye
			eye = CreateEye (mounting.Frame);

			View.AddSubview (eye);

			View.Frame = new CGRect (ann.Frame.X, ann.Frame.Y, mounting.Frame.Width, mounting.Frame.Height);
			View.Transform = CGAffineTransform.MakeRotation(ann.Rotation);

			EyeOpen = false;
		}

		public void ScrollEnabled(bool value)
		{
			textview.ScrollEnabled = value;
		}

		private UITextView CreateText()
		{
			UIFont font = UIFont.SystemFontOfSize (20);

			textview = new UITextView ();
			textview.BackgroundColor = UIColor.FromRGB(250,250,250);
			textview.Editable = false;
			textview.Selectable = true;
			textview.ScrollEnabled = true;
			textview.AttributedText = announcement.Text;
			textview.SizeToFit ();
			textview.TextColor = BoardInterface.board.MainColor;

			if (textview.Frame.Width < 160) {
				textview.Frame = new CGRect (10, 10, 160, textview.Frame.Height);
			} else if (textview.Frame.Width > 250) {
				textview.Frame = new CGRect (10, 10, 250, textview.Frame.Height);
			}

			if (textview.Frame.Height < 80) {
				textview.Frame = new CGRect (10, 10, textview.Frame.Width, 80);
			} else if (textview.Frame.Height > 180) {
				textview.Frame = new CGRect (10, 10, textview.Frame.Width, 180);
			}

			return textview;
		}

		public void SetFrame(CGRect frame)
		{
			View.Frame = frame;
		}

	}
}

