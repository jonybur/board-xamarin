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

			UIImage uiImage = UIImage.FromFile ("./boardscreen/buttons/accept2.png");
			uiButton.SetImage (uiImage, UIControlState.Normal);

			uiButton.Frame = new CGRect (0,0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint ((AppDelegate.ScreenWidth + ButtonSize) / 2 +
				(AppDelegate.ScreenWidth - ButtonSize) / 4, AppDelegate.ScreenHeight - ButtonSize / 2 - 10);

			uiButton.TouchUpInside +=  (object sender, EventArgs e) => {
				// remove interaction capabilities from the preview
				Preview.RemoveUserInteraction ();

				// takes out the confirmation bar and resets navigation
				ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.NavigationBar);

				switch (Preview.TypeOfPreview)
				{
				case (int)Preview.Type.Picture:
					// create the picture from the preview
					Picture p = Preview.GetPicture ();

					// if the picture is not null...
					if (p != null) {
						// uploads

						BoardInterface.ListPictures.Add(p);
					}
					break;

				case (int)Preview.Type.Video:
					Video v = Preview.GetVideo();

					if (v != null)
					{
						BoardInterface.ListVideos.Add(v);
					}
					break;

				case (int)Preview.Type.TextBox:
					TextBox tb = Preview.GetTextBox ();

					if (tb != null)
					{
						// uploads

					}
					break;
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

