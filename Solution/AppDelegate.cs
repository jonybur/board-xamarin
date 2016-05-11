﻿using System;
using Board.Infrastructure;
using Board.Interface;
using Board.JsonResponses;
using Board.Screens;
using Board.Schema;
using CoreAnimation;
using CoreLocation;
using Facebook.CoreKit;
using Foundation;
using Google.Maps;
using UIKit;

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
		public static UINavigationController NavigationController;
		public static float ScreenWidth;
		public static float ScreenHeight;

		public static UIColor BoardOrange;
		public static UIColor BoardBlue;
		public static UIColor BoardLightBlue;
		public static UIColor BoardBlack;

		public static UIFont Narwhal12;
		public static UIFont Narwhal14;
		public static UIFont Narwhal16;
		public static UIFont Narwhal18;
		public static UIFont Narwhal20;
		public static UIFont Narwhal24;
		public static UIFont Narwhal30;
		public static UIFont SystemFontOfSize16;
		public static UIFont SystemFontOfSize18;
		public static UIFont SystemFontOfSize20;

		public static User BoardUser;
		public static CLLocationCoordinate2D UserLocation;
		public static UIBoardInterface BoardInterface;

		public static string PhoneVersion;

		public const string APIAddress = "45.55.232.144";
		public const string FacebookAppId = "793699580736093";
		public const string FacebookDisplayName = "Board Alpha - Deve­l­o­p­ment";
		public const string GoogleMapsAPIKey = "AIzaSyAUO-UX9QKVWK421yjXqoo02N5TYrG_hY8";
		public const string UberServerToken = "4y1kRu3Kt-LWdTeXcktgphAN7qZlltsTRTbvwIQ_";
		const string MapsApiKey = "AIzaSyAyjPtEvhmhHHa5_aPiZPiPN3GUtIXxO6I";

		public static string BoardToken;
		public static AmazonS3TicketResponse AmazonS3Ticket;
		public static string EncodedBoardToken;
	
		public static ContainerScreen containerScreen;

		// This method is invoked when the application hqas loaded and is ready to run. In this
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			BoardOrange = UIColor.FromRGB(244, 108, 85);
			BoardBlue = UIColor.FromRGB(38, 106, 154);
			BoardLightBlue = UIColor.FromRGB(45, 121, 180);
			BoardBlack = UIColor.FromRGB (40, 40, 40);
			Narwhal12 = UIFont.FromName ("narwhal-bold", 12);
			Narwhal14 = UIFont.FromName ("narwhal-bold", 14);
			Narwhal16 = UIFont.FromName ("narwhal-bold", 16);
			Narwhal18 = UIFont.FromName ("narwhal-bold", 18);
			Narwhal20 = UIFont.FromName ("narwhal-bold", 20);
			Narwhal24 = UIFont.FromName ("narwhal-bold", 24);
			Narwhal30 = UIFont.FromName ("narwhal-bold", 29);
			SystemFontOfSize16 = UIFont.SystemFontOfSize (16);
			SystemFontOfSize18 = UIFont.SystemFontOfSize (18);
			SystemFontOfSize20 = UIFont.SystemFontOfSize (20);

			StorageController.Initialize ();

			MapServices.ProvideAPIKey (MapsApiKey);

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
			} else if (ScreenWidth == 320) {
				if (ScreenHeight == 480) {
					PhoneVersion = "4";
				} else if (ScreenHeight == 568) {
					PhoneVersion = "5";
				}
			} else {
				PhoneVersion = "6";
			}

			UIViewController screen;

			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			bool result = CloudController.LogIn();

			if (result) {
				containerScreen = new ContainerScreen ();
				screen = containerScreen;
			} else {
				screen = new LoginScreen ();
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
			BoardInterface.ExitBoard ();
			BoardInterface.Dispose ();
			BoardInterface = null;
			GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced);
		}

		// TODO: use to kill screens (memoryutility) after popping them
		public static void PopViewControllerWithCallback(Action callback)
		{
			CATransaction.Begin ();

			CATransaction.CompletionBlock = callback;

			NavigationController.PopViewController (true);

			CATransaction.Begin ();
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

