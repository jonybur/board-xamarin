using Board.Schema;
using Board.Utilities;
using System;
using CoreGraphics;
using UIKit;
using MGImageUtilitiesBinding;
using Board.Interface.LookUp;

namespace Board.Interface.Widgets
{
	public class PictureWidget : Widget
	{
		// UIView contains ScrollView and BackButton
		// ScrollView contains LookUpImage

		public Picture picture
		{
			get { return (Picture)content; }
		}

		public PictureWidget()
		{

		}

		public PictureWidget(Picture pic)
		{
			content = pic;

			CGRect frame = GetFrame (pic);

			// mounting

			CreateMounting (frame);
			View = new UIView(MountingView.Frame);
			View.AddSubview (MountingView);

			// picture

			CGRect pictureFrame = new CGRect (MountingView.Frame.X + 10, 10, frame.Width, frame.Height);
			UIImageView uiv = new UIImageView (pictureFrame);
			uiv.Image = picture.ThumbnailView.Image;
			uiv.Layer.AllowsEdgeAntialiasing = true;
			View.AddSubview (uiv);

			View.Frame = new CGRect (pic.Position.X, pic.Position.Y, MountingView.Frame.Width, MountingView.Frame.Height);
			View.Transform = CGAffineTransform.MakeRotation(pic.Rotation);

			View.BackgroundColor = UIColor.FromRGB (250, 250, 250);
		
			EyeOpen = false;

			CreateGestures ();
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

			picture.ThumbnailView = new UIImageView(picture.ImageView.Image.ImageScaledToFitSize(new CGSize (imgw, imgh)));

			CGRect frame = new CGRect (picture.Position.X, picture.Position.Y, imgw, imgh);

			return frame;
		}

		public void SetFrame(CGRect frame)
		{
			View.Frame = frame;
		}

	}
}

