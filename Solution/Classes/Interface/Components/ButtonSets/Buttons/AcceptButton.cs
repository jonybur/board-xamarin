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
	public class AcceptButton : Button
	{
		public AcceptButton (Action refreshPictures)
		{
			uiButton = new UIButton (UIButtonType.Custom);

			UIImage uiImage = UIImage.FromFile ("./boardscreen/buttons/accept.png");
			uiButton.SetImage (uiImage, UIControlState.Normal);

			uiButton.Frame = new CGRect (0,0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint ((AppDelegate.ScreenWidth + ButtonSize) / 2 +
				(AppDelegate.ScreenWidth - ButtonSize) / 4, AppDelegate.ScreenHeight - ButtonSize / 2);

			uiButton.TouchUpInside +=  async (object sender, EventArgs e) => {
				// checks if there's a collision
				if (Preview.Collision) {
					return;
				}

				// remove interaction capabilities from the preview
				Preview.RemoveUserInteraction ();

				// takes out the confirmation bar and resets navigation
				ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.NavigationBar);

				if (Preview.IsPicturePreview)
				{
					// create the picture from the preview
					Picture p = Preview.ConvertToPicture ();

					// if the picture is not null...
					if (p != null) {
						// uploads
						await AppDelegate.CloudController.InsertPictureAsync (p);
					}
				}
				else
				{
					TextBox tb = Preview.ConvertToTextBox();

					await AppDelegate.CloudController.InsertTextBoxAsync(tb);
				}

				// remove the preview imageview from the superview
				Preview.RemoveFromSuperview ();

				// refreshes the scrollview
				refreshPictures ();
			};
			uiButton.Alpha = 0f;
		}


	}
}

