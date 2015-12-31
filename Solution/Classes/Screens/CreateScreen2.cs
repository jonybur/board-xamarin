using System;
using System.Drawing;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using AdvancedColorPicker;
using CoreText;

using System.Net.Http;

using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Facebook.CoreKit;
using Facebook.LoginKit;

namespace Solution
{
	public class CreateScreen2 : UIViewController
	{
		UIImageView banner;

		public CreateScreen2 () : base ("Board", null){

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
			UIImage contentImage = UIImage.FromFile ("./createscreens/screen2/content.jpg");
			UIImageView contentImageView = new UIImageView (new CGRect(0, banner.Frame.Bottom, contentImage.Size.Width / 2, contentImage.Size.Height / 2));
			contentImageView.Image = contentImage;

			View.AddSubviews (contentImageView);

			UIImageView color1 = CreateColorSquare (new CGSize (316, 40), 
				                     new CGPoint (AppDelegate.ScreenWidth / 2, 350),
									UIColor.Blue.CGColor);

			View.AddSubviews (color1);
		}

		private UIImageView CreateColorSquare(CGSize size, CGPoint center, CGColor startcolor)
		{
			CGRect frame = new CGRect (0, 0, size.Width, size.Height);

			CGRect frame2 = frame;

			UIGraphics.BeginImageContext (new CGSize(frame.Size.Width, frame.Size.Height));
			CGContext context = UIGraphics.GetCurrentContext ();

			context.SetFillColor(startcolor);
			context.FillRect(frame);

			UIImage orange = UIGraphics.GetImageFromCurrentImageContext ();
			UIImageView uiv = new UIImageView (orange);
			uiv.Center = center;

			UITapGestureRecognizer tap = new UITapGestureRecognizer (async (tg) => {

				UIColor color = await ColorPickerViewController.PresentAsync (
					NavigationController, 
					"Pick a Color",
					View.BackgroundColor);

				UIGraphics.BeginImageContextWithOptions (new CGSize(BoardInterface.ScreenWidth, BoardInterface.ScreenHeight), false, 0);
				CGContext ctx = UIGraphics.GetCurrentContext ();

				ctx.SetFillColor(color.CGColor);
				ctx.FillRect(frame2);

				UIImage img = UIGraphics.GetImageFromCurrentImageContext ();
				uiv.Image = img;


			});

			uiv.AddGestureRecognizer (tap);
			uiv.UserInteractionEnabled = true;

			return uiv;
		}


		private void LoadBanner()
		{
			UIImage bannerImage = UIImage.FromFile ("./createscreens/screen2/banner.jpg");

			banner = new UIImageView(new CGRect(0,0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
			banner.Image = bannerImage;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 3){
					NavigationController.PopViewController(false);
				} else if (tg.LocationInView(this.View).X > (AppDelegate.ScreenWidth / 3) * 2){
					CreateScreen3 createScreen3 = new CreateScreen3();
					NavigationController.PushViewController(createScreen3, false);
				}
			});

			banner.UserInteractionEnabled = true;
			banner.AddGestureRecognizer (tap);
			banner.Alpha = .95f;
			View.AddSubview (banner);
		}
	}
}