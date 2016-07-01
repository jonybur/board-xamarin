using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using MGImageUtilitiesBinding;
using Clubby.Schema;
using UIKit;

namespace Clubby.Screens.Controls
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

		public UIMagazine(List<Venue> boardList){
			GeneratePages (boardList);
			//Banner = new UIMagazineBanner ();
		}

		static class TimelineContent{
			public static List<Content> ContentList;
			public static DateTime UpdatedTime;

			public static void Update(){
				// get from all venues
				ContentList = MainMenuScreen.FetchedVenues.GetTimeline();
				UpdatedTime = DateTime.Now;
			}
		}

		// generates the magazine headers
		private void GeneratePages(List<Venue> boardList){
			
			if (TimelineContent.ContentList == null || TimelineContent.UpdatedTime.TimeOfDay.TotalMinutes + 10 < DateTime.Now.TimeOfDay.TotalMinutes){
				TimelineContent.Update ();
			}

			bool theresTimeline = TimelineContent.ContentList.Count > 0;

			var pagesName = new List<string> ();

			if (theresTimeline) {
				pagesName.Add ("TRENDING");
			}

			pagesName.Add("DIRECTORY");

			var pages = new UIMagazinePage[pagesName.Count];

			for (int i = 0; i < pagesName.Count; i++) {
				pages [i] = new UIMagazinePage ();

				var banner = new UIMagazineBannerPage (pagesName [i]);

				var controller = new UIViewController ();
				controller.Add (banner);
				controller.View.Frame = banner.Frame;

				pages [i].Banner = controller;
			}

			int screenNumber = 0;

			if (theresTimeline) {
				pages [screenNumber].ContentDisplay = new UITimelineContentDisplay (boardList, TimelineContent.ContentList);
				screenNumber++;
			}

			pages [screenNumber].ContentDisplay = new UIThumbsContentDisplay (boardList, UIThumbsContentDisplay.OrderMode.Distance, UIMagazineBannerPage.Height, UIActionButton.Height);

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
				var scaledImage = img.ImageScaledToFitSize (Frame.Size);
				backgroundImage.Image = scaledImage;

			}

			MagazineBannerPageController = new UIMagazineBannerPageController (UIPageViewControllerTransitionStyle.Scroll, UIPageViewControllerNavigationOrientation.Horizontal, Frame.Size);
			AddSubviews (backgroundImage, MagazineBannerPageController.View);
		}
	}


}

