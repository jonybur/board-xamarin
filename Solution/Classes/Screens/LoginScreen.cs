using System.Drawing;
using System.Threading;

using AVFoundation;
using Board.JsonResponses;

using Board.Utilities;
using CoreGraphics;
using Board.Infrastructure;
using CoreMedia;
using Facebook.CoreKit;
using Newtonsoft.Json;
using Facebook.LoginKit;

using Foundation;
using UIKit;
  
namespace Board.Screens
{
	public class LoginScreen : UIViewController
	{
		const int fontSize = 18;

		AVPlayer _player;
		Thread looper;

		bool keepLooping = true;

		LoginButton logInButton;

		public LoginScreen (){
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.AutomaticallyAdjustsScrollViewInsets = false;

			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;

			InitializeInterface ();
		}

		private void LoadFBButton()
		{
			logInButton = new LoginButton (new CGRect (0, 0, AppDelegate.ScreenWidth - 70, 50)) {
				LoginBehavior = LoginBehavior.Native,
				Center = new CGPoint(AppDelegate.ScreenWidth/2, AppDelegate.ScreenHeight * (.90f)),
				ReadPermissions = new [] { "public_profile", "user_birthday" }
			};

			logInButton.Completed += (sender, e) => {
				if (e.Error != null) {
					return;
				}

				bool result = CloudController.LogIn();

				if (result) {
					AppDelegate.containerScreen = new ContainerScreen ();
					AppDelegate.NavigationController.PushViewController(AppDelegate.containerScreen, true);
				} else {
					UIAlertController alert = UIAlertController.Create("Couldn't connect", "Please ensure you have a connection to the Internet.", UIAlertControllerStyle.Alert);
					alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));
					NavigationController.PresentViewController (alert, true, null);
				}
			};

			// Handle actions once the user is logged out
			logInButton.LoggedOut += (sender, e) => {
				// Handle your logout
				CloudController.LogOut();
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
					Thread.Sleep (1000);
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
			AVPlayerItem _playerItem;
			using (AVAsset _asset = AVAsset.FromUrl (NSUrl.FromFilename ("./timelapse.mp4"))) {
				_playerItem = new AVPlayerItem (_asset);
			}
			_player = new AVPlayer (_playerItem);
			AVPlayerLayer _playerLayer = AVPlayerLayer.FromPlayer (_player);

			_playerLayer.VideoGravity = AVLayerVideoGravity.Resize;

			_playerLayer.Frame = new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);

			_player.ActionAtItemEnd = AVPlayerActionAtItemEnd.Pause;
			_player.Volume = 0;

			_player.Play ();

			looper = new Thread (new ThreadStart (LooperMethod));
			looper.Start ();

			UIImageView logoView;
			using (UIImage logo = UIImage.FromFile ("./screens/login/logo.png")) {
				logoView = new UIImageView (logo);
				logoView.Frame = new RectangleF (0, 0, (float)(logo.Size.Width/2), (float)(logo.Size.Height/2));
			}
			logoView.Center = new PointF (AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight/6);

			View.Layer.AddSublayer (_playerLayer);
			View.AddSubviews (logoView);
		}
	}
}