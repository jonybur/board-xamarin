using Clubby.Screens.Controls;
using CoreGraphics;
using Foundation;
using Haneke;
using UIKit;

namespace Clubby.Interface
{
	public class UITopBanner : UIView {
		UIImageView CoverImage;
		public const int Height = 200;

		public float Bottom{
			get { 
				return (float)CoverImage.Frame.Bottom;
			}
		}

		public UITopBanner(float width){
			CoverImage = new UIImageView (new CGRect(0, 0, width, Height));

			CoverImage.ClipsToBounds = true;

			var logo = GenerateLogo ();

			LoadCoverImage();

			AddSubviews (CoverImage, logo);

		}

		public void LoadCoverImage(){
			var localBoard = UIVenueInterface.venue;

			var scaledImageView = new UIImageView ();
			scaledImageView.Frame = CoverImage.Frame;
			scaledImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			scaledImageView.SetImage (new NSUrl (localBoard.CoverImageUrl));

			CoverImage.AddSubview (scaledImageView);
		}

		private UIImageView GenerateLogo(){
			var flagLogoBackground = new UIImageView ();
			flagLogoBackground.Frame = new CGRect (0, 0, 125, 125);
			flagLogoBackground.BackgroundColor = UIColor.White;
			flagLogoBackground.Center = new CGPoint(CoverImage.Frame.Width / 2, CoverImage.Frame.Height);
			flagLogoBackground.Layer.CornerRadius = flagLogoBackground.Frame.Width / 2;
			flagLogoBackground.ClipsToBounds = true;

			var flagLogo = new UIImageView ();
			flagLogo.Frame = new CGRect (0, 0, 120, 120);
			flagLogo.BackgroundColor = UIColor.White;
			flagLogo.ContentMode = UIViewContentMode.ScaleAspectFit;
			flagLogo.SetImage (new NSUrl(UIVenueInterface.venue.LogoUrl));
			flagLogo.Center = new CGPoint(flagLogoBackground.Frame.Width / 2, flagLogoBackground.Frame.Height / 2);
			flagLogo.Layer.CornerRadius = flagLogo.Frame.Width / 2;
			flagLogo.ClipsToBounds = true;

			flagLogoBackground.AddSubview (flagLogo);

			return flagLogoBackground;
		}
	}
}

