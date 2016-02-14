using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;
using Solution;

namespace Board.Interface.Buttons
{
	public class CancelButton : Button
	{
		public CancelButton ()
		{
			uiButton = new UIButton (UIButtonType.Custom);

			UIImage image = UIImage.FromFile ("./boardscreen/buttons/cancel3.png");
			uiButton = new UIButton (UIButtonType.Custom);
			uiButton.SetImage (image, UIControlState.Normal);

			uiButton.Frame = new CGRect (0, 0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint ((AppDelegate.ScreenWidth - ButtonSize) / 4, AppDelegate.ScreenHeight - ButtonSize /2 - 10);


			uiButton.TouchUpInside += (object sender, EventArgs e) => {
				// discards preview
				Preview.RemoveFromSuperview ();
				// resets navigation
				ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.NavigationBar);
			};
			uiButton.Alpha = 0f;
		}
	}
}

