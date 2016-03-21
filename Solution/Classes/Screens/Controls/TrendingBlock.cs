using UIKit;
using CoreGraphics;
using System;
using MGImageUtilitiesBinding;

namespace Board
{
	public sealed class TrendingBlock : UIImageView
	{
		public UIImageView ParallaxBlock;

		private float centerY;
		private float offsetDelta;

		public void ParallaxMove(float yoffset)
		{
			if (offsetDelta == 0f) {
				offsetDelta = yoffset;
			}

			ParallaxBlock.Center = new CGPoint (ParallaxBlock.Center.X, centerY - (yoffset - offsetDelta)/10);
		}

		public TrendingBlock(float yposition)
		{
			Frame = new CGRect (0, yposition, AppDelegate.ScreenWidth, 250);
			BackgroundColor = UIColor.White;

			ClipsToBounds = true;

			Random random = new Random ();
			int file = random.Next (0, 6);

			using (UIImage img = UIImage.FromFile ("./demo/pictures/"+file+".jpg")) {

				float scale = (float)(img.Size.Width/img.Size.Height);
				float imgw, imgh, autosize;
				autosize = (float)Frame.Width;

				imgw = autosize * scale;
				imgh = autosize;

				UIImage scaledImage = img.ImageScaledToFitSize (new CGSize (imgw, imgh));

				ParallaxBlock = new UIImageView (scaledImage);
			}

			ParallaxBlock.ClipsToBounds = true;

			centerY = (float)ParallaxBlock.Center.Y;
			offsetDelta = 0f;

			AddSubview (ParallaxBlock);

			UILabel trendingLabel = new UILabel (new CGRect (15, 15, 300, 30));
			trendingLabel.Text = "TRENDING";
			trendingLabel.TextColor = UIColor.White;
			trendingLabel.Font = AppDelegate.Narwhal20;
			trendingLabel.SizeToFit ();

			AddSubview (trendingLabel);
		}

	}
}

