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
	public class CardButton : Button
	{
		public CardButton (UINavigationController navigationController)
		{
			uiButton = new UIButton (UIButtonType.Custom);

			UIImage uiImage = UIImage.FromFile ("./boardscreen/buttons/card.png");
			uiButton.SetImage (uiImage, UIControlState.Normal);

			uiButton.Frame = new CGRect (0,0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint ((AppDelegate.ScreenWidth + ButtonSize) / 2 + 
				(AppDelegate.ScreenWidth - ButtonSize) / 8 * 3, AppDelegate.ScreenHeight - ButtonSize / 2 - 10);
			
			uiButton.TouchUpInside += (object sender, EventArgs e) => {
				UIAlertController alert = UIAlertController.Create(null, "Select the type of component", UIAlertControllerStyle.ActionSheet);

				alert.AddAction (UIAlertAction.Create ("Announcement", UIAlertActionStyle.Default, null));
				alert.AddAction (UIAlertAction.Create ("Event", UIAlertActionStyle.Default, null));
				alert.AddAction (UIAlertAction.Create ("Survey", UIAlertActionStyle.Default, null));
				alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));

				navigationController.PresentViewController (alert, true, null);
			};
		}
	}
}

