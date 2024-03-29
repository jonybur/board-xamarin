﻿using Board.Infrastructure;
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

		public void LoadSettingsScreen()
		{
			CurrentScreenViewController = new SettingsScreen ();
			AddChildViewController (CurrentScreenViewController);
			View.AddSubview (CurrentScreenViewController.View);
			CurrentScreen = Screens.Settings;
		}

		public override void ViewDidDisappear(bool animated)
		{
			AppDelegate.VenueInterface = null;
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

