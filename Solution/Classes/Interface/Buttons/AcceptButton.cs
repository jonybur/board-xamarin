using CoreGraphics;
using System;
using UIKit;

namespace Board.Interface.Buttons
{
	public class AcceptButton : BIButton
	{
		public AcceptButton (EventHandler onTapEvent)
		{
			using (UIImage uiImage = UIImage.FromFile ("./boardinterface/nubuttons/nuaccept.png")) {
				SetImage (uiImage, UIControlState.Normal);
			}

			Frame = new CGRect (0,0, ButtonSize, ButtonSize);
			Center = new CGPoint ((AppDelegate.ScreenWidth + ButtonSize) / 2 +
				(AppDelegate.ScreenWidth - ButtonSize) / 4, AppDelegate.ScreenHeight - ButtonSize / 2);
			
			eventHandlers.Add (onTapEvent);

			Alpha = 0f;
		}
	}
}

