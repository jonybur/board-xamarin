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
	public class GalleryButton : Button
	{
		public GalleryButton ()
		{
			uiButton = new UIButton (UIButtonType.Custom);

			UIImage uiImage = UIImage.FromFile ("./boardscreen/buttons/gallery.png");
			uiButton.SetImage (uiImage, UIControlState.Normal);

			uiButton.Frame = new CGRect (0,0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint ((AppDelegate.ScreenWidth + ButtonSize) / 2 + 
				(AppDelegate.ScreenWidth - ButtonSize) / 8 * 3, AppDelegate.ScreenHeight - ButtonSize / 2);

			uiButton.Alpha = 0f;
			uiButton.TouchUpInside += (object sender, EventArgs e) => {
				
			};
		}
	}
}

