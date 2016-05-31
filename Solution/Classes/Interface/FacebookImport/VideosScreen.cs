using System.Collections.Generic;
using System.Threading.Tasks;
using BigTed;
using Board.Facebook;
using Board.Screens.Controls;
using Board.Utilities;
using CoreGraphics;
using Foundation;
using Haneke;
using UIKit;

namespace Board.Interface.FacebookImport
{
	public class VideosScreen : UIViewController
	{
		UIMenuBanner Banner;
		bool CanGoBack;
		int VideoCount;
		UIGalleryScrollView GallerySV;
		List<FacebookVideo> FacebookVideos;
		bool ConnectionError;

		public override void ViewDidLoad () {
			View.BackgroundColor = UIColor.White;

			LoadBanner ();

			CanGoBack = true;

			GallerySV = new UIGalleryScrollView (AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);
			GallerySV.Center = new CGPoint (AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight / 2);
			View.AddSubview (GallerySV);

			BTProgressHUD.Show ();
			FacebookUtils.MakeGraphRequest (UIBoardInterface.board.FBPage.Id, "videos?fields=source,description,updated_time,thumbnails", Completion);

			View.AddSubviews (Banner);
		}

		public override void ViewDidAppear(bool animated){
			Banner.SuscribeToEvents ();
		}

		private void Completion(List<FacebookElement> elementList) {
			FacebookVideos = new List<FacebookVideo> ();

			VideoCount = elementList.Count;

			foreach (var element in elementList) {
				LoadVideoURL(element as FacebookVideo);
			}
		}

		int DownloadsCount = 0;

		private void LoadVideoURL(FacebookVideo fbVideo){

			var thumbImageView = new UIImageView ();
			thumbImageView.Frame = new CGRect (0, 0, UIGalleryScrollView.ButtonSize, UIGalleryScrollView.ButtonSize);
			thumbImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			thumbImageView.SetImage (new NSUrl(fbVideo.ThumbnailUris[0]), UIImage.FromFile("./demo/magazine/nantucket.png"), 
				image => SetImages(image, fbVideo), delegate { });
		}

		private void SetImages(UIImage thumbImage, FacebookVideo fbVideo){
			if (thumbImage == null) {
				ConnectionError = true;
				return;
			}

			GallerySV.SetImage (thumbImage, new System.EventHandler ((sender, e) => {
				var video = new Board.Schema.Video ();
				video.AmazonUrl = fbVideo.Source;
				video.FacebookId = fbVideo.Id;
				video.Description = fbVideo.Description;
				var importLookUp = new VideoImportLookUp (video);
				AppDelegate.PushViewLikePresentView (importLookUp);
			}));
			DownloadsCount++;

			if (DownloadsCount == VideoCount){
				GallerySV.Fill (false, (float)Banner.Frame.Bottom - 20);
				CanGoBack = true;
				BTProgressHUD.Dismiss ();

				if (ConnectionError) {
					UIAlertController alert = UIAlertController.Create("Couldn't access videos", "Please ensure you have a connection to the Internet.", UIAlertControllerStyle.Alert);
					alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));
					NavigationController.PresentViewController (alert, true, null);

					ConnectionError = false;
				}
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
				}
			});

			Banner.AddTap (tap);
		}
	}
}

