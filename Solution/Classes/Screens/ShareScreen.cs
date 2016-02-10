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

namespace Solution
{
	public class ShareScreen : UIViewController
	{
		UIImageView banner;
		UIImageView nextbutton;
		UIScrollView content;

		public ShareScreen (){

		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;

			InitializeInterface ();
		}

		private void InitializeInterface()
		{
			LoadContent ();
			LoadBanner ();
			LoadNextButton ();
		}

		private void LoadContent()
		{
			content = new UIScrollView(new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			content.BackgroundColor = UIColor.White;
			content.ContentSize = new CGSize (AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);
			View.AddSubview (content);
		}

		private void LoadBanner()
		{
			UIImage bannerImage = UIImage.FromFile ("./screens/share/banner/" + AppDelegate.PhoneVersion + ".jpg");

			banner = new UIImageView(new CGRect(0, 0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
			banner.Image = bannerImage;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					NavigationController.PopViewController(true);
				}
			});

			banner.UserInteractionEnabled = true;
			banner.AddGestureRecognizer (tap);
			banner.Alpha = .95f;
			View.AddSubview (banner);
		}


		private void LoadNextButton()
		{
			UIImage mapImage = UIImage.FromFile ("./screens/share/next/" + AppDelegate.PhoneVersion + ".jpg");;

			nextbutton = new UIImageView(new CGRect(0,AppDelegate.ScreenHeight - (mapImage.Size.Height / 2),
				mapImage.Size.Width / 2, mapImage.Size.Height / 2));
			nextbutton.Image = mapImage;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				NavigationController.PopViewController(false);

				// shows the image preview so that the user can position the image
				BoardInterface.scrollView.AddSubview(Preview.View);

				// switches to confbar
				ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.ConfirmationBar);
			});

			nextbutton.UserInteractionEnabled = true;
			nextbutton.AddGestureRecognizer (tap);
			nextbutton.Alpha = .95f;
			View.AddSubview (nextbutton);
		}


	}
}