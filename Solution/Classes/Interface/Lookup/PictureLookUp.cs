using Board.Interface.Buttons;

using Board.Schema;

using CoreGraphics;
using Facebook.CoreKit;
using UIKit;

namespace Board.Interface.LookUp
{
	public class PictureLookUp : LookUp
	{
		UITapGestureRecognizer doubletap;
		UIScrollViewGetZoomView zoomView;

		public PictureLookUp(Picture picture)
		{
			content = picture;

			View.BackgroundColor = UIColor.Black;

			CreateButtons (UIColor.White);

			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			ScrollView.UserInteractionEnabled = true;

			UIImageView lookUpImage = CreateImageFrame (picture.ImageView.Image);
			ScrollView.AddSubview (lookUpImage);
			ScrollView.MaximumZoomScale = 4f;
			ScrollView.MinimumZoomScale = 1f;

			zoomView = sv => lookUpImage;

			doubletap = new UITapGestureRecognizer  ((tg) => {
				if (ScrollView.ZoomScale > 1)
					ScrollView.SetZoomScale(1f, true);
				else
					ScrollView.SetZoomScale(3f, true);

				tg.NumberOfTapsRequired = 2;
			});

			View.AddSubviews (ScrollView, BackButton, LikeButton, FacebookButton, ShareButton, TrashButton);
		}

		private UIImageView CreateImageFrame(UIImage image)
		{
			float imgw, imgh;
			float scale = (float)(image.Size.Height/image.Size.Width);

			imgw = AppDelegate.ScreenWidth;
			imgh = AppDelegate.ScreenWidth * scale;

			UIImageView imageView = new UIImageView (new CGRect (0, AppDelegate.ScreenHeight/2 - imgh / 2, imgw, imgh));
			imageView.Layer.AnchorPoint = new CGPoint(.5f, .5f);
			imageView.Image = image;
			imageView.UserInteractionEnabled = true;

			UIImageView blackTop = new UIImageView (new CGRect (0, 0, AppDelegate.ScreenWidth, imageView.Frame.Top));
			blackTop.BackgroundColor = UIColor.Black;

			UIImageView composite = new UIImageView(new CGRect(0,0,AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));

			composite.AddSubviews (blackTop, imageView);

			return composite;
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear (animated);
			ScrollView.AddGestureRecognizer (doubletap);
			ScrollView.ViewForZoomingInScrollView += zoomView;
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear (animated);
			ScrollView.RemoveGestureRecognizer (doubletap);
			ScrollView.ViewForZoomingInScrollView -= zoomView;
		}
	}
}

