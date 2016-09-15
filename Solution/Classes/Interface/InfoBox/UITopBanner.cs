using CoreGraphics;
using Foundation;
using Board.Interface.VenueInterface;
using Haneke;
using UIKit;

namespace Board.Interface
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

			var logo = new UILogoImage (CoverImage.Frame.Size, UIVenueInterface.board.LogoUrl);

			LoadCoverImage();

			AddSubviews (CoverImage, logo);

		}

		public void LoadCoverImage(){
			var localBoard = UIVenueInterface.board;

			var scaledImageView = new UIImageView ();
			scaledImageView.Frame = CoverImage.Frame;
			scaledImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			scaledImageView.SetImage (new NSUrl (localBoard.CoverImageUrl));

			CoverImage.AddSubview (scaledImageView);
		}
	}

	public class UILogoImage : UIImageView{

		public UILogoImage(string logoUrl){
			Frame = new CGRect (0, 0, 125, 125);
			BackgroundColor = UIColor.White;
			Center = new CGPoint(AppDelegate.ScreenWidth / 2, 115);
			Layer.CornerRadius = Frame.Width / 2;
			ClipsToBounds = true;

			var flagLogo = new UIImageView ();
			flagLogo.Frame = new CGRect (0, 0, 120, 120);
			flagLogo.BackgroundColor = UIColor.White;
			flagLogo.ContentMode = UIViewContentMode.ScaleAspectFit;
			flagLogo.SetImage (new NSUrl(logoUrl));
			flagLogo.Center = new CGPoint(Frame.Width / 2, Frame.Height / 2);
			flagLogo.Layer.CornerRadius = flagLogo.Frame.Width / 2;
			flagLogo.ClipsToBounds = true;

			AddSubview (flagLogo);
		}

		public UILogoImage(CGSize bannerSize, string logoUrl){
			Frame = new CGRect (0, 0, 125, 125);
			BackgroundColor = UIColor.White;
			Center = new CGPoint(bannerSize.Width / 2, bannerSize.Height);
			Layer.CornerRadius = Frame.Width / 2;
			ClipsToBounds = true;

			var flagLogo = new UIImageView ();
			flagLogo.Frame = new CGRect (0, 0, 120, 120);
			flagLogo.BackgroundColor = UIColor.White;
			flagLogo.ContentMode = UIViewContentMode.ScaleAspectFit;
			flagLogo.SetImage (new NSUrl(logoUrl));
			flagLogo.Center = new CGPoint(Frame.Width / 2, Frame.Height / 2);
			flagLogo.Layer.CornerRadius = flagLogo.Frame.Width / 2;
			flagLogo.ClipsToBounds = true;

			AddSubview (flagLogo);
		}
	}
}

