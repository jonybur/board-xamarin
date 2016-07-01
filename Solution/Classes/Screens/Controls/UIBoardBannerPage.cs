using Clubby.Interface;
using CoreGraphics;
using Foundation;
using Haneke;
using UIKit;

namespace Clubby.Screens.Controls
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
			
			var flagLogo = new UIImageView ();
			flagLogo.Frame = new CGRect (0, 0, 90, 90);
			flagLogo.BackgroundColor = UIColor.White;
			flagLogo.ContentMode = UIViewContentMode.ScaleAspectFit;
			flagLogo.SetImage (new NSUrl(UIVenueInterface.venue.LogoUrl));
			flagLogo.Center = Center;
			flagLogo.Layer.CornerRadius = flagLogo.Frame.Width / 2;
			flagLogo.ClipsToBounds = true;
			flagLogo.Layer.BorderWidth = 10;
			flagLogo.Layer.BorderColor = UIColor.White.CGColor;

			return flagLogo;
		}
	}
}

