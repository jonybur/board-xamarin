using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Facebook.CoreKit;

using Google.Maps;
using System.Net.Http;

using System.Dynamic;

namespace Solution
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
		public static List<Board> ListNewBoards;

		public static UINavigationController NavigationController;

		public static float ScreenWidth;
		public static float ScreenHeight;

		public static UIColor CityboardOrange;
		public static UIColor CityboardBlue;
		public static UIColor CityboardLightBlue;

		public static double Latitude;
		public static double Longitude;

		public static string PhoneVersion;

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
			CityboardLightBlue = UIColor.FromRGB(45, 121, 180);

			MapServices.ProvideAPIKey (MapsApiKey);

			Latitude = 0;
			Longitude = 0;

			ListNewBoards = new List<Board> ();

			// FACEBOOK

			Profile.EnableUpdatesOnAccessTokenChange (true);
			Settings.AppID = FacebookAppId;
			Settings.DisplayName = FacebookDisplayName;
			ApplicationDelegate.SharedInstance.FinishedLaunching (app, options);

			ScreenWidth = (float)UIScreen.MainScreen.Bounds.Width;
			ScreenHeight = (float)UIScreen.MainScreen.Bounds.Height;

			if (ScreenWidth == 375) {
				PhoneVersion = "6";
			} else if (ScreenWidth == 414) {
				PhoneVersion = "6plus";
			} else {
				PhoneVersion = "6";
			}


			UIViewController screen;

			// cxreate a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			if (AccessToken.CurrentAccessToken != null) {
				string json = "{ \"userId\": \"" + AccessToken.CurrentAccessToken.UserID + "\", " +
				              "\"accessToken\": \"" + AccessToken.CurrentAccessToken.TokenString + "\" }";

				string result = CommonUtils.JsonRequest ("http://10.0.11.144:5000/api/account/login", json);
			
				// change || for && when testing server
				if (Profile.CurrentProfile != null || (result != "InternalServerError" && result != "ConnectFailure")) {
					screen = new MainMenuScreen ();
				} else {
					screen = new LoginScreen (result);	
				}
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

