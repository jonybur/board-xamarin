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

		private UIImageView CreateMounting(CGRect frame)
		{
			CGRect mountingFrame = new CGRect (0, 0, frame.Width + 20, frame.Height + 50);

			UIImageView mountingView = CreateColorView (mountingFrame, UIColor.FromRGB(250,250,250).CGColor);

			return mountingView;
		}

		private UILabel CreateLikeLabel(CGRect frame)
		{
			UIFont likeFont = UIFont.SystemFontOfSize (20);
			Random rand = new Random ();
			string likeText = rand.Next(16, 98).ToString();
			CGSize likeLabelSize = likeText.StringSize (likeFont);
			UILabel likeLabel = new UILabel(new CGRect(frame.X - likeLabelSize.Width - 4, frame.Y + 4, likeLabelSize.Width, likeLabelSize.Height));
			likeLabel.TextColor = BoardInterface.board.MainColor;
			likeLabel.Font = likeFont;
			likeLabel.Text = likeText;
			likeLabel.TextAlignment = UITextAlignment.Right;

			return likeLabel;
		}

		private UIImageView CreateEye(CGRect frame)
		{
			CGSize iconSize = new CGSize (30, 30);

			UIImageView eyeView = new UIImageView(new CGRect (frame.X + 10, frame.Height - iconSize.Height - 5, iconSize.Width, iconSize.Height));

			eyeView.Image = Widget.ClosedEyeImageView.Image;
			eyeView.TintColor = UIColor.FromRGB(140,140,140);

			return eyeView;
		}

		private UIImageView CreateLike(CGRect frame)
		{
			CGSize iconSize = new CGSize (30, 30);

			UIImageView likeView = new UIImageView(new CGRect (frame.Width - iconSize.Width - 10,
				frame.Height - iconSize.Height - 5, iconSize.Width, iconSize.Height));

			using (UIImage image = UIImage.FromFile ("./boardinterface/widget/like.png")) {
				likeView.Image = image;
			}

			likeView.Image = likeView.Image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
			likeView.TintColor = UIColor.FromRGB(140,140,140);

			return likeView;
		}


		private UIImageView CreateColorView(CGRect frame, CGColor color)
		{
			UIGraphics.BeginImageContext (new CGSize(frame.Size.Width, frame.Size.Height));
			CGContext context = UIGraphics.GetCurrentContext ();

			context.SetFillColor(color);
			context.FillRect(frame);

			UIImageView uiv;
			using (UIImage img = UIGraphics.GetImageFromCurrentImageContext ()) {
				uiv = new UIImageView (img);
			}
			uiv.Frame = frame;

			return uiv;
		}

		public void SetFrame(CGRect frame)
		{
			View.Frame = frame;
		}

	}
}

