using CoreGraphics;
using Board.Screens;
using UIKit;

namespace Board.Interface.Buttons
{
	public class BackButton : BIButton
	{
		public BackButton ()
		{
			using (UIImage uiImage = UIImage.FromFile ("./boardinterface/nubuttons/nuback.png")) {
				SetImage (uiImage, UIControlState.Normal);
			}
			Frame = new CGRect (0,0, ButtonSize, ButtonSize);
			Center = new CGPoint ((AppDelegate.ScreenWidth - ButtonSize) / 8,
				AppDelegate.ScreenHeight - ButtonSize / 2);

			bool blockButton = false;

			eventHandlers.Add ((sender, e) => {
				if (!blockButton){
					var containerScreen = AppDelegate.NavigationController.ViewControllers[AppDelegate.NavigationController.ViewControllers.Length - 2] as ContainerScreen;
					if (containerScreen != null) {
						containerScreen.LoadLastScreen();
					}
					AppDelegate.PopViewControllerWithCallback(AppDelegate.ExitBoardInterface);
					blockButton = true;
				}
			});

		}
	}
}

