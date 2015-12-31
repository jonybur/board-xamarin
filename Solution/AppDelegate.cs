using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Facebook.CoreKit;

using System.Net.Http;

namespace Solution
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.

	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		public static UINavigationController NavigationController;
		public static UIViewController LoginController;

		public static CloudController CloudController;

		public static float ScreenWidth;
		public static float ScreenHeight;

		const string FacebookAppId = "761616930611025";
		const string FacebookDisplayName = "CityBoard";

		// This method is invoked when the application hqas loaded and is ready to run. In this
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			ScreenWidth = (float)UIScreen.MainScreen.Bounds.Width;
			ScreenHeight = (float)UIScreen.MainScreen.Bounds.Height;

			//CloudController = CloudController.DefaultService;

			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			LoginController = new LoginScreen ();
			NavigationController = new UINavigationController();

			window.RootViewController = LoginController;

			NavigationController.PushViewController (LoginController, false);

			window.AddSubview (NavigationController.View);

			window.MakeKeyAndVisible ();

			// FACEBOOK

			Profile.EnableUpdatesOnAccessTokenChange (true);
			Settings.AppID = FacebookAppId;
			Settings.DisplayName = FacebookDisplayName;

			// If you have defined a root view controller, set it here:
			// window.RootViewController = myViewController;

			ApplicationDelegate.SharedInstance.FinishedLaunching (app, options);

			return true;
		}

		public override bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			// We need to handle URLs by passing them to their own OpenUrl in order to make the SSO authentication works.
			return ApplicationDelegate.SharedInstance.OpenUrl (application, url, sourceApplication, annotation);
		}


	}
}

