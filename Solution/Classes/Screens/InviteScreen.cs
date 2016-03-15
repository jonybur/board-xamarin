using System;
using Board.Utilities;
using CoreGraphics;
using MessageUI;
using Board.Screens.Controls;
using UIKit;

namespace Board.Screens
{
	public class InviteScreen : UIViewController
	{
		MenuBanner Banner;
		UIWindow window;
		const int hborder = 65;
		float yposition;

		public override void ViewDidLoad ()
		{
			yposition = hborder + 120;

			LoadBanner ();

			LoadContent ();

			LoadMailButton ();

			View.BackgroundColor = UIColor.White;
		}

		public override void ViewDidAppear(bool animated)
		{
			Banner.SuscribeToEvents ();
		}

		public override void ViewDidDisappear(bool animated)
		{
			Banner.UnsuscribeToEvents ();
			MemoryUtility.ReleaseUIViewWithChildren (View, true);
		}

		private void LoadContent()
		{
			UIImageView uivmailicon;
			using (UIImage mailicon = UIImage.FromFile ("./screens/invite/icon.png")) {
				uivmailicon = new UIImageView (mailicon);
				uivmailicon.Frame = new CGRect (0, 0, mailicon.Size.Width / 2, mailicon.Size.Height / 2);
			}

			uivmailicon.Center = new CGPoint (AppDelegate.ScreenWidth / 2, yposition);

			yposition += (float)(uivmailicon.Frame.Height / 2) + 40;

			UIFont bold = AppDelegate.Narwhal24;
			UIFont regular = UIFont.SystemFontOfSize(20);

			string earningsString = "SHARE BOARD";
			string withdrawString = "Earn commissions and win prizes\nfor referring friends and businesses\nto get on Board!";

			UILabel lbl1 = new UILabel (new CGRect (0, (nfloat)yposition, (nfloat)AppDelegate.ScreenWidth, earningsString.StringSize(bold).Height));
			lbl1.Text = earningsString;
			lbl1.Enabled = false;
			lbl1.AdjustsFontSizeToFitWidth = true;
			lbl1.TextColor = AppDelegate.BoardOrange;
			lbl1.TextAlignment = UITextAlignment.Center;
			yposition += (float)lbl1.Frame.Height + 5;
			lbl1.Font = bold;

			UITextView uit = new UITextView (new CGRect (0,yposition,AppDelegate.ScreenWidth, 420));
			uit.Text = withdrawString;
			uit.Font = regular;
			uit.Selectable = false;
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
			uib.BackgroundColor = AppDelegate.BoardBlue;
			uib.Center = new CGPoint (AppDelegate.ScreenWidth / 2, yposition+ 200);

			UILabel lbl = new UILabel ();
			lbl.Font = AppDelegate.SystemFontOfSize20;

			string text = "EMAIL INVITE";
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
					mailController.SetSubject ("Get on Board!");
					mailController.SetMessageBody ("", false);
					mailController.Finished += (s, args) => {
						args.Controller.DismissViewController (true, HideWindow);
					};
					window = new UIWindow(new CGRect(0,0,AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
					window.RootViewController = new UIViewController();
					window.MakeKeyAndVisible();
					window.RootViewController.PresentViewController(mailController, true, null);				}
			};

			uib.AddSubview (lbl);
			View.AddSubview (uib);
		}

		private void HideWindow()
		{
			window.Hidden = true;
			window.Dispose();
		}

		private void LoadBanner()
		{
			Banner = new MenuBanner ("./screens/invite/banner/" + AppDelegate.PhoneVersion + ".jpg");

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					AppDelegate.containerScreen.BringSideMenuUp("invite");
				}
			});

			Banner.AddTap (tap);
			View.AddSubview (Banner);
		}
	}
}

