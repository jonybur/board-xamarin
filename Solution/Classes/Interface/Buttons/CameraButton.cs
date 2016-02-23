using System;
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

			UIImage uiImage = UIImage.FromFile ("./boardinterface/strokebuttons/camera.png");

			uiButton = new UIButton (UIButtonType.Custom);
			uiButton.SetImage (uiImage, UIControlState.Normal);

			uiButton.Frame = new CGRect (0, 0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint ((AppDelegate.ScreenWidth - ButtonSize) / 8 * 3 - 5, AppDelegate.ScreenHeight - ButtonSize / 2);

			uiButton.TouchUpInside += (sender, e) => {
				UIAlertController alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);

				alert.AddAction (UIAlertAction.Create ("Photo Library", UIAlertActionStyle.Default, OpenPhotoGallery));
				alert.AddAction (UIAlertAction.Create ("Take Photo or Video", UIAlertActionStyle.Default, OpenCamera));
				alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));

				AppDelegate.NavigationController.PresentViewController (alert, true, null);
			};
		}

		private void OpenPhotoGallery(UIAlertAction action)
		{
			ImagePicker ip = new ImagePicker (UIImagePickerControllerSourceType.PhotoLibrary);

			AppDelegate.NavigationController.PresentViewController (ip.UIImagePicker, true, null);
		}

		private void OpenCamera(UIAlertAction action)
		{
			ImagePicker ip = new ImagePicker (UIImagePickerControllerSourceType.Camera);

			AppDelegate.NavigationController.PresentViewController (ip.UIImagePicker, true, null);
		}
	}
}

