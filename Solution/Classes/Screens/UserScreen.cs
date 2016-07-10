using System;
using Haneke;
using Foundation;
using Clubby.Screens;
using Clubby.Screens.Controls;
using Clubby.Infrastructure;
using Facebook.LoginKit;
using CoreGraphics;
using UIKit;

namespace Clubby.Screens
{
	public class UserScreen : UIViewController
	{
		protected UIImageView BackButton;
		UITapGestureRecognizer backTap;

		public UserScreen ()
		{
		}

		public override void ViewDidLoad ()
		{
			View.BackgroundColor = AppDelegate.ClubbyBlack;

			var settingsView = new SettingsView ();

			CreateBackButton ();

			View.AddSubviews (settingsView, BackButton);
		}

		public override void ViewDidAppear (bool animated)
		{
			NavigationController.InteractivePopGestureRecognizer.Enabled = false;
			NavigationController.InteractivePopGestureRecognizer.Delegate = null;
		}

		protected void CreateBackButton()
		{
			BackButton = new UIImageView ();
			BackButton.Frame = new CGRect (0, 0, 50, 50);

			var subView = new UIImageView ();
			subView.Frame = new CGRect (0, 0, 17, 17);
			subView.SetImage ("./boardinterface/lookup/cancel.png");

			BackButton.AddSubview (subView);
			subView.Center = new CGPoint (BackButton.Frame.Width / 2 - 3, BackButton.Frame.Height / 2 + 7);

			BackButton.Center = new CGPoint (BackButton.Frame.Width / 2 + 10, 35);
			BackButton.UserInteractionEnabled = true;

			backTap = new UITapGestureRecognizer (tg => AppDelegate.PopViewControllerLikeDismissView ());
			BackButton.AddGestureRecognizer (backTap);
		}
	}

	class SettingsView : UIScrollView{
		LoginButton LogOutButton;

		public SettingsView(){
			Frame = new CGRect(0,0,AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);

			var flagView = new UIImageView();
			flagView.Frame = new CGRect(0, 0, 150, 100);
			flagView.Center = new CGPoint(AppDelegate.ScreenWidth / 2, flagView.Frame.Height);
			flagView.ContentMode = UIViewContentMode.ScaleAspectFit;
			flagView.SetImage("./screens/settings/long_flag.png");
			flagView.Layer.CornerRadius = 10;
			flagView.ClipsToBounds = true;
			flagView.Alpha = .95f;

			var boardVersionLabel = new UILabel();
			boardVersionLabel.Frame = new CGRect(10, flagView.Frame.Bottom + 20, AppDelegate.ScreenWidth - 20, 32);
			var ver = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"];
			boardVersionLabel.Text = "Clubby " + ver.ToString();
			boardVersionLabel.Font = UIFont.SystemFontOfSize(30, UIFontWeight.Regular);
			boardVersionLabel.TextColor = AppDelegate.ClubbyYellow;
			boardVersionLabel.TextAlignment = UITextAlignment.Center;

			var legalLabel = new UILabel();
			legalLabel.Frame = new CGRect(15, boardVersionLabel.Frame.Bottom + 50, AppDelegate.ScreenWidth - 20, 14);
			legalLabel.Text = "LEGAL";
			legalLabel.Font = UIFont.SystemFontOfSize(14, UIFontWeight.Light);
			legalLabel.TextColor = UIColor.White;

			var privacyButton = new UIOneLineMenuButton((float)legalLabel.Frame.Bottom + 5);
			privacyButton.SetLabel("Privacy Policy");
			privacyButton.SetTapEvent (delegate {
				AppsController.OpenWebsite("http://getonboard.us/legal/privacy.pdf");
			});
			privacyButton.SuscribeToEvent();

			var termsButton = new UIOneLineMenuButton((float)privacyButton.Frame.Bottom + 1);
			termsButton.SetLabel("Terms of Service");
			termsButton.SetTapEvent (delegate {
				AppsController.OpenWebsite("http://getonboard.us/legal/terms.pdf");
			});
			termsButton.SuscribeToEvent();

			var licensesButton = new UIOneLineMenuButton((float)termsButton.Frame.Bottom + 1);
			licensesButton.SetLabel("Licenses");
			licensesButton.SetTapEvent (delegate {
				var licensesScreen = new LicensesScreen();
				AppDelegate.NavigationController.PushViewController(licensesScreen, true);
			});
			licensesButton.SuscribeToEvent();

			LoadFBButton ((float)licensesButton.Frame.Bottom + 50);

			AddSubviews(flagView, boardVersionLabel, legalLabel, privacyButton, termsButton, licensesButton, LogOutButton);

			ContentSize = new CGSize(AppDelegate.ScreenWidth, LogOutButton.Frame.Bottom + UIActionButton.Height * 2);
		}

		private void LoadFBButton(float yposition)
		{
			float height = 50;
			LogOutButton = new LoginButton (new CGRect (0, 0, AppDelegate.ScreenWidth - 70, height)) {
				LoginBehavior = LoginBehavior.Native,
				Center = new CGPoint(AppDelegate.ScreenWidth/2, yposition + height / 2)
			};

			// Handle actions once the user is logged out
			LogOutButton.LoggedOut += (sender, e) => {
				// Handle your logout
				CloudController.LogOut();
				var controllers = new UIViewController[1];
				controllers[0] = new LoginScreen();
				AppDelegate.NavigationController.SetViewControllers(controllers, true);
			};
		}
	}

}

