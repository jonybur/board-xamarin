using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;

using Board.Schema;

using MediaPlayer;
using Board.Interface;

namespace Board.Picker
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

		public ImagePicker (UIScrollView scrollView, UIImagePickerControllerSourceType sourceType, UINavigationController navigationController)
		{
			imagePickerController = new UIImagePickerController();

			imagePickerController.SourceType = sourceType;
			imagePickerController.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);

			imagePickerController.FinishedPickingMedia += (sender, e) => {
				// determines what was selected, video or image
				bool isImage = false;

				if (e.Info[UIImagePickerController.MediaType].ToString() == "public.image")
				{
					isImage = true;
				}

				// if it was an image, get the other image info
				if(isImage) {
					// get the original image
					UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
					if(originalImage != null) {
						// call addimage
						LaunchPicturePreview (originalImage, navigationController);
					}
				} else { // if it's a video
					// get video url
					NSUrl mediaURL = e.Info[UIImagePickerController.MediaURL] as NSUrl;

					if(mediaURL != null) {
						// launch video
						LaunchVideoPreview(mediaURL, navigationController);
					}
				}          
				// dismiss the picker
				imagePickerController.DismissViewController(true, null);
			};

			imagePickerController.Canceled += OnCancelation;
		}

		public ImagePicker (UIImageView icon, UIImageView preview_icon, Board.Schema.Board board)
		{
			imagePickerController = new UIImagePickerController();

			imagePickerController.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			imagePickerController.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);

			imagePickerController.FinishedPickingMedia += (sender, e) => {
				// determines what was selected, video or image
				bool isImage = false;

				if (e.Info[UIImagePickerController.MediaType].ToString() == "public.image")
				{
					isImage = true;
				}

				// get common info (shared between images and video)
				NSUrl referenceURL = e.Info[new NSString("UIImagePickerControllerReferenceUrl")] as NSUrl;

				// if it was an image, get the other image info
				if(isImage) {
					// get the original image
					UIImage image = e.Info[UIImagePickerController.OriginalImage] as UIImage;

					if(image != null) {
						// call addimage
						float autosize = AppDelegate.ScreenWidth / 3;
						float scale = (float)(image.Size.Height/image.Size.Width);
						float imgh; float imgw;

						if (scale > 1) {
							scale = (float)(image.Size.Width/image.Size.Height);
							imgh = autosize;
							imgw = autosize * scale;
						}
						else {
							imgw = autosize;
							imgh = autosize * scale;	
						}

						icon.Frame = new CGRect (0, 0, imgw * .8f, imgh * .8f);
						icon.Center = new CGPoint(autosize/2, autosize/2);
						icon.Image = image;

						(preview_icon.Subviews[0] as UIImageView).Frame = new CGRect(0, 0, icon.Frame.Width * .6f, icon.Frame.Height * .6f);
						(preview_icon.Subviews[0] as UIImageView).Center = new CGPoint(preview_icon.Frame.Width / 2, preview_icon.Frame.Height / 2);
						(preview_icon.Subviews[0] as UIImageView).Image = image;

						board.Image = image;

					}
				}

				// dismiss the picker
				imagePickerController.DismissViewController(true, null);
			};

			imagePickerController.Canceled += OnCancelation;
		}

		private void LaunchVideoPreview(NSUrl url, UINavigationController navigationController)
		{
			Video video = Preview.Initialize (url.ToString(), navigationController);

			MPMoviePlayerController moviePlayer = new MPMoviePlayerController (url);

			ShareScreen shareScreen = new ShareScreen(moviePlayer.ThumbnailImageAt (0, MPMovieTimeOption.Exact), video);

			moviePlayer.Pause ();
			moviePlayer.Dispose ();

			navigationController.DismissViewController(false, null);

			navigationController.PushViewController(shareScreen, false);
		}

		private void LaunchPicturePreview(UIImage image, UINavigationController navigationController)
		{		
			Picture picture = Preview.Initialize(image, navigationController);

			ShareScreen shareScreen = new ShareScreen(image, picture);

			navigationController.DismissViewController(false, null);

			navigationController.PushViewController(shareScreen, false);
		}


		protected void OnCancelation(object sender, System.EventArgs e)
		{
			imagePickerController.DismissViewController(true, null);
		}
	}
}

