using Clubby.Infrastructure;
using Clubby.Screens.Controls;
using Clubby.Utilities;
using Foundation;
using UIKit;

namespace Clubby.Screens
{
	public class ContainerScreen : UIViewController
	{
		public UIViewController CurrentScreenViewController;

		public static Screens CurrentScreen;

		public enum Screens{
			MainMenu = 1, Business, Settings, Invite, Support
		}

		public override void ViewDidLoad ()
		{
			View.BackgroundColor = AppDelegate.ClubbyBlack;

			AutomaticallyAdjustsScrollViewInsets = false;
			NavigationController.NavigationBarHidden = true;

			LoadMainMenu ();

			CheckForNotifications ();
		}

		private void CheckForNotifications(){
			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				var pushSettings = UIUserNotificationSettings.GetSettingsForTypes (
					UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
					new NSSet ());

				UIApplication.SharedApplication.RegisterUserNotificationSettings (pushSettings);
				UIApplication.SharedApplication.RegisterForRemoteNotifications ();
			} else {
				UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
				UIApplication.SharedApplication.RegisterForRemoteNotificationTypes (notificationTypes);
			}
		}

		public void LoadChangelogAlert()
		{
			var defaults = NSUserDefaults.StandardUserDefaults;
			const string key = "LaunchedBeforeKey041";
			if (!defaults.BoolForKey (key)) {
				// First launch
				NSUserDefaults.StandardUserDefaults.SetBool(true, key);
				defaults.Synchronize ();

				UIAlertController alert = UIAlertController.Create ("Welcome to Board 0.4.1", "", UIAlertControllerStyle.Alert);

				alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));	

				AppDelegate.NavigationController.PresentViewController (alert, true, null);
			}
		}

		public void LoadLastScreen(){
			switch (CurrentScreen) {
			case Screens.MainMenu:
				LoadMainMenu ();
				break;
			}
		}

		public void LoadMainMenu()
		{
			CurrentScreenViewController = new MainMenuScreen ();
			AddChildViewController (CurrentScreenViewController);
			View.AddSubview (CurrentScreenViewController.View);
			CurrentScreen = Screens.MainMenu;
		}

		public override void ViewDidDisappear(bool animated)
		{
			CurrentScreenViewController.ViewDidDisappear (animated);
			MemoryUtility.ReleaseUIViewWithChildren (View);
		}

		public void ChangeToMainScreen()
		{
			RemoveCurrentScreenViewController ();
			CurrentScreenViewController = new MainMenuScreen ();
			AddChildViewController (CurrentScreenViewController);
			View.AddSubview (CurrentScreenViewController.View);
			CurrentScreen = Screens.MainMenu;
		}

		private void RemoveCurrentScreenViewController()
		{
			CurrentScreenViewController.WillMoveToParentViewController (null);
			CurrentScreenViewController.View.RemoveFromSuperview ();
			CurrentScreenViewController.RemoveFromParentViewController ();
		}
	}
}

