﻿using System;
using CoreGraphics;
using Facebook.CoreKit;
using UIKit;

namespace Clubby.Interface.UserInterface
{
	// user interface - connects to the board controller
	// also called BoardView
	public class UIUserInterface : UIViewController
	{
		public const int BannerHeight = 66;
		string userId;

		public UIUserInterface (string _userId){
			userId = _userId;
		}

		public override void DidReceiveMemoryWarning  ()
		{
			GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced);
		}

		public override void ViewDidLoad ()
		{
			AppEvents.LogEvent ("entersUser");

			// if it reaches this section, user has been logged in and authorized
			base.ViewDidLoad ();

			AutomaticallyAdjustsScrollViewInsets = false;

			InitializeInterface ();

			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;
		}

		private void InitializeInterface()
		{
			// This was main color
			View.BackgroundColor = AppDelegate.ClubbyBlack;

			// generate the scrollview and the zoomingscrollview
			var statusBarView = new UIView (new CGRect (0, 0, AppDelegate.ScreenWidth, 20));
			statusBarView.Alpha = .6f;
			statusBarView.BackgroundColor = AppDelegate.ClubbyBlack;

			View.AddSubview (statusBarView);

			var infoBox = new UIInfoBox (userId);
			View.AddSubview (infoBox);

			var backButton = new UIBackButton ();
			View.AddSubview (backButton);
		}

		public override void ViewDidAppear(bool animated)
		{
			NavigationController.InteractivePopGestureRecognizer.Enabled = true;
			NavigationController.InteractivePopGestureRecognizer.Delegate = null;
		}
	}


	sealed class UIBackButton : UIButton {
		bool blockButton;

		public UIBackButton(){
			Frame = new CGRect(0, 0, 70, 60);

			using (var image = UIImage.FromFile ("./boardinterface/back_button3.png")) {
				var imageView = new UIImageView ();
				imageView.Frame = new CGRect (0, 0, image.Size.Width / 2, image.Size.Height / 2);
				imageView.Image = image;
				imageView.Center = new CGPoint (Frame.Width / 2 + 5, Frame.Height / 2 + 10);

				AddSubview (imageView);
			}

			TouchUpInside += TouchButton;
		}

		private void TouchButton(object obj, EventArgs args){
			if (!blockButton){
				AppDelegate.NavigationController.PopViewController (true);

				blockButton = true;
			}
		}
	}
}