using Board.Screens.Controls;
using UIKit;
using Board.Utilities;
using Foundation;

namespace Board.Screens
{
	public class ContainerScreen : UIViewController
	{
		UISideMenu sideMenu;
		public UIViewController CurrentScreen;

		public override void ViewDidLoad ()
		{
			View.BackgroundColor = UIColor.White;

			AutomaticallyAdjustsScrollViewInsets = false;

			NavigationController.NavigationBarHidden = true;

			LoadMainMenu ();

			LoadChangelogAlert ();
		}

		public void LoadChangelogAlert()
		{
			var defaults = NSUserDefaults.StandardUserDefaults;
			const string key = "LaunchedBeforeKey038";
			if (!defaults.BoolForKey (key)) {
				// First launch
				NSUserDefaults.StandardUserDefaults.SetBool(true, key);
				defaults.Synchronize ();

				UIAlertController alert = UIAlertController.Create ("Welcome to Board 0.3.8", "Changelog:\n" +
					"· Whole new Board interface\n" +
					"· Unlimited scroll\n" +
					"· Snapchat-inspired camera\n" +
					"· Bug fixes", UIAlertControllerStyle.Alert);

				alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));	

				AppDelegate.NavigationController.PresentViewController (alert, true, null);
			}
		}

		public void LoadMainMenu()
		{
			CurrentScreen = new MainMenuScreen ();
			AddChildViewController (CurrentScreen);
			View.AddSubview (CurrentScreen.View);
		}

		public void LoadBusinessScreen()
		{
			CurrentScreen = new BusinessScreen ();
			AddChildViewController (CurrentScreen);
			View.AddSubview (CurrentScreen.View);
		}

		public override void ViewDidDisappear(bool animated)
		{
			CurrentScreen.ViewDidDisappear (animated);
			MemoryUtility.ReleaseUIViewWithChildren (View);
		}

		public void BringSideMenuUp(string fromScreen)
		{
			sideMenu = new UISideMenu (fromScreen);
			View.AddSubview (sideMenu.View);
		}

		public void ChangeToMainScreen()
		{
			RemoveCurrentScreen ();
			CurrentScreen = new MainMenuScreen ();
			AddChildViewController (CurrentScreen);
			View.AddSubview (CurrentScreen.View);
		}

		public void ChangeToBusinessScreen()
		{	
			RemoveCurrentScreen ();
			CurrentScreen = new BusinessScreen ();
			AddChildViewController (CurrentScreen);
			View.AddSubview (CurrentScreen.View);
		}

		public void ChangeToSettingsScreen()
		{
			RemoveCurrentScreen ();
			CurrentScreen = new SettingsScreen ();
			AddChildViewController (CurrentScreen);
			View.AddSubview (CurrentScreen.View);
		}

		public void ChangeToInviteScreen()
		{
			RemoveCurrentScreen ();
			CurrentScreen = new InviteScreen ();
			AddChildViewController (CurrentScreen);
			View.AddSubview (CurrentScreen.View);
		}

		public void ChangeToSupportScreen()
		{
			RemoveCurrentScreen ();
			CurrentScreen = new SupportScreen ();
			AddChildViewController (CurrentScreen);
			View.AddSubview (CurrentScreen.View);
		}

		private void RemoveCurrentScreen()
		{
			CurrentScreen.WillMoveToParentViewController (null);
			CurrentScreen.View.RemoveFromSuperview ();
			CurrentScreen.RemoveFromParentViewController ();
		}
	}
}

