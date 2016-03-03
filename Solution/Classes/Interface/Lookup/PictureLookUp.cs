using Board.Interface.Buttons;

using Board.Schema;

using CoreGraphics;
using UIKit;

namespace Board.Interface.LookUp
{
	public class PictureLookUp : LookUp
	{
		UITapGestureRecognizer doubletap;
		UIScrollViewGetZoomView zoomView;

		public PictureLookUp(Picture picture)
		{
			View.BackgroundColor = UIColor.Black;

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

			ScrollView.UserInteractionEnabled = true;

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

			UIImageView blackTop = CreateColorView(new CGRect(0,0,AppDelegate.ScreenWidth, imageView.Frame.Top), UIColor.Black.CGColor);

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
	}
}

