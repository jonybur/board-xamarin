using System;
using System.Drawing;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreMedia;
using AVFoundation;
using CoreAnimation;
using CoreText;

using System.Net.Http;

using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

using Facebook.LoginKit;
using Facebook.CoreKit;

namespace Solution
{
	public partial class LoginScreen : UIViewController
	{
		const int fontSize = 18;

		AVPlayer _player;
		AVPlayerLayer _playerLayer;
		AVAsset _asset;
		AVPlayerItem _playerItem;
		Thread looper;


		bool keepLooping = true;


		string [] extendedPermissions = new [] { "user_about_me", "read_stream"};
		string [] publishPermissions = new [] { "publish_actions" };
		LoginButton logInButton;

		const string FacebookAppId = "761616930611025";
		const string DisplayName = "CityBoard";

		public LoginScreen () : base ("Board", null){}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.AutomaticallyAdjustsScrollViewInsets = false;

			if (Profile.CurrentProfile != null) {
				UIViewController nextScreen = new MainMenuScreen ();
				NavigationController.PushViewController (nextScreen, false);
			} else {
				NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;

				NavigationController.NavigationBarHidden = true;

				InitializeInterface ();
			}
		}


		private void LoadFBButton()
		{
			logInButton = new LoginButton (new CGRect (0, 0, AppDelegate.ScreenWidth - 70, 50)) {
				LoginBehavior = LoginBehavior.Native,
				Center = new CGPoint(AppDelegate.ScreenWidth/2, AppDelegate.ScreenHeight * (.90f)),
				ReadPermissions = extendedPermissions,
				PublishPermissions = publishPermissions
			};

			logInButton.Completed += (sender, e) => {
				if (e.Error != null) {
					// Handle if there was an error
				}

				UIViewController nextScreen = new MainMenuScreen();

				NavigationController.PushViewController(nextScreen, true);

			};

			// Handle actions once the user is logged out
			logInButton.LoggedOut += (sender, e) => {
				// Handle your logout
			};

			View.AddSubview (logInButton);
		}

		private void InitializeInterface()
		{
			// create our image view
			LoadBackground ();

			// load buttons
			LoadFBButton ();
		}


		private void LooperMethod()
		{
			const int NSEC_PER_SEC = 1000000000;

			while (keepLooping) {

				int time = 0;

				while (time < 9) {
					System.Threading.Thread.Sleep (1000);
					time++;
				}

				InvokeOnMainThread (() => {
					_player.Seek (new CMTime (0, NSEC_PER_SEC));
				});
			}
		}



		private void LoadBackground()
		{
			this.AutomaticallyAdjustsScrollViewInsets = false;


			_asset = AVAsset.FromUrl (NSUrl.FromFilename ("./timelapse.mp4"));
			_playerItem = new AVPlayerItem (_asset);
			_player = new AVPlayer (_playerItem);
			_playerLayer = AVPlayerLayer.FromPlayer (_player);
			_playerLayer.Frame = new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);
			_player.ActionAtItemEnd = AVPlayerActionAtItemEnd.Pause;
			_player.Volume = 0;

			_player.Play ();

			looper = new Thread (new ThreadStart (LooperMethod));
			looper.Start ();

			UIImage logo = UIImage.FromFile ("./intro/logo.png");
			UIImageView logoView = new UIImageView (logo);
			logoView.Frame = new RectangleF (0, 0, (float)(logo.Size.Width/2), (float)(logo.Size.Height/2));
			logoView.Center = new PointF (AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight/6);

			View.Layer.AddSublayer (_playerLayer);
			View.AddSubviews (logoView);
		}
	}
}