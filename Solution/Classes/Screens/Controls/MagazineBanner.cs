using UIKit;
using CoreGraphics;
using System;
using MGImageUtilitiesBinding;

namespace Board.Screens.Controls
{
	public class MagazineBanner : UIImageView
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

		public MagazineBanner()
		{
			Frame = new CGRect (0, MenuBanner.MenuHeight, AppDelegate.ScreenWidth, 175);
			BackgroundColor = UIColor.White;

			ClipsToBounds = true;

			using (UIImage img = UIImage.FromFile ("./screens/magazine/wpb_banner.png")) {
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
			flagView.Frame = new CGRect (0, 0, 200, 100);

			var flagBackground = new UIImageView ();
			flagBackground.Alpha = .95f;
			flagBackground.Frame = flagView.Frame;
			flagBackground.BackgroundColor = AppDelegate.BoardOrange;
			flagBackground.Center = ParallaxBlock.Center;

			var pin = new UIImageView();
			pin.Frame = flagView.Frame;
			pin.Center = flagBackground.Center;
			using (var img = UIImage.FromFile ("./screens/magazine/pin.png")) {
				pin.Image = img;
			}

			var placeName = new UILabel ();
			placeName.Frame = new CGRect (5, 0, flagView.Frame.Width-10, 20);
			placeName.Font = AppDelegate.Narwhal18;
			placeName.Text = "WEST PALM BEACH";
			placeName.TextColor = UIColor.White;
			placeName.AdjustsFontSizeToFitWidth = true;
			placeName.Center = new CGPoint(flagBackground.Center.X, flagBackground.Center.Y + 15);
			placeName.TextAlignment = UITextAlignment.Center;

			var subtitle = new UILabel ();
			subtitle.Frame = new CGRect (5, 0, flagView.Frame.Width-10, 20);
			subtitle.Font = AppDelegate.Narwhal14;
			subtitle.Text = "RIGHT NOW";
			subtitle.TextColor = UIColor.White;
			subtitle.AdjustsFontSizeToFitWidth = true;
			subtitle.Center = new CGPoint(flagBackground.Center.X, placeName.Center.Y + 20);
			subtitle.TextAlignment = UITextAlignment.Center;

			flagView.AddSubviews (flagBackground, pin, placeName, subtitle);

			return flagView;
		}
	}
}

