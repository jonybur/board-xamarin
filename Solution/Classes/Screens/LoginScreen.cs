using System.Drawing;
using Board.Infrastructure;
using Board.Screens.Controls;
using CoreGraphics;
using Facebook.LoginKit;
using Foundation;
using UIKit;
  
namespace Board.Screens
{
	public class LoginScreen : UIViewController
	{
		const int fontSize = 18;

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

		private void LoadBackground()
		{
			this.AutomaticallyAdjustsScrollViewInsets = false;

			var repeaterVideo = new UIRepeatVideo (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight), NSUrl.FromFilename ("./timelapse.mp4"));

			UIImageView logoView;
			using (UIImage logo = UIImage.FromFile ("./screens/login/logo.png")) {
				logoView = new UIImageView (logo);
				logoView.Frame = new RectangleF (0, 0, (float)(logo.Size.Width/2), (float)(logo.Size.Height/2));
			}
			logoView.Center = new PointF (AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight/6);

			View.AddSubviews (repeaterVideo.View, logoView);
		}
	}
}