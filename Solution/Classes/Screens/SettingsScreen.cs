using CoreGraphics;
using UIKit;
using Facebook.LoginKit;
using Board.Screens.Controls;

namespace Board.Screens
{
	public class SettingsScreen : UIViewController
	{
		MenuBanner Banner;
		LoginButton logInButton;

		public SettingsScreen ()
		{
		}

		public override void ViewDidLoad ()
		{
			LoadBanner ();

			LoadFBButton ();

			View.BackgroundColor = UIColor.White;
		}


		public override void ViewDidAppear(bool animated)
		{
			Banner.SuscribeToEvents ();
		}

		public override void ViewDidDisappear(bool animated)
		{
			Banner.UnsuscribeToEvents ();
		}

		private void LoadFBButton()
		{
			logInButton = new LoginButton (new CGRect (0, 0, AppDelegate.ScreenWidth - 70, 50)) {
				LoginBehavior = LoginBehavior.Native,
				Center = new CGPoint(AppDelegate.ScreenWidth/2, AppDelegate.ScreenHeight * (.90f))
			};

			// Handle actions once the user is logged out
			logInButton.LoggedOut += (sender, e) => {
				// Handle your logout
				UIViewController[] controllers = new UIViewController[1];
				controllers[0] = new LoginScreen();

				NavigationController.SetViewControllers(controllers, true);
			};

			View.AddSubview (logInButton);
		}

		private void LoadBanner()
		{
			Banner = new MenuBanner ("./screens/settings/banner/" + AppDelegate.PhoneVersion + ".jpg");

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					NavigationController.PopViewController(false);
				}
			});

			Banner.AddTap (tap);
			View.AddSubview (Banner);
		}
	}
}

