using UIKit;
using Board.Utilities;
using Board.Screens.Controls;
using Board.Infrastructure;
using Board.Screens;

namespace Board.Interface
{
	public class SettingsScreen : UIViewController
	{
		UIMenuBanner Banner;
		UIOneLineMenuButton SyncButton, AnalyticsButton, DeleteButton, ImportButton;

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

					// gets 5 announcements
					// gets 5 events
					// gets 5 pictures
					// gets 5 videos

					// - updates board -
				}));
				NavigationController.PresentViewController (alert, true, null);

			};
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

