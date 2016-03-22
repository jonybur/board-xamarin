using UIKit;
using CoreGraphics;
using System;
using MGImageUtilitiesBinding;

namespace Board
{
	public sealed class TrendingBlock : UIImageView
	{
		public UIImageView ParallaxBlock;
		private UIImageView LikeComponent;
		private const int iconSize = 35;

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

			CreateLikeComponent ();

			AddSubview (LikeComponent);
		}

		private void CreateLikeComponent()
		{
			LikeComponent = new UIImageView (new CGRect (Frame.Width - 80,
				Frame.Height - 40, 80, 40));
			
			UIImageView likeView = CreateLike (LikeComponent.Frame);
			UILabel likeLabel = CreateLikeLabel (likeView.Center);

			LikeComponent.AddSubviews (likeView, likeLabel);
		}


		private UIImageView CreateLike(CGRect frame)
		{
			UIImageView likeView = new UIImageView(new CGRect(0, 0, iconSize, iconSize));

			using (UIImage img = UIImage.FromFile ("./boardinterface/widget/like.png")) {
				likeView.Image = img;
				likeView.Image = likeView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
			}

			likeView.TintColor = UIColor.White;
			likeView.Center = new CGPoint (frame.Width - 5 - iconSize / 2, frame.Height / 2);
			return likeView;
		}

		private UILabel CreateLikeLabel(CGPoint center)
		{
			const int fontSize = 18;

			UIFont likeFont = UIFont.SystemFontOfSize (fontSize);
			Random rand = new Random ();
			int randomLike = rand.Next (102, 256);

			string likeText = randomLike.ToString();
			CGSize likeLabelSize = likeText.StringSize (likeFont);

			UILabel likeLabel = new UILabel(new CGRect(0, 0, likeLabelSize.Width + 20, likeLabelSize.Height));
			likeLabel.TextColor = UIColor.White;
			likeLabel.Font = likeFont;
			likeLabel.Text = likeText;
			likeLabel.Center = new CGPoint (center.X - likeLabel.Frame.Width / 2 - 10, center.Y);
			likeLabel.TextAlignment = UITextAlignment.Center;

			return likeLabel;
		}


	}
}

