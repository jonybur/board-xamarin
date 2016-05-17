using CoreGraphics;
using UIKit;
using MGImageUtilitiesBinding;
using Board.Interface;

namespace Board.Screens.Controls
{
	public sealed class UIBoardBannerPage : UIImageView
	{
		public const int Height = 175;

		public UIBoardBannerPage(UIImage boardImage, float width)
		{
			Frame = new CGRect (0, 0, width, Height);
			BackgroundColor = UIColor.White;

			ClipsToBounds = true;

			var flagView = GenerateFlag (boardImage);

			BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);

			AddSubview (flagView);
		}

		private UIImageView GenerateFlag(UIImage boardImage){
			var flagView = new UIImageView ();
			flagView.Frame = new CGRect (0, 0, 100, 100);

			var flagBackground = new UIImageView ();
			flagBackground.Frame = flagView.Frame;
			flagBackground.BackgroundColor = UIColor.White;
			flagBackground.Center = Center;

			var flagLogo = new UIImageView ();
			flagLogo.Image = boardImage.ImageScaledToFitSize (new CGSize(flagView.Frame.Width - 10, flagView.Frame.Height - 10));
			flagLogo.Frame = new CGRect (0, 0, flagLogo.Image.Size.Width, flagLogo.Image.Size.Height);
			flagLogo.Center = flagBackground.Center;

			flagView.AddSubviews (flagBackground, flagLogo);

			return flagView;
		}
	}
}

