using System.Drawing;
using Clubby.Infrastructure;
using Clubby.Screens.Controls;
using CoreGraphics;
using Facebook.CoreKit;
using Facebook.LoginKit;
using Foundation;
using UIKit;
  
namespace Clubby.Screens
{
	public class LoginScreen : UIViewController
	{
		const int fontSize = 18;

		LoginButton logInButton;
		UIImageView guestButton;
		bool TapsEmailButton;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = AppDelegate.ClubbyBlack;

			this.AutomaticallyAdjustsScrollViewInsets = false;

			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;

			InitializeInterface ();
		}

		public override void ViewDidAppear(bool animated){
			TapsEmailButton = false;			
		}

		private void InitializeInterface()
		{
			// create our image view
			LoadBackground ();

			// load buttons
			LoadFBButton ();

			LoadGuestButton ();

			LoadWarning ();
		}

		private void LoadGuestButton(){
			guestButton = new UIImageView ();
			guestButton.Frame = new CGRect (logInButton.Frame.X, logInButton.Frame.Bottom + 10, logInButton.Frame.Width, 30);
			guestButton.BackgroundColor = UIColor.FromRGBA (0,0,0,0);

			var tapEmailView = new UITapGestureRecognizer (delegate(UITapGestureRecognizer obj) {
				if (!TapsEmailButton){
					TapsEmailButton = true;
					AppDelegate.NavigationController.PushViewController(new MainMenuScreen(), true);
				}
			});
			guestButton.AddGestureRecognizer (tapEmailView);
			guestButton.UserInteractionEnabled = true;

			var emailLabel = new UILabel ();
			emailLabel.Frame = new CGRect (0, 0, guestButton.Frame.Width, 0);
			emailLabel.Font = UIFont.SystemFontOfSize (14, UIFontWeight.Light);
			emailLabel.TextColor = UIColor.White;
			emailLabel.Text = "or Enter as Guest";
			emailLabel.TextAlignment = UITextAlignment.Center;
			emailLabel.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);

			var size = emailLabel.SizeThatFits (emailLabel.Frame.Size);
			emailLabel.Frame = new CGRect (emailLabel.Frame.X, emailLabel.Frame.Y, emailLabel.Frame.Width, size.Height);

			emailLabel.Center = new CGPoint (guestButton.Frame.Width / 2, guestButton.Frame.Height / 2);

			guestButton.AddSubview (emailLabel);
			View.AddSubview (guestButton);
		}

		private void LoadBackground()
		{
			var videoRect = new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);

			if (AppDelegate.PhoneVersion == AppDelegate.PhoneVersions.iPhone4) {
				videoRect = new CGRect (-50, -50, AppDelegate.ScreenWidth * 1.5f, AppDelegate.ScreenHeight * 1.5f);
			} else {
				videoRect = new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);
			}
			var repeatVideo = new UIRepeatVideo(videoRect, NSUrl.FromFilename("./screens/login/video.mp4"));
				

			var logoView = new UIImageView ();
			using (var logo = UIImage.FromFile ("./screens/login/logo_2.png")) {
				logoView.Image = logo;
				logoView.Frame = new RectangleF (0, 0, (float)(logo.Size.Width * .4), (float)(logo.Size.Height * .4f));
			}
			logoView.Center = new PointF (AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight / 2);

			View.AddSubviews (repeatVideo.View, logoView);
		}

		private void LoadFBButton()
		{
			logInButton = new LoginButton (new CGRect (40, AppDelegate.ScreenHeight - 130, AppDelegate.ScreenWidth - 80, 50)) {
				LoginBehavior = LoginBehavior.Native,
				ReadPermissions = new [] { "public_profile", "user_friends" }
			};

			logInButton.Completed += (sender, e) => {
				if (e.Error != null) {
					return;
				}

				if (AccessToken.CurrentAccessToken != null){
					
					AppDelegate.NavigationController.PushViewController(new MainMenuScreen(), true);	
				}
			};

			// Handle actions once the user is logged out
			logInButton.LoggedOut += (sender, e) => CloudController.LogOut ();

			View.AddSubview (logInButton);
		}

		private void LoadWarning (){
			var label = new UITextView ();
			label.Frame = new CGRect (5, guestButton.Frame.Bottom, AppDelegate.ScreenWidth - 10, 0);
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