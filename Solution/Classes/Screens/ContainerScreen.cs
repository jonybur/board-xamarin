using Board.Infrastructure;
using Board.Screens.Controls;
using Board.Utilities;
using Foundation;
using UIKit;

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

		public override async void ViewDidAppear(bool animated){
			await CloudController.GetUserProfile ();
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
					"· Server integration\n" +
					"· Whole new main menu\n" +
					"· Ability to post Boards"
					, UIAlertControllerStyle.Alert);

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

