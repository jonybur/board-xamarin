using System;
using UIKit;
using CoreGraphics;
using MessageUI;
using Haneke;

namespace Board.Screens
{
	public class UIContactView : UIView
	{
		UIImageView LogoView;
		UILabel TitleLabel;
		UITextView DescriptionView;
		UIButton ActionButton;


		UIWindow window;
		private void HideWindow()
		{
			window.Hidden = true;
			window.Dispose();
		}

		public enum ScreenContact { InviteScreen, SupportScreen, BusinessScreen };

		public UIContactView (ScreenContact screenContact)
		{
			string logoURL = string.Empty, titleString = string.Empty, descriptionString = string.Empty, actionString = string.Empty;

			switch (screenContact) {
			case ScreenContact.SupportScreen:
				logoURL = "./screens/support/icon.png";
				titleString = "CONTACT BOARD";
				descriptionString = "Want to contact our staff?\nJust send us an email and\na representative will assist you\nas soon as possible";
				actionString = "EMAIL STAFF";
				break;
			case ScreenContact.InviteScreen:
				logoURL = "./screens/invite/icon.png";
				titleString = "SHARE BOARD";
				descriptionString = "Invite your friends\nand tell them to get on Board!";
				actionString = "EMAIL INVITE";
				break;
			case ScreenContact.BusinessScreen:
				logoURL = "./screens/business/icon.png";
				titleString = "CREATE A BOARD";
				descriptionString = "Want to create a Board for your business?\nJust send us an email and\na representative will assist you\nas soon as possible";
				actionString = "EMAIL STAFF";
				break;
			}

		
			LogoView = new UIImageView ();
			LogoView.Frame = new CGRect (0, 0, 100, 100);
			LogoView.ContentMode = UIViewContentMode.ScaleAspectFit;
			LogoView.SetImage (logoURL);

			TitleLabel = new UILabel ();
			TitleLabel.Text = titleString;
			TitleLabel.AdjustsFontSizeToFitWidth = true;
			TitleLabel.TextColor = AppDelegate.BoardOrange;
			TitleLabel.TextAlignment = UITextAlignment.Center;
			TitleLabel.Font = AppDelegate.Narwhal24;
			var size = TitleLabel.Text.StringSize (TitleLabel.Font);
			TitleLabel.Frame = new CGRect (0, LogoView.Frame.Bottom + 30, AppDelegate.ScreenWidth, size.Height);

			DescriptionView = new UITextView ();
			DescriptionView.Text = descriptionString;
			DescriptionView.Font = UIFont.SystemFontOfSize(18, UIFontWeight.Light);
			DescriptionView.Selectable = false;
			DescriptionView.ScrollEnabled = false;
			DescriptionView.PagingEnabled = false;
			DescriptionView.Editable = false;
			DescriptionView.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
			DescriptionView.TextColor = UIColor.Black;
			DescriptionView.TextAlignment = UITextAlignment.Center;
			DescriptionView.Frame = new CGRect (0, TitleLabel.Frame.Bottom + 10, AppDelegate.ScreenWidth, 10);
			size = DescriptionView.SizeThatFits (DescriptionView.Frame.Size);
			DescriptionView.Frame = new CGRect(DescriptionView.Frame.X, DescriptionView.Frame.Y, DescriptionView.Frame.Width, size.Height);

			ActionButton = new UIButton ();
			ActionButton.Frame = new CGRect (0, DescriptionView.Frame.Bottom + 30, AppDelegate.ScreenWidth / 2, 50);
			ActionButton.BackgroundColor = AppDelegate.BoardBlue;
			ActionButton.SetTitle (actionString, UIControlState.Normal);

			ActionButton.TouchUpInside += (sender, e) => {

				if (MFMailComposeViewController.CanSendMail) {
					MFMailComposeViewController mailController = new MFMailComposeViewController ();

					if (screenContact == ScreenContact.SupportScreen || screenContact == ScreenContact.BusinessScreen){
						mailController.SetToRecipients(new [] {"support@getonboard.us"} );
					} else {
						mailController.SetSubject ("Get on Board!");
					}

					mailController.SetMessageBody ("", false);
					mailController.Finished += (s, args) => args.Controller.DismissViewController (true, HideWindow);
					window = new UIWindow(new CGRect(0,0,AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
					window.RootViewController = new UIViewController();
					window.MakeKeyAndVisible();
					window.RootViewController.PresentViewController(mailController, true, null);
				}

			};

			AddSubviews (LogoView, TitleLabel, DescriptionView, ActionButton);

			Frame = new CGRect (0, 0, DescriptionView.Frame.Width, ActionButton.Frame.Bottom);

			LogoView.Center = new CGPoint (Frame.Width / 2, LogoView.Center.Y);
			ActionButton.Center = new CGPoint (Frame.Width / 2, ActionButton.Center.Y);

			Center = new CGPoint (AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight / 2);

		}

	}
}
