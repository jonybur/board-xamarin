using System;
using System.Net;
using System.Collections.Generic;

using Foundation;
using UIKit;
using Facebook.CoreKit;

using Google.Maps;
using Board.Utilities;
using CoreAnimation;
using Board.JsonResponses;
using Board.Interface;
using Board.Screens;
using Board.Facebook;

namespace Board
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
		public static List<Board.Schema.Board> ListNewBoards;

		public static UINavigationController NavigationController;

		public static float ScreenWidth;
		public static float ScreenHeight;

		public static UIColor BoardOrange;
		public static UIColor BoardBlue;
		public static UIColor BoardLightBlue;
		public static UIColor BoardBlack;

		public static UIFont Narwhal20;
		public static UIFont Narwhal24;
		public static UIFont SystemFontOfSize16;
		public static UIFont SystemFontOfSize18;
		public static UIFont SystemFontOfSize20;

		public static BoardInterface boardInterface;

		public static double Latitude;
		public static double Longitude;

		public static string PhoneVersion;

		public const string FacebookAppId = "793699580736093";
		public const string GoogleMapsAPIKey = "AIzaSyAUO-UX9QKVWK421yjXqoo02N5TYrG_hY8";
		public const string FacebookDisplayName = "Board Alpha - Deve­l­o­p­ment";

		public static string BoardToken;
		public static string EncodedBoardToken;
	
		public static int Autosize = 140;

		public static ContainerScreen containerScreen;

		/*
		public const string FacebookAppId = "761616930611025";
		public const string FacebookDisplayName = "Board";
		*/

		// This method is invoked when the application hqas loaded and is ready to run. In this
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		const string MapsApiKey = "AIzaSyAyjPtEvhmhHHa5_aPiZPiPN3GUtIXxO6I";

		public const bool ServerActive = false;


		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			BoardOrange = UIColor.FromRGB(244, 108, 85);
			BoardBlue = UIColor.FromRGB(38, 106, 154);
			BoardLightBlue = UIColor.FromRGB(45, 121, 180);
			BoardBlack = UIColor.FromRGB (40, 40, 40);
			Narwhal20 = UIFont.FromName ("narwhal-bold", 20);
			Narwhal24 = UIFont.FromName ("narwhal-bold", 24);
			SystemFontOfSize16 = UIFont.SystemFontOfSize (16);
			SystemFontOfSize18 = UIFont.SystemFontOfSize (18);
			SystemFontOfSize20 = UIFont.SystemFontOfSize (20);

			MapServices.ProvideAPIKey (MapsApiKey);

			Latitude = 0;
			Longitude = 0;

			ListNewBoards = new List<Board.Schema.Board> ();

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

			if (ServerActive) {
				if (AccessToken.CurrentAccessToken != null) {
					string json = "{ \"userId\": \"" + AccessToken.CurrentAccessToken.UserID + "\", " +
					              "\"accessToken\": \"" + AccessToken.CurrentAccessToken.TokenString + "\" }";

					string result = CommonUtils.JsonPOSTRequest ("http://192.168.1.114:5000/api/account/login", json);
			
					TokenResponse tk = TokenResponse.Deserialize (result);

					if (Profile.CurrentProfile != null && result != "InternalServerError" && result != "ConnectFailure" && tk != null && tk.authToken != null & tk.authToken != string.Empty) {
						BoardToken = tk.authToken;
						EncodedBoardToken = WebUtility.UrlEncode(AppDelegate.BoardToken);

						containerScreen = new ContainerScreen ();
						screen = containerScreen;
					} else {
						screen = new LoginScreen (result);	
					}
				} else {
					screen = new LoginScreen ();
				}
			} else {
				if (Profile.CurrentProfile != null && AccessToken.CurrentAccessToken != null && FacebookUtils.HasPermission("public_profile")) {
					containerScreen = new ContainerScreen ();
					screen = containerScreen;
				} else {
					screen = new LoginScreen ();	
				}
			}

			NavigationController = new UINavigationController();

			window.RootViewController = screen;

			NavigationController.PushViewController (screen, false);

			window.AddSubview (NavigationController.View);

			window.MakeKeyAndVisible ();


			/*
			new System.Threading.Thread (() => 
				{
					while (true) {
						System.Threading.Thread.Sleep (1000);
						GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced);
					}
				}).Start ();
			*/

			return true;
		}

		public override bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			// We need to handle URLs by passing them to their own OpenUrl in order to make the SSO authentication works.
			return ApplicationDelegate.SharedInstance.OpenUrl (application, url, sourceApplication, annotation);
		}

		public static void ExitBoardInterface()
		{
			boardInterface.ExitBoard ();
			boardInterface.Dispose ();
			boardInterface = null;
			GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced);
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

		public static void PopViewLikeDismissView()
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
	}
}

