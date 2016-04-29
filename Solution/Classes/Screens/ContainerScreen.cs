using Board.Screens.Controls;
using UIKit;
using Board.Utilities;
using Foundation;

namespace Board.Screens
{
	public class ContainerScreen : UIViewController
	{
		SideMenu sideMenu;
		UIViewController currentScreen;

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
			currentScreen = new MainMenuScreen ();
			AddChildViewController (currentScreen);
			View.AddSubview (currentScreen.View);
		}

		public void LoadBusinessScreen()
		{
			currentScreen = new BusinessScreen ();
			AddChildViewController (currentScreen);
			View.AddSubview (currentScreen.View);
		}

		public override void ViewDidDisappear(bool animated)
		{
			currentScreen.ViewDidDisappear (animated);
			MemoryUtility.ReleaseUIViewWithChildren (View);
		}

		public void BringSideMenuUp(string fromScreen)
		{
			sideMenu = new SideMenu (fromScreen);
			View.AddSubview (sideMenu.View);
		}

		public void ChangeToMainScreen()
		{
			RemoveCurrentScreen ();
			currentScreen = new MainMenuScreen ();
			AddChildViewController (currentScreen);
			View.AddSubview (currentScreen.View);
		}

		public void ChangeToBusinessScreen()
		{	
			RemoveCurrentScreen ();
			currentScreen = new BusinessScreen ();
			AddChildViewController (currentScreen);
			View.AddSubview (currentScreen.View);
		}

		public void ChangeToSettingsScreen()
		{
			RemoveCurrentScreen ();
			currentScreen = new SettingsScreen ();
			AddChildViewController (currentScreen);
			View.AddSubview (currentScreen.View);
		}

		public void ChangeToInviteScreen()
		{
			RemoveCurrentScreen ();
			currentScreen = new InviteScreen ();
			AddChildViewController (currentScreen);
			View.AddSubview (currentScreen.View);
		}

		public void ChangeToSupportScreen()
		{
			RemoveCurrentScreen ();
			currentScreen = new SupportScreen ();
			AddChildViewController (currentScreen);
			View.AddSubview (currentScreen.View);
		}

		private void RemoveCurrentScreen()
		{
			currentScreen.WillMoveToParentViewController (null);
			currentScreen.View.RemoveFromSuperview ();
			currentScreen.RemoveFromParentViewController ();
		}
	}
}

