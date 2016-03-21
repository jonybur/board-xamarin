using System;
using System.Collections.Generic;
using System.Linq;
using Board.Interface;
using MGImageUtilitiesBinding;
using Board.JsonResponses;
using Board.Screens.Controls;
using Board.Utilities;
using CoreGraphics;
using UIKit;

namespace Board.Screens
{
	public class BusinessScreen : UIViewController
	{
		MenuBanner Banner;
		UIScrollView content;
		List<Board.Schema.Board> boardList;
		float thumbSize;

		public override void ViewDidLoad ()
		{
			//base.ViewDidLoad ();
			this.AutomaticallyAdjustsScrollViewInsets = false;

			//NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			//NavigationController.NavigationBarHidden = true;
			content = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			thumbSize = AppDelegate.ScreenWidth / 4;
		}

		public override async void ViewDidAppear(bool animated)
		{
			boardList = new List<Board.Schema.Board> ();

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

			InitializeInterface ();

			LoadBanner ();
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
			} else {
				LoadContent ();
			}
		}

		private void LoadContent()
		{
			boardList = boardList.OrderBy(o=>o.Location).ToList();

			int i = 1;

			string location = String.Empty;

			// starting point
			float yposition = 25;

			foreach (Board.Schema.Board b in boardList) {
				string hood = b.GeolocatorObject.results [0].address_components [2].long_name;
				if (location != hood) {
					// draw new location string
					yposition += 70;
					UILabel lblLocation = new UILabel(new CGRect(30, yposition, AppDelegate.ScreenWidth - 40, 24));
					yposition += (float)lblLocation.Frame.Height + thumbSize / 2 + 10;
					lblLocation.Font = AppDelegate.Narwhal20;
					lblLocation.TextColor = UIColor.FromRGB (241, 93, 74);
					location = hood;
					lblLocation.Text = location.ToUpper();
					lblLocation.AdjustsFontSizeToFitWidth = true;
					content.AddSubview (lblLocation);
					i = 1;
				}

				UIImageView igv = GenerateBoardThumb (b, new CGPoint ((AppDelegate.ScreenWidth/ 4) * i, yposition));
				i++;
				if (i >= 4) {
					i = 1;
					yposition += thumbSize + 6;
				}
				content.AddSubview (igv);
			}

			content.ScrollEnabled = true;
			content.UserInteractionEnabled = true;

			content.UserInteractionEnabled = true;
			content.ContentSize = new CGSize (AppDelegate.ScreenWidth, yposition + thumbSize);

			View.AddSubview (content);
		}

		private UIImageView GenerateBoardThumb(Board.Schema.Board board, CGPoint ContentOffset)
		{
			float imgx, imgy, imgw, imgh;

			float autosize = thumbSize;
			float scale = (float)(board.ImageView.Frame.Height/board.ImageView.Frame.Width);

			if (scale > 1) {
				scale = (float)(board.ImageView.Frame.Width/board.ImageView.Frame.Height);
				imgh = autosize;
				imgw = autosize * scale;
			}
			else {
				imgw = autosize;
				imgh = autosize * scale;	
			}

			imgx = (float)(ContentOffset.X);

			if (imgx < AppDelegate.ScreenWidth / 2) {
				imgx -= autosize / 4;
			} else if (AppDelegate.ScreenWidth / 2 < imgx) {
				imgx += autosize / 4;
			}

			imgy = (float)(ContentOffset.Y);

			// launches the image preview
			UIImageView boardIcon = new UIImageView (new CGRect (0, 0, autosize, autosize));
			boardIcon.Center = new CGPoint (imgx, imgy);
			//boardIcon.BackgroundColor = UIColor.Black;

			UIImageView boardImage = new UIImageView(new CGRect (0, 0, imgw* .8f, imgh* .8f));
			boardImage.Center = new CGPoint (autosize/2, autosize/2);
			UIImage img = board.ImageView.Image.ImageScaledToFitSize(boardIcon.Frame.Size);
			boardImage.Image = img;

			boardIcon.AddSubview (boardImage);

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				BoardInterface boardInterface = new BoardInterface(board, false);
				NavigationController.PushViewController(boardInterface, true);
			});

			boardIcon.AddGestureRecognizer (tap);
			boardIcon.UserInteractionEnabled = true;

			return boardIcon;
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