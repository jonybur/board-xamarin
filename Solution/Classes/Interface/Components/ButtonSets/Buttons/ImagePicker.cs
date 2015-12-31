using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;

namespace Solution
{
	public class ImagePicker
	{
		private UIImagePickerController imagePickerController;

		public UIImagePickerController UIImagePicker 
		{
			get 
			{
				return imagePickerController; 
			} 
		}

		public ImagePicker (UIScrollView scrollView)
		{
			imagePickerController = new UIImagePickerController();

			imagePickerController.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			imagePickerController.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);

			imagePickerController.FinishedPickingMedia += (sender, e) => {
				// determines what was selected, video or image
				bool isImage = false;
				switch(e.Info[UIImagePickerController.MediaType].ToString()) {
				case "public.image":
					isImage = true;
					break;
				case "public.video":
					Console.WriteLine("Video selected");
					break;
				}

				// get common info (shared between images and video)
				NSUrl referenceURL = e.Info[new NSString("UIImagePickerControllerReferenceUrl")] as NSUrl;
				if (referenceURL != null)
					Console.WriteLine("Url:"+referenceURL.ToString ());

				// if it was an image, get the other image info
				if(isImage) {
					// get the original image
					UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
					if(originalImage != null) {
						// call addimage
						LaunchPicturePreview (originalImage, scrollView);
					}
				} else { // if it's a video
					// get video url
					NSUrl mediaURL = e.Info[UIImagePickerController.MediaURL] as NSUrl;
					if(mediaURL != null) {
						// TODO: leave the video input here or separate video and pictures in two different functions
						Console.WriteLine(mediaURL.ToString());
					}
				}          
				// dismiss the picker
				imagePickerController.DismissViewController(true, null);
			};

			imagePickerController.Canceled += OnCancelation;
		}

		private void LaunchPicturePreview(UIImage image, UIScrollView scrollView)
		{
			Preview.Initialize (image, scrollView.ContentOffset);

			// shows the image preview so that the user can position the image
			scrollView.AddSubview(Preview.GetUIView());

			// switches to confbar
			ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.ConfirmationBar);
		}


		protected void OnCancelation(object sender, System.EventArgs e)
		{
			imagePickerController.DismissViewController(true, null);
		}
	}
}

