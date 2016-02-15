using System;
using System.Drawing;
using System.Linq;

using CoreGraphics;
using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;

using System.Net.Http;

using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Facebook.CoreKit;
using Facebook.LoginKit;

using Google.Maps;

namespace Board.Screens
{
	public class SettingsScreen : UIViewController
	{
		UIImageView banner;
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
			UIImage bannerImage = UIImage.FromFile ("./screens/settings/banner/" + AppDelegate.PhoneVersion + ".jpg");

			banner = new UIImageView(new CGRect(0,0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
			banner.Image = bannerImage;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					NavigationController.PopViewController(false);
				}
			});

			banner.UserInteractionEnabled = true;
			banner.AddGestureRecognizer (tap);
			banner.Alpha = .95f;
			View.AddSubview (banner);
		}
	}
}

