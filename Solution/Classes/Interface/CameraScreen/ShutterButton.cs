using UIKit;
using CoreGraphics;
using System.Threading;
using PBJVisionBinding;

namespace Board.Interface.Camera
{
	public sealed class UIShutterButton : UIImageView{
		public UIImageView RingView;
		private UITapGestureRecognizer Tap;
		private UILongPressGestureRecognizer LongPress;
		private Thread thread;
		bool timer;

		public UIShutterButton()
		{
			Frame = new CGRect(0, 0, 80, 80);
			CameraController.Vision.CameraMode = PBJCameraMode.Video;

			using (UIImage img = UIImage.FromFile ("./camera/cameraback.png")) {
				Image = img;
				Center = new CGPoint (AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight - Frame.Height / 2 - 20);
			}

			using (UIImage img = UIImage.FromFile ("./camera/camerafront.png")) {
				RingView = new UIImageView(new CGRect(0, 0, Frame.Width, Frame.Height));
				UIImage ringImage = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				RingView.Image = ringImage;
			}

			RingView.TintColor = UIColor.White;
			AddSubview(RingView);

			Tap = new UITapGestureRecognizer (tg => {
				if (CameraController.Vision.FlashMode == PBJFlashMode.Auto){
					CameraController.Vision.FlashMode = PBJFlashMode.On;
					Thread.Sleep(300);

					CameraController.Vision.FlashMode = PBJFlashMode.Off;
					Thread.Sleep(100);

					CameraController.Vision.FlashMode = PBJFlashMode.On;
				}

				CameraController.Vision.CapturePreviewPhoto ();
			});

			LongPress = new UILongPressGestureRecognizer(tg => {
				switch (tg.State)
				{
					case UIGestureRecognizerState.Began:
						if (CameraController.Vision.FlashMode == PBJFlashMode.Auto){
							CameraController.Vision.FlashMode = PBJFlashMode.On;
						}

						if (!CameraController.Vision.Recording) {
							CameraController.Vision.StartVideoCapture();
						} else {
							CameraController.Vision.ResumeVideoCapture();
						}
						RingView.TintColor = UIColor.Red;
						timer = true;
						thread = new Thread(new ThreadStart(Timer));
						thread.Start();
						break;

					case UIGestureRecognizerState.Ended:
					case UIGestureRecognizerState.Cancelled:
					case UIGestureRecognizerState.Failed:
						timer = false;
						Thread.Sleep(1000);
						CameraController.Vision.EndVideoCapture();
						RingView.TintColor = UIColor.White;
						break;
				}
			});

			LongPress.MinimumPressDuration = .3f;
			UserInteractionEnabled = true;
		}

		private void Timer(){
			int secondTimer = 0;

			while (secondTimer < 19 && timer) {
				secondTimer++;
				Thread.Sleep (1000);
			}

			if (timer) {
				Alpha = 0f;
			}
		}

		public void EnableGestures()
		{
			AddGestureRecognizer (Tap);	
			AddGestureRecognizer (LongPress);
		}

		public void DisableGestures()
		{
			RemoveGestureRecognizer (Tap);
			RemoveGestureRecognizer (LongPress);
		}
	}
}

