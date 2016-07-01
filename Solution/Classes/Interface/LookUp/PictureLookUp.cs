using AssetsLibrary;
using Clubby.Schema;
using Clubby.Utilities;
using Haneke;
using Foundation;
using CoreGraphics;
using UIKit;

namespace Clubby.Interface.LookUp
{
	public class PictureLookUp : UILookUp
	{
		UITapGestureRecognizer doubletap;
		UIScrollViewGetZoomView zoomView;

		public PictureLookUp(Picture picture, bool likeButton = true)
		{
			content = picture;

			View.BackgroundColor = UIColor.Black;

			CreateButtons (UIColor.White);

			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			ScrollView.UserInteractionEnabled = true;

			var lookUpImage = CreateImageFrame (picture.ImageUrl);
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

			var descriptionBox = CreateDescriptionBox (picture.Description);
			descriptionBox.Center = new CGPoint (AppDelegate.ScreenWidth / 2, LikeButton.Frame.Top - descriptionBox.Frame.Height / 2 - 5);

			View.AddSubviews (ScrollView, descriptionBox, BackButton);
			if (likeButton) {
				View.AddSubview (LikeButton);
			}
		}

		private UITextView CreateDescriptionBox(string description){
			var textview = new UITextView ();

			textview.Editable = false;
			textview.Selectable = false;
			textview.ScrollEnabled = true;
			textview.DataDetectorTypes = UIDataDetectorType.Link;
			textview.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
			if (description != "<null>" && !string.IsNullOrEmpty(description)) {
				textview.Text = description;
			} else {
				textview.Text = string.Empty;
			}
			textview.Font = UIFont.SystemFontOfSize (14);
			textview.TextColor = UIColor.White;
			var size = textview.SizeThatFits (new CGSize (AppDelegate.ScreenWidth - 10, 60));
			textview.Frame = new CGRect (5, 0, size.Width, size.Height);

			textview.ContentOffset = new CGPoint (0, 0);

			return textview;
		}

		/*
		private async void SavePhoto(UIAlertAction action)
		{
			ALAssetsLibrary lib = new ALAssetsLibrary ();
			await lib.WriteImageToSavedPhotosAlbumAsync(((Picture)content).Image.CGImage, ALAssetOrientation.Up);
			lib.Dispose();
		}
		*/

		private UIImageView CreateImageFrame(string imageUrl)
		{
			var InternalImageView = new UIImageView ();

			InternalImageView.Frame = new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);
			InternalImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

			InternalImageView.SetImage (new NSUrl (imageUrl));
			InternalImageView.Center = new CGPoint (AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight / 2);
			InternalImageView.Layer.AnchorPoint = new CGPoint(.5f, .5f);
			InternalImageView.UserInteractionEnabled = true;

			return InternalImageView;
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
			ScrollView = null;

			MemoryUtility.ReleaseUIViewWithChildren (View);
		}
	}
}

