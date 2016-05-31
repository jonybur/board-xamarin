using System.Collections.Generic;
using BigTed;
using Board.Infrastructure;
using Board.Screens.Controls;
using Board.Utilities;
using CoreGraphics;
using UIKit;

namespace Board.Screens
{
	public class BusinessScreen : UIViewController
	{
		UIMenuBanner Banner;
		UIScrollView ScrollView;
		List<Board.Schema.Board> boardList;
		UIThumbsContentDisplay ThumbsScreen;

		public override void ViewDidLoad ()
		{
			BTProgressHUD.Show ();

			boardList = new List<Board.Schema.Board> ();
			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			ScrollView.BackgroundColor = UIColor.White;

			View.AddSubview (ScrollView);

			LoadBanner ();
		}

		public override void ViewDidAppear(bool animated)
		{
			BTProgressHUD.Show ();

			Banner.SuscribeToEvents ();

			boardList = CloudController.GetUserBoards ();

			InitializeInterface ();

			BTProgressHUD.Dismiss ();
		}

		public override void ViewDidDisappear(bool animated)
		{
			Banner.UnsuscribeToEvents ();

			if (ThumbsScreen != null) {
				ThumbsScreen.UnsuscribeToEvents ();
			}

			MemoryUtility.ReleaseUIViewWithChildren (View);
		}

		private void InitializeInterface()
		{
			foreach (UIView v in ScrollView.Subviews) {
				v.RemoveFromSuperview ();
			}

			if (boardList.Count == 0) {
				//LoadNoContent ();
			} else {
				LoadContent ();
			}
		}

		private void LoadContent()
		{
			ThumbsScreen = new UIThumbsContentDisplay (boardList, UIThumbsContentDisplay.OrderMode.Neighborhood);
			ScrollView.AddSubview (ThumbsScreen);
			ScrollView.ContentSize = new CGSize (AppDelegate.ScreenWidth, ThumbsScreen.Frame.Bottom);

			ScrollView.ScrollEnabled = true;
			ScrollView.UserInteractionEnabled = true;
		}

		private void LoadNoContent()
		{
			// TODO: add empty image
			// si el usuario no tiene boards creados...
			ScrollView = new UIScrollView(new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));

			var imgv = new UIImageView (ScrollView.Frame);

			ScrollView.AddSubview (imgv);
			ScrollView.ScrollEnabled = true;
			ScrollView.UserInteractionEnabled = true;
		
			View.AddSubview (ScrollView);
			View.SendSubviewToBack (ScrollView);
		}

		private void LoadBanner()
		{
			Banner = new UIMenuBanner ("BUSINESS", "menu_left", "plus_right");

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					AppDelegate.containerScreen.BringSideMenuUp("business");
				}
				else if (AppDelegate.ScreenWidth / 4 * 3 < tg.LocationInView(this.View).X){

					UIAlertController alert = UIAlertController.Create("Facebook Page Importer", null, UIAlertControllerStyle.Alert);
					alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));
					alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, delegate {
						if (alert.TextFields.Length == 0){
							return;
						}

						var textField = alert.TextFields[0];

						if (textField.Text == string.Empty || textField.Text == null){ 
							return;
						}

						Board.Facebook.FacebookAutoImporter.ImportPage(textField.Text);
					}));

					alert.AddTextField(delegate(UITextField obj) {
						obj.Placeholder = "Facebook Page ID";
					});

					NavigationController.PresentViewController(alert, true, null);
				}
			});

			Banner.AddTap (tap);

			View.AddSubview (Banner);
		}
	}
}