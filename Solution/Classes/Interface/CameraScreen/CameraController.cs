using Board.Utilities;
using System;
using CoreGraphics;
using UIKit;
using Foundation;
using Board.Schema;
using PBJVisionBinding;
using Board.Interface.CreateScreens;
using System.Threading;

namespace Board.Interface.Camera
{
	public class CameraController : UIViewController{
		CameraPreview cameraPreview;
		VideoPreview videoPreview;
		UIImageView photoPreview;

		UITapGestureRecognizer BackTap, TrashTap, FlipTap, FlashTap, NextTap, FocusTap, DoubleTap;
		UIImageView BackButton, TrashButton, FlipButton, FlashButton, NextButton, FocusImage;
		UIShutterButton ShutterButton;

		UIImage PhotoTaken;
		NSUrl VideoTakenURL;

		public static PBJVision Vision;
		public static string CaptureDirectory;
		bool isFocusing;

		public override void ViewDidAppear (bool animated)
		{ 
			AppDelegate.CameraPhoto = null;
			PhotoTaken = null;
			VideoTakenURL = null;

			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;

			View.UserInteractionEnabled = true;

			CaptureDirectory = (NSFileManager.DefaultManager.GetUrls (
				NSSearchPathDirectory.LibraryDirectory, 
				NSSearchPathDomain.User) [0]).Path;
			
			videoPreview = new VideoPreview ();
			photoPreview = new UIImageView (new CGRect(0,0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			cameraPreview = new CameraPreview ();
			ShutterButton = new UIShutterButton ();

			CreateBackButton (UIColor.White);
			CreateTrashButton (UIColor.White);
			CreateNextButton (UIColor.White);
			CreateFlipButton (UIColor.White);
			CreateFlashButton (UIColor.White);

			View.AddSubviews (cameraPreview, videoPreview, photoPreview, BackButton, TrashButton, FlipButton, NextButton, ShutterButton, FlashButton);

			FocusImage = new UIImageView (new CGRect(0, 0, 50, 50));
			using (UIImage img = UIImage.FromFile("./camera/focus.png")) {
				FocusImage.Image = img;
			}
			FocusImage.Alpha = 0f;
			cameraPreview.AddSubview(FocusImage);

			FocusTap = new UITapGestureRecognizer (obj => {
				if (!isFocusing)
				{
					isFocusing = true;

					var position = obj.LocationInView(cameraPreview);
					var converted = CameraPreview.ConvertToPointOfInterestFromViewCoordinates(position, cameraPreview.Frame);

					CameraController.Vision.FocusAtAdjustedPointOfInterest (converted);
					CameraController.Vision.ExposeAtAdjustedPointOfInterest (converted);

					FocusImage.Center = position;
					FocusImage.Alpha = .6f;

					Thread thread = new Thread(new ThreadStart(Focusing));
					thread.Start();
				}
			});

			DoubleTap = new UITapGestureRecognizer (obj => {
				if (Vision.CameraDevice == PBJCameraDevice.Back) {
					Vision.CameraDevice = PBJCameraDevice.Front;
					FlashButton.Alpha = 0f;
				} else {
					Vision.CameraDevice = PBJCameraDevice.Back;
					FlashButton.Alpha = 1f;
				}
			});
			DoubleTap.NumberOfTapsRequired = 2;

			ShutterButton.EnableGestures ();
			BackButton.AddGestureRecognizer (BackTap);
			TrashButton.AddGestureRecognizer (TrashTap);
			FlipButton.AddGestureRecognizer (FlipTap);
			FlashButton.AddGestureRecognizer (FlashTap);
			NextButton.AddGestureRecognizer (NextTap);
			View.AddGestureRecognizer (FocusTap);
			View.AddGestureRecognizer (DoubleTap);
		}

		private void Focusing(){
			bool hasAlpha = true;
			Thread.Sleep (400);
			while (hasAlpha) {
				Thread.Sleep (3);
				InvokeOnMainThread(() => {
					FocusImage.Alpha -= .01f;
					if (FocusImage.Alpha <= 0f){
						hasAlpha = false;
					}
				});
			}
			isFocusing = false;
		}

		public override void ViewDidDisappear(bool animated)
		{
			ShutterButton.DisableGestures ();
			BackButton.RemoveGestureRecognizer (BackTap);
			TrashButton.RemoveGestureRecognizer (TrashTap);
			FlipButton.RemoveGestureRecognizer (FlipTap);
			FlashButton.RemoveGestureRecognizer (FlashTap);
			NextButton.RemoveGestureRecognizer (NextTap);
			View.RemoveGestureRecognizer (FocusTap);
			View.RemoveGestureRecognizer (DoubleTap);

			videoPreview.KillVideo ();

			if (cameraPreview.Layer.Sublayers != null) {
				foreach (var layer in cameraPreview.Layer.Sublayers) {
					layer.RemoveFromSuperLayer ();
				}
			}

			Vision.StopPreview ();
			Vision.Delegate.Dispose ();
			Vision.Dispose ();

			MemoryUtility.ReleaseUIViewWithChildren (videoPreview);
			MemoryUtility.ReleaseUIViewWithChildren (cameraPreview);
			MemoryUtility.ReleaseUIViewWithChildren (View);

			GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced);
		}

		public void ImportImage(UIImage image) {
			photoPreview.Image = CommonUtils.ScaleAndRotateImage(image, UIImageOrientation.Up);

			FlipButton.Alpha = 0f;
			ShutterButton.Alpha = 0f;
			videoPreview.Alpha = 0f;
			cameraPreview.Alpha = 0f;
			FlashButton.Alpha = 0f;

			photoPreview.Alpha = 1f;
			NextButton.Alpha = 1f;
			TrashButton.Alpha = 1f;

			View.AddSubview (photoPreview);
			View.BringSubviewToFront (BackButton);
			View.BringSubviewToFront (FlipButton);
			View.BringSubviewToFront (NextButton);
			View.BringSubviewToFront (TrashButton);

			PhotoTaken = photoPreview.Image;
			View.RemoveGestureRecognizer (FocusTap);
		}

		public void ImportVideo()
		{
			//ALAssetsLibrary lib = new ALAssetsLibrary ();
			//await lib.WriteVideoToSavedPhotosAlbumAsync (NSUrl.FromFilename(CaptureDirectory + "/" + compressedGuid + ".mp4"));
			//lib.Dispose ();

			// TODO: does something with the compressed video

			// TODO: deletes full res video

			//File.Delete (CaptureDirectory + "/" + compressedGuid + ".mp4");
			//compressedGuid = null;

			InvokeOnMainThread (() => videoPreview.LoadVideo (CustomPBJVisionDelegate.VideoPath));
			InvokeOnMainThread (() => cameraPreview.Alpha = 0f);
			InvokeOnMainThread (() => ShutterButton.Alpha = 0f);
			InvokeOnMainThread (() => FlashButton.Alpha = 0f);
			InvokeOnMainThread (() => FlipButton.Alpha = 0f);
			InvokeOnMainThread (() => TrashButton.Alpha = 1f);
			InvokeOnMainThread (() => NextButton.Alpha = 1f);
			InvokeOnMainThread (() => View.RemoveGestureRecognizer (FocusTap));

			VideoTakenURL = CustomPBJVisionDelegate.VideoPath;
		}

		private void CreateNextButton(UIColor buttonColor)
		{
			using (UIImage img = UIImage.FromFile ("./camera/nextbutton.png")) {
				NextButton = new UIImageView(new CGRect(0,0,img.Size.Width * 2,img.Size.Height * 2));

				UIImageView subView = new UIImageView (new CGRect (0, 0, img.Size.Width / 2, img.Size.Height / 2));
				subView.Image = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				subView.Center = new CGPoint (NextButton.Frame.Width / 2, NextButton.Frame.Height / 2);
				subView.TintColor = buttonColor;

				NextButton.AddSubview (subView);
				NextButton.Center = new CGPoint (AppDelegate.ScreenWidth - img.Size.Width / 2 - 5, AppDelegate.ScreenHeight - img.Size.Height / 2 - 5);
			}

			NextButton.UserInteractionEnabled = true;

			NextTap = new UITapGestureRecognizer (tg => {
				CreateMediaScreen createScreen;
				if (VideoTakenURL != null){
					createScreen = new CreateMediaScreen(VideoTakenURL);
				} else {
					AppDelegate.CameraPhoto = PhotoTaken;
					PhotoTaken = null;
					photoPreview.Image = null;
					createScreen = new CreateMediaScreen();
				}
				AppDelegate.NavigationController.PushViewController(createScreen, true);
			});
			NextButton.Alpha = 0f;
		}


		private void CreateBackButton(UIColor buttonColor)
		{
			using (UIImage img = UIImage.FromFile ("./boardinterface/lookup/cancel.png")) {
				BackButton = new UIImageView(new CGRect(0,0,img.Size.Width * 2,img.Size.Height * 2));

				UIImageView subView = new UIImageView (new CGRect (0, 0, img.Size.Width / 2, img.Size.Height / 2));
				subView.Image = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				subView.Center = new CGPoint (BackButton.Frame.Width / 2, BackButton.Frame.Height / 2);
				subView.TintColor = buttonColor;

				BackButton.AddSubview (subView);
				BackButton.Center = new CGPoint (img.Size.Width / 2 + 10, 35);
			}

			BackButton.UserInteractionEnabled = true;

			BackTap = new UITapGestureRecognizer (tg => AppDelegate.PopViewLikeDismissView ());
		}

		private void CreateFlipButton(UIColor buttonColor)
		{
			using (UIImage img = UIImage.FromFile ("./camera/flipcamera21.png")) {
				FlipButton = new UIImageView(new CGRect(0,0,img.Size.Width * 2,img.Size.Height * 2));

				UIImageView subView = new UIImageView (new CGRect (0, 0, img.Size.Width / 2, img.Size.Height / 2));
				subView.Image = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				subView.Center = new CGPoint (FlipButton.Frame.Width / 2, FlipButton.Frame.Height / 2);
				subView.TintColor = buttonColor;

				FlipButton.AddSubview (subView);
				FlipButton.Center = new CGPoint (AppDelegate.ScreenWidth - img.Size.Width / 2 - 5, 35);
			}

			FlipButton.UserInteractionEnabled = true;

			FlipTap = new UITapGestureRecognizer (tg => {
				if (Vision.CameraDevice == PBJCameraDevice.Back) {
					Vision.CameraDevice = PBJCameraDevice.Front;
					FlashButton.Alpha = 0f;
				} else {
					Vision.CameraDevice = PBJCameraDevice.Back;
					FlashButton.Alpha = 1f;
				}
			});
		}

		private void CreateTrashButton(UIColor buttonColor)
		{
			using (UIImage img = UIImage.FromFile ("./boardinterface/lookup/trash.png")) {
				TrashButton = new UIImageView(new CGRect(0,0,img.Size.Width * 2,img.Size.Height * 2));

				UIImageView subView = new UIImageView (new CGRect (0, 0, img.Size.Width / 2, img.Size.Height / 2));
				subView.Image = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				subView.Center = new CGPoint (TrashButton.Frame.Width / 2, TrashButton.Frame.Height / 2);
				subView.TintColor = buttonColor;

				TrashButton.AddSubview (subView);
				TrashButton.Center = new CGPoint (AppDelegate.ScreenWidth - img.Size.Width / 2 - 10, 35);
			}

			TrashButton.UserInteractionEnabled = true;

			TrashTap = new UITapGestureRecognizer (tg => {
				videoPreview.KillVideo();

				cameraPreview = new CameraPreview();
				photoPreview.RemoveFromSuperview();

				videoPreview.Alpha = 0f;
				TrashButton.Alpha = 0f;
				photoPreview.Alpha = 0f;
				NextButton.Alpha = 0f;
				cameraPreview.Alpha = 1f;
				ShutterButton.Alpha = 1f;
				FlipButton.Alpha = 1f;
				FlashButton.Alpha = 1f;
				VideoTakenURL = null;
				PhotoTaken = null;

				View.AddSubview(cameraPreview);
				View.BringSubviewToFront(BackButton);

				View.AddSubview(FlipButton);
				View.BringSubviewToFront(FlipButton);

				View.AddSubview(ShutterButton);
				View.BringSubviewToFront(ShutterButton);

				View.AddSubview(FlashButton);
				View.BringSubviewToFront(FlashButton);

				cameraPreview.AddSubview(FocusImage);

				View.AddGestureRecognizer (FocusTap);
			});

			TrashButton.Alpha = 0f;
		}

		private void CreateFlashButton(UIColor buttonColor)
		{
			UIImageView subView;

			using (UIImage img = UIImage.FromFile ("./camera/flash_off.png")) {
				FlashButton = new UIImageView(new CGRect(0, 0, img.Size.Width * 2, img.Size.Height * 2));
				Vision.FlashMode = PBJFlashMode.Off;

				subView = new UIImageView (new CGRect (0, 0, img.Size.Width / 2, img.Size.Height / 2));
				subView.Image = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				subView.Center = new CGPoint (FlashButton.Frame.Width / 2, TrashButton.Frame.Height / 2);
				subView.TintColor = buttonColor;

				FlashButton.AddSubview (subView);
				FlashButton.Center = new CGPoint (AppDelegate.ScreenWidth / 2, 35);
			}

			FlashButton.UserInteractionEnabled = true;

			FlashTap = new UITapGestureRecognizer (tg => {
				if (Vision.FlashMode == PBJFlashMode.Off){
					Vision.FlashMode = PBJFlashMode.Auto;
					using (UIImage img = UIImage.FromFile ("./camera/flash_on.png")) {
						subView.Image = img;
					}

				} else if (Vision.FlashMode == PBJFlashMode.Auto){
					Vision.FlashMode = PBJFlashMode.Off;
					using (UIImage img = UIImage.FromFile ("./camera/flash_off.png")) {
						subView.Image = img;
					}

				}
			});
		}

	}
}

