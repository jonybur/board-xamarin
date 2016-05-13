using System;
using CoreGraphics;
using UIKit;

namespace Board.Interface.Buttons
{
	public class CancelButton : Button
	{
		public CancelButton ()
		{
			uiButton = new UIButton (UIButtonType.Custom);

			using (UIImage image = UIImage.FromFile ("./boardinterface/nubuttons/nucancel.png")) {
				uiButton.SetImage (image, UIControlState.Normal);
			}

			uiButton.Frame = new CGRect (0, 0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint ((AppDelegate.ScreenWidth - ButtonSize) / 4, AppDelegate.ScreenHeight - ButtonSize / 2);

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

			uiButton.Alpha = 0f;
		}
	}
}

