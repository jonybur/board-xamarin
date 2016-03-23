using System.Collections.Generic;
using Board.JsonResponses;
using Board.Screens.Controls;
using Board.Utilities;
using System;
using CoreGraphics;
using UIKit;

namespace Board.Screens
{
	public class BusinessScreen : UIViewController
	{
		MenuBanner Banner;
		UIScrollView content;
		List<Board.Schema.Board> boardList;

		public override void ViewDidLoad ()
		{
			this.AutomaticallyAdjustsScrollViewInsets = false;

			boardList = new List<Board.Schema.Board> ();
			content = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
		
			InitializeInterface ();
			LoadBanner ();
		}

		public override async void ViewDidAppear(bool animated)
		{
			if (AppDelegate.ServerActive) {
				string result = CommonUtils.JsonGETRequest ("http://192.168.1.101:5000/api/user/boards?authToken=" + AppDelegate.EncodedBoardToken);

				BoardResponse response = BoardResponse.Deserialize (result);

				if (response != null) {
					foreach (BoardResponse.Datum r in response.data) {
						// gets image from url
						UIImage boardImage = await CommonUtils.DownloadUIImageFromURL (r.logoURL);

						// gets address
						string jsonobj = JsonHandler.GET ("https://maps.googleapis.com/maps/api/geocode/json?address=" + r.address + "&key=" + AppDelegate.GoogleMapsAPIKey);
						GoogleGeolocatorObject geolocatorObject = JsonHandler.DeserializeObject (jsonobj);

						// compiles the board, adds the geolocator object for further reference
						Board.Schema.Board board = new Board.Schema.Board (r.name, new UIImageView(boardImage), CommonUtils.HexToUIColor (r.mainColorCode), CommonUtils.HexToUIColor (r.secondaryColorCode), r.address, null);
						board.GeolocatorObject = geolocatorObject;

						boardList.Add (board);
					}
				}
			}

			Banner.SuscribeToEvents ();
		}

		public override void ViewDidDisappear(bool animated)
		{
			Banner.UnsuscribeToEvents ();
			MemoryUtility.ReleaseUIViewWithChildren (View, true);
		}

		private void InitializeInterface()
		{
			foreach (UIView v in content.Subviews) {
				v.RemoveFromSuperview ();
			}

			if (boardList.Count == 0) {
				LoadNoContent ();
			}
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

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
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