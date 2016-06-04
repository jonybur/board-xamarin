using CoreGraphics;
using Haneke;
using UIKit;

namespace Board.Screens
{
	public class UINoContent : UIView
	{
		UITextView DescriptionView;
		UIImageView ImageView;

		public enum Presets { NotInArea };

		public UINoContent (Presets preset)
		{
			string imageURL = string.Empty, descriptionText = string.Empty;

			if (preset == Presets.NotInArea) {
				imageURL = "./screens/nocontent/noapp.png";
				descriptionText = "Board is not yet available in your area.\nPlease come back later!";
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

			AddSubviews (ImageView, DescriptionView);

			Frame = new CGRect (0, 0, DescriptionView.Frame.Width, DescriptionView.Frame.Bottom);

			ImageView.Center = new CGPoint (Frame.Width / 2, ImageView.Center.Y);

			Center = new CGPoint (AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight / 2);
		}
	}
}

