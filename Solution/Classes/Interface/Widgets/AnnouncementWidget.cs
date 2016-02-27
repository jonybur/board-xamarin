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

			eyeOpen = false;
		}

		private UITextView CreateText()
		{
			UIFont font = UIFont.SystemFontOfSize (20);
			UITextView textview = new UITextView (new CGRect(10, 10, 250, 100));
			textview.Font = font;
			textview.BackgroundColor = UIColor.FromRGB(250,250,250);
			textview.TextColor = AppDelegate.BoardBlue;
			textview.Editable = false;
			textview.Selectable = true;
			textview.Text = announcement.Text;
			textview.SizeToFit ();

			if (textview.Frame.Width < 150) {
				textview.Frame = new CGRect (10, 10, 150, textview.Frame.Height);
			} else if (textview.Frame.Width > 300) {
				textview.Frame = new CGRect (10, 10, 330, textview.Frame.Height);
			}

			if (textview.Frame.Height < 100) {
				textview.Frame = new CGRect (10, 10, textview.Frame.Width, 80);
			} else if (textview.Frame.Height > 300) {
				textview.Frame = new CGRect (10, 10, textview.Frame.Width, 300);
			}

			font = UIFont.SystemFontOfSize ((textview.Frame.Width * 20 / AppDelegate.ScreenWidth));
			textview.Font = font;

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
			likeLabel.TextColor = AppDelegate.BoardOrange;
			likeLabel.Font = likeFont;
			likeLabel.Text = likeText;
			likeLabel.TextAlignment = UITextAlignment.Right;

			return likeLabel;
		}

		private UIImageView CreateEye(CGRect frame)
		{
			CGSize iconSize = new CGSize (30, 30);

			UIImageView eyeView = new UIImageView(new CGRect (frame.X + 10, frame.Height - iconSize.Height - 5, iconSize.Width, iconSize.Height));

			eyeView.Image = Widget.ClosedEyeImage;
			eyeView.TintColor = UIColor.FromRGB(140,140,140);

			return eyeView;
		}

		private UIImageView CreateLike(CGRect frame)
		{
			CGSize iconSize = new CGSize (30, 30);

			UIImageView likeView = new UIImageView(new CGRect (frame.Width - iconSize.Width - 10,
				frame.Height - iconSize.Height - 5, iconSize.Width, iconSize.Height));
			likeView.Image = UIImage.FromFile ("./boardinterface/widget/like.png");
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

			UIImage orange = UIGraphics.GetImageFromCurrentImageContext ();
			UIImageView uiv = new UIImageView (orange);
			uiv.Frame = frame;

			return uiv;
		}

		public void SetFrame(CGRect frame)
		{
			View.Frame = frame;
		}

	}
}

