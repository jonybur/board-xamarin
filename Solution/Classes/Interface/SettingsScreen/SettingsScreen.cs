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

			AnalyticsButton.SetUnpressedColors ();
			SyncButton.SetUnpressedColors ();
			ColorPicker.SetUnpressedColors ();
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
				AppDelegate.NavigationController.PresentViewController (alert, true, null);
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

					if (string.IsNullOrEmpty (textField.Text)) { 
						return;
					}

					ImportButton.SetUnpressedColors();

					// gets 5 announcements
					BTProgressHUD.Show("Importing Announcements...");

					FacebookAutoImporter.ImportPageContent(textField.Text);
					// - updates board -
					ImportButton.SetUnpressedColors();
				}));
				alert.AddTextField (obj => obj.Placeholder = "Facebook Page ID");

				AppDelegate.NavigationController.PresentViewController (alert, true, null);

			};
		}

		private void CreateColorPicker(float yPosition)
		{
			ColorPicker = new UIOneLineMenuButton (yPosition);
			ColorPicker.SetLabel ("Change Background Color >");

			ColorPicker.TouchUpInside += (sender, e) => {
				ColorPicker.SetPressedColors();
				var colorScreen = new ColorPickerScreen();
				Banner.UnsuscribeToEvents ();
				AppDelegate.NavigationController.PushViewController(colorScreen, true);
			};
		}

		private void CreateAnalyticsButton(float yPosition)
		{
			AnalyticsButton = new UIOneLineMenuButton (yPosition);
			AnalyticsButton.SetLabel ("Get Analytics >");

			AnalyticsButton.TouchUpInside += (sender, e) => {
				AnalyticsButton.SetPressedColors();
				var analScreen = new AnalyticsScreen();
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

