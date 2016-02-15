using System;
using Board.Infrastructure;
using Board.Interface.Buttons;

using Board.Schema;

using CoreGraphics;
using UIKit;

namespace Board.Interface.Lookup
{
	public class ArchiveButton : Button
	{
		public ArchiveButton (Action<UIViewController, bool> presentViewController, Picture picture, Action refreshPictures)
		{
			uiButton = new UIButton (UIButtonType.Custom);

			UIImage uiImage = UIImage.FromFile ("./boardscreen/lookup/archive.png");
			uiButton.SetImage (uiImage, UIControlState.Normal);

			uiButton.Frame = new CGRect (0,0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint (ButtonSize/2 + ButtonSize * 2 + 10, ButtonSize/2 + 15);

			uiButton.TouchUpInside += (object sender, EventArgs e) => {
				UIAlertController alert = UIAlertController.Create("Are you sure you want to remove this picture from the board?", "The picture will still be visible in the gallery", UIAlertControllerStyle.ActionSheet);

				alert.AddAction (UIAlertAction.Create ("Accept", UIAlertActionStyle.Default, action => ArchivePicture (picture, refreshPictures)));
				alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));

				presentViewController (alert, true);
			};
		}

		private async void ArchivePicture(Picture picture, Action refreshPictures)
		{
			// first sets the OnGallery status on true for the picture
			StorageController.SendPictureToGallery (picture.Id);
			// then removes the picture from the cloud storage

			// refreshes the main view
			refreshPictures ();
			// kills this button
			DisableButton ();
		}
	}
}

