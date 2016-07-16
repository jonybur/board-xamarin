using CoreGraphics;
using Haneke;
using UIKit;

namespace Clubby.Screens
{
	public class UINoContent : UIView
	{
		UITextView DescriptionView;
		UIImageView ImageView;
		bool LoadingNantucket;

		public enum Presets { LocationDisabled };

		public UINoContent (Presets preset)
		{
			string imageURL = string.Empty, descriptionText = string.Empty;

			imageURL = "./screens/nocontent/noapp.png";
			if (preset == Presets.LocationDisabled) {
				descriptionText = "Location services are disabled.\nPlease enable them to enjoy Clubby.";
			}

			ImageView = new UIImageView ();
			ImageView.Frame = new CGRect (0, 0, 140, 140);
			ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			ImageView.SetImage (imageURL);
			ImageView.Alpha = .75f;

			DescriptionView = new UITextView ();
			DescriptionView.Frame = new CGRect (0, ImageView.Frame.Bottom + 40, AppDelegate.ScreenWidth * .9f, 10);
			DescriptionView.Font = UIFont.SystemFontOfSize (16, UIFontWeight.Light);
			DescriptionView.Text = descriptionText;
			DescriptionView.ScrollEnabled = false;
			DescriptionView.Editable = false;
			DescriptionView.Selectable = false;
			DescriptionView.TextAlignment = UITextAlignment.Center;
			DescriptionView.TextColor = UIColor.White;
			DescriptionView.BackgroundColor = UIColor.Clear;
			var size = DescriptionView.SizeThatFits (DescriptionView.Frame.Size);
			DescriptionView.Frame = new CGRect (DescriptionView.Frame.X, DescriptionView.Frame.Y, DescriptionView.Frame.Width, size.Height);

			AddSubviews (ImageView, DescriptionView);

			Frame = new CGRect (0, 0, DescriptionView.Frame.Width, DescriptionView.Frame.Height);

			ImageView.Center = new CGPoint (Frame.Width / 2, ImageView.Center.Y);

			Center = new CGPoint (AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight / 2 - ImageView.Frame.Height + 30);
		}

	}
}

