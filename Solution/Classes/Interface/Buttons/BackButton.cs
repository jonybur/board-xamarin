using CoreGraphics;
using UIKit;

namespace Board.Interface.Buttons
{
	public class BackButton : Button
	{
		public BackButton ()
		{
			uiButton = new UIButton (UIButtonType.Custom);

			UIImage uiImage = UIImage.FromFile ("./boardinterface/strokebuttons/back_3px.png");

			uiButton.SetImage (uiImage, UIControlState.Normal);
			uiButton.Frame = new CGRect (0,0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint ((AppDelegate.ScreenWidth - ButtonSize) / 8,
				AppDelegate.ScreenHeight - ButtonSize / 2);
			
			eventHandlers.Add ((sender, e) => {
				AppDelegate.NavigationController.PopViewController (true);
				AppDelegate.ExitBoardInterface();
			});

		}
	}
}

