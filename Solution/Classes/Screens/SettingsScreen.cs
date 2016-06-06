using CoreGraphics;
using UIKit;
using Facebook.LoginKit;
using Board.Screens.Controls;
using Foundation;
using Board.Infrastructure;
using Board.Utilities;
using Haneke;

namespace Board.Screens
{
	public class SettingsScreen : UIViewController
	{
		UIMenuBanner Banner;

		public override void ViewDidLoad ()
		{
			LoadBanner ();

			var settingsView = new SettingsView ();
			View.AddSubviews (settingsView, Banner);

			View.BackgroundColor = UIColor.White;
		}

		public override void ViewDidAppear(bool animated)
		{
			Banner.SuscribeToEvents ();
		}

		public override void ViewDidDisappear(bool animated)
		{
			Banner.UnsuscribeToEvents ();
			//MemoryUtility.ReleaseUIViewWithChildren (View);
		}

		class SettingsView : UIScrollView{
			LoginButton LogOutButton;

			public SettingsView(){
				Frame = new CGRect(0,0,AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);

				var flagView = new UIImageView();
				flagView.Frame = new CGRect(0, 0, 150, 100);
				flagView.Center = new CGPoint(AppDelegate.ScreenWidth / 2, UIMenuBanner.Height * .75f + flagView.Frame.Height);
				flagView.ContentMode = UIViewContentMode.ScaleAspectFit;
				flagView.SetImage("./screens/settings/long_flag.png");
				flagView.Layer.CornerRadius = 10;
				flagView.ClipsToBounds = true;
				flagView.Alpha = .95f;

				var boardVersionLabel = new UILabel();
				boardVersionLabel.Frame = new CGRect(10, flagView.Frame.Bottom + 20, AppDelegate.ScreenWidth - 20, 30);
				var ver = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"];
				boardVersionLabel.Text = "BOARD " + ver.ToString();
				boardVersionLabel.Font = AppDelegate.Narwhal30;
				boardVersionLabel.TextColor = AppDelegate.BoardOrange;
				boardVersionLabel.TextAlignment = UITextAlignment.Center;

				var aboutLabel = new UILabel();
				aboutLabel.Frame = new CGRect(15, boardVersionLabel.Frame.Bottom + 40, AppDelegate.ScreenWidth - 20, 14);
				aboutLabel.Text = "ABOUT";
				aboutLabel.Font = AppDelegate.Narwhal14;
				aboutLabel.TextColor = AppDelegate.BoardOrange;

				var creditsButton = new UIOneLineMenuButton((float)aboutLabel.Frame.Bottom + 5);
				creditsButton.SetLabel("Credits >");
				creditsButton.SetTapEvent (delegate {
					var creditsScreen = new CreditsScreen();
					AppDelegate.NavigationController.PushViewController(creditsScreen, true);
				});
				creditsButton.SuscribeToEvent();


				var legalLabel = new UILabel();
				legalLabel.Frame = new CGRect(15, creditsButton.Frame.Bottom + 50, AppDelegate.ScreenWidth - 20, 14);
				legalLabel.Text = "LEGAL";
				legalLabel.Font = AppDelegate.Narwhal14;
				legalLabel.TextColor = AppDelegate.BoardOrange;

				var privacyButton = new UIOneLineMenuButton((float)legalLabel.Frame.Bottom + 5);
				privacyButton.SetLabel("Privacy Policy >");
				privacyButton.SetTapEvent (delegate {
					AppsController.OpenWebsite("http://getonboard.us/legal/privacy.pdf");
				});
				privacyButton.SuscribeToEvent();

				var termsButton = new UIOneLineMenuButton((float)privacyButton.Frame.Bottom + 1);
				termsButton.SetLabel("Terms of Service >");
				termsButton.SetTapEvent (delegate {
					AppsController.OpenWebsite("http://getonboard.us/legal/terms.pdf");
				});
				termsButton.SuscribeToEvent();

				var licensesButton = new UIOneLineMenuButton((float)termsButton.Frame.Bottom + 1);
				licensesButton.SetLabel("Licenses >");
				licensesButton.SetTapEvent (delegate {
					var licensesScreen = new LicensesScreen();
					AppDelegate.NavigationController.PushViewController(licensesScreen, true);
				});
				licensesButton.SuscribeToEvent();

				LoadFBButton ((float)licensesButton.Frame.Bottom + 50);

				AddSubviews(flagView, boardVersionLabel, aboutLabel, creditsButton, legalLabel, privacyButton, termsButton, licensesButton, LogOutButton);

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

		private void LoadBanner()
		{
			Banner = new UIMenuBanner ("SETTINGS", "menu_left");

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					AppDelegate.containerScreen.BringSideMenuUp("settings");
				}
			});

			Banner.AddTap (tap);
		}
	}
}

