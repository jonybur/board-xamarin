using CoreGraphics;
using CoreAnimation;
using Haneke;
using UIKit;
using Foundation;

namespace Clubby.Screens
{
	public class UINoContent : UIView
	{
		UITextView DescriptionView;
		UIImageView ImageView;
		bool LoadingNantucket;

		public enum Presets { NotInArea, LocationDisabled };

		public UINoContent (Presets preset)
		{
			string imageURL = string.Empty, descriptionText = string.Empty;

			imageURL = "./screens/nocontent/noapp.png";
			if (preset == Presets.NotInArea) {
				descriptionText = "Clubby is not yet available in your area.\nPlease come back later!";
			} else if (preset == Presets.LocationDisabled) {
				descriptionText = "Location services are disabled.\nPlease enable them to enjoy Clubby.";
			}

			ImageView = new UIImageView ();
			ImageView.Frame = new CGRect (0,0, AppDelegate.ScreenWidth / 2, AppDelegate.ScreenWidth / 2);
			ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			ImageView.SetImage (imageURL);
			ImageView.Alpha = .75f;

			DescriptionView = new UITextView ();
			DescriptionView.Frame = new CGRect (0, ImageView.Frame.Bottom + 10, AppDelegate.ScreenWidth * .9f, 10);
			DescriptionView.Font = UIFont.SystemFontOfSize (16, UIFontWeight.Light);
			DescriptionView.Text = descriptionText;
			DescriptionView.ScrollEnabled = false;
			DescriptionView.Editable = false;
			DescriptionView.Selectable = false;
			DescriptionView.TextAlignment = UITextAlignment.Center;
			DescriptionView.TextColor = UIColor.Black;
			DescriptionView.Alpha = .75f;
			var size = DescriptionView.SizeThatFits (DescriptionView.Frame.Size);
			DescriptionView.Frame = new CGRect (DescriptionView.Frame.X, DescriptionView.Frame.Y, DescriptionView.Frame.Width, size.Height);

			var nantucketButton = new UIButton ();
			nantucketButton.Frame = new CGRect (0, DescriptionView.Frame.Bottom + 30, AppDelegate.ScreenWidth * 0.75f, 50);
			nantucketButton.BackgroundColor = AppDelegate.BoardBlue;
			nantucketButton.Center = new CGPoint (DescriptionView.Frame.Width / 2, nantucketButton.Center.Y);
			nantucketButton.SetTitle ("Discover Nantucket", UIControlState.Normal);

			nantucketButton.ClipsToBounds = true;
			nantucketButton.Layer.MasksToBounds = true;
			nantucketButton.Layer.CornerRadius = 10;

			nantucketButton.TouchUpInside += (sender, e) => {
				CATransaction.Begin ();

				ShowFirstTimeUseMessage();
				Alpha = 0f;

				CATransaction.Commit();

				//CATransaction.CompletionBlock = LoadNantucket;
			};

			AddSubviews (ImageView, DescriptionView, nantucketButton);

			Frame = new CGRect (0, 0, DescriptionView.Frame.Width, nantucketButton.Frame.Bottom);

			ImageView.Center = new CGPoint (Frame.Width / 2, ImageView.Center.Y);

			Center = new CGPoint (AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight / 2);
		}

		private void ShowFirstTimeUseMessage(){
			var defaults = NSUserDefaults.StandardUserDefaults;
			const string key = "FirstTimeNantucketUse";
			if (!defaults.BoolForKey (key)) {
				// First launch
				NSUserDefaults.StandardUserDefaults.SetBool (true, key);
				defaults.Synchronize ();
				BigTed.BTProgressHUD.Show ("Setting up Board\nfor first time use...");
			} else { 
				BigTed.BTProgressHUD.Show("Loading Nantucket...");
			}
		}
	}
}

