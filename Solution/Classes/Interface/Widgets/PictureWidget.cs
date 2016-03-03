using Board.Schema;
using Board.Utilities;
using System;
using CoreGraphics;
using UIKit;
using Board.Interface.LookUp;

namespace Board.Interface.Widgets
{
	public class PictureWidget : Widget
	{
		// UIView contains ScrollView and BackButton
		// ScrollView contains LookUpImage

		private Picture picture;

		public Picture Picture
		{
			get { return picture; }
		}

		public PictureWidget()
		{

		}

		public PictureWidget(Picture pic)
		{
			picture = pic;

			CGRect frame = GetFrame (pic);

			// mounting

			UIImageView mounting = CreateMounting (frame);
			View = new UIView(mounting.Frame);
			View.AddSubview (mounting);

			// picture

			CGRect pictureFrame = new CGRect (mounting.Frame.X + 10, 10, frame.Width, frame.Height);
			UIImageView uiv = new UIImageView (pictureFrame);
			uiv.Image = picture.ThumbnailView.Image;
			View.AddSubview (uiv);

			// like

			UIImageView like = CreateLike (mounting.Frame);
			View.AddSubview (like);

			// like label

			UILabel likeLabel = CreateLikeLabel (like.Frame);
			View.AddSubview (likeLabel);

			// eye

			eye = CreateEye (mounting.Frame);
			View.AddSubview (eye);

			View.Frame = new CGRect (pic.Frame.X, pic.Frame.Y, mounting.Frame.Width, mounting.Frame.Height);
			View.Transform = CGAffineTransform.MakeRotation(pic.Rotation);

			View.BackgroundColor = UIColor.FromRGB (250, 250, 250);

			EyeOpen = false;

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				if (Preview.View != null) { return; }

				PictureLookUp lookUp = new PictureLookUp(picture);
				AppDelegate.NavigationController.PresentViewController(lookUp, true, null);
			});

			gestureRecognizers.Add (tap);

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

			using (UIImage image = UIImage.FromFile ("./boardinterface/widget/like.png")){
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

		private CGRect GetFrame(Picture picture)
		{
			float imgw, imgh;
			float autosize = AppDelegate.Autosize;

			float scale = (float)(picture.ImageView.Frame.Width/picture.ImageView.Frame.Height);

			if (scale >= 1) {
				imgw = autosize * scale;
				imgh = autosize;

				if (imgw > AppDelegate.ScreenWidth) {
					scale = (float)(picture.ImageView.Frame.Height/picture.ImageView.Frame.Width);
					imgw = AppDelegate.ScreenWidth;
					imgh = imgw * scale;
				}
			} else {
				scale = (float)(picture.ImageView.Frame.Height / picture.ImageView.Frame.Width);
				imgw = autosize;
				imgh = autosize * scale;
			}

			picture.ThumbnailView = new UIImageView(CommonUtils.ResizeImage (picture.ImageView.Image, new CGSize (imgw, imgh)));

			CGRect frame = new CGRect (picture.Frame.X, picture.Frame.Y, imgw, imgh);

			return frame;
		}

		public void SetFrame(CGRect frame)
		{
			View.Frame = frame;
		}

	}
}

