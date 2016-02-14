using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;
using Board.Interface.Buttons;

namespace Board.Lookup.Buttons
{
	public class ChatButton : Button
	{
		public ChatButton (string contentId, UINavigationController navigationController)
		{
			uiButton = new UIButton (UIButtonType.Custom);

			UIImage chatImage = UIImage.FromFile ("./boardscreen/buttons/chat.png");

			uiButton.Frame = new CGRect (0,0, ButtonSize, ButtonSize);

			uiButton.Center = new CGPoint (ButtonSize + ButtonSize/2 + 10, ButtonSize/2 + 15);
			uiButton.SetImage (chatImage, UIControlState.Normal);

			uiButton.TouchUpInside += async (object sender, EventArgs e) => {
				
			};
		}

	}
}

