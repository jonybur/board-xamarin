using System.Collections.Generic;
using System.Linq;
using Board.Infrastructure;
using Board.Schema;
using Board.JsonResponses;
using System;
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

		public static Dictionary<string, int> ContentLikes;
		public static Dictionary<string, bool> UserLikes;

		static MagazineResponse magazine;

		class Timeline{
			public static List<Content> ContentList;
			public static DateTime UpdatedTime;

			public static void Update(){
				ContentList = CloudController.GetTimeline (AppDelegate.UserLocation);

				var publicationIds = Timeline.ContentList.Select (x => x.Id).ToArray ();
				ContentLikes = CloudController.GetLikes (publicationIds);
				UserLikes = CloudController.GetUserLikes (publicationIds);

				UpdatedTime = DateTime.Now;
			}
		}


		// generates the magazine headers
		private void GeneratePages(List<Board.Schema.Board> boardList){

			if (magazine == null || magazine.UpdatedTime.TimeOfDay.TotalMinutes + 60 < DateTime.Now.TimeOfDay.TotalMinutes) {
				Console.WriteLine ("Gets magazine");
				magazine = CloudController.GetMagazine (AppDelegate.UserLocation);
			}

			bool theresMagazine = MagazineResponse.IsValidMagazine (magazine);

			if (Timeline.ContentList == null || Timeline.UpdatedTime.TimeOfDay.TotalMinutes + 10 < DateTime.Now.TimeOfDay.TotalMinutes){
				Console.WriteLine ("Gets timeline");
				Timeline.Update ();
			}

			bool theresTimeline = Timeline.ContentList.Count > 0;

			var pagesName = new List<string> ();

			if (theresMagazine) {
				pagesName.Add("FEATURED");
			}

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

			if (theresMagazine) {
				pages [screenNumber].ContentDisplay = new UICarouselContentDisplay (magazine);
				screenNumber++;
			}

			if (theresTimeline) {
				pages [screenNumber].ContentDisplay = new UITimelineContentDisplay (boardList, Timeline.ContentList);
				screenNumber++;
			}

			pages [screenNumber].ContentDisplay = new UIThumbsContentDisplay (boardList, UIThumbsContentDisplay.OrderMode.Category, UIMagazineBannerPage.Height, UIActionButton.Height);

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

