using Board.Infrastructure;
using Board.Screens.Controls;
using Board.Utilities;
using Foundation;
using UIKit;

namespace Board.Screens
{
	public class ContainerScreen : UIViewController
	{
		public UIViewController CurrentScreenViewController;
		UISideMenu sideMenu;

		public static Screens CurrentScreen;

		public enum Screens{
			MainMenu = 1, Business, Settings, Invite, Support
		}

		public override void ViewDidLoad ()
		{
			View.BackgroundColor = UIColor.White;

			AutomaticallyAdjustsScrollViewInsets = false;
			NavigationController.NavigationBarHidden = true;

			//CloudController.DeleteBoard ("7c776389-7318-4d0a-ab59-2ed0d929b23c");

			LoadMainMenu ();
			LoadChangelogAlert ();
		}

		public override void ViewDidAppear(bool animated){
			if (AppDelegate.BoardUser == null) {
				CloudController.GetUserProfile ();
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

				UIAlertController alert = UIAlertController.Create ("Welcome to Board 0.4.1", "Changelog:\n" +
					"· Boards have been autoimported from Facebook"
					, UIAlertControllerStyle.Alert);

				alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));	

				//AppDelegate.NavigationController.PresentViewController (alert, true, null);
			}
		}

		public void LoadLastScreen(){
			switch (CurrentScreen) {
			case Screens.MainMenu:
				LoadMainMenu ();
				break;
			case Screens.Business:
				LoadBusinessScreen ();
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

		public void LoadBusinessScreen()
		{
			CurrentScreenViewController = new BusinessScreen ();
			AddChildViewController (CurrentScreenViewController);
			View.AddSubview (CurrentScreenViewController.View);
			CurrentScreen = Screens.Business;
		}

		public override void ViewDidDisappear(bool animated)
		{
			CurrentScreenViewController.ViewDidDisappear (animated);
			MemoryUtility.ReleaseUIViewWithChildren (View);
		}

		public void BringSideMenuUp(string fromScreen)
		{
			sideMenu = new UISideMenu (fromScreen);
			View.AddSubview (sideMenu.View);
		}

		public void ChangeToMainScreen()
		{
			RemoveCurrentScreenViewController ();
			CurrentScreenViewController = new MainMenuScreen ();
			AddChildViewController (CurrentScreenViewController);
			View.AddSubview (CurrentScreenViewController.View);
			CurrentScreen = Screens.MainMenu;
		}

		public void ChangeToBusinessScreen()
		{	
			RemoveCurrentScreenViewController ();
			CurrentScreenViewController = new BusinessScreen ();
			AddChildViewController (CurrentScreenViewController);
			View.AddSubview (CurrentScreenViewController.View);
			CurrentScreen = Screens.Business;
		}

		public void ChangeToSettingsScreen()
		{
			RemoveCurrentScreenViewController ();
			CurrentScreenViewController = new SettingsScreen ();
			AddChildViewController (CurrentScreenViewController);
			View.AddSubview (CurrentScreenViewController.View);
			CurrentScreen = Screens.Settings;
		}

		public void ChangeToInviteScreen()
		{
			RemoveCurrentScreenViewController ();
			CurrentScreenViewController = new InviteScreen ();
			AddChildViewController (CurrentScreenViewController);
			View.AddSubview (CurrentScreenViewController.View);
			CurrentScreen = Screens.Invite;
		}

		public void ChangeToSupportScreen()
		{
			RemoveCurrentScreenViewController ();
			CurrentScreenViewController = new SupportScreen ();
			AddChildViewController (CurrentScreenViewController);
			View.AddSubview (CurrentScreenViewController.View);
			CurrentScreen = Screens.Support;
		}

		private void RemoveCurrentScreenViewController()
		{
			CurrentScreenViewController.WillMoveToParentViewController (null);
			CurrentScreenViewController.View.RemoveFromSuperview ();
			CurrentScreenViewController.RemoveFromParentViewController ();
		}
	}
}

