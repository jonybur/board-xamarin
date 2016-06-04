using CoreGraphics;
using CoreAnimation;
using Foundation;
using Haneke;
using UIKit;

namespace Board.Screens.Controls
{
	public sealed class UIMagazineBannerPage : UIImageView
	{
		public const int Height = 175;

		public UIMagazineBannerPage(string subtitleText)
		{
			Frame = new CGRect (0, 0, AppDelegate.ScreenWidth, Height);
			BackgroundColor = UIColor.White;

			ClipsToBounds = true;

			var flagView = GenerateFlag (subtitleText);

			BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);

			AddSubview (flagView);
		}

		const int FlagHeight = 110;
		const int FlagWidth = 200;

		private UIImageView GenerateFlag(string subtitleText){
			var flagView = new UIImageView ();
			flagView.Frame = new CGRect (0, 0, FlagWidth, FlagHeight);
			flagView.Center = Center;
			flagView.Alpha = .95f;
			flagView.BackgroundColor = AppDelegate.BoardOrange;
			flagView.Layer.CornerRadius = 10;
			flagView.ClipsToBounds = true;

			var pin = new UIImageView();
			pin.Frame = flagView.Frame;
			pin.SetImage ("./screens/main/magazine/flagpin.png");
			pin.Center = new CGPoint (flagView.Frame.Width / 2, flagView.Frame.Height / 2);

			var animation =  new CABasicAnimation();
			animation.KeyPath = "position.y";
			animation.From = new NSNumber(-100);
			animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut);
			animation.To = new NSNumber(pin.Center.Y);
			animation.Duration = 1f;
			pin.Layer.AddAnimation(animation, "dropPin");

			var rightNow = new UILabel ();
			rightNow.Frame = new CGRect (0, 0, flagView.Frame.Width-10, 20);
			rightNow.Font = AppDelegate.Narwhal14;
			rightNow.Text = "RIGHT NOW AT";
			rightNow.TextColor = UIColor.White;
			rightNow.AdjustsFontSizeToFitWidth = true;
			rightNow.Center = new CGPoint(flagView.Frame.Width / 2, flagView.Frame.Height / 2 + 2);
			rightNow.TextAlignment = UITextAlignment.Center;

			var placeName = new UILabel ();
			placeName.Frame = new CGRect (5, 0, flagView.Frame.Width-10, 20);
			placeName.Font = AppDelegate.Narwhal18;
			placeName.Text = "NANTUCKET";
			placeName.TextColor = UIColor.White;
			placeName.AdjustsFontSizeToFitWidth = true;
			placeName.Center = new CGPoint(flagView.Frame.Width / 2, flagView.Frame.Height / 2 + 20);
			placeName.TextAlignment = UITextAlignment.Center;

			var separationLine = new UIImageView ();
			separationLine.Frame = new CGRect (0, 0, flagView.Frame.Width - 20, 1);
			separationLine.BackgroundColor = UIColor.White;
			separationLine.Alpha = .75f;
			separationLine.Center = new CGPoint (flagView.Frame.Width / 2, placeName.Frame.Bottom + 1);

			var subtitle = new UILabel ();
			subtitle.Frame = new CGRect (5, 0, flagView.Frame.Width-10, 20);
			subtitle.Font = AppDelegate.Narwhal14;
			subtitle.Text = subtitleText;
			subtitle.TextColor = UIColor.White;
			subtitle.AdjustsFontSizeToFitWidth = true;
			subtitle.Center = new CGPoint(flagView.Frame.Width / 2, placeName.Center.Y + 25);
			subtitle.TextAlignment = UITextAlignment.Center;

			flagView.AddSubviews (pin, rightNow, placeName, separationLine, subtitle);

			return flagView;
		}
	}
}

