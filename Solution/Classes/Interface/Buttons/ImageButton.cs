using System;
using CoreGraphics;
using UIKit;

namespace Board.Interface.Buttons
{
	public class ImageButton : Button
	{
		public ImageButton ()
		{
			uiButton = new UIButton (UIButtonType.Custom);

			UIImage uiImage = UIImage.FromFile ("./boardinterface/buttons/picture.png");

			uiButton = new UIButton (UIButtonType.Custom);
			uiButton.SetImage (uiImage, UIControlState.Normal);

			uiButton.Frame = new CGRect (0, 0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint ((AppDelegate.ScreenWidth - ButtonSize) / 8 * 3 - 5, AppDelegate.ScreenHeight - ButtonSize / 2 - 10);

			uiButton.TouchUpInside += (object sender, EventArgs e) => {

			};
		}
	}
}

