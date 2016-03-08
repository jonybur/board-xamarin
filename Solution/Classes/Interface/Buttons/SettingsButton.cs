using CoreGraphics;
using UIKit;

namespace Board.Interface.Buttons
{
	public class SettingsButton : Button
	{
		public SettingsButton ()
		{
			uiButton = new UIButton (UIButtonType.Custom);

			using (UIImage uiImage = UIImage.FromFile ("./boardinterface/strokebuttons/cog_3px.png")) {
				uiButton.SetImage (uiImage, UIControlState.Normal);
			}

			uiButton.Frame = new CGRect (0, 0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint ((AppDelegate.ScreenWidth + ButtonSize) / 2 + 
				(AppDelegate.ScreenWidth - ButtonSize) / 8 * 3, AppDelegate.ScreenHeight - ButtonSize / 2);

			eventHandlers.Add ((sender, e) => {
				SettingsScreen settingsScreen = new SettingsScreen();
				AppDelegate.NavigationController.PushViewController(settingsScreen, true);
			});
		}
	}
}

