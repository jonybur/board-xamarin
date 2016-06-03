using Board.Interface.FacebookImport;
using CoreGraphics;
using UIKit;

namespace Board.Interface.Buttons
{
	public sealed class ImportButton : BIButton
	{
		public ImportButton ()
		{
			using (UIImage uiImage = UIImage.FromFile ("./boardinterface/nubuttons/nuback.png")) {
				SetImage (uiImage, UIControlState.Normal);
			}
			Frame = new CGRect (0,0, ButtonSize, ButtonSize);
			Center = new CGPoint ((AppDelegate.ScreenWidth - ButtonSize) / 8,
				AppDelegate.ScreenHeight - ButtonSize / 2);

			bool blockButton = false;

			eventHandlers.Add ((sender, e) => {
				if (UIBoardInterface.board.FacebookId != null) {
					var importScreen = new AlbumsScreen();
					AppDelegate.PushViewLikePresentView (importScreen);
				} else { 
					UIAlertController alert = UIAlertController.Create("Facebook Page Importer", null, UIAlertControllerStyle.Alert);
					alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));
					alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, delegate {
						var importScreen = new AlbumsScreen(alert.TextFields[0].Text);
						AppDelegate.PushViewLikePresentView (importScreen);
					}));

					alert.AddTextField(delegate(UITextField obj) {
						obj.Placeholder = "Facebook Page ID";
					});

					AppDelegate.NavigationController.PresentViewController(alert, true, null);

					/*UIAlertController alert = UIAlertController.Create("Board not connected to a page", "Do you wish to go to settings to connect to a Facebook page?", UIAlertControllerStyle.Alert);
				alert.AddAction (UIAlertAction.Create ("Later", UIAlertActionStyle.Cancel, null));
				alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, delegate(UIAlertAction obj) {
					SettingsScreen settingsScreen = new SettingsScreen();
					AppDelegate.PushViewLikePresentView(settingsScreen);
				}));
				AppDelegate.NavigationController.PresentViewController (alert, true, null);*/
				}
			});
		}

	}
}

