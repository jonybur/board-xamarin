using System.Collections.Generic;
using BigTed;
using Board.Facebook;
using Board.Screens.Controls;
using Board.Utilities;
using CoreGraphics;
using UIKit;

namespace Board.Interface.FacebookImport
{
	public class VideosScreen : UIViewController
	{
		UIMenuBanner Banner;
		bool CanGoBack;
		int VideoCount, DownloadedImages;
		UIGalleryScrollView GallerySV;
		List<FacebookVideo> FacebookVideos;

		public VideosScreen(){
		}

		public override void ViewDidLoad () {
			View.BackgroundColor = UIColor.White;

			LoadBanner ();

			CanGoBack = true;

			GallerySV = new UIGalleryScrollView (AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);
			GallerySV.Center = new CGPoint (AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight / 2);
			View.AddSubview (GallerySV);

			BTProgressHUD.Show ();
			FacebookUtils.MakeGraphRequest (UIBoardInterface.board.FBPage.Id, "videos", Completion);

			View.AddSubviews (Banner);
		}

		public override void ViewDidAppear(bool animated){
			Banner.SuscribeToEvents ();
		}

		private void Completion(List<FacebookElement> elementList) {
			FacebookVideos = new List<FacebookVideo> ();

			VideoCount = elementList.Count;

			foreach (var element in elementList) {
				FacebookUtils.MakeGraphRequest (element.Id, "?fields=id,source,thumbnails", LoadVideoURL);
			}

			BTProgressHUD.Dismiss ();
		}

		private async void LoadVideoURL(List<FacebookElement> elementList){
			if (elementList.Count == 0){return;}

			var fbVideoSource = (FacebookVideoSource)elementList[0];
			var thumbImage = await CommonUtils.DownloadUIImageFromURL(fbVideoSource.ThumbnailUrl);

			GallerySV.SetImage (thumbImage, new System.EventHandler(async delegate {
				var video = new Board.Schema.Video();
				video.AmazonUrl = fbVideoSource.Source;
				var importLookUp = new VideoImportLookUp(video);
				AppDelegate.PushViewLikePresentView(importLookUp);
			}));

			DownloadedImages++;

			if (DownloadedImages == VideoCount) {
				GallerySV.Fill (false, (float)Banner.Frame.Bottom - 20);
				CanGoBack = true;
				BTProgressHUD.Dismiss ();
			}
		}

		private void LoadBanner()
		{
			Banner = new UIMenuBanner ("VIDEOS", "arrow_left");

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				if (!CanGoBack){
					return;
				}

				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					CanGoBack = false;
					AppDelegate.NavigationController.PopViewController(true);
					Banner.UnsuscribeToEvents ();
					//MemoryUtility.ReleaseUIViewWithChildren (View);
				}
			});

			Banner.AddTap (tap);
		}
	}
}

