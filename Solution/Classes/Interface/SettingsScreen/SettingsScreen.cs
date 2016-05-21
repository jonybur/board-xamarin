using System.Collections.Generic;
using BigTed;
using Board.Facebook;
using Board.Infrastructure;
using Board.Schema;
using Board.Screens;
using System.Linq;
using Board.Screens.Controls;
using System;
using Board.Utilities;
using CoreGraphics;
using UIKit;

namespace Board.Interface
{
	public class SettingsScreen : UIViewController
	{
		UIMenuBanner Banner;
		UIOneLineMenuButton SyncButton, AnalyticsButton, DeleteButton, ImportButton;

		List<Content> ContentToImport;
		CGPoint ItemLocation = new CGPoint(0,110);

		public override void ViewDidLoad ()
		{
			LoadBanner ();

			CreateAnalyticsButton ((float)Banner.Frame.Bottom);
			CreateSyncButton ((float)AnalyticsButton.Frame.Bottom + 1);
			CreateImportButton ((float)SyncButton.Frame.Bottom + 1);
			CreateDeleteButton ((float)ImportButton.Frame.Bottom + 1);

			View.AddSubviews (SyncButton, AnalyticsButton, ImportButton, DeleteButton);

			View.BackgroundColor = UIColor.White;
		}

		public override void ViewDidAppear(bool animated)
		{
			if (UIBoardInterface.board.FBPage == null) {
				SyncButton.SetLabel("Connect to Facebook Page >");
			} else {
				SyncButton.SetLabel(string.Format ("Connected to {0}", UIBoardInterface.board.FBPage.Name));
			}

			AnalyticsButton.SetUnpressedColors ();
			SyncButton.SetUnpressedColors ();
			Banner.SuscribeToEvents ();
		}

		private void CreateDeleteButton(float yPosition)
		{
			DeleteButton = new UIOneLineMenuButton (yPosition);
			DeleteButton.SetLabel ("Delete Board");
			DeleteButton.SetUnpressedColors ();

			DeleteButton.TouchUpInside += (sender, e) => {
				DeleteButton.SetPressedColors();

				UIAlertController alert = UIAlertController.Create("Warning!", "This will delete the entire Board.\nYou CANNOT undo this action.", UIAlertControllerStyle.Alert);
				alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, delegate {
					DeleteButton.SetUnpressedColors();
				}));
				alert.AddAction (UIAlertAction.Create ("Delete", UIAlertActionStyle.Destructive, delegate {
					CloudController.DeleteBoard(UIBoardInterface.board.Id);

					DeleteButton.SetUnpressedColors();

					var containerScreen = AppDelegate.NavigationController.ViewControllers[AppDelegate.NavigationController.ViewControllers.Length - 3] as ContainerScreen;
					if (containerScreen!= null)
					{
						containerScreen.LoadMainMenu();
					}
					AppDelegate.PopToViewControllerWithCallback (containerScreen, AppDelegate.ExitBoardInterface);
				}));
				NavigationController.PresentViewController (alert, true, null);
			};
		}

		private void CreateImportButton(float yPosition)
		{
			ImportButton = new UIOneLineMenuButton (yPosition);
			ImportButton.SetLabel ("Import content from Facebook");
			ImportButton.SetUnpressedColors ();

			ImportButton.TouchUpInside += (sender, e) => {
				ImportButton.SetPressedColors();

				UIAlertController alert = UIAlertController.Create("Continue?", "This will fill the Board with content from a Facebook page.\nThis process might take a few minutes.", UIAlertControllerStyle.Alert);
				alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, delegate {
					ImportButton.SetUnpressedColors();
				}));
				alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, delegate {
					if (alert.TextFields.Length == 0){
						return;
					}

					var textField = alert.TextFields[0];

					if (textField.Text == string.Empty || textField.Text == null){ 
						return;
					}

					ImportButton.SetUnpressedColors();

					ContentToImport = new List<Content>();

					// gets 5 announcements
					BTProgressHUD.Show("Importing Announcements...");

					FBIdToImportFrom = textField.Text;

					FacebookUtils.MakeGraphRequest(FBIdToImportFrom, "?fields=posts.limit(3)", GetAnnouncements);

					// - updates board -
					ImportButton.SetUnpressedColors();
				}));
				alert.AddTextField(delegate(UITextField obj) {
					obj.Placeholder = "Facebook Page ID";
				});

				NavigationController.PresentViewController (alert, true, null);

			};
		}

		string FBIdToImportFrom;

		void GetAnnouncements(List<FacebookElement> FacebookElements){
			// parses all posts
			foreach (FacebookPost fbPost in FacebookElements) {

				if (fbPost.Message != "<null>" && fbPost.Message != null) {
					ItemLocation.X += 290;
					var announcement = new Announcement (fbPost, ItemLocation, CGAffineTransform.MakeIdentity());

					ContentToImport.Add (announcement);
				} else {
					continue;
				}
			}

			// gets 5 events
			BTProgressHUD.Show("Importing Events...");
			FacebookUtils.MakeGraphRequest(FBIdToImportFrom, "?fields=events.limit(3)", GetEvents);
		}

		void GetEvents(List<FacebookElement> FacebookElements){
			// parses all events
			int coversToLoad = 0;
			var boardEvents = new List<BoardEvent> ();
			ItemLocation = new CGPoint (0, 300);

			foreach (FacebookEvent fbEvent in FacebookElements) {

				ItemLocation.X += 290;
				var boardEvent = new BoardEvent (fbEvent, ItemLocation, CGAffineTransform.MakeIdentity());
				boardEvents.Add (boardEvent);

				FacebookUtils.MakeGraphRequest (fbEvent.Id, "?fields=cover", delegate(List<FacebookElement> elementList) {
					if (elementList.Count > 0) {
						var cover = elementList [0] as FacebookCover;
						if (cover != null) {
							boardEvents[coversToLoad].ImageUrl = cover.Source;
						}
					}

					coversToLoad++;

					if (coversToLoad == FacebookElements.Count){
						ContentToImport.AddRange(boardEvents);

						// gets albums
						BTProgressHUD.Show("Importing Photos...");
						//FacebookUtils.MakeGraphRequest(UIBoardInterface.board.FBPage.Id, "albums", GetAlbums);
						FacebookUtils.MakeGraphRequest(FBIdToImportFrom, "photos/uploaded?limit=9", GetPhotos);
					}
				});
			}

			if (FacebookElements.Count == 0) {
				// gets albums
				BTProgressHUD.Show("Importing Photos...");
				//FacebookUtils.MakeGraphRequest(UIBoardInterface.board.FBPage.Id, "albums", GetAlbums);
				FacebookUtils.MakeGraphRequest(FBIdToImportFrom, "photos/uploaded?limit=9", GetPhotos);
			}

		}
			
		void GetAlbums(List<FacebookElement> elementList)
		{
			if (elementList.Count > 0) {
				var album = elementList [0] as FacebookAlbum;
				FacebookUtils.MakeGraphRequest(album.Id, "photos?limit=3", GetPhotos);
			}
		}

		void GetPhotos(List<FacebookElement> FacebookElements){
			// parses all pictures
			var pictures = new List<Picture> ();
			int picturesToLoad = 0;
			ItemLocation = new CGPoint (0, 510);

			int photoNumber = 0;
			foreach (FacebookPhoto fbPhoto in FacebookElements) {
				if (photoNumber == 3) {
					ItemLocation = new CGPoint (1400, 310);
				} else if (photoNumber == 6) {
					ItemLocation = new CGPoint (1400, 500);
				}
				ItemLocation.X += 290;
				var picture = new Picture (fbPhoto, ItemLocation, CGAffineTransform.MakeIdentity());
				pictures.Add (picture);
				photoNumber++;
				FacebookUtils.MakeGraphRequest (fbPhoto.Id, "?fields=images", async delegate(List<FacebookElement> elementList) {
					if (elementList.Count > 0) {
						elementList = elementList.OrderByDescending(x => ((FacebookImage)x).Height).ToList();

						FacebookImage fbImage;
						if (elementList.Count > 2) {
							fbImage = elementList [2] as FacebookImage;
						} else if (elementList.Count > 1){
							fbImage = elementList [1] as FacebookImage;
						} else {
							fbImage = elementList [0] as FacebookImage;
						}

						if (fbImage != null) {
							int forDisplay = picturesToLoad+1;
							var image = await CommonUtils.DownloadUIImageFromURL(fbImage.Source);
							var amazonUrl = CloudController.UploadToAmazon(image);
							pictures[picturesToLoad].ImageUrl = amazonUrl;
						}
					}

					picturesToLoad++;

					if (picturesToLoad == FacebookElements.Count){

						ContentToImport.AddRange(pictures);
						BTProgressHUD.Show("Importing Videos...");
						FacebookUtils.MakeGraphRequest (FBIdToImportFrom, "videos?fields=source,description,updated_time,thumbnails&limit=3", GetVideos);
						//Test();
					}
				});
			}

			if (FacebookElements.Count == 0) {
				BTProgressHUD.Show("Importing Videos...");
				FacebookUtils.MakeGraphRequest (FBIdToImportFrom, "videos?fields=source,description,updated_time,thumbnails&limit=3", GetVideos);
			}

			// gets 5 videos
		}

		void Test(){
			Console.WriteLine ("stop");
			BTProgressHUD.Dismiss ();
		}

		async void GetVideos(List<FacebookElement> FacebookElements){
			// parses all videos

			ItemLocation = new CGPoint (1700, 130);
			int i = 1;
			foreach (FacebookVideo fbVideo in FacebookElements) {
				
				var video = new Video (fbVideo, ItemLocation, CGAffineTransform.MakeIdentity());
				BTProgressHUD.Show("Importing Videos... " + i + "/" + FacebookElements.Count);
				var byteArray = await CommonUtils.DownloadByteArrayFromURL (fbVideo.Source);
				video.AmazonUrl = CloudController.UploadToAmazon (byteArray);

				ItemLocation.X += 290;
				ContentToImport.Add (video);
				i++;
			}

			UploadContent (); 
		}

		void UploadContent(){
			var json = JsonUtilty.GenerateUpdateJson (ContentToImport);
			CloudController.UpdateBoard (UIBoardInterface.board.Id, json);
			BTProgressHUD.Dismiss();
		}

		private void CreateAnalyticsButton(float yPosition)
		{
			AnalyticsButton = new UIOneLineMenuButton (yPosition);
			AnalyticsButton.SetLabel ("Get Analytics >");

			AnalyticsButton.TouchUpInside += (sender, e) => {
				AnalyticsButton.SetPressedColors();
				AnalyticsScreen analScreen = new AnalyticsScreen();
				Banner.UnsuscribeToEvents ();
				AppDelegate.NavigationController.PushViewController(analScreen, true);
			};
		}


		private void CreateSyncButton(float yPosition)
		{
			SyncButton = new UIOneLineMenuButton (yPosition);

			SyncButton.TouchUpInside += (sender, e) => {
				SyncButton.SetPressedColors();
				PageSelectorScreen pgScreen = new PageSelectorScreen();
				Banner.UnsuscribeToEvents ();
				AppDelegate.NavigationController.PushViewController(pgScreen, true);
			};
		}

		private void LoadBanner()
		{
			Banner = new UIMenuBanner ("SETTINGS", "cross_left");

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					AppDelegate.PopViewControllerLikeDismissView();
					Banner.UnsuscribeToEvents ();
					MemoryUtility.ReleaseUIViewWithChildren (View);
				}
			});

			Banner.AddTap (tap);

			View.AddSubview (Banner);
		}
	}
}

