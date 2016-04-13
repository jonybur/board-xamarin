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
		MenuBanner Banner;
		UIScrollView content;
		List<Board.Schema.Board> boardList;
		List<BoardThumb> ListThumbs;

		float yposition;
		float thumbsize;

		public override void ViewDidLoad ()
		{
			AutomaticallyAdjustsScrollViewInsets = false;

			BTProgressHUD.Show ();

			boardList = new List<Board.Schema.Board> ();
			content = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			content.BackgroundColor = UIColor.White;

			View.AddSubview (content);

			LoadBanner ();

			ListThumbs = new List<BoardThumb> ();
		}

		public override async void ViewDidAppear(bool animated)
		{
			BTProgressHUD.Show ();

			boardList = await CloudController.GetUserBoards ();

			InitializeInterface ();

			Banner.SuscribeToEvents ();

			foreach (var thumb in ListThumbs) {
				thumb.SuscribeToEvent ();
			}

			BTProgressHUD.Dismiss ();
		}

		public override void ViewDidDisappear(bool animated)
		{
			Banner.UnsuscribeToEvents ();

			foreach (var thumb in ListThumbs) {
				thumb.UnsuscribeToEvent ();
			}

			MemoryUtility.ReleaseUIViewWithChildren (View);
		}

		private void InitializeInterface()
		{
			foreach (UIView v in content.Subviews) {
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
			thumbsize = AppDelegate.ScreenWidth / 3.5f;

			boardList = boardList.OrderBy(o=>o.Neighborhood).ToList();

			string location = string.Empty;

			LocationLabel.font = AppDelegate.Narwhal20;

			// starting point
			int linecounter = 1, neighborhoodnumber = 0, i = 0;
			yposition = (float)Banner.Frame.Bottom + 20;

			foreach (Board.Schema.Board b in boardList) {
				if (location != b.Neighborhood) {

					if (neighborhoodnumber > 0) {
						yposition += thumbsize / 2 + 10;
					}

					// draw new location string
					LocationLabel locationLabel = new LocationLabel (yposition, b.Neighborhood);
					yposition += (float)locationLabel.Frame.Height + thumbsize / 2 + 10;
					location = b.Neighborhood;
					content.AddSubview (locationLabel);

					linecounter = 1;
					neighborhoodnumber++;
				}

				var boardThumb = new BoardThumb (b, new CGPoint ((AppDelegate.ScreenWidth/ 4) * linecounter, yposition), thumbsize);
				linecounter++;
				if (linecounter >= 4) {
					linecounter = 1;
					// nueva linea de thumbs
					yposition += thumbsize+ 10;
				}

				ListThumbs.Add (boardThumb);
				content.AddSubview (boardThumb);
				i++;
			}

			content.ScrollEnabled = true;
			content.UserInteractionEnabled = true;

			content.ContentSize = new CGSize (AppDelegate.ScreenWidth, yposition + thumbsize * 2 / 3);
		}

		private void LoadNoContent()
		{
			// si el usuario no tiene boards creados...
			content = new UIScrollView(new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));

			UIImageView imgv = new UIImageView (new CGRect(0,0,AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));

			using (UIImage image = UIImage.FromFile ("./screens/business/empty/" + AppDelegate.PhoneVersion + ".jpg")) {
				imgv.Image = image;
			}

			content.AddSubview (imgv);
			content.ScrollEnabled = true;
			content.UserInteractionEnabled = true;
		
			View.AddSubview (content);
		}

		private void LoadBanner()
		{
			Banner = new MenuBanner ("./screens/business/banner/" + AppDelegate.PhoneVersion + ".jpg");

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