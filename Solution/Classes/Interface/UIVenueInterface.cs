using System;
using System.Collections.Generic;
using BigTed;
using Clubby.Schema;
using Clubby.Screens;
using Clubby.Utilities;
using Facebook.CoreKit;
using System.Threading;
using CoreGraphics;
using UIKit;

namespace Clubby.Interface
{
	// user interface - connects to the board controller
	// also called BoardView
	public class UIVenueInterface : UIViewController
	{
		public const int BannerHeight = 66;

		public static Venue venue;

		public static CancellationTokenSource DownloadCancellation;

		bool firstLoad;

		public UIVenueInterface (Venue _venue){
			venue = _venue;
			DownloadCancellation = new CancellationTokenSource();
			firstLoad = true;
		}

		public override void DidReceiveMemoryWarning  ()
		{
			GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced);
		}

		public override void ViewDidLoad ()
		{
			AppEvents.LogEvent ("entersBoard");

			//var json = JsonUtilty.GenerateDeleteAllJson ();
			//CloudController.UpdateBoard (board.Id, json);

			// if it reaches this section, user has been logged in and authorized
			base.ViewDidLoad ();

			AutomaticallyAdjustsScrollViewInsets = false;

			BTProgressHUD.Show();

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

			var infoBox = new UIInfoBox (venue);
			View.AddSubview (infoBox);

			var backButton = new UIBackButton ();
			View.AddSubview (backButton);
		}

		public override void ViewDidAppear(bool animated)
		{
			if (firstLoad) {
				
				firstLoad = false;

				BTProgressHUD.Dismiss ();
			}
		}

		public void ExitBoard()
		{
			View.BackgroundColor = UIColor.Black;  
			MemoryUtility.ReleaseUIViewWithChildren (View);
		}
	}


	sealed class UIBackButton : UIButton {
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

		bool blockButton = false;

		private void TouchButton(object obj, EventArgs args){
			if (!blockButton){
				UIVenueInterface.DownloadCancellation.Cancel ();

				var containerScreen = AppDelegate.NavigationController.ViewControllers[AppDelegate.NavigationController.ViewControllers.Length - 2] as ContainerScreen;
				if (containerScreen != null) {
					containerScreen.LoadLastScreen();
				}
				AppDelegate.PopViewControllerWithCallback(AppDelegate.ExitBoardInterface);
				blockButton = true;
			}
		}
	}
}