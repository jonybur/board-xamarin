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

			using (UIImage image = UIImage.FromFile ("./boardinterface/strokebuttons/cancel_3px.png")) {
				uiButton.SetImage (image, UIControlState.Normal);
			}

			uiButton.Frame = new CGRect (0, 0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint ((AppDelegate.ScreenWidth - ButtonSize) / 4, AppDelegate.ScreenHeight - ButtonSize / 2);

			eventHandlers.Add ((sender, e) => {
				// discards preview
				Preview.RemoveFromSuperview ();
				// resets navigation
				ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.NavigationBar);
			});

			uiButton.Alpha = 0f;
		}
	}
}

