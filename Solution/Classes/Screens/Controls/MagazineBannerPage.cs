using CoreGraphics;
using UIKit;

namespace Board.Screens.Controls
{
	public sealed class MagazineBannerPage : UIImageView
	{
		public const int Height = 175;

		public MagazineBannerPage(string subtitleText)
		{
			Frame = new CGRect (0, 0, AppDelegate.ScreenWidth, Height);
			BackgroundColor = UIColor.White;

			ClipsToBounds = true;

			var flagView = GenerateFlag (subtitleText);

			BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);

			AddSubview (flagView);
		}

		private UIImageView GenerateFlag(string subtitleText){
			var flagView = new UIImageView ();
			flagView.Frame = new CGRect (0, 0, 200, 110);

			var flagBackground = new UIImageView ();
			flagBackground.Alpha = .95f;
			flagBackground.Frame = flagView.Frame;
			flagBackground.BackgroundColor = AppDelegate.BoardOrange;
			flagBackground.Center = Center;

			var pin = new UIImageView();
			pin.Frame = flagView.Frame;
			pin.Center = flagBackground.Center;
			using (var img = UIImage.FromFile ("./screens/main/magazine/pin.png")) {
				pin.Image = img;
			}

			var rightNow = new UILabel ();
			rightNow.Frame = new CGRect (5, 0, flagView.Frame.Width-10, 20);
			rightNow.Font = AppDelegate.Narwhal14;
			rightNow.Text = "RIGHT NOW AT";
			rightNow.TextColor = UIColor.White;
			rightNow.AdjustsFontSizeToFitWidth = true;
			rightNow.Center = new CGPoint(flagBackground.Center.X, flagBackground.Center.Y+2);
			rightNow.TextAlignment = UITextAlignment.Center;

			var placeName = new UILabel ();
			placeName.Frame = new CGRect (5, 0, flagView.Frame.Width-10, 20);
			placeName.Font = AppDelegate.Narwhal18;
			placeName.Text = "WEST PALM BEACH";
			placeName.TextColor = UIColor.White;
			placeName.AdjustsFontSizeToFitWidth = true;
			placeName.Center = new CGPoint(flagBackground.Center.X, flagBackground.Center.Y + 20);
			placeName.TextAlignment = UITextAlignment.Center;

			var subtitle = new UILabel ();
			subtitle.Frame = new CGRect (5, 0, flagView.Frame.Width-10, 20);
			subtitle.Font = AppDelegate.Narwhal14;
			subtitle.Text = subtitleText;
			subtitle.TextColor = UIColor.White;
			subtitle.AdjustsFontSizeToFitWidth = true;
			subtitle.Center = new CGPoint(flagBackground.Center.X, placeName.Center.Y + 25);
			subtitle.TextAlignment = UITextAlignment.Center;

			flagView.AddSubviews (flagBackground, pin, rightNow, placeName, subtitle);

			return flagView;
		}
	}
}

