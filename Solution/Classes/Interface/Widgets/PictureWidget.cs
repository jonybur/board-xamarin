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

			CreateMounting (frame);
			View = new UIView(mountingView.Frame);
			View.AddSubview (mountingView);

			// picture

			CGRect pictureFrame = new CGRect (mountingView.Frame.X + 10, 10, frame.Width, frame.Height);
			UIImageView uiv = new UIImageView (pictureFrame);
			uiv.Image = picture.ThumbnailView.Image;
			View.AddSubview (uiv);

			// like

			UIImageView like = CreateLike (mountingView.Frame);
			View.AddSubview (like);

			// like label

			UILabel likeLabel = CreateLikeLabel (like.Frame);
			View.AddSubview (likeLabel);

			// eye

			eye = CreateEye (mountingView.Frame);
			View.AddSubview (eye);

			View.Frame = new CGRect (pic.Frame.X, pic.Frame.Y, mountingView.Frame.Width, mountingView.Frame.Height);
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

