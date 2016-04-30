using System;
using CoreGraphics;
using MGImageUtilitiesBinding;
using UIKit;

namespace Board.Screens.Controls
{
	public class MagazineBannerPage : UIImageView
	{
		public UIImageView ParallaxBlock;
		private float centerY;
		private float offsetDelta;

		public void ParallaxMove(float yoffset)
		{
			if (offsetDelta == 0f) {
				offsetDelta = yoffset;
			}

			Console.WriteLine (centerY - (yoffset - offsetDelta) / 10);

			ParallaxBlock.Center = new CGPoint (ParallaxBlock.Center.X, centerY - (yoffset + 10 - offsetDelta)/10);
		}

		public MagazineBannerPage()
		{
			Frame = new CGRect (0, MenuBanner.MenuHeight, AppDelegate.ScreenWidth, 175);
			BackgroundColor = UIColor.White;

			ClipsToBounds = true;

			using (UIImage img = UIImage.FromFile ("./screens/main/magazine/westpalmbeach.png")) {
				float scale = (float)(img.Size.Width/img.Size.Height);
				float imgw, imgh, autosize;
				autosize = (float)Frame.Width;

				imgw = autosize * scale;
				imgh = autosize;

				UIImage scaledImage = img.ImageScaledToFitSize (Frame.Size);

				ParallaxBlock = new UIImageView (scaledImage);
			}

			ParallaxBlock.ClipsToBounds = true;

			centerY = (float)ParallaxBlock.Center.Y;
			offsetDelta = 0f;

			var flagView = GenerateFlag ();
			ParallaxBlock.AddSubview (flagView);

			AddSubview (ParallaxBlock);
		}

		private UIImageView GenerateFlag(){
			var flagView = new UIImageView ();
			flagView.Frame = new CGRect (0, 0, 200, 110);

			var flagBackground = new UIImageView ();
			flagBackground.Alpha = .95f;
			flagBackground.Frame = flagView.Frame;
			flagBackground.BackgroundColor = AppDelegate.BoardOrange;
			flagBackground.Center = ParallaxBlock.Center;

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
			subtitle.Text = "EDITOR'S CHOICE";
			subtitle.TextColor = UIColor.White;
			subtitle.AdjustsFontSizeToFitWidth = true;
			subtitle.Center = new CGPoint(flagBackground.Center.X, placeName.Center.Y + 25);
			subtitle.TextAlignment = UITextAlignment.Center;

			flagView.AddSubviews (flagBackground, pin, rightNow, placeName, subtitle);

			return flagView;
		}
	}
}

