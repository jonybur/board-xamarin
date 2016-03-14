using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Facebook.CoreKit;
using Foundation;
using Board.Utilities;
using Board.Interface;
using Google.Maps;
using UIKit;
using Board.Screens.Controls;

namespace Board.Screens
{
	public partial class MainMenuScreen : UIViewController
	{
		MenuBanner Banner;
		UIImageView sidemenu;
		UIButton map_button;

		List<BoardThumb> ListThumbs;

		ProfilePictureView profileView;

		UIScrollView content;

		EventHandler MapButtonEvent;
		UITapGestureRecognizer SideMenuTap;

		MapView map;

		bool sideMenuIsUp;
		bool mapInfoTapped;
		bool firstLocationUpdate;

		public override void DidReceiveMemoryWarning ()
		{
			GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced);
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
			mapInfoTapped = false;
			Banner.SuscribeToEvents ();
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
			Banner.UnsuscribeToEvents ();

			MemoryUtility.ReleaseUIViewWithChildren (View, true);
		}

		public void InitializeInterface()
		{
			LoadContent ();
			LoadBanner ();
			LoadMapButton ();
			LoadMap ();
			LoadSideMenu ();
		}

		class LocationLabel : UILabel{
			public static UIFont font;
		
			public LocationLabel(float yposition, string location)
			{
				Frame = new CGRect(30, yposition, AppDelegate.ScreenWidth - 40, 24);
				Font = font;
				TextColor = AppDelegate.BoardOrange;
				Text = location;
			}
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

			LocationLabel.font = AppDelegate.Narwhal20;

			// starting point
			float yposition = 5;
			bool newLine = false;
			foreach (Board.Schema.Board b in boardList) {
				if (location != b.Location) {
					// draw new location string
					if (!newLine) {
						yposition += 70;
					}
					LocationLabel locationLabel = new LocationLabel (yposition, b.Location);
					yposition += (float)locationLabel.Frame.Height + BoardThumb.Size / 2 + 10;
					location = b.Location;
					content.AddSubview (locationLabel);
					i = 1;
				}
				 
				BoardThumb boardThumb = new BoardThumb (b, new CGPoint ((AppDelegate.ScreenWidth/ 4) * i, yposition));
				i++;
				if (i >= 4) {
					i = 1;
					yposition += BoardThumb.Size + 6;
					newLine = true;
				} else { newLine = false;}

				ListThumbs.Add (boardThumb);
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
			using (UIImage bannerImage = UIImage.FromFile ("./screens/home/sidemenu/" + AppDelegate.PhoneVersion + ".png")) {
				sidemenu = new UIImageView (new CGRect (0, 0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
				sidemenu.Image = bannerImage;
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

		private void LoadBanner()
		{
			Banner = new MenuBanner("./screens/home/banner/" + AppDelegate.PhoneVersion + ".jpg");

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (sideMenuIsUp)
				{ sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; return; }

				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					sidemenu.Alpha = 1f;
					profileView.Alpha = 1f;
					sideMenuIsUp = true;
				}
			});

			Banner.AddTap (tap);

			View.AddSubview (Banner);
		}
			
		private void LoadMap()
		{
			var camera = CameraPosition.FromCamera (40, -100, -2);
			
			map = MapView.FromCamera (new CGRect (0, Banner.Frame.Bottom, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight - Banner.Frame.Height - map_button.Frame.Height), camera);
			map.Alpha = 0f;
			map.Settings.CompassButton = true;
			map.Settings.MyLocationButton = true;
			map.UserInteractionEnabled = true;

			map.InfoTapped += (sender, e) => {
				if (!mapInfoTapped)
				{
					BoardThumb bthumb = ListThumbs.Find(t=>t.Board.Id == ((NSString)e.Marker.UserData).ToString());
					AppDelegate.boardInterface = new BoardInterface(bthumb.Board, false);
					AppDelegate.NavigationController.PushViewController(AppDelegate.boardInterface, true);
					mapInfoTapped = true;
				}
			};

			View.AddSubview (map);

			InvokeOnMainThread (()=> map.MyLocationEnabled = true);
		}

		public override void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			if (!firstLocationUpdate) {
				firstLocationUpdate = true; 

				var location = change.ObjectForKey (NSValue.ChangeNewKey) as CoreLocation.CLLocation;
				map.Camera = CameraPosition.FromCamera (location.Coordinate, 16);
				GenerateMarkers (location.Coordinate);
			}
		}

		private void GenerateMarkers(CoreLocation.CLLocationCoordinate2D location)
		{
			Random rnd = new Random ();
			foreach (BoardThumb thumb in ListThumbs) {
				double lat = rnd.NextDouble () - .5;
				double lon = rnd.NextDouble () - .5;

				Marker marker = new Marker ();
				marker.AppearAnimation = MarkerAnimation.Pop;
				marker.Position = new CoreLocation.CLLocationCoordinate2D (location.Latitude - (lat * .02), location.Longitude + (lon * .02));
				marker.Map = map;
				marker.Icon = CreateMarkerImage (thumb.Board.ImageView.Image);
				marker.Draggable = false;
				marker.Title = thumb.Board.Name;
				marker.Snippet = "2 Cooper Street, Wynwood, FL 33880" + "\n\nTAP TO ENTER BOARD";
				marker.InfoWindowAnchor = new CGPoint (.5, .5);
				marker.Tappable = true;
				marker.UserData = new NSString(thumb.Board.Id);
			}
		}

		static UIImage markerImage;

		// this one just creates a color square
		private UIImage CreateMarkerImage(UIImage logo)
		{
			UIGraphics.BeginImageContext (new CGSize(66, 96));

			if (markerImage == null) {
				using (UIImage circle = UIImage.FromFile ("./screens/home/map/marker_blue.png")) {
					markerImage = circle;
				}	
			}

			markerImage.Draw (new CGRect (0, 0, 66, 96));

			float imgw, imgh;

			float scale = (float)(logo.Size.Height/logo.Size.Width);
			imgw = 40;
			imgh = imgw * scale;

			logo.Draw (new CGRect (33 - imgw / 2, 33 - imgh / 2, imgw, imgh));

			return UIGraphics.GetImageFromCurrentImageContext ();
		}

		private void LoadMapButton()
		{
			using (UIImage mapImage = UIImage.FromFile ("./screens/home/map/" + AppDelegate.PhoneVersion + ".jpg")) {
				map_button = new UIButton(new CGRect(0,AppDelegate.ScreenHeight - (mapImage.Size.Height / 2),
					mapImage.Size.Width / 2, mapImage.Size.Height / 2));
				map_button.SetImage(mapImage, UIControlState.Normal);
			}

			MapButtonEvent = (sender, e) => {
				if (sideMenuIsUp)
				{ sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; return; }

				if (map.Alpha == 0f)
				{ 
					map.Alpha = 1f; 

					using (UIImage listImage = UIImage.FromFile ("./screens/home/list/" + AppDelegate.PhoneVersion + ".jpg")) {
						map_button.SetImage(listImage, UIControlState.Normal);
					}
				} else {
					map.Alpha = 0f;

					using (UIImage mapImage = UIImage.FromFile ("./screens/home/map/" + AppDelegate.PhoneVersion + ".jpg")) {
						map_button.SetImage(mapImage, UIControlState.Normal);
					}
				} 
			};

			map_button.Alpha = .95f;
			View.AddSubview (map_button);
		}
	}
}