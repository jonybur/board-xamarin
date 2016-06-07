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
using UIKit;

namespace Board.Interface
{
	public class SettingsScreen : UIViewController
	{
		UIMenuBanner Banner;
		UIOneLineMenuButton SyncButton, AnalyticsButton, DeleteButton, ImportButton, ColorPicker;

		public override void ViewDidLoad ()
		{
			LoadBanner ();

			CreateAnalyticsButton ((float)Banner.Frame.Bottom);
			CreateColorPicker ((float)AnalyticsButton.Frame.Bottom + 1);
			CreateSyncButton ((float)ColorPicker.Frame.Bottom + 1);
			CreateImportButton ((float)SyncButton.Frame.Bottom + 1);
			CreateDeleteButton ((float)ImportButton.Frame.Bottom + 1);

			View.AddSubviews (SyncButton, AnalyticsButton, ColorPicker, ImportButton, DeleteButton);

			View.BackgroundColor = UIColor.White;
		}

		public override void ViewDidAppear(bool animated)
		{
			if (UIBoardInterface.board.FacebookId == null) {
				SyncButton.SetLabel("Connect to Facebook Page >");
			} else {
				SyncButton.SetLabel(string.Format ("Connected to {0}", UIBoardInterface.board.FacebookId));
			}

			Banner.SuscribeToEvents ();
		}

		private void CreateDeleteButton(float yPosition)
		{
			DeleteButton = new UIOneLineMenuButton (yPosition);
			DeleteButton.SetLabel ("Delete Board");

			DeleteButton.SetTapEvent (delegate {

				UIAlertController alert = UIAlertController.Create("Warning!", "This will delete the entire Board.\nYou CANNOT undo this action.", UIAlertControllerStyle.Alert);
				alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, delegate {
				}));
				alert.AddAction (UIAlertAction.Create ("Delete", UIAlertActionStyle.Destructive, delegate {
					CloudController.DeleteBoard(UIBoardInterface.board.Id);


					var containerScreen = AppDelegate.NavigationController.ViewControllers[AppDelegate.NavigationController.ViewControllers.Length - 3] as ContainerScreen;
					if (containerScreen!= null)
					{
						containerScreen.LoadMainMenu();
					}
					AppDelegate.PopToViewControllerWithCallback (containerScreen, AppDelegate.ExitBoardInterface);
				}));
				AppDelegate.NavigationController.PresentViewController (alert, true, null);
			});
			DeleteButton.SuscribeToEvent ();
		}

		private void CreateImportButton(float yPosition)
		{
			ImportButton = new UIOneLineMenuButton (yPosition);
			ImportButton.SetLabel ("Import content from Facebook");

			ImportButton.SetTapEvent (delegate {

				UIAlertController alert = UIAlertController.Create("Continue?", "This will fill the Board with content from a Facebook page.\nThis process might take a few minutes.", UIAlertControllerStyle.Alert);
				alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, delegate {
				}));
				alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, delegate {
					FacebookAutoImporter.ImportPageContent(UIBoardInterface.board.FacebookId);
				}));

				AppDelegate.NavigationController.PresentViewController (alert, true, null);

			});
			ImportButton.SuscribeToEvent ();
		}

		private void CreateColorPicker(float yPosition)
		{
			ColorPicker = new UIOneLineMenuButton (yPosition);
			ColorPicker.SetLabel ("Change Background Color >");

			ColorPicker.SetTapEvent (delegate {
				var colorScreen = new ColorPickerScreen();
				Banner.UnsuscribeToEvents ();
				AppDelegate.NavigationController.PushViewController(colorScreen, true);
			});
			ColorPicker.SuscribeToEvent ();
		}

		private void CreateAnalyticsButton(float yPosition)
		{
			AnalyticsButton = new UIOneLineMenuButton (yPosition);
			AnalyticsButton.SetLabel ("Get Analytics >");

			AnalyticsButton.SetTapEvent (delegate {
				var analScreen = new AnalyticsScreen();
				Banner.UnsuscribeToEvents ();
				AppDelegate.NavigationController.PushViewController(analScreen, true);
			});
			AnalyticsButton.SuscribeToEvent ();
		}


		private void CreateSyncButton(float yPosition)
		{
			SyncButton = new UIOneLineMenuButton (yPosition);

			SyncButton.SetTapEvent (delegate {
				PageSelectorScreen pgScreen = new PageSelectorScreen();
				Banner.UnsuscribeToEvents ();
				AppDelegate.NavigationController.PushViewController(pgScreen, true);
			});
			SyncButton.SuscribeToEvent ();
		}

		private void LoadBanner()
		{
			Banner = new UIMenuBanner ("SETTINGS", "cross_left");

			var tap = new UITapGestureRecognizer (tg => {
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

