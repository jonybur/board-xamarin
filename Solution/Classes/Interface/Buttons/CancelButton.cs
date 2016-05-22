using System;
using CoreGraphics;
using UIKit;

namespace Board.Interface.Buttons
{
	public class CancelButton : BIButton
	{
		public CancelButton ()
		{
			using (UIImage image = UIImage.FromFile ("./boardinterface/nubuttons/nucancel.png")) {
				SetImage (image, UIControlState.Normal);
			}

			Frame = new CGRect (0, 0, ButtonSize, ButtonSize);
			Center = new CGPoint ((AppDelegate.ScreenWidth - ButtonSize) / 4, AppDelegate.ScreenHeight - ButtonSize / 2);

			eventHandlers.Add ((sender, e) => {
				
				if (Preview.IsAlive){
					// discards preview
					Preview.RemoveFromSuperview ();
				} else {
					// discards sticker
					UIPreviewSticker.PreviewSticker.RemoveFromSuperview();
				}

				// resets navigation
				ButtonInterface.SwitchButtonLayout (ButtonInterface.ButtonLayout.NavigationBar);
			});

			Alpha = 0f;
		}
	}
}

