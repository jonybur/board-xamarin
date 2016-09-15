using System;
using System.Threading;
using Board.Schema;
using CoreGraphics;
using Facebook.CoreKit;
using UIKit;

namespace Board.Interface.VenueInterface
{
	// user interface - connects to the board controller
	// also called BoardView
	public class UIVenueInterface : UIViewController
	{
		public const int BannerHeight = 66;

		public static Board.Schema.Board board;

		public static CancellationTokenSource DownloadCancellation;

		public UIVenueInterface (Board.Schema.Board _board){
			board = _board;
			DownloadCancellation = new CancellationTokenSource();
		}

		public override void DidReceiveMemoryWarning  ()
		{
			GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced);
		}

		public override void ViewDidLoad ()
		{
			AppEvents.LogEvent ("entersBoard");

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
			View.BackgroundColor = UIColor.White;

			// generate the scrollview and the zoomingscrollview
			var statusBarView = new UIView (new CGRect (0, 0, AppDelegate.ScreenWidth, 20));
			statusBarView.Alpha = .6f;
			statusBarView.BackgroundColor = UIColor.White;

			View.AddSubview (statusBarView);

			var infoBox = new UIInfoBox (board);
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
				UIVenueInterface.DownloadCancellation.Cancel ();

				AppDelegate.NavigationController.PopViewController (true);

				blockButton = true;
			}
		}
	}
}