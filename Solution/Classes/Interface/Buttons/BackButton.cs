using CoreGraphics;
using Board.Screens;
using UIKit;

namespace Board.Interface.Buttons
{
	public class BackButton : Button
	{
		public BackButton ()
		{
			uiButton = new UIButton (UIButtonType.Custom);

			using (UIImage uiImage = UIImage.FromFile ("./boardinterface/nubuttons/nuback.png")) {
				uiButton.SetImage (uiImage, UIControlState.Normal);
			}
			uiButton.Frame = new CGRect (0,0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint ((AppDelegate.ScreenWidth - ButtonSize) / 8,
				AppDelegate.ScreenHeight - ButtonSize / 2);

			bool blockButton = false;

			eventHandlers.Add ((sender, e) => {
				if (!blockButton){
					var containerScreen = AppDelegate.NavigationController.ViewControllers[AppDelegate.NavigationController.ViewControllers.Length - 2] as ContainerScreen;
					if (containerScreen!= null)
					{
						containerScreen.LoadMainMenu();
					}
					AppDelegate.PopViewControllerWithCallback(AppDelegate.ExitBoardInterface);
					blockButton = true;
				}
			});

		}
	}
}

