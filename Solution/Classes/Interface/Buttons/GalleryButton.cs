using System;
using CoreGraphics;
using UIKit;

namespace Board.Interface.Buttons
{
	public class GalleryButton : BIButton
	{
		public GalleryButton ()
		{
			using (UIImage uiImage = UIImage.FromFile ("./boardinterface/buttons/gallery.png")) {
				SetImage (uiImage, UIControlState.Normal);
			}

			Frame = new CGRect (0,0, ButtonSize, ButtonSize);
			Center = new CGPoint ((AppDelegate.ScreenWidth + ButtonSize) / 2 + 
				(AppDelegate.ScreenWidth - ButtonSize) / 8 * 3 - 5, AppDelegate.ScreenHeight - ButtonSize / 2 - 10);
			
			eventHandlers.Add ((sender, e) => {
				
			});
		}
	}
}

