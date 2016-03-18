using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
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
		UIScrollView content;
		List<BoardThumb> ListThumbs;

		UIButton map_button;
		EventHandler MapButtonEvent;
		MapView map;

		bool mapInfoTapped;
		bool firstLocationUpdate;
		bool generatedMarkers;

		public override void DidReceiveMemoryWarning ()
		{
			GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ListThumbs = new List<BoardThumb> ();

			InitializeInterface ();
		}

		public override void ViewDidAppear(bool animated)
		{
			// suscribe to observers, gesture recgonizers, events
			map.AddObserver (this, new NSString ("myLocation"), NSKeyValueObservingOptions.New, IntPtr.Zero);
			map_button.TouchUpInside += MapButtonEvent;	
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
		}

		class LocationLabel : UILabel{
			public static UIFont font;
		
			public LocationLabel(float yposition, string location)
			{
				Frame = new CGRect(10, yposition, AppDelegate.ScreenWidth - 20, 24);
				//Frame = new CGRect(30, yposition, AppDelegate.ScreenWidth - 40, 24);
				Font = font;
				TextAlignment = UITextAlignment.Center;
				TextColor = AppDelegate.BoardOrange;
				Text = location;
			}
		}

		float yposition;
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
			bool newLine = false;
			int neighborhoodnumber = 0;
			yposition = 15;
			foreach (Board.Schema.Board b in boardList) {
				if (location != b.Location) {
					// draw new location string
					if (!newLine) {
						yposition += 60;
					}

					if (neighborhoodnumber > 0) {
						DrawTrendingBanner (neighborhoodnumber, false);
					}

					LocationLabel locationLabel = new LocationLabel (yposition, b.Location);
					yposition += (float)locationLabel.Frame.Height + BoardThumb.Size / 2 + 5;
					location = b.Location;
					content.AddSubview (locationLabel);

					i = 1;
					neighborhoodnumber++;
				}
				 
				BoardThumb boardThumb = new BoardThumb (b, new CGPoint ((AppDelegate.ScreenWidth/ 4) * i, yposition));
				i++;
				if (i >= 4) {
					i = 1;
					yposition += BoardThumb.Size + 6;
					newLine = true;
				} else { newLine = false; }

				ListThumbs.Add (boardThumb);
				content.AddSubview (boardThumb);
			}
			yposition += 60;
			DrawTrendingBanner (neighborhoodnumber, true);

			content.ScrollEnabled = true;
			content.UserInteractionEnabled = true;

			content.ContentSize = new CGSize (AppDelegate.ScreenWidth, yposition + BoardThumb.Size + 5);

			View.AddSubview (content);
		}

		private void DrawTrendingBanner(int number, bool last)
		{
			yposition += 15;
			UIImageView featuredBlock = new UIImageView(new CGRect(0, yposition, AppDelegate.ScreenWidth, 150));
			using (UIImage img = UIImage.FromFile ("./demo/main/trending"+number+".jpg")) {
				featuredBlock.Image = img;
			}
			content.AddSubview (featuredBlock);

			yposition += (float)featuredBlock.Frame.Height;

			if (!last) {
				yposition += 40;
			}
		}

		private void LoadBanner()
		{
			Banner = new MenuBanner("./screens/main/banner/" + AppDelegate.PhoneVersion + ".jpg");

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					AppDelegate.containerScreen.BringSideMenuUp("main");
				}
			});

			Banner.AddTap (tap);

			View.AddSubview (Banner);
		}
			
		private void LoadMap()
		{
			var camera = CameraPosition.FromCamera (40, -100, -2);

			firstLocationUpdate = false;

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
				map.Camera = CameraPosition.FromCamera (location.Coordinate, 15);

				if (!generatedMarkers) {
					GenerateMarkers (location.Coordinate);
					generatedMarkers = true;
				}
			}
		}

		private void GenerateMarkers(CoreLocation.CLLocationCoordinate2D location)
		{
			Random rnd = new Random ();
			UIImage container = UIImage.FromFile ("./screens/main/map/marker_blue.png");

			foreach (BoardThumb thumb in ListThumbs) {
				double lat = rnd.NextDouble () - .5;
				double lon = rnd.NextDouble () - .5;

				Marker marker = new Marker ();
				marker.AppearAnimation = MarkerAnimation.Pop;
				marker.Position = new CoreLocation.CLLocationCoordinate2D (location.Latitude - (lat * .02), location.Longitude + (lon * .02));
				marker.Map = map;
				marker.Icon = CreateMarkerImage (container, thumb.Board.ImageView.Image);
				marker.Draggable = false;
				marker.Title = thumb.Board.Name;
				marker.Snippet = "2 Cooper Street, Wynwood, FL 33880" + "\n\nTAP TO ENTER BOARD";
				marker.InfoWindowAnchor = new CGPoint (.5, .5);
				marker.Tappable = true;
				marker.UserData = new NSString(thumb.Board.Id);
			}
		}

		// this one just creates a color square
		private UIImage CreateMarkerImage(UIImage container, UIImage logo)
		{
			UIGraphics.BeginImageContext (new CGSize(66, 96));

			container.Draw (new CGRect (0, 0, 66, 96));

			float imgw, imgh;

			float scale = (float)(logo.Size.Height/logo.Size.Width);
			imgw = 40;
			imgh = imgw * scale;

			logo.Draw (new CGRect (33 - imgw / 2, 33 - imgh / 2, imgw, imgh));

			return UIGraphics.GetImageFromCurrentImageContext ();
		}

		private void LoadMapButton()
		{
			using (UIImage mapImage = UIImage.FromFile ("./screens/main/map/" + AppDelegate.PhoneVersion + ".jpg")) {
				map_button = new UIButton(new CGRect(0,AppDelegate.ScreenHeight - (mapImage.Size.Height / 2),
					mapImage.Size.Width / 2, mapImage.Size.Height / 2));
				map_button.SetImage(mapImage, UIControlState.Normal);
			}

			MapButtonEvent = (sender, e) => {
				if (map.Alpha == 0f)
				{ 
					map.Alpha = 1f; 

					using (UIImage listImage = UIImage.FromFile ("./screens/main/list/" + AppDelegate.PhoneVersion + ".jpg")) {
						map_button.SetImage(listImage, UIControlState.Normal);
					}
				} else {
					map.Alpha = 0f;

					using (UIImage mapImage = UIImage.FromFile ("./screens/main/map/" + AppDelegate.PhoneVersion + ".jpg")) {
						map_button.SetImage(mapImage, UIControlState.Normal);
					}
				} 
			};

			map_button.Alpha = .95f;
			View.AddSubview (map_button);
		}
	}
}