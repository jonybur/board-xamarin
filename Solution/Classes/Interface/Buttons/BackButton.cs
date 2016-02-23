using System;
using CoreGraphics;
using UIKit;

namespace Board.Interface.Buttons
{
	public class BackButton : Button
	{
		public BackButton ()
		{
			uiButton = new UIButton (UIButtonType.Custom);

			UIImage uiImage = UIImage.FromFile ("./boardinterface/strokebuttons/back.png");

			uiButton.SetImage (uiImage, UIControlState.Normal);
			uiButton.Frame = new CGRect (0,0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint ((AppDelegate.ScreenWidth - ButtonSize) / 8,
				AppDelegate.ScreenHeight - ButtonSize / 2);


			uiButton.TouchUpInside += (object sender, EventArgs e) => {

				AppDelegate.NavigationController.PopViewController (true);
			};

		}
	}
}

