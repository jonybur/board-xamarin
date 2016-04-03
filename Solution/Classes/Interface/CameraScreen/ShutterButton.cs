using UIKit;
using CoreGraphics;
using PBJVisionBinding;

namespace Board.Interface.Camera
{
	public class UIShutterButton : UIImageView{
		public UIImageView RingView;
		private UITapGestureRecognizer Tap;
		private UILongPressGestureRecognizer LongPress;

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

			Tap = new UITapGestureRecognizer (tg => CameraController.Vision.CapturePreviewPhoto ());

			LongPress = new UILongPressGestureRecognizer(tg => {
				switch (tg.State)
				{
					case UIGestureRecognizerState.Began:
					if (!CameraController.Vision.Recording) {
							CameraController.Vision.StartVideoCapture();
						} else {
							CameraController.Vision.ResumeVideoCapture();
						}
						RingView.TintColor = UIColor.Red;
						break;
					case UIGestureRecognizerState.Ended:
					case UIGestureRecognizerState.Cancelled:
					case UIGestureRecognizerState.Failed:
						System.Threading.Thread.Sleep(1000);
						CameraController.Vision.EndVideoCapture();
						RingView.TintColor = UIColor.White;
						break;
				}
			});

			LongPress.MinimumPressDuration = .3f;
			UserInteractionEnabled = true;
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

