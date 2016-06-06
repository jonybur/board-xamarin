using AssetsLibrary;
using Board.Schema;
using Board.Utilities;
using CoreGraphics;
using UIKit;
using Board.Interface.LookUp;
using Board.Interface.CreateScreens;

namespace Board.Interface.FacebookImport
{
	public class PictureImportLookUp : UILookUp
	{
		UITapGestureRecognizer doubletap, NextTap;
		UILongPressGestureRecognizer longpress;
		UIScrollViewGetZoomView zoomView;
		UIImageView InternalImageView, NextButton;

		public PictureImportLookUp(Picture picture)
		{
			content = picture;

			View.BackgroundColor = UIColor.Black;

			CreateButtons (UIColor.White);

			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			ScrollView.UserInteractionEnabled = true;

			var lookUpImage = CreateImageFrame (picture.Image);
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
				var alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);

				alert.AddAction (UIAlertAction.Create ("Save Photo", UIAlertActionStyle.Default, SavePhoto));
				alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));	

				AppDelegate.NavigationController.PresentViewController(alert, true, null);	
			});

			longpress.MinimumPressDuration = .3f;

			CreateNextButton (UIColor.White);

			var descriptionBox = CreateDescriptionBox (picture.Description);
			descriptionBox.Center = new CGPoint (AppDelegate.ScreenWidth / 2, LikeButton.Frame.Top - descriptionBox.Frame.Height / 2 - 5);

			View.AddSubviews (ScrollView, descriptionBox, BackButton, FacebookButton, NextButton);
		}

		private UITextView CreateDescriptionBox(string description){
			var textview = new UITextView ();

			textview.Editable = false;
			textview.Selectable = false;
			textview.ScrollEnabled = true;
			textview.DataDetectorTypes = UIDataDetectorType.Link;
			textview.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
			textview.Text = description;
			textview.Font = UIFont.SystemFontOfSize (14);
			textview.TextColor = UIColor.White;
			var size = textview.SizeThatFits (new CGSize (AppDelegate.ScreenWidth - 10, 60));
			textview.Frame = new CGRect (5, 0, size.Width, size.Height);

			textview.ContentOffset = new CGPoint (0, 0);

			return textview;
		}

		private void CreateNextButton(UIColor buttonColor)
		{
			using (UIImage img = UIImage.FromFile ("./camera/nextbutton.png")) {
				NextButton = new UIImageView(new CGRect(0,0,img.Size.Width * 2,img.Size.Height * 2));

				var subView = new UIImageView (new CGRect (0, 0, img.Size.Width / 2, img.Size.Height / 2));
				subView.Image = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				subView.Center = new CGPoint (NextButton.Frame.Width / 2, NextButton.Frame.Height / 2);
				subView.TintColor = buttonColor;

				NextButton.AddSubview (subView);
				NextButton.Center = new CGPoint (AppDelegate.ScreenWidth - img.Size.Width / 2 - 5, AppDelegate.ScreenHeight - img.Size.Height / 2 - 5);
			}

			NextButton.UserInteractionEnabled = true;

			NextTap = new UITapGestureRecognizer (tg => {
				var createScreen = new CreateMediaScreen((Picture)content);
				AppDelegate.NavigationController.PushViewController(createScreen, true);
			});
		}

		private async void SavePhoto(UIAlertAction action)
		{
			var lib = new ALAssetsLibrary ();
			await lib.WriteImageToSavedPhotosAlbumAsync(((Picture)content).Image.CGImage, ALAssetOrientation.Up);
			lib.Dispose();
		}

		private UIImageView CreateImageFrame(UIImage image)
		{
			float imgw, imgh;
			float scale = (float)(image.Size.Height/image.Size.Width);

			imgw = AppDelegate.ScreenWidth;
			imgh = AppDelegate.ScreenWidth * scale;

			InternalImageView = new UIImageView (new CGRect (0, AppDelegate.ScreenHeight/2 - imgh / 2, imgw, imgh));
			InternalImageView.Layer.AnchorPoint = new CGPoint(.5f, .5f);
			InternalImageView.Image = image;
			InternalImageView.UserInteractionEnabled = true;

			var blackTop = new UIImageView (new CGRect (0, 0, AppDelegate.ScreenWidth, InternalImageView.Frame.Top));
			blackTop.BackgroundColor = UIColor.Black;

			var composite = new UIImageView(new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));

			composite.AddSubviews (blackTop, InternalImageView);

			return composite;
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear (animated);
			ScrollView.AddGestureRecognizer (doubletap);
			ScrollView.AddGestureRecognizer (longpress);
			NextButton.AddGestureRecognizer (NextTap);
			ScrollView.ViewForZoomingInScrollView += zoomView;
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear (animated);
			ScrollView.RemoveGestureRecognizer (doubletap);
			ScrollView.RemoveGestureRecognizer (longpress);
			NextButton.RemoveGestureRecognizer (NextTap);
			ScrollView.ViewForZoomingInScrollView -= zoomView;
			ScrollView = null;

			InternalImageView.Image = null;

			MemoryUtility.ReleaseUIViewWithChildren (View);
		}
	}
}

