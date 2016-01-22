using System;
using System.Drawing;
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

namespace Solution
{
	public class CreateScreen1 : UIViewController
	{
		UIImageView banner;

		public CreateScreen1 () : base ("Board", null){

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
			// loads center button
			LoadBanner ();
			LoadContent ();
		}

		private void LoadContent()
		{
			UIImage contentImage = UIImage.FromFile ("./createscreens/screen1/content.jpg");
			UIImageView contentImageView = new UIImageView (new CGRect(0, banner.Frame.Bottom, contentImage.Size.Width / 2, contentImage.Size.Height / 2));
			contentImageView.Image = contentImage;
			float contentHeight = (float)(contentImage.Size.Height - (AppDelegate.ScreenHeight));
			View.AddSubview (contentImageView);
		}


		private void LoadBanner()
		{
			UIImage bannerImage = UIImage.FromFile ("./createscreens/screen1/banner.jpg");

			banner = new UIImageView(new CGRect(0,0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
			banner.Image = bannerImage;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 3){
					NavigationController.PopViewController(false);
				} else if (tg.LocationInView(this.View).X > (AppDelegate.ScreenWidth / 3) * 2){
					CreateScreen2 createScreen2 = new CreateScreen2();
					NavigationController.PushViewController(createScreen2, false);
				}
			});

			banner.UserInteractionEnabled = true;
			banner.AddGestureRecognizer (tap);
			banner.Alpha = .95f;
			View.AddSubview (banner);
		}
	}
}