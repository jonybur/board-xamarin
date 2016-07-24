using System;
using Haneke;
using Foundation;
using Clubby.Screens;
using Facebook.CoreKit;
using Clubby.Screens.Controls;
using Clubby.Infrastructure;
using Plugin.Share;
using Newtonsoft.Json.Linq;
using Facebook.LoginKit;
using CoreGraphics;
using MessageUI;
using UIKit;

namespace Clubby.Screens
{
	public class UserScreen : UIViewController
	{
		protected UIImageView BackButton;
		UITapGestureRecognizer backTap;

		public UserScreen () {}

		public override void ViewDidLoad ()
		{
			View.BackgroundColor = AppDelegate.ClubbyBlack;

			var settingsView = new SettingsView ();
			settingsView.LoadFacebookContent ();

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

		UIWindow window;
		UIImageView profileView;
		UILabel nameLabel;

		public void LoadFacebookContent(){
			LoadName ();
			LoadPicture ();
		}

		private async void LoadPicture(){
			string json = await CloudController.AsyncGraphAPIRequest ("me", "?fields=picture.type(large)");

			if (json == "400" || json == "404") {
				Console.WriteLine("failed");
				return;
			}

			var jobject = JObject.Parse (json);
			string picture = jobject ["picture"]["data"]["url"].ToString();

			profileView.SetImage(new NSUrl(picture));
			profileView.Layer.CornerRadius = profileView.Frame.Width / 2;

			AddSubview (profileView);
		}

		private async void LoadName(){
			string json = await CloudController.AsyncGraphAPIRequest ("me", "?fields=name");

			var jobject = JObject.Parse (json);

			string name = jobject ["name"].ToString();
			nameLabel.Text = name;

			AddSubview (nameLabel);
		}

		public SettingsView(){
			Frame = new CGRect(0,0,AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);

			profileView = new UIImageView ();
			profileView.Frame = new CGRect(0, 0, 160, 160);
			profileView.Center = new CGPoint(AppDelegate.ScreenWidth / 2, profileView.Frame.Height / 2 + 40);
			profileView.ContentMode = UIViewContentMode.ScaleAspectFit;
			profileView.ClipsToBounds = true;

			nameLabel = new UILabel();
			nameLabel.Frame = new CGRect(10, profileView.Frame.Bottom + 20, AppDelegate.ScreenWidth - 20, 32);
			nameLabel.Font = UIFont.SystemFontOfSize(30, UIFontWeight.Light);
			nameLabel.TextColor = UIColor.White;
			nameLabel.AdjustsFontSizeToFitWidth = true;
			nameLabel.TextAlignment = UITextAlignment.Center;

			var legalLabel = new UILabel();
			legalLabel.Frame = new CGRect(15, nameLabel.Frame.Bottom + 50, AppDelegate.ScreenWidth - 20, 14);
			legalLabel.Text = "LEGAL";
			legalLabel.Font = UIFont.SystemFontOfSize(14, UIFontWeight.Medium);
			legalLabel.TextColor = UIColor.White;

			var privacyButton = new UIOneLineMenuButton((float)legalLabel.Frame.Bottom + 5);
			privacyButton.SetLabel("Privacy Policy");
			privacyButton.SetTapEvent (delegate {
				AppsController.OpenWebsite("http://getonboard.us/legal/clubbyprivacy.pdf");
			});
			privacyButton.SuscribeToEvent();

			var termsButton = new UIOneLineMenuButton((float)privacyButton.Frame.Bottom + 1);
			termsButton.SetLabel("Terms of Service");
			termsButton.SetTapEvent (delegate {
				AppsController.OpenWebsite("http://getonboard.us/legal/clubbyterms.pdf");
			});
			termsButton.SuscribeToEvent();

			var licensesButton = new UIOneLineMenuButton((float)termsButton.Frame.Bottom + 1);
			licensesButton.SetLabel("Licenses");
			licensesButton.SetTapEvent (delegate {
				var licensesScreen = new LicensesScreen();
				AppDelegate.NavigationController.PushViewController(licensesScreen, true);
			});
			licensesButton.SuscribeToEvent();

			var shareButton = new UIOneLineMenuButton((float)licensesButton.Frame.Bottom + 35, true);
			shareButton.SetLabel("Share Clubby");
			shareButton.SetTapEvent (async delegate {
				await ShareImplementation.Init ();
				var shareImplementation = new ShareImplementation ();

				// TODO: add clubby app link
				await shareImplementation.Share("Check out Clubby... it shows you where the party is at!\nDownload now: ", UIActivityType.Mail);
			});
			shareButton.SuscribeToEvent();

			var flagView = new UIImageView();
			flagView.Frame = new CGRect(0, shareButton.Frame.Bottom + 20, 100, 100);
			flagView.Center = new CGPoint(AppDelegate.ScreenWidth / 2, flagView.Center.Y);
			flagView.ContentMode = UIViewContentMode.ScaleAspectFit;
			flagView.SetImage("./screens/settings/300.png");

			var boardVersionLabel = new UILabel();
			boardVersionLabel.Frame = new CGRect(10, flagView.Frame.Bottom, AppDelegate.ScreenWidth - 20, 32);
			var ver = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"];
			boardVersionLabel.Text = "Version " + ver.ToString();
			boardVersionLabel.Font = UIFont.SystemFontOfSize(20, UIFontWeight.Light);
			boardVersionLabel.TextColor = AppDelegate.ClubbyBlue;
			boardVersionLabel.TextAlignment = UITextAlignment.Center;

			var contactLabel = new UILabel();
			contactLabel.Frame = new CGRect(15, boardVersionLabel.Frame.Bottom + 30, AppDelegate.ScreenWidth - 20, 14);
			contactLabel.Text = "CONTACT US";
			contactLabel.Font = UIFont.SystemFontOfSize(14, UIFontWeight.Medium);
			contactLabel.TextColor = UIColor.White;

			var helpButton = new UIOneLineMenuButton((float)contactLabel.Frame.Bottom + 5, true);
			helpButton.SetLabel("Help & Support");
			helpButton.SetTapEvent (delegate {
				if (MFMailComposeViewController.CanSendMail) {
					MFMailComposeViewController mailController = new MFMailComposeViewController ();

					mailController.SetToRecipients(new [] {"support@getonboard.us"} );
					mailController.SetMessageBody ("", false);
					mailController.Finished += (s, args) => args.Controller.DismissViewController (true, HideWindow);
					window = new UIWindow(new CGRect(0,0,AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
					window.RootViewController = new UIViewController();
					window.MakeKeyAndVisible();
					window.RootViewController.PresentViewController(mailController, true, null);
				}
			});
			helpButton.SuscribeToEvent();

			LoadFBButton ((float)helpButton.Frame.Bottom + 40);

			AddSubviews(flagView, boardVersionLabel, legalLabel, privacyButton, termsButton, licensesButton, shareButton, LogOutButton, contactLabel, helpButton);

			ContentSize = new CGSize(AppDelegate.ScreenWidth, LogOutButton.Frame.Bottom + UIActionButton.Height * 2);
		}

		private void HideWindow()
		{
			window.Hidden = true;
			window.Dispose();
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

