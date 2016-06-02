using System.Collections.Generic;
using System.Linq;
using Board.Infrastructure;
using CoreGraphics;
using MGImageUtilitiesBinding;
using UIKit;

namespace Board.Screens.Controls
{
	public static class UIMagazineServices{
		public static UIMagazinePage[] Pages;

		public static UIViewController[] PageBanners{
			get {
				return Pages.Select(x => x.Banner).ToArray();
			}
		}
	}

	public class UIMagazine{
		// contains bannercontroller, switches between pages banners
		public UIMagazineBanner Banner;
		// contains (page banner + content display)
		public UIMagazinePage[] Pages{
			get { return UIMagazineServices.Pages; }
			set { UIMagazineServices.Pages = value; }
		}

		public UIMagazine(List<Board.Schema.Board> boardList){
			GeneratePages (boardList);
			Banner = new UIMagazineBanner ();
		}

		// generates the magazine headers
		private void GeneratePages(List<Board.Schema.Board> boardList){

			CloudController.GetMagazine (AppDelegate.UserLocation);

			string[] demopages = new []{ "TRENDING"/*, "EDITOR'S CHOICE"*/, "ALL" };

			var pages = new UIMagazinePage[demopages.Length];

			for (int i = 0; i < demopages.Length; i++) {
				pages [i] = new UIMagazinePage ();

				var banner = new UIMagazineBannerPage (demopages[i]);

				var controller = new UIViewController ();
				controller.Add (banner);
				controller.View.Frame = banner.Frame;

				pages [i].Banner = controller;
			}

			pages [0].ContentDisplay = new UITimelineContentDisplay (boardList);
			//pages [1].ContentDisplay = new UICarouselContentDisplay ();
			pages [1].ContentDisplay = new UIThumbsContentDisplay (boardList, UIThumbsContentDisplay.OrderMode.Neighborhood, UIMagazineBannerPage.Height, UIActionButton.Height);

			Pages = pages;
		}
	}

	public class UIMagazineBanner : UIView
	{
		public readonly UIMagazineBannerPageController MagazineBannerPageController;

		public UIMagazineBanner ()
		{
			Frame = new CGRect(0, UIMenuBanner.Height, AppDelegate.ScreenWidth, UIMagazineBannerPage.Height);

			BackgroundColor = UIColor.FromRGBA(0,0,0,0);

			var backgroundImage = new UIImageView (new CGRect(0,0,Frame.Width, Frame.Height));
			using (UIImage img = UIImage.FromFile ("./demo/magazine/nantucket.png")) {
				UIImage scaledImage = img.ImageScaledToFitSize (Frame.Size);
				backgroundImage.Image = scaledImage;

			}

			MagazineBannerPageController = new UIMagazineBannerPageController (UIPageViewControllerTransitionStyle.Scroll, UIPageViewControllerNavigationOrientation.Horizontal, Frame.Size);
			AddSubviews (backgroundImage, MagazineBannerPageController.View);
		}
	}


}

