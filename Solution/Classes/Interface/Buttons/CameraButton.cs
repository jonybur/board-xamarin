using Board.Interface.Camera;
using Board.Picker;
using CoreGraphics;
using UIKit;
using Board.Interface.FacebookImport;

namespace Board.Interface.Buttons
{
	public class CameraButton : BIButton
	{
		public CameraButton ()
		{
			using (UIImage uiImage = UIImage.FromFile ("./boardinterface/nubuttons/nucamera.png")) {
				SetImage (uiImage, UIControlState.Normal);
			}

			Frame = new CGRect (0, 0, ButtonSize, ButtonSize);
			Center = new CGPoint ((AppDelegate.ScreenWidth - ButtonSize) / 8 * 3 - 10, AppDelegate.ScreenHeight - ButtonSize / 2);

			var tapPress = new UITapGestureRecognizer(tg => {
				UIAlertController alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);

				//alert.AddAction (UIAlertAction.Create ("Import from Facebook", UIAlertActionStyle.Default, OpenFacebookImporter));
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
			AppDelegate.PushViewLikePresentView (cameraController);
		}

		private void OpenCamera(UIAlertAction action)
		{
			var cameraController = new CameraController ();
			AppDelegate.PushViewLikePresentView (cameraController);
		}
	}
}

