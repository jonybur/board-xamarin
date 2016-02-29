using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Facebook.CoreKit;
using Foundation;
using Google.Maps;
using UIKit;
using Board.Screens.Menu;

namespace Board.Screens
{
	public class MainMenuScreen : UIViewController
	{
		UIImageView banner;
		UIImageView sidemenu;
		UIButton map_button;

		List<BoardThumb> ListThumbs;

		ProfilePictureView profileView;

		UIScrollView content;

		EventHandler MapButtonEvent;
		UITapGestureRecognizer SideMenuTap;

		MapView map;

		bool sideMenuIsUp;
		bool firstLocationUpdate;

		public override void DidReceiveMemoryWarning ()
		{
			GC.Collect ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;

			ListThumbs = new List<BoardThumb> ();

			InitializeInterface ();
		}

		public override void ViewDidAppear(bool animated)
		{
			// suscribe to observers, gesture recgonizers, events
			map.AddObserver (this, new NSString ("myLocation"), NSKeyValueObservingOptions.New, IntPtr.Zero);
			map_button.TouchUpInside += MapButtonEvent;
			sidemenu.AddGestureRecognizer (SideMenuTap);
			foreach (BoardThumb bt in ListThumbs) {
				bt.SuscribeToEvent ();
			}
		}

		public override void ViewDidDisappear(bool animated)
		{
			// unsuscribe from observers, gesture recgonizers, events
			map.RemoveObserver (this, new NSString ("myLocation"));
			map_button.TouchUpInside -= MapButtonEvent;
			sidemenu.RemoveGestureRecognizer (SideMenuTap);
			foreach (BoardThumb bt in ListThumbs) {
				bt.UnsuscribeToEvent ();
			}
		}

		private void InitializeInterface()
		{
			LoadContent ();
			LoadBanner ();
			LoadMapButton ();
			LoadMap ();
			LoadSideMenu ();
		}

		private static List<Board.Schema.Board> GenerateBoardList()
		{
			List<Board.Schema.Board> boardList = new List<Board.Schema.Board> ();

			using (UIImage img = UIImage.FromFile("./logos/americansocial.jpeg")){
				boardList.Add(new Board.Schema.Board ("American Social", img, UIColor.FromRGB (67, 15, 0), UIColor.FromRGB (221, 169, 91), "Brickell", Profile.CurrentProfile.UserID));
			}
			using (UIImage img = UIImage.FromFile("./logos/doghouse.jpeg")){
				boardList.Add(new Board.Schema.Board ("Dog House", img, UIColor.FromRGB (35, 32, 35), UIColor.FromRGB (220, 31, 24), "Brickell", string.Empty));
			}
			using (UIImage img = UIImage.FromFile("./logos/doloreslolita.jpg")){
				boardList.Add(new Board.Schema.Board ("Dolores Lolita", img, UIColor.FromRGB (185, 143, 6), UIColor.FromRGB (2, 0, 6), "Brickell", string.Empty));	
			}
			using (UIImage img = UIImage.FromFile ("./logos/bluemartini.png")) {
				boardList.Add (new Board.Schema.Board ("Blue Martini", img, UIColor.FromRGB (0, 0, 0), UIColor.FromRGB (0, 165, 216), "Brickell", string.Empty));
			}
			using (UIImage img = UIImage.FromFile ("./logos/tavernopa.png")) {
				boardList.Add (new Board.Schema.Board ("Taverna Opa", img, UIColor.FromRGB (140, 52, 50), UIColor.FromRGB (77, 185, 155), "Brickell", string.Empty));	
			}
			using (UIImage img = UIImage.FromFile ("./logos/clevelander.png")) {
				boardList.Add (new Board.Schema.Board ("Clevelander", img, UIColor.FromRGB (0, 158, 216), UIColor.FromRGB (158, 208, 96), "South Beach", string.Empty));
			}
			using (UIImage img = UIImage.FromFile ("./logos/fattuesdays.jpg")) {
				boardList.Add (new Board.Schema.Board ("Fat Tuesdays", img, UIColor.FromRGB (52, 59, 155), UIColor.FromRGB (201, 30, 67), "South Beach", string.Empty));	
			}
			using (UIImage img = UIImage.FromFile ("./logos/liv.jpg")) {
				boardList.Add (new Board.Schema.Board ("LIV", img, UIColor.Black, UIColor.FromRGB (20, 20, 20), "South Beach", string.Empty));
			}
			using (UIImage img = UIImage.FromFile ("./logos/mangos.png")) {
				boardList.Add (new Board.Schema.Board ("Mangos", img, UIColor.FromRGB (240, 35, 0), UIColor.FromRGB (0, 168, 67), "South Beach", string.Empty));
			}
			using (UIImage img = UIImage.FromFile ("./logos/coyo.jpeg")) {
				boardList.Add (new Board.Schema.Board ("Coyo", img, UIColor.FromRGB (33, 58, 171), UIColor.FromRGB (100, 215, 223), "Wynwood", string.Empty));	
			}
			using (UIImage img = UIImage.FromFile ("./logos/mansion.jpg")) {
				boardList.Add (new Board.Schema.Board ("Mansion", img, UIColor.Black, UIColor.FromRGB (20, 20, 20), "South Beach", string.Empty));
			}
			using (UIImage img = UIImage.FromFile ("./logos/nikki.jpg")) {
				boardList.Add (new Board.Schema.Board ("Nikki Beach", img, UIColor.FromRGB (1, 73, 159), UIColor.White, "South Beach", string.Empty));
			}
			using (UIImage img = UIImage.FromFile ("./logos/wetwillies.jpg")) {
				boardList.Add (new Board.Schema.Board ("Wet Willies", img, UIColor.Black, UIColor.White, "South Beach", string.Empty));	
			}
			using (UIImage img = UIImage.FromFile ("./logos/brickhouse.png")) {
				boardList.Add (new Board.Schema.Board ("Brickhouse", img, UIColor.FromRGB (35, 30, 32), UIColor.Black, "Wynwood", string.Empty));	
			}
			using (UIImage img = UIImage.FromFile ("./logos/panther.JPG")) {
				boardList.Add (new Board.Schema.Board ("Panther Coffee", img, UIColor.Black, UIColor.FromRGB (20, 20, 20), "Wynwood", string.Empty));	
			}
			using (UIImage img = UIImage.FromFile("./logos/wood.jpg")){
				boardList.Add (new Board.Schema.Board ("Wood Tavern", img, UIColor.Black, UIColor.FromRGB (20, 20, 20), "Wynwood", string.Empty));
			}
			using (UIImage img = UIImage.FromFile ("./logos/electricpickle.jpg")) {
				boardList.Add (new Board.Schema.Board ("Electric Pickle", img, UIColor.Black, UIColor.FromRGB (20, 20, 20), "Wynwood", string.Empty));	
			}

			return boardList;
		}  


		private void LoadContent()
		{
			BoardThumb.Size = AppDelegate.ScreenWidth / 4;

			content = new UIScrollView(new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			content.BackgroundColor = UIColor.White;

			List<Board.Schema.Board> boardList = GenerateBoardList ();
			boardList = boardList.OrderBy(o=>o.Location).ToList();

			int i = 1;

			string location = String.Empty;

			// starting point
			float yposition = 5;

			foreach (Board.Schema.Board b in boardList) {
				if (location != b.Location) {
					// draw new location string
					yposition += 70;
					UILabel lblLocation = new UILabel(new CGRect(30, yposition, AppDelegate.ScreenWidth - 40, 24));
					yposition += (float)lblLocation.Frame.Height + BoardThumb.Size / 2+ 10;
					lblLocation.Font = UIFont.FromName ("narwhal-bold", 20);
					lblLocation.TextColor = UIColor.FromRGB (241, 93, 74);
					location = b.Location;
					lblLocation.Text = location.ToUpper();
					content.AddSubview (lblLocation);
					i = 1;
				}
				 
				BoardThumb boardThumb = new BoardThumb (b, new CGPoint ((AppDelegate.ScreenWidth/ 4) * i, yposition));
				ListThumbs.Add (boardThumb);
				i++;
				if (i >= 4) {
					i = 1;
					yposition += BoardThumb.Size + 6;
				}
				content.AddSubview (boardThumb);
			}

			content.ScrollEnabled = true;
			content.UserInteractionEnabled = true;

			content.ContentSize = new CGSize (AppDelegate.ScreenWidth, yposition + BoardThumb.Size + 25);

			View.AddSubview (content);
		}

		public void HideSideMenu()
		{
			if (sideMenuIsUp)
			{ sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; }
		}

		private void LoadSideMenu()
		{
			UIImage bannerImage = UIImage.FromFile ("./screens/home/sidemenu/" + AppDelegate.PhoneVersion + ".png");

			sidemenu = new UIImageView(new CGRect(0,0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
			sidemenu.Image = bannerImage;

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

			SideMenuTap = new UITapGestureRecognizer ((tg) => {
				if (tg.LocationInView(this.View).Y > buttonLocations[0]-35 && tg.LocationInView(this.View).Y < buttonLocations[0]+35 ){
					BusinessScreen screen = new BusinessScreen();
					NavigationController.PushViewController(screen, false);
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
			sidemenu.Alpha = 0f;

			profileView = new ProfilePictureView (new CGRect (11, 78, 149, 149));
			profileView.ProfileId = Profile.CurrentProfile.UserID;
			profileView.Alpha = 0f;

			sideMenuIsUp = false;

			View.AddSubviews (profileView, sidemenu);

			UIFont namefont = UIFont.FromName("narwhal-bold", 20);
			UIFont lastnamefont = UIFont.FromName("narwhal-bold", 24);

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

		private void LoadBanner()
		{
			UIImage bannerImage = UIImage.FromFile ("./screens/home/banner/" + AppDelegate.PhoneVersion + ".jpg");

			banner = new UIImageView(new CGRect(0,0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
			banner.Image = bannerImage;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (sideMenuIsUp)
				{ sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; return; }

				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					sidemenu.Alpha = 1f;
					profileView.Alpha = 1f;
					sideMenuIsUp = true;
				}
			});

			banner.UserInteractionEnabled = true;
			banner.AddGestureRecognizer (tap);
			banner.Alpha = .95f;
			View.AddSubview (banner);
		}
			
		private void LoadMap()
		{
			var camera = CameraPosition.FromCamera (latitude: 40, 
				longitude: -100, 
				zoom: -2);
			
			map = MapView.FromCamera (new CGRect (0, banner.Frame.Bottom, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight - banner.Frame.Height - map_button.Frame.Height), camera);
			map.Alpha = 0f;
			map.Settings.CompassButton = true;
			map.Settings.MyLocationButton = true;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (sideMenuIsUp)
				{ sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; return; }
			});

			map.UserInteractionEnabled = true;
			map.AddGestureRecognizer (tap);

			View.AddSubview (map);

			InvokeOnMainThread (()=> map.MyLocationEnabled = true);
		}

		public override void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			if (!firstLocationUpdate) {
				firstLocationUpdate = true; 

				var location = change.ObjectForKey (NSValue.ChangeNewKey) as CoreLocation.CLLocation;
				map.Camera = CameraPosition.FromCamera (location.Coordinate, 15);
			}
		}

		private void LoadMapButton()
		{
			UIImage mapImage = UIImage.FromFile ("./screens/home/map/" + AppDelegate.PhoneVersion + ".jpg");
			UIImage listImage = UIImage.FromFile ("./screens/home/list/" + AppDelegate.PhoneVersion + ".jpg");

			map_button = new UIButton(new CGRect(0,AppDelegate.ScreenHeight - (mapImage.Size.Height / 2),
				mapImage.Size.Width / 2, mapImage.Size.Height / 2));
			map_button.SetImage(mapImage, UIControlState.Normal);

			MapButtonEvent = (sender, e) => {
				if (sideMenuIsUp)
				{ sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; return; }

				if (map.Alpha == 0f)
				{ 
					map.Alpha = 1f; 
					map_button.SetImage(listImage, UIControlState.Normal);
				} else {
					map.Alpha = 0f;
					map_button.SetImage(mapImage, UIControlState.Normal);
				} 
			};

			map_button.Alpha = .95f;
			View.AddSubview (map_button);
		}
	}
}