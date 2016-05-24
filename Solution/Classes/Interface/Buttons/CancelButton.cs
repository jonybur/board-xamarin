using System;
using CoreGraphics;
using UIKit;

namespace Board.Interface.Buttons
{
	public class CancelButton : BIButton
	{
		public CancelButton (EventHandler onTapEvent)
		{
			using (UIImage image = UIImage.FromFile ("./boardinterface/nubuttons/nucancel.png")) {
				SetImage (image, UIControlState.Normal);
			}

			Frame = new CGRect (0, 0, ButtonSize, ButtonSize);
			Center = new CGPoint ((AppDelegate.ScreenWidth - ButtonSize) / 4, AppDelegate.ScreenHeight - ButtonSize / 2);

			eventHandlers.Add (onTapEvent);

			Alpha = 0f;
		}
	}
}

