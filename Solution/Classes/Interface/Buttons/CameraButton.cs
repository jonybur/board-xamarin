using Board.Interface.Camera;
using Board.Picker;
using CoreGraphics;
using UIKit;

namespace Board.Interface.Buttons
{
	public class CameraButton : Button
	{
		public CameraButton ()
		{
			uiButton = new UIButton (UIButtonType.Custom);

			using (UIImage uiImage = UIImage.FromFile ("./boardinterface/strokebuttons/camera_3px.png")) {
				uiButton = new UIButton (UIButtonType.Custom);
				uiButton.SetImage (uiImage, UIControlState.Normal);
			}

			uiButton.Frame = new CGRect (0, 0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint ((AppDelegate.ScreenWidth - ButtonSize) / 8 * 3 - 10, AppDelegate.ScreenHeight - ButtonSize / 2);

			var tapPress = new UITapGestureRecognizer((tg) => {
				UIAlertController alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);

				alert.AddAction (UIAlertAction.Create ("Import from Facebook (coming soon)", UIAlertActionStyle.Default, null));
				alert.AddAction (UIAlertAction.Create ("Photo Library", UIAlertActionStyle.Default, OpenPhotoGallery));
				alert.AddAction (UIAlertAction.Create ("Take Photo or Video", UIAlertActionStyle.Default, OpenCamera));
				alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));

				AppDelegate.NavigationController.PresentViewController (alert, true, null);
			});

			var longPress = new UILongPressGestureRecognizer(tg => OpenCamera ());
			longPress.MinimumPressDuration = .3f;

			gestureRecognizers.Add (tapPress);
			gestureRecognizers.Add (longPress);
		}

		private void OpenPhotoGallery(UIAlertAction action)
		{
			ImagePicker ip = new ImagePicker (UIImagePickerControllerSourceType.PhotoLibrary);
			AppDelegate.NavigationController.PresentViewController (ip.UIImagePicker, true, null);
		}

		private void OpenCamera()
		{
			var cameraController = new CameraController ();
			AppDelegate.boardInterface.RemoveAllContent();
			AppDelegate.PushViewLikePresentView (cameraController);
		}

		private void OpenCamera(UIAlertAction action)
		{
			var cameraController = new CameraController ();
			AppDelegate.boardInterface.RemoveAllContent();
			AppDelegate.PushViewLikePresentView (cameraController);

			//ImagePicker ip = new ImagePicker (UIImagePickerControllerSourceType.Camera);
			//AppDelegate.NavigationController.PresentViewController (ip.UIImagePicker, true, null);
		}
	}
}

