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

		LoginButton logInButton;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.AutomaticallyAdjustsScrollViewInsets = false;

			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;

			InitializeInterface ();
		}

		private void InitializeInterface()
		{
			// create our image view
			LoadBackground ();

			// load buttons
			LoadFBButton ();

			LoadWarning ();
		}

		private void LoadBackground()
		{
			var repeaterVideo = new UIRepeatVideo (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight), NSUrl.FromFilename ("./timelapse.mp4"));

			var logoView = new UIImageView ();
			using (var logo = UIImage.FromFile ("./screens/login/logo.png")) {
				logoView.Image = logo;
				logoView.Frame = new RectangleF (0, 0, (float)(logo.Size.Width/2), (float)(logo.Size.Height/2));
			}
			logoView.Center = new PointF (AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight * 0.35f);

			View.AddSubviews (repeaterVideo.View, logoView);
		}

		private void LoadFBButton()
		{
			logInButton = new LoginButton (new CGRect (40, AppDelegate.ScreenHeight - 100, AppDelegate.ScreenWidth - 80, 50)) {
				LoginBehavior = LoginBehavior.Native,
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

		private void LoadWarning (){
			var label = new UITextView ();
			label.Frame = new CGRect (5, logInButton.Frame.Bottom, AppDelegate.ScreenWidth - 10, 0);
			label.Font = UIFont.SystemFontOfSize (11, UIFontWeight.Light);
			label.TextColor = UIColor.White;
			label.Text = "By continuing, you agree to our Terms of Service\nand Privacy Policy";
			label.TextAlignment = UITextAlignment.Center;
			label.ScrollEnabled = false;
			label.Editable = false;
			label.Selectable = false;
			label.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);

			var size = label.SizeThatFits (label.Frame.Size);
			label.Frame = new CGRect (label.Frame.X, label.Frame.Y, label.Frame.Width, size.Height);

			View.AddSubview (label);
		}

	}
}