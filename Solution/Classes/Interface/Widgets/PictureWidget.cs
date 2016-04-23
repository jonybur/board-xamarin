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

			CGRect pictureFrame = new CGRect (MountingView.Frame.X + SideMargin, TopMargin, frame.Width, frame.Height);
			UIImageView uiv = new UIImageView (pictureFrame);
			uiv.Image = picture.ThumbnailView.Image;
			uiv.Layer.AllowsEdgeAntialiasing = true;
			View.AddSubview (uiv);

			EyeOpen = false;

			CreateGestures ();
		}

		private CGRect GetFrame(Picture picture)
		{
			float autosize = Widget.Autosize;

			picture.ThumbnailView = new UIImageView(picture.ImageView.Image.ImageScaledToFitSize(new CGSize (autosize, autosize)));

			CGRect frame = new CGRect (0, 0, picture.ThumbnailView.Frame.Width, picture.ThumbnailView.Frame.Height);

			return frame;
		}

		public void SetFrame(CGRect frame)
		{
			View.Frame = frame;
		}

	}
}

