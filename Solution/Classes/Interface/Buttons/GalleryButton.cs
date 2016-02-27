using System;
using CoreGraphics;
using UIKit;

namespace Board.Interface.Buttons
{
	public class GalleryButton : Button
	{
		public GalleryButton ()
		{
			uiButton = new UIButton (UIButtonType.Custom);

			UIImage uiImage = UIImage.FromFile ("./boardinterface/buttons/gallery.png");
			uiButton.SetImage (uiImage, UIControlState.Normal);

			uiButton.Frame = new CGRect (0,0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint ((AppDelegate.ScreenWidth + ButtonSize) / 2 + 
				(AppDelegate.ScreenWidth - ButtonSize) / 8 * 3 - 5, AppDelegate.ScreenHeight - ButtonSize / 2 - 10);
			
			eventHandlers.Add ((sender, e) => {
				
			});
		}
	}
}

