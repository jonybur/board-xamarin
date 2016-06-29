using Board.Interface;
using CoreGraphics;
using Foundation;
using Haneke;
using UIKit;

namespace Board.Screens.Controls
{
	public sealed class UIBoardBannerPage : UIImageView
	{
		public const int Height = 175;

		public UIBoardBannerPage(float width)
		{
			Frame = new CGRect (0, 0, width, Height);
			BackgroundColor = UIColor.White;

			ClipsToBounds = true;

			var flagView = GenerateFlag ();

			BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);

			AddSubview (flagView);
		}

		private UIImageView GenerateFlag(){
			var flagView = new UIImageView ();
			flagView.Frame = new CGRect (0, 0, 100, 100);
			flagView.BackgroundColor = UIColor.White;
			flagView.Center = Center;
			flagView.Layer.CornerRadius = flagView.Frame.Width / 2;
			flagView.ClipsToBounds = true;

			var flagLogo = new UIImageView ();
			flagLogo.Frame = new CGRect (0, 0, 90, 90);
			flagLogo.BackgroundColor = UIColor.White;
			flagLogo.ContentMode = UIViewContentMode.ScaleAspectFit;
			flagLogo.SetImage (new NSUrl(UIBoardInterface.board.LogoUrl));
			flagLogo.Center = new CGPoint (flagView.Frame.Size.Width / 2, flagView.Frame.Size.Height / 2);
			flagLogo.Layer.CornerRadius = flagLogo.Frame.Width / 2;
			flagLogo.ClipsToBounds = true;

			flagView.AddSubview (flagLogo);

			return flagView;
		}
	}
}

