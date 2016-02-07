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
	public class BusinessScreen : UIViewController
	{
		UIImageView banner;
		UIImageView sidemenu;
		ProfilePictureView profileView;
		UIScrollView content;
		float thumbSize;
		bool sideMenuIsUp;

		public BusinessScreen () : base ("Board", null){

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

		private async void InitializeInterface()
		{
			LoadContent ();
			LoadBanner ();
			LoadSideMenu ();
		}

		private void LoadContent()
		{ 
			// si el usuario no tiene boards creados...
			thumbSize = AppDelegate.ScreenWidth / 4;
			content = new UIScrollView(new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));

			UIImageView imgv = new UIImageView (new CGRect(0,0,AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			imgv.Image = UIImage.FromFile ("./screens/business/empty/" + AppDelegate.PhoneVersion + ".jpg");;

			content.AddSubview (imgv);
			content.ScrollEnabled = true;
			content.UserInteractionEnabled = true;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (sideMenuIsUp)
				{sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; return;}
			});

			content.AddGestureRecognizer (tap);

			View.AddSubview (content);
		}

		private void LoadSideMenu()
		{
			UIImage bannerImage = UIImage.FromFile ("./screens/business/sidemenu/" + AppDelegate.PhoneVersion + ".png");

			sidemenu = new UIImageView(new CGRect(0,0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
			sidemenu.Image = bannerImage;

			float[] buttonLocations = new float[4];
			if (AppDelegate.PhoneVersion == "6") {
				buttonLocations [0] = 350;
				buttonLocations [1] = 440;
				buttonLocations [2] = 525;
				buttonLocations [3] = 605;
			} else {
				buttonLocations [0] = 390;
				buttonLocations [1] = 470;
				buttonLocations [2] = 550;
				buttonLocations [3] = 630;
			}

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (tg.LocationInView(this.View).Y > buttonLocations[0]-35 && tg.LocationInView(this.View).Y < buttonLocations[0]+35 ){
					NavigationController.PopViewController(false);
				}
				else if (tg.LocationInView(this.View).Y > buttonLocations[1]-35 && tg.LocationInView(this.View).Y < buttonLocations[1]+35){
					SettingsScreen screen = new SettingsScreen();
					NavigationController.PushViewController(screen, false);
				}
				else if (tg.LocationInView(this.View).Y > buttonLocations[2]-35 && tg.LocationInView(this.View).Y < buttonLocations[2]+35){
					SupportScreen screen = new SupportScreen();
					NavigationController.PushViewController(screen, false);
				}
				else if (tg.LocationInView(this.View).Y > buttonLocations[3]-35 && tg.LocationInView(this.View).Y < buttonLocations[3]+35){
					InviteScreen screen = new InviteScreen();
					NavigationController.PushViewController(screen, false);
				}
				HideSideMenu();
			});

			sidemenu.UserInteractionEnabled = true;
			sidemenu.AddGestureRecognizer (tap);
			sidemenu.Alpha = 0f;

			profileView = new ProfilePictureView (new CGRect (11, 78, 149, 149));
			profileView.ProfileId = Profile.CurrentProfile.UserID;
			profileView.Alpha = 0f;

			sideMenuIsUp = false;

			View.AddSubviews (profileView, sidemenu);

			UIFont namefont = UIFont.FromName("narwhal-bold", 20);
			UIFont lastnamefont = UIFont.FromName("narwhal-bold", 24);

			UILabel name = new UILabel (new CGRect(10, profileView.Frame.Bottom + 15, sidemenu.Frame.Width - 20, 20));
			name.Font = namefont;
			name.Text = Profile.CurrentProfile.FirstName;
			name.TextColor = UIColor.White;
			name.TextAlignment = UITextAlignment.Center;
			name.AdjustsFontSizeToFitWidth = true;
			sidemenu.AddSubview (name);

			UILabel lastname = new UILabel (new CGRect(10, name.Frame.Bottom + 3, sidemenu.Frame.Width - 20, 24));
			lastname.Font = lastnamefont;
			lastname.AdjustsFontSizeToFitWidth = true;
			lastname.Text = Profile.CurrentProfile.LastName;
			lastname.TextColor = UIColor.White;
			lastname.TextAlignment = UITextAlignment.Center;
			sidemenu.AddSubview (lastname);
		}

		public void HideSideMenu()
		{
			if (sideMenuIsUp)
			{ sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; }
		}

		private void LoadBanner()
		{
			UIImage bannerImage = UIImage.FromFile ("./screens/business/banner/" + AppDelegate.PhoneVersion + ".jpg");

			banner = new UIImageView(new CGRect(0,0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
			banner.Image = bannerImage;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (sideMenuIsUp)
				{sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; return;}

				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					sidemenu.Alpha = 1f;
					profileView.Alpha = 1f;
					sideMenuIsUp = true;
				}
				else if (AppDelegate.ScreenWidth / 4 * 3 < tg.LocationInView(this.View).X){
					CreateScreen1 createScreen1 = new CreateScreen1();
					NavigationController.PushViewController(createScreen1, false);
				}
			});

			banner.UserInteractionEnabled = true;
			banner.AddGestureRecognizer (tap);
			banner.Alpha = .95f;
			View.AddSubview (banner);
		}


	}
}