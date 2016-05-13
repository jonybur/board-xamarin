using Board.Schema;
using CoreGraphics;
using AssetsLibrary;
using UIKit;

namespace Board.Interface.LookUp
{
	public class PictureLookUp : UILookUp
	{
		UITapGestureRecognizer doubletap;
		UILongPressGestureRecognizer longpress;
		UIScrollViewGetZoomView zoomView;

		public PictureLookUp(Picture picture)
		{
			content = picture;

			View.BackgroundColor = UIColor.Black;

			CreateButtons (UIColor.White);

			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			ScrollView.UserInteractionEnabled = true;

			UIImageView lookUpImage = CreateImageFrame (picture.Image);
			ScrollView.AddSubview (lookUpImage);
			ScrollView.MaximumZoomScale = 4f;
			ScrollView.MinimumZoomScale = 1f;

			zoomView = sv => lookUpImage;

			doubletap = new UITapGestureRecognizer  (tg => {
				if (ScrollView.ZoomScale > 1)
					ScrollView.SetZoomScale(1f, true);
				else
					ScrollView.SetZoomScale(3f, true);
			});

			doubletap.NumberOfTapsRequired = 2;

			longpress = new UILongPressGestureRecognizer (tg => {
				UIAlertController alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);

				alert.AddAction (UIAlertAction.Create ("Save Photo", UIAlertActionStyle.Default, SavePhoto));
				alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));	

				AppDelegate.NavigationController.PresentViewController(alert, true, null);	
			});

			longpress.MinimumPressDuration = .3f;

			View.AddSubviews (ScrollView, BackButton, LikeButton, FacebookButton, TrashButton);
		}

		private async void SavePhoto(UIAlertAction action)
		{
			ALAssetsLibrary lib = new ALAssetsLibrary ();
			await lib.WriteImageToSavedPhotosAlbumAsync(((Picture)content).Image.CGImage, ALAssetOrientation.Up);
			lib.Dispose();
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
			ScrollView.AddGestureRecognizer (longpress);
			ScrollView.ViewForZoomingInScrollView += zoomView;
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear (animated);
			ScrollView.RemoveGestureRecognizer (doubletap);
			ScrollView.RemoveGestureRecognizer (longpress);
			ScrollView.ViewForZoomingInScrollView -= zoomView;
			ScrollView = null;
		}


		/*
		if (sideMenuIsUp)
				{ sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; return; }
		*/
	}
}

