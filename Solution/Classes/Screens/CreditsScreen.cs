using System;
using UIKit;
using Board.Screens.Controls;
using Board.Utilities;
using CoreGraphics;
using Foundation;

namespace Board.Screens
{
	public class CreditsScreen : UIViewController
	{
		UIMenuBanner Banner;


		public override void ViewDidLoad ()
		{
			View.BackgroundColor = UIColor.White;
			LoadBanner ();

			var thankyouLabel = new UILabel();
			thankyouLabel.Frame = new CGRect(10, Banner.Frame.Bottom + 40, AppDelegate.ScreenWidth - 20, 30);
			thankyouLabel.Text = "";
			thankyouLabel.Font = AppDelegate.Narwhal30;
			thankyouLabel.TextColor = AppDelegate.BoardOrange;
			thankyouLabel.TextAlignment = UITextAlignment.Center;
			thankyouLabel.AdjustsFontSizeToFitWidth = true;

			var creditsTextView = new UITextView ();
			creditsTextView.Frame = new CGRect (10, (float)thankyouLabel.Frame.Bottom + 20, (float)thankyouLabel.Frame.Width, 0);
			creditsTextView.Font = UIFont.SystemFontOfSize (16, UIFontWeight.Light);
			creditsTextView.Text = "JONATHAN BURSZTYN - Frontend, Design\n"+
				"HERNAN FUENTES ARAUJO - Backend\n\n" +
				"Board\nCopyright 2016 Board Social. All rights reserved.";
			creditsTextView.TextAlignment = UITextAlignment.Center;
			var size = creditsTextView.SizeThatFits (creditsTextView.Frame.Size);
			creditsTextView.Frame = new CGRect (creditsTextView.Frame.X, creditsTextView.Frame.Y, creditsTextView.Frame.Width, size.Height);

			var argentinaLabel = new UILabel ();
			argentinaLabel.Frame = new CGRect(0, AppDelegate.ScreenHeight - 24, AppDelegate.ScreenWidth, 24);
			argentinaLabel.Font = AppDelegate.Narwhal18;
			argentinaLabel.Text = "FROM 🇦🇷 WITH ❤";
			argentinaLabel.TextAlignment = UITextAlignment.Center;
			argentinaLabel.TextColor = AppDelegate.BoardOrange;

			View.AddSubviews (thankyouLabel, creditsTextView, argentinaLabel, Banner);
		}

		public override void ViewDidDisappear (bool animated)
		{
			MemoryUtility.ReleaseUIViewWithChildren (View);
		}

		private void LoadBanner()
		{
			Banner = new UIMenuBanner ("CREDITS", "arrow_left");

			bool taps = false;

			var tap = new UITapGestureRecognizer (tg => {
				if (taps){
					return;
				}

				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					taps = true;

					var containerScreen = AppDelegate.NavigationController.ViewControllers[AppDelegate.NavigationController.ViewControllers.Length - 2] as ContainerScreen;
					if (containerScreen!= null)
					{
						containerScreen.LoadSettingsScreen();
					}
					AppDelegate.PopViewControllerWithCallback (delegate{
						MemoryUtility.ReleaseUIViewWithChildren (View);
					});
				}
			});

			Banner.AddTap (tap);
			Banner.SuscribeToEvents ();
		}

	}
}

