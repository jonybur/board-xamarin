using System;
using System.Drawing;

using Foundation;
using UIKit;
using CoreGraphics;

using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using MessageUI;

namespace Solution
{
	public class SupportScreen : UIViewController
	{
		UIImageView banner;
		const int hborder = 65;
		float yposition;

		public SupportScreen ()
		{
		}

		public override void ViewDidLoad ()
		{
			yposition = hborder + 120;

			LoadBanner ();

			LoadContent ();

			LoadMailButton ();

			View.BackgroundColor = UIColor.White;
		}


		private void LoadContent()
		{
			UIImage mailicon = UIImage.FromFile("./screens/support/icon.png");
			UIImageView uivmailicon = new UIImageView (mailicon);
			uivmailicon.Frame = new CGRect (0, 0, mailicon.Size.Width / 2, mailicon.Size.Height / 2);
			uivmailicon.Center = new CGPoint (AppDelegate.ScreenWidth / 2, yposition);

			yposition += (float)(uivmailicon.Frame.Height / 2) + 40;

			UIFont bold = UIFont.FromName("narwhal-bold", 24);
			UIFont regular = UIFont.SystemFontOfSize(20);

			string earningsString = "CONTACT BOARD";
			string withdrawString;

			if (AppDelegate.PhoneVersion == "6") {
				withdrawString = "Want to contact our staff?\nJust send us an email and a representative will assist you\nas soon as possible";
			} else {
				withdrawString = "Want to contact our staff?\nJust send us an email and\na representative will assist you\nas soon as possible";
			}

			UILabel lbl1 = new UILabel (new CGRect (10, (nfloat)yposition, (nfloat)AppDelegate.ScreenWidth - 20, earningsString.StringSize(bold).Height));
			lbl1.Text = earningsString;
			lbl1.AdjustsFontSizeToFitWidth = true;
			lbl1.TextColor = AppDelegate.CityboardOrange;
			lbl1.TextAlignment = UITextAlignment.Center;
			yposition += (float)lbl1.Frame.Height + 5;
			lbl1.Enabled = false;
			lbl1.Font = bold;

			UITextView uit = new UITextView (new CGRect (10, yposition, AppDelegate.ScreenWidth - 20, 420));
			uit.Text = withdrawString;
			uit.Selectable = false;
			uit.Font = regular;
			uit.ScrollEnabled = false;
			uit.PagingEnabled = false;
			uit.Editable = false;
			uit.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
			uit.TextColor = UIColor.Black;
			uit.TextAlignment = UITextAlignment.Center;

			View.AddSubviews (uivmailicon, lbl1, uit);
		}


		private void LoadMailButton()
		{
			float heightButton = 50;
			float widthButton = (AppDelegate.ScreenWidth / 2);

			UIButton uib = new UIButton ();
			CGRect uibframe = new CGRect (0, 0, widthButton, heightButton);
			uib.Frame = uibframe;
			uib.BackgroundColor = AppDelegate.CityboardBlue;
			uib.Center = new CGPoint (AppDelegate.ScreenWidth / 2, yposition+ 200);

			UILabel lbl = new UILabel ();
			lbl.Font = UIFont.SystemFontOfSize (20);

			string text = "EMAIL STAFF";
			float lblheight =  (float)text.StringSize(lbl.Font).Height;

			CGRect lblframe = new CGRect (0, 0, uibframe.Size.Width, lblheight);
			lbl.Frame = lblframe;
			lbl.Center = new CGPoint (uibframe.Width / 2, (uibframe.Height / 2));
			lbl.TextAlignment = UITextAlignment.Center;
			lbl.TextColor = UIColor.White;
			lbl.Text = text;

			uib.TouchUpInside += (sender, e) => {

				if (MFMailComposeViewController.CanSendMail) {
					// do mail operations here
					MFMailComposeViewController mailController = new MFMailComposeViewController ();
					mailController.SetToRecipients(new string[]{"support@cityboard.co"});
					mailController.SetMessageBody ("", false);
					mailController.Finished += ( object s, MFComposeResultEventArgs args) => {
						Console.WriteLine (args.Result.ToString ());
						args.Controller.DismissViewController (true, null);
					};
					NavigationController.PresentViewController (mailController, true, null);
				}

			};

			uib.AddSubview (lbl);
			View.AddSubview (uib);
		}

		private void LoadBanner()
		{
			UIImage bannerImage = UIImage.FromFile ("./screens/support/banner/"+ AppDelegate.PhoneVersion +".jpg");

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

