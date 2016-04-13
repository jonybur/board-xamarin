using UIKit;
using Board.Utilities;
using Board.Screens.Controls;
using System.Collections.Generic;
using BigTed;
using Board.Utilities;
using Board.Facebook;
using System.Linq;
using CoreGraphics;
using System;

namespace Board.Interface.FacebookImport
{
	public class PhotosScreen : UIViewController
	{
		MenuBanner Banner;
		string AlbumID;

		GalleryScrollView GallerySV;
		List<FacebookImage> FacebookImages;

		public PhotosScreen(string albumid){
			AlbumID = albumid;
		}

		public override void ViewDidLoad () {
			View.BackgroundColor = UIColor.White;

			LoadBanner ();

			GallerySV = new GalleryScrollView (AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);
			GallerySV.Center = new CGPoint (AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight / 2);
			View.AddSubview (GallerySV);

			BTProgressHUD.Show ();
			FacebookUtils.MakeGraphRequest (AlbumID, "photos", Completion);

			View.AddSubviews (Banner);
		}

		private void Completion(List<FacebookElement> elementList) {
			FacebookImages = new List<FacebookImage> ();
		
			PictureCount = elementList.Count - 1;
			DownloadedImages = 0;

			foreach (var element in elementList) {
				FacebookUtils.MakeGraphRequest (element.Id, "?fields=images", LoadImageURL);
			}

			if (elementList.Count == 0) {
				BTProgressHUD.Dismiss ();
			}
		}

		int PictureCount;
		int DownloadedImages;
		bool CanGoBack;

		private async void LoadImageURL(List<FacebookElement> elementList){
			
			int minwidth = -1;

			FacebookImage minImage = new FacebookImage();

			foreach (var element in elementList) {
				var fbImage = ((FacebookImage)element);
				if (fbImage.Width < minwidth || minwidth == -1) {
					minwidth = fbImage.Width;
					minImage = fbImage;
				}
			}

			FacebookImages.Add (minImage);
			var image = await CommonUtils.DownloadUIImageFromURL(minImage.Source);
			GallerySV.SetImage (image);
			DownloadedImages++;

			if (DownloadedImages == PictureCount) {
				GallerySV.Fill (false, (float)Banner.Frame.Bottom - 20);
				CanGoBack = true;
				BTProgressHUD.Dismiss ();
			}
		}

		public override void ViewDidAppear(bool animated)
		{
			Banner.SuscribeToEvents ();
		}
			
		private void LoadBanner()
		{
			Banner = new MenuBanner ("./boardinterface/screens/photos/banner/" + AppDelegate.PhoneVersion + ".jpg");

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				if (!CanGoBack){
					return;
				}

				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					AppDelegate.NavigationController.PopViewController(true);
					Banner.UnsuscribeToEvents ();
					MemoryUtility.ReleaseUIViewWithChildren (View);
				}
			});

			Banner.AddTap (tap);
		}
	}
}

