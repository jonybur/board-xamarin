using System.Collections.Generic;
using System.Linq;
using Board.Infrastructure;
using Board.Schema;
using Board.Utilities;
using Board.Facebook;
using Board.JsonResponses;
using System;
using Newtonsoft.Json.Linq;
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

		static class TimelineContent{
			public static List<Content> ContentList;
			public static DateTime UpdatedTime;

			public static void Update(){
				ContentList = CloudController.GetTimeline (AppDelegate.UserLocation);

				var publicationIds = TimelineContent.ContentList.Select (x => x.Id).ToArray ();

				ContentLikes = CloudController.GetLikesSync (publicationIds);
				UserLikes = CloudController.GetUserLikes (publicationIds);

				foreach (var content in TimelineContent.ContentList) {
					FacebookUtils.MakeGraphRequest (content.FacebookId, "?fields=likes", LoadFacebookLike);
				}

				UpdatedTime = DateTime.Now;
			}

			private static void LoadFacebookLike(List<FacebookElement> obj){

				if (obj.Count > 0) {
					int facebookLikeCount = 0;
					var likes = (FacebookLikes)obj [0];

					if (likes.LikesData != null) {
						facebookLikeCount = CommonUtils.CountStringOccurrences (likes.LikesData, "id");
					}

					var contentId = TimelineContent.ContentList.Find (x => x.FacebookId == obj [0].Id).Id;
					ContentLikes [contentId] += facebookLikeCount;

					UITimelineContentDisplay.UpdateWidgetLikeCount (contentId, ContentLikes [contentId]);
				}

			}
		}

		public static void AddLikeToContent(string contentId){
			UIMagazine.ContentLikes [contentId]++;
			UIMagazine.UserLikes [contentId] = true;
		}

		public static void RemoveLikeToContent(string contentId){
			UIMagazine.ContentLikes [contentId]--;
			UIMagazine.UserLikes [contentId] = false;
		}

		// generates the magazine headers
		private void GeneratePages(List<Board.Schema.Board> boardList){

			if (magazine == null || magazine.UpdatedTime.TimeOfDay.TotalMinutes + 60 < DateTime.Now.TimeOfDay.TotalMinutes) {
				Console.WriteLine ("Gets magazine");
				magazine = CloudController.GetMagazine (AppDelegate.UserLocation);
			}

			bool theresMagazine = MagazineResponse.IsValidMagazine (magazine);

			if (TimelineContent.ContentList == null || TimelineContent.UpdatedTime.TimeOfDay.TotalMinutes + 10 < DateTime.Now.TimeOfDay.TotalMinutes){
				Console.WriteLine ("Gets timeline");
				TimelineContent.Update ();
			}

			bool theresTimeline = TimelineContent.ContentList.Count > 0;

			var pagesName = new List<string> ();

			if (theresTimeline) {
				pagesName.Add ("TRENDING");
			}

			if (theresMagazine) {
				pagesName.Add("FEATURED");
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

			if (theresMagazine) {
				pages [screenNumber].ContentDisplay = new UICarouselContentDisplay (magazine);
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

