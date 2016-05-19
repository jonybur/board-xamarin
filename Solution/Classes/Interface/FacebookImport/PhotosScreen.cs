using System.Collections.Generic;
using BigTed;
using Board.Facebook;
using Board.Screens.Controls;
using Board.Utilities;
using System.Linq;
using CoreGraphics;
using Board.Schema;
using UIKit;

namespace Board.Interface.FacebookImport
{
	public class PhotosScreen : UIViewController
	{
		UIMenuBanner Banner;
		string AlbumID;
		int PictureCount;
		int DownloadedImages;
		bool CanGoBack;
		bool CanEnterImport;
		UIGalleryScrollView GallerySV;
		List<FacebookImage> FacebookImages;

		public PhotosScreen(string albumid){
			AlbumID = albumid;
		}

		public override void ViewDidLoad () {
			View.BackgroundColor = UIColor.White;

			LoadBanner ();

			GallerySV = new UIGalleryScrollView (AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);
			GallerySV.Center = new CGPoint (AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight / 2);
			View.AddSubview (GallerySV);

			BTProgressHUD.Show ();
			FacebookUtils.MakeGraphRequest (AlbumID, "photos", Completion);

			View.AddSubviews (Banner);
		}

		public override void ViewDidAppear(bool animated){
			CanEnterImport = true;
			Banner.SuscribeToEvents ();
		}

		private void Completion(List<FacebookElement> elementList) {
			FacebookImages = new List<FacebookImage> ();
		
			PictureCount = elementList.Count;
			DownloadedImages = 0;

			foreach (var element in elementList) {
				FacebookUtils.MakeGraphRequest (element.Id, "?fields=images", LoadImageURL);
			}

			if (elementList.Count == 0) {
				BTProgressHUD.Dismiss ();
			}
		}

		private async void LoadImageURL(List<FacebookElement> elementList){
			if (elementList.Count == 0){return;}

			elementList = elementList.OrderByDescending(x => ((FacebookImage)x).Height).ToList();

			var minElement = (FacebookImage)elementList [elementList.Count - 1];
			FacebookImage maxElement;
			if (elementList.Count > 2) {
				maxElement = elementList [2] as FacebookImage;
			} else if (elementList.Count > 1){
				maxElement = elementList [1] as FacebookImage;
			} else {
				maxElement = elementList [0] as FacebookImage;
			}

			FacebookImages.Add (minElement);
			var minImage = await CommonUtils.DownloadUIImageFromURL(minElement.Source);

			GallerySV.SetImage (minImage, new System.EventHandler(async delegate {
				if (CanEnterImport){
					CanEnterImport = false;
					var picture = new Picture();
					var maxImage = await CommonUtils.DownloadUIImageFromURL(maxElement.Source);
					picture.SetImageFromUIImage(maxImage);
					var importLookUp = new PictureImportLookUp(picture);
					AppDelegate.PushViewLikePresentView(importLookUp);
				}
			}));
			DownloadedImages++;

			if (DownloadedImages == PictureCount) {
				GallerySV.Fill (false, (float)Banner.Frame.Bottom - 20);
				CanGoBack = true;
				BTProgressHUD.Dismiss ();
			}
		}
			
		private void LoadBanner()
		{
			Banner = new UIMenuBanner ("PHOTOS", "arrow_left");

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

