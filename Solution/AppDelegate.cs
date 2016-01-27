using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Facebook.CoreKit;

using Google.Maps;
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

		public static CloudController CloudController;

		public static float ScreenWidth;
		public static float ScreenHeight;

		public static UIColor CityboardOrange;
		public static UIColor CityboardBlue;

		public static double Latitude;
		public static double Longitude;

		const string FacebookAppId = "761616930611025";
		const string FacebookDisplayName = "CityBoard";

		// This method is invoked when the application hqas loaded and is ready to run. In this
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		const string MapsApiKey = "AIzaSyAyjPtEvhmhHHa5_aPiZPiPN3GUtIXxO6I";


		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			CityboardOrange = UIColor.FromRGB(244, 108, 85);
			CityboardBlue = UIColor.FromRGB(38, 106, 154);

			MapServices.ProvideAPIKey (MapsApiKey);

			Latitude = 0;
			Longitude = 0;

			// FACEBOOK

			Profile.EnableUpdatesOnAccessTokenChange (true);
			Settings.AppID = FacebookAppId;
			Settings.DisplayName = FacebookDisplayName;
			ApplicationDelegate.SharedInstance.FinishedLaunching (app, options);

			ScreenWidth = (float)UIScreen.MainScreen.Bounds.Width;
			ScreenHeight = (float)UIScreen.MainScreen.Bounds.Height;

			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			UIViewController screen;

			if (Profile.CurrentProfile != null) {
				screen = new MainMenuScreen ();
			} else {
				screen = new LoginScreen ();	
			}


			NavigationController = new UINavigationController();

			window.RootViewController = screen;

			NavigationController.PushViewController (screen, false);

			window.AddSubview (NavigationController.View);

			window.MakeKeyAndVisible ();

			return true;
		}

		public override bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			// We need to handle URLs by passing them to their own OpenUrl in order to make the SSO authentication works.
			return ApplicationDelegate.SharedInstance.OpenUrl (application, url, sourceApplication, annotation);
		}


	}
}

