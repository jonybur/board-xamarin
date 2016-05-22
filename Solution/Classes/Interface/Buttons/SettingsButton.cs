using CoreGraphics;
using UIKit;

namespace Board.Interface.Buttons
{
	public class SettingsButton : BIButton
	{
		public SettingsButton ()
		{
			using (UIImage uiImage = UIImage.FromFile ("./boardinterface/nubuttons/nusettings.png")) {
				SetImage (uiImage, UIControlState.Normal);
			}

			Frame = new CGRect (0, 0, ButtonSize, ButtonSize);
			Center = new CGPoint ((AppDelegate.ScreenWidth + ButtonSize) / 2 + 
				(AppDelegate.ScreenWidth - ButtonSize) / 8 * 3, AppDelegate.ScreenHeight - ButtonSize / 2);

			eventHandlers.Add ((sender, e) => {
				SettingsScreen settingsScreen = new SettingsScreen();
				AppDelegate.PushViewLikePresentView (settingsScreen);
			});
		}
	}
}

