using System.Collections.Generic;
using System.Linq;
using Board.Screens.Controls;
using BigTed;
using Board.Utilities;
using Board.Infrastructure;
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

		public override async void ViewDidAppear(bool animated)
		{
			BTProgressHUD.Show ();

			boardList = await CloudController.GetUserBoards ();

			InitializeInterface ();

			Banner.SuscribeToEvents ();

			ThumbsScreen.SuscribeToEvents ();

			BTProgressHUD.Dismiss ();
		}

		public override void ViewDidDisappear(bool animated)
		{
			Banner.UnsuscribeToEvents ();

			ThumbsScreen.UnsuscribeToEvents ();

			MemoryUtility.ReleaseUIViewWithChildren (View);
		}

		private void InitializeInterface()
		{
			foreach (UIView v in ScrollView.Subviews) {
				v.RemoveFromSuperview ();
			}

			if (boardList.Count == 0) {
				LoadNoContent ();
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
			// si el usuario no tiene boards creados...
			ScrollView = new UIScrollView(new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));

			var imgv = new UIImageView (ScrollView.Frame);

			using (UIImage image = UIImage.FromFile ("./screens/business/empty/" + AppDelegate.PhoneVersion + ".jpg")) {
				imgv.Image = image;
			}

			ScrollView.AddSubview (imgv);
			ScrollView.ScrollEnabled = true;
			ScrollView.UserInteractionEnabled = true;
		
			View.AddSubview (ScrollView);
		}

		private void LoadBanner()
		{
			Banner = new UIMenuBanner ("./screens/business/banner/" + AppDelegate.PhoneVersion + ".jpg");

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					AppDelegate.containerScreen.BringSideMenuUp("business");
				}
				else if (AppDelegate.ScreenWidth / 4 * 3 < tg.LocationInView(this.View).X){
					CreateScreen1 createScreen1 = new CreateScreen1();
					AppDelegate.NavigationController.PushViewController(createScreen1, false);
				}
			});

			Banner.AddTap (tap);

			View.AddSubview (Banner);
		}

	}
}