using System;
using CoreGraphics;
using UIKit;
using Clubby.Utilities;
using Clubby.Screens.Controls;

namespace Clubby.Screens
{
	public class LicenseScreen : UIViewController
	{
		UIMenuBanner Banner;
		License CurrentLicense;
		UITextView DescriptionView;

		public LicenseScreen(License currentLicense){
			CurrentLicense = currentLicense;
		}

		public override void ViewDidLoad ()
		{
			View.BackgroundColor = UIColor.White;
			LoadBanner (CurrentLicense.ProductName);

			DescriptionView = new UITextView ();
			DescriptionView.Frame = new CGRect (5, Banner.Frame.Bottom + 5, AppDelegate.ScreenWidth - 10, AppDelegate.ScreenHeight - 5 - UIMenuBanner.Height);
			DescriptionView.Font = UIFont.SystemFontOfSize (14, UIFontWeight.Light);
			DescriptionView.Editable = false;
			DescriptionView.DataDetectorTypes = UIDataDetectorType.Link;
			DescriptionView.Text = CurrentLicense.Description;

			View.AddSubviews (Banner, DescriptionView);
		}

		public override void ViewDidDisappear(bool animated)
		{
			Banner.UnsuscribeToEvents ();
			MemoryUtility.ReleaseUIViewWithChildren (View);
		}

		private void LoadBanner(string name)
		{
			Banner = new UIMenuBanner (name.ToUpper(), "arrow_left");

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					AppDelegate.NavigationController.PopViewController(true);
				}
			});

			Banner.AddTap (tap);
			Banner.SuscribeToEvents ();
		}
	}
}

