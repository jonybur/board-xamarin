using System;
using System.Drawing;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;

using System.Net.Http;

using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Facebook.CoreKit;
using Facebook.LoginKit;

using Geolocator.Plugin;
using Google.Maps;

namespace Solution
{
	public partial class MainMenuScreen : UIViewController
	{
		UIImageView banner;
		UIImageView sidemenu;
		UIImageView map_button;
		//UIImageView map;
		bool sideMenuIsUp;
		ProfilePictureView profileView;
		UIScrollView content;

		MapView map;

		public MainMenuScreen () : base ("Board", null){
			
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;

			InitializeInterface ();
		}

		private async void InitializeInterface()
		{
			LoadContent ();
			LoadMap ();
			LoadBanner ();
			LoadMapButton ();
			LoadSideMenu ();
		}


		private List<Board> GenerateBoardList()
		{
			List<Board> boardList = new List<Board> ();
			boardList.Add(new Board ("./logos/mangos.png", UIColor.FromRGB (195, 27, 23), UIColor.FromRGB (0, 167, 73), "South Beach"));
			boardList.Add(new Board ("./logos/clevelander.png", UIColor.FromRGB (0, 158, 217), UIColor.FromRGB (159, 208, 97), "South Beach"));	
			boardList.Add(new Board ("./logos/mansion.jpg", UIColor.FromRGB (35, 32, 35), UIColor.FromRGB (35, 32, 35), "South Beach"));
			boardList.Add(new Board ("./logos/liv.jpg", UIColor.FromRGB (0, 0, 0), UIColor.FromRGB (0, 0, 0), "South Beach"));
			return boardList;
		}


		private UIImageView GenerateBoardThumb(Board board, CGPoint ContentOffset)
		{

			// the image is uploadable
			// so now launch image preview to choose position in the board
			float imgx, imgy, imgw, imgh;
			float autosize = AppDelegate.ScreenWidth / 5;

			float scale = (float)(board.Image.Size.Height/board.Image.Size.Width);

			imgw = autosize;
			imgh = autosize * scale;

			imgx = (float)(ContentOffset.X - imgw / 2);
			imgy = (float)(ContentOffset.Y + BoardInterface.ScreenHeight / 2 - imgh / 2 - Button.ButtonSize / 2);

			// launches the image preview

			CGRect frame = new CGRect (imgx, imgy, imgw, imgh);

			Console.WriteLine (imgw + " " +imgh);
			UIImageView boardIcon = new UIImageView(frame);

			UIImageView uiImageView =  new UIImageView (new CGRect(0,0,frame.Width, frame.Height));

			UIImage thumbImg = board.Image.Scale (new CGSize (imgw, imgh));
			uiImageView.Image = thumbImg;

			boardIcon.AddSubviews(uiImageView);


			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				BoardInterface boardInterface = new BoardInterface(board);
				NavigationController.PushViewController(boardInterface, true);
			});

			boardIcon.AddGestureRecognizer (tap);
			boardIcon.UserInteractionEnabled = true;


			return boardIcon;
		}

		int i = 0;
		private void LoadContent()
		{
			content = new UIScrollView(new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			List<Board> boardList = GenerateBoardList ();

			int i = 1;
			int n = 0;
			foreach (Board b in boardList) {
				UIImageView igv = GenerateBoardThumb (b, new CGPoint (((AppDelegate.ScreenWidth/ 4) + 18 ) * i - 40, 235 + 110 * n));
				i++;
				if (i >= 4) {
					i = 1;
					n++;
				}
				content.AddSubview (igv);
			}

			content.ScrollEnabled = true;
			content.UserInteractionEnabled = true;


			//float contentHeight = (float)(contentImage.Size.Height - (AppDelegate.ScreenHeight));

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (sideMenuIsUp)
				{sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; return;}
			});

			content.AddGestureRecognizer (tap);
			content.UserInteractionEnabled = true;
			content.ContentSize = new CGSize (AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);

			View.AddSubview (content);
		}

		private void LoadSideMenu()
		{
			UIImage bannerImage = UIImage.FromFile ("./mainmenu/sidemenu.png");

			sidemenu = new UIImageView(new CGRect(0,0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
			sidemenu.Image = bannerImage;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				
			});

			sidemenu.UserInteractionEnabled = true;
			sidemenu.AddGestureRecognizer (tap);
			sidemenu.Alpha = 0f;

			profileView = new ProfilePictureView (new CGRect (11, 78, 149, 149));
			profileView.ProfileId = Profile.CurrentProfile.UserID;
			profileView.Alpha = 0f;

			sideMenuIsUp = false;

			View.AddSubviews (profileView, sidemenu);

			UIFont namefont = UIFont.FromName("narwhal-bold", 20);
			UIFont lastnamefont = UIFont.FromName("narwhal-bold", 24);

			UILabel name = new UILabel (new CGRect(0, profileView.Frame.Bottom + 15, sidemenu.Frame.Width, 20));
			name.Font = namefont;
			name.Text = Profile.CurrentProfile.FirstName;
			name.TextColor = UIColor.White;
			name.TextAlignment = UITextAlignment.Center;
			name.AdjustsFontSizeToFitWidth = true;
			sidemenu.AddSubview (name);

			UILabel lastname = new UILabel (new CGRect(0, name.Frame.Bottom + 3, sidemenu.Frame.Width, 24));
			lastname.Font = lastnamefont;
			lastname.AdjustsFontSizeToFitWidth = true;
			lastname.Text = Profile.CurrentProfile.LastName;
			lastname.TextColor = UIColor.White;
			lastname.TextAlignment = UITextAlignment.Center;
			sidemenu.AddSubview (lastname);
		}

		private void LoadBanner()
		{
			UIImage bannerImage = UIImage.FromFile ("./mainmenu/menu_banner.jpg");

			banner = new UIImageView(new CGRect(0,0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
			banner.Image = bannerImage;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (sideMenuIsUp)
				{sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; return;}

				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 3){
					sidemenu.Alpha = 1f;
					profileView.Alpha = 1f;
					sideMenuIsUp = true;
				} else if (tg.LocationInView(this.View).X > (AppDelegate.ScreenWidth / 3) * 2){
					CreateScreen1 createScreen1 = new CreateScreen1();
					NavigationController.PushViewController(createScreen1, false);	
				}
			});

			banner.UserInteractionEnabled = true;
			banner.AddGestureRecognizer (tap);
			banner.Alpha = .95f;
			View.AddSubview (banner);
		}


		private async System.Threading.Tasks.Task LoadLocation()
		{
			Console.WriteLine ("started");

			var locator = CrossGeolocator.Current;
			locator.DesiredAccuracy = 50;
			var position = await locator.GetPositionAsync (timeoutMilliseconds: 10000);

			Console.WriteLine ("position is " + position.Latitude.ToString() + " long " + position.Longitude.ToString() );

			// Create a GMSCameraPosition that tells the map to display the
			// coordinate 37.79,-122.40 at zoom level 6.
			var camera = CameraPosition.FromCamera (latitude: position.Latitude, 
				longitude: position.Longitude, 
				zoom: 12);

			map.Camera = camera;
		}

		private async void LoadMap()
		{
			var camera = CameraPosition.FromCamera (latitude: 40, 
				longitude: -100, 
				zoom: -2);
			
			map = MapView.FromCamera (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight), camera);
			map.MyLocationEnabled = true;
			map.Alpha = 0f;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (sideMenuIsUp)
				{sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; return;}
			});

			map.UserInteractionEnabled = true;
			map.AddGestureRecognizer (tap);

			View.AddSubview (map);

			await LoadLocation ();
		}

		private void LoadMapButton()
		{
			UIImage mapImage = UIImage.FromFile ("./mainmenu/mapbutton.jpg");
			UIImage listImage = UIImage.FromFile ("./mainmenu/listbutton.jpg");

			map_button = new UIImageView(new CGRect(0,AppDelegate.ScreenHeight - (mapImage.Size.Height / 2),
				mapImage.Size.Width / 2, mapImage.Size.Height / 2));
			map_button.Image = mapImage;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (sideMenuIsUp)
				{sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; return;}

				if (map.Alpha == 0f)
				{ map.Alpha = 1f; map_button.Image = listImage; } else { map.Alpha = 0f; map_button.Image = mapImage; } 
			});

			map_button.UserInteractionEnabled = true;
			map_button.AddGestureRecognizer (tap);
			map_button.Alpha = .95f;
			View.AddSubview (map_button);
		}


	}
}