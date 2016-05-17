using System.Collections.Generic;
using BigTed;
using Board.Facebook;
using Board.Infrastructure;
using Board.Schema;
using Board.Screens;
using Board.Screens.Controls;
using Board.Utilities;
using System.Linq;
using CoreGraphics;
using UIKit;

namespace Board.Interface
{
	public class SettingsScreen : UIViewController
	{
		UIMenuBanner Banner;
		UIOneLineMenuButton SyncButton, AnalyticsButton, DeleteButton, ImportButton;

		List<Content> ContentToImport;
		CGPoint ItemLocation = new CGPoint(0,120);

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

				UIAlertController alert = UIAlertController.Create("Continue?", "This will fill the Board with content from the linked Facebook page.", UIAlertControllerStyle.Alert);
				alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, delegate {
					ImportButton.SetUnpressedColors();
				}));
				alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, delegate {
					ImportButton.SetUnpressedColors();

					ContentToImport = new List<Content>();

					// gets 5 announcements
					BTProgressHUD.Show("Downloading Announcements...");
					FacebookUtils.MakeGraphRequest(UIBoardInterface.board.FBPage.Id, "?fields=posts.limit(5)", GetAnnouncements);

					// - updates board -

				}));
				NavigationController.PresentViewController (alert, true, null);

			};
		}

		void GetAnnouncements(List<FacebookElement> FacebookElements){
			// parses all posts
			foreach (var fbelement in FacebookElements) {
				FacebookPost fbpost = (FacebookPost)fbelement;

				if (fbpost.Message != "<null>" && fbpost.Message != null) {
					ItemLocation.X += 290;
					var announcement = new Announcement (fbpost, ItemLocation, 0);

					ContentToImport.Add (announcement);
				} else {
					continue;
				}
			}

			// gets 5 events
			BTProgressHUD.Show("Downloading Events...");
			FacebookUtils.MakeGraphRequest(UIBoardInterface.board.FBPage.Id, "?fields=events.limit(5)", GetEvents);
		}

		void GetEvents(List<FacebookElement> FacebookElements){
			// parses all events
			int coversToLoad = 0;
			ItemLocation = new CGPoint (0, 300);

			foreach (var fbelement in FacebookElements) {
				var fbevent = (FacebookEvent)fbelement;
				ItemLocation.X += 290;
				var boardEvent = new BoardEvent (fbevent, ItemLocation, 0);

				FacebookUtils.MakeGraphRequest (fbevent.Id, "?fields=cover", delegate(List<FacebookElement> elementList) {
					if (elementList.Count > 0) {
						var cover = elementList [0] as FacebookCover;
						if (cover != null) {
							boardEvent.ImageUrl = cover.Source;
						}
					}

					ContentToImport.Add (boardEvent);
					coversToLoad++;

					if (coversToLoad == FacebookElements.Count){
						// gets 5 pictures
						// TODO estamos acá
						// to get the pictures first I need to get an album id
						//FacebookUtils.MakeGraphRequest(UIBoardInterface.board.FBPage.Id, "?fields=events.limit(5)", GetPictures);

						UploadContent();
					}
				});
			}
					

		}
			
		void UploadContent(){
			var json = JsonUtilty.GenerateUpdateJson (ContentToImport);
			CloudController.UpdateBoard (UIBoardInterface.board.Id, json);
			BTProgressHUD.Dismiss();
		}

		void GetPictures(List<FacebookElement> FacebookElements){
			// parses all pictures

			// gets 5 videos
		}

		void GetVideos(List<FacebookElement> FacebookElements){
			// parses all videos

			var json = JsonUtilty.GenerateUpdateJson (ContentToImport);
			//CloudController.UpdateBoard (UIBoardInterface.board.Id, json);
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
					AppDelegate.PopViewLikeDismissView();
					Banner.UnsuscribeToEvents ();
					MemoryUtility.ReleaseUIViewWithChildren (View);
				}
			});

			Banner.AddTap (tap);

			View.AddSubview (Banner);
		}
	}
}

