using Clubby.Screens.Controls;
using CoreGraphics;
using Haneke;
using UIKit;

namespace Clubby.Interface
{
	public class UITopBanner : UIView {
		UIImageView BackgroundImage;
		UIBoardBannerPage BannerPage;
		public const int Height = 175;

		public float Bottom{
			get { 
				return (float)BackgroundImage.Frame.Bottom;
			}
		}

		public UITopBanner(float width){
			BackgroundImage = new UIImageView (new CGRect(0, 0, width, Height));

			BackgroundImage.ClipsToBounds = true;
			BannerPage = new UIBoardBannerPage (width);

			AddSubviews(BackgroundImage, BannerPage);

			LoadCoverImage();
		}

		public void LoadCoverImage(){
			var localBoard = UIVenueInterface.venue;
			var scaledImageView = new UIImageView ();
			scaledImageView.Frame = BackgroundImage.Frame;
			scaledImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			scaledImageView.SetImage (new Foundation.NSUrl (localBoard.CoverImageUrl));
			BackgroundImage.AddSubview (scaledImageView);
		}
	}
}

