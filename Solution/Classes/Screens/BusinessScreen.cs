using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Board.Interface;
using Board.JsonResponses;
using Board.Screens.Controls;
using Board.Utilities;
using CoreGraphics;
using Facebook.CoreKit;
using UIKit;

namespace Board.Screens
{
	public class BusinessScreen : UIViewController
	{
		MenuBanner Banner;
		UIImageView sidemenu;
		ProfilePictureView profileView;
		UIScrollView content;
		List<Board.Schema.Board> boardList;
		float thumbSize;
		bool sideMenuIsUp;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.AutomaticallyAdjustsScrollViewInsets = false;

			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;
			content = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			thumbSize = AppDelegate.ScreenWidth / 4;

			LoadSideMenu ();
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

			sidemenu.RemoveFromSuperview ();
			profileView.RemoveFromSuperview ();

			LoadBanner ();
			LoadSideMenu ();
			Banner.SuscribeToEvents ();
		}

		public override void ViewDidDisappear(bool animated)
		{
			Banner.UnsuscribeToEvents ();
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

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (sideMenuIsUp)
				{ sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; return; }
			});

			content.AddGestureRecognizer (tap);
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
			UIImage img = CommonUtils.ResizeImage (board.ImageView.Image, boardIcon.Frame.Size);
			boardImage.Image = img;

			boardIcon.AddSubview (boardImage);

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (sideMenuIsUp)
				{sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; return;}

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

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (sideMenuIsUp)
				{sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; return;}
			});

			content.AddGestureRecognizer (tap);

			View.AddSubview (content);
		}

		private void LoadSideMenu()
		{
			using (UIImage image = UIImage.FromFile ("./screens/business/sidemenu/" + AppDelegate.PhoneVersion + ".png")) {
				sidemenu = new UIImageView(new CGRect(0,0, image.Size.Width / 2, image.Size.Height / 2));
				sidemenu.Image = image;	
			}

			float[] buttonLocations = new float[4];
			if (AppDelegate.PhoneVersion == "6") {
				buttonLocations [0] = 350;
				buttonLocations [1] = 440;
				buttonLocations [2] = 525;
				buttonLocations [3] = 605;
			} else {
				buttonLocations [0] = 390;
				buttonLocations [1] = 470;
				buttonLocations [2] = 550;
				buttonLocations [3] = 630;
			}

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (tg.LocationInView(this.View).Y > buttonLocations[0]-35 && tg.LocationInView(this.View).Y < buttonLocations[0]+35 ){
					NavigationController.PopViewController(false);
				}
				else if (tg.LocationInView(this.View).Y > buttonLocations[1]-35 && tg.LocationInView(this.View).Y < buttonLocations[1]+35){
					SettingsScreen screen = new SettingsScreen();
					NavigationController.PushViewController(screen, false);
				}
				else if (tg.LocationInView(this.View).Y > buttonLocations[2]-35 && tg.LocationInView(this.View).Y < buttonLocations[2]+35){
					SupportScreen screen = new SupportScreen();
					NavigationController.PushViewController(screen, false);
				}
				else if (tg.LocationInView(this.View).Y > buttonLocations[3]-35 && tg.LocationInView(this.View).Y < buttonLocations[3]+35){
					InviteScreen screen = new InviteScreen();
					NavigationController.PushViewController(screen, false);
				}
				HideSideMenu();
			});

			sidemenu.UserInteractionEnabled = true;
			sidemenu.AddGestureRecognizer (tap);
			sidemenu.Alpha = 0f;

			profileView = new ProfilePictureView (new CGRect (11, 78, 149, 149));
			profileView.ProfileId = Profile.CurrentProfile.UserID;
			profileView.Alpha = 0f;

			sideMenuIsUp = false;

			View.AddSubviews (profileView, sidemenu);

			UIFont namefont = AppDelegate.Narwhal20;
			UIFont lastnamefont = AppDelegate.Narwhal24;


			UILabel name = new UILabel (new CGRect(10, profileView.Frame.Bottom + 15, sidemenu.Frame.Width - 20, 20));
			name.Font = namefont;
			name.Text = Profile.CurrentProfile.FirstName;
			name.TextColor = UIColor.White;
			name.TextAlignment = UITextAlignment.Center;
			name.AdjustsFontSizeToFitWidth = true;
			sidemenu.AddSubview (name);

			UILabel lastname = new UILabel (new CGRect(10, name.Frame.Bottom + 3, sidemenu.Frame.Width - 20, 24));
			lastname.Font = lastnamefont;
			lastname.AdjustsFontSizeToFitWidth = true;
			lastname.Text = Profile.CurrentProfile.LastName;
			lastname.TextColor = UIColor.White;
			lastname.TextAlignment = UITextAlignment.Center;
			sidemenu.AddSubview (lastname);
		}

		public void HideSideMenu()
		{
			if (sideMenuIsUp)
			{ sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; }
		}

		private void LoadBanner()
		{
			Banner = new MenuBanner ("./screens/business/banner/" + AppDelegate.PhoneVersion + ".jpg");

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (sideMenuIsUp)
				{sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; return;}

				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					sidemenu.Alpha = 1f;
					profileView.Alpha = 1f;
					sideMenuIsUp = true;
				}
				else if (AppDelegate.ScreenWidth / 4 * 3 < tg.LocationInView(this.View).X){
					CreateScreen1 createScreen1 = new CreateScreen1();
					NavigationController.PushViewController(createScreen1, false);
				}
			});

			Banner.AddTap (tap);

			View.AddSubview (Banner);
		}

	}
}