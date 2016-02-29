using System;
using Board.Interface.Buttons;

using CoreGraphics;

using UIKit;

namespace Board.Interface.Lookup
{
	public class ChatButton : Button
	{
		public ChatButton ()
		{
			uiButton = new UIButton (UIButtonType.Custom);

			using (UIImage chatImage = UIImage.FromFile ("./boardinterface/buttons/chat.png")) {
				uiButton.SetImage (chatImage, UIControlState.Normal);
			}

			uiButton.Frame = new CGRect (0,0, ButtonSize, ButtonSize);

			uiButton.Center = new CGPoint (ButtonSize + ButtonSize/2 + 10, ButtonSize/2 + 15);

			uiButton.TouchUpInside += async (sender, e) => {
				
			};
		}

	}
}

