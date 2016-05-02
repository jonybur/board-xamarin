using CoreGraphics;
using UIKit;
using MGImageUtilitiesBinding;

namespace Board.Screens.Controls
{
	public class UIMagazineBanner : UIView
	{
		public UIMagazineBanner ()
		{
			Frame = new CGRect(0, UIMenuBanner.MenuHeight, AppDelegate.ScreenWidth, MagazineBannerPage.Height);

			BackgroundColor = UIColor.FromRGBA(0,0,0,0);

			var backgroundImage = new UIImageView (new CGRect(0,0,Frame.Width, Frame.Height));
			using (UIImage img = UIImage.FromFile ("./screens/main/magazine/westpalmbeach.png")) {
				UIImage scaledImage = img.ImageScaledToFitSize (Frame.Size);
				backgroundImage.Image = scaledImage;
			}

			var bannerPageController = new MagazineBannerPageController (UIPageViewControllerTransitionStyle.Scroll, UIPageViewControllerNavigationOrientation.Horizontal, Frame.Size);

			AddSubviews (backgroundImage, bannerPageController.View);
		}
	}


}

