using System;
using System.Net;
using Clubby.Infrastructure;
using Clubby.Interface;
using Clubby.JsonResponses;
using Facebook.CoreKit;
using Clubby.Schema;
using Clubby.Screens;
using CoreAnimation;
using CoreLocation;
using Foundation;
using Google.Maps;
using UIKit;

namespace Clubby
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.

	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;

		// until we get backend support and we can actually store new boards online
		public static UIImage CameraPhoto;
		public static UINavigationController NavigationController;
		public static float ScreenWidth;
		public static float ScreenHeight;

		public static UIColor ClubbyYellow;
		public static UIColor ClubbyBlack;
		public static UIColor BoardBlue;
		public static UIColor BoardLightBlue;
		public static UIColor ClubbyOrange;

		public static UIFont Narwhal12;
		public static UIFont Narwhal14;
		public static UIFont Narwhal16;
		public static UIFont Narwhal18;
		public static UIFont Narwhal20;
		public static UIFont Narwhal24;
		public static UIFont Narwhal26;
		public static UIFont Narwhal30;
		public static UIFont SystemFontOfSize16;
		public static UIFont SystemFontOfSize18;
		public static UIFont SystemFontOfSize20;
		public static bool HasLoggedSession;

		//public static User BoardUser;
		public static CLLocationCoordinate2D UserLocation;

		public enum PhoneVersions { iPhone4, iPhone5, iPhone6, iPhone6Plus };
		public static PhoneVersions PhoneVersion;

		public const string APIAddress = "api.boardack.com";

		public const string FacebookAppId = "1614192198892777";
		public const string FacebookDisplayName = "Clubby";

		public const string GoogleMapsAPIKey = "AIzaSyCDZ9asTW293TTiaYkMrlLNtlBdzBD_FQw";
		public const string UberServerToken = "X0Vn_KDBicJ_U-wJzS3at6SoirGqWhSydSftTHpm";

		public static string BoardToken;
		public static AmazonS3TicketResponse AmazonS3Ticket;
		public static string EncodedBoardToken;
	
		// This method is invoked when the application hqas loaded and is ready to run. In this
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//

		public override void OnActivated (UIApplication application)
		{
			// Call the 'activateApp' method to log an app event for use
			// in analytics and advertising reporting.
			AppEvents.ActivateApp ();
			AppEvents.LogEvent ("activatesApp");
		}

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

			BoardLightBlue = UIColor.FromRGB (45, 121, 180);
			BoardBlue = UIColor.FromRGB (38, 106, 154);
			ClubbyOrange = UIColor.FromRGB (244, 108, 85);
			ClubbyBlack = UIColor.FromRGB (16, 16, 16);
			ClubbyYellow = UIColor.FromRGB (0, 157, 255);//(0, 167, 255);

			Narwhal12 = UIFont.FromName ("narwhal-bold", 12);
			Narwhal14 = UIFont.FromName ("narwhal-bold", 14);
			Narwhal16 = UIFont.FromName ("narwhal-bold", 16);
			Narwhal18 = UIFont.FromName ("narwhal-bold", 18);
			Narwhal20 = UIFont.FromName ("narwhal-bold", 20);
			Narwhal24 = UIFont.FromName ("narwhal-bold", 24);
			Narwhal26 = UIFont.FromName ("narwhal-bold", 26);
			Narwhal30 = UIFont.FromName ("narwhal-bold", 29);

			SystemFontOfSize16 = UIFont.SystemFontOfSize (16);
			SystemFontOfSize18 = UIFont.SystemFontOfSize (18);
			SystemFontOfSize20 = UIFont.SystemFontOfSize (20);

			StorageController.Initialize ();

			MapServices.ProvideAPIKey (GoogleMapsAPIKey);

			// FACEBOOK
			Profile.EnableUpdatesOnAccessTokenChange (true);
			Settings.AppID = FacebookAppId;
			Settings.DisplayName = FacebookDisplayName;
			ApplicationDelegate.SharedInstance.FinishedLaunching (app, options);

			ScreenWidth = (float)UIScreen.MainScreen.Bounds.Width;
			ScreenHeight = (float)UIScreen.MainScreen.Bounds.Height;

			if (ScreenWidth == 375) {
				PhoneVersion = PhoneVersions.iPhone6;
			} else if (ScreenWidth == 414) {
				PhoneVersion = PhoneVersions.iPhone6Plus;
			} else if (ScreenWidth == 320) {
				if (ScreenHeight == 480) {
					PhoneVersion = PhoneVersions.iPhone4;
				} else if (ScreenHeight == 568) {
					PhoneVersion = PhoneVersions.iPhone5;
				}
			} else {
				PhoneVersion = PhoneVersions.iPhone6;
			}

			UIViewController screen;

			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			if (AccessToken.CurrentAccessToken != null) {
				screen = new MainMenuScreen ();
			} else {
				screen = new LoginScreen ();
			}

			NavigationController = new UINavigationController();

			window.RootViewController = screen;
			NavigationController.PushViewController (screen, false);
			window.AddSubview (NavigationController.View);
			window.MakeKeyAndVisible ();

			UIApplication.SharedApplication.SetStatusBarStyle (UIStatusBarStyle.LightContent, false);

			return true;
		}

		public override bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			// We need to handle URLs by passing them to their own OpenUrl in order to make the SSO authentication works.
			return ApplicationDelegate.SharedInstance.OpenUrl (application, url, sourceApplication, annotation);
		}

		public static void PopViewControllerWithCallback(Action callback)
		{
			CATransaction.Begin ();

			AppDelegate.NavigationController.PopViewController (true);

			CATransaction.Commit();

			CATransaction.CompletionBlock = callback;
		}

		public static void PopToViewControllerWithCallback(UIViewController viewController, Action callback)
		{
			CATransaction.Begin ();

			AppDelegate.NavigationController.PopToViewController(viewController, true);

			CATransaction.Commit ();

			CATransaction.CompletionBlock = callback;
		}

		public static void PushViewLikePresentView(UIViewController screen)
		{
			CATransition transition = new CATransition ();

			transition.Duration = .3f;
			transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.Linear);
			transition.Type = CAAnimation.TransitionMoveIn;
			transition.Subtype = CAAnimation.TransitionFromTop;
			NavigationController.View.Layer.RemoveAllAnimations ();
			NavigationController.View.Layer.AddAnimation (transition, null);

			NavigationController.PushViewController (screen, false);
		}

		public static void PopViewControllerLikeDismissViewWithCallback(Action callback)
		{
			CATransaction.Begin ();

			CATransaction.CompletionBlock = callback;

			CATransition transition = new CATransition ();

			transition.Duration = .3f;
			transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.Linear);
			transition.Type = CAAnimation.TransitionReveal;
			transition.Subtype = CAAnimation.TransitionFromBottom;
			NavigationController.View.Layer.RemoveAllAnimations ();
			NavigationController.View.Layer.AddAnimation (transition, null);

			NavigationController.PopViewController (false);

			CATransaction.Begin ();
		}


		public static void PopViewControllerLikeDismissView()
		{
			CATransition transition = new CATransition ();

			transition.Duration = .3f;
			transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.Linear);
			transition.Type = CAAnimation.TransitionReveal;
			transition.Subtype = CAAnimation.TransitionFromBottom;
			NavigationController.View.Layer.RemoveAllAnimations ();
			NavigationController.View.Layer.AddAnimation (transition, null);

			NavigationController.PopViewController (false);
		}

		public static void PopToViewControllerLikeDismissView(UIViewController screen)
		{
			CATransition transition = new CATransition ();

			transition.Duration = .3f;
			transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.Linear);
			transition.Type = CAAnimation.TransitionReveal;
			transition.Subtype = CAAnimation.TransitionFromBottom;
			NavigationController.View.Layer.RemoveAllAnimations ();
			NavigationController.View.Layer.AddAnimation (transition, null);

			NavigationController.PopToViewController(screen, false);
		}

		public static void OpenBoard(Venue board){
			var venueInterface = new UIVenueInterface (board);
			AppDelegate.NavigationController.PushViewController (venueInterface, true);
		}

		public override void RegisteredForRemoteNotifications (
			UIApplication application, NSData deviceToken)
		{
			// Get current device token
			var DeviceToken = deviceToken.Description;
			if (!string.IsNullOrWhiteSpace(DeviceToken)) {
				DeviceToken = DeviceToken.Trim('<').Trim('>');
			}

			// Get previous device token
			var oldDeviceToken = NSUserDefaults.StandardUserDefaults.StringForKey("PushDeviceToken");

			// Has the token changed?
			if (string.IsNullOrEmpty(oldDeviceToken) || !oldDeviceToken.Equals(DeviceToken))
			{
				//TODO: Put your own logic here to notify your server that the device token has changed/been created!
			}

			// Save new device token 
			NSUserDefaults.StandardUserDefaults.SetString(DeviceToken, "PushDeviceToken");
		}

		public override void FailedToRegisterForRemoteNotifications (UIApplication application , NSError error)
		{
			new UIAlertView("Error registering push notifications", error.LocalizedDescription, null, "OK", null).Show();
		}
	}
}

