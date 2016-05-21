using System.Collections.Generic;
using System.Linq;
using BigTed;
using Board.Facebook;
using Board.Schema;
using Board.Screens.Controls;
using Board.Utilities;
using CoreGraphics;
using UIKit;

namespace Board.Interface.FacebookImport
{
	public class PhotosScreen : UIViewController
	{
		UIMenuBanner Banner;
		string AlbumID;
		int PictureCount, DownloadedImages;
		bool CanGoBack, CanEnterImport;
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

				var fbId = element.Id;
				var fbDescription = ((FacebookPhoto)element).Description;


				FacebookUtils.MakeGraphRequest (element.Id, "?fields=images", async delegate(List<FacebookElement> obj) {
					if (obj.Count == 0){ CanGoBack = true; return; }

					obj = obj.OrderByDescending(x => ((FacebookImage)x).Height).ToList();

					var minElement = (FacebookImage)obj [obj.Count - 1];
					FacebookImage maxElement;
					if (obj.Count > 2) {
						maxElement = obj [2] as FacebookImage;
					} else if (obj.Count > 1){
						maxElement = obj [1] as FacebookImage;
					} else {
						maxElement = obj [0] as FacebookImage;
					}

					FacebookImages.Add (minElement);
					var minImage = await CommonUtils.DownloadUIImageFromURL(minElement.Source);

					GallerySV.SetImage (minImage, new System.EventHandler(async delegate {
						if (CanEnterImport){
							CanEnterImport = false;
							var picture = new Picture();

							var maxImage = await CommonUtils.DownloadUIImageFromURL(maxElement.Source);

							picture.FacebookId = fbId;
							picture.Description = fbDescription;
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
				});
			}

			if (elementList.Count == 0) {
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
					BTProgressHUD.Dismiss();
					AppDelegate.PopViewControllerWithCallback(delegate {
						MemoryUtility.ReleaseUIViewWithChildren(View);
					});
					Banner.UnsuscribeToEvents ();
				}
			});

			Banner.AddTap (tap);
		}
	}
}

