using System;
using System.Collections.Generic;
using Board.Infrastructure;
using System.Linq;
using CoreGraphics;
using Foundation;
using Board.Utilities;
using Board.Interface;
using Google.Maps;
using Facebook.CoreKit;
using UIKit;
using Board.Screens.Controls;
using MGImageUtilitiesBinding;
using BigTed;

namespace Board.Screens
{
	public partial class MainMenuScreen : UIViewController
	{
		MenuBanner Banner;
		UIScrollView content;
		List<BoardThumb> ListThumbs;
		List<Marker> ListMapMarkers;

		UIButton map_button;
		EventHandler MapButtonEvent;
		MapView map;

		bool mapInfoTapped;
		bool firstLocationUpdate;
		bool generatedMarkers;

		float yposition;
		float thumbsize;

		public override void DidReceiveMemoryWarning ()
		{
			GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ListThumbs = new List<BoardThumb> ();
			ListMapMarkers = new List<Marker> ();

			if (Profile.CurrentProfile == null) {
				AppDelegate.NavigationController.PopViewController (true);
			}

			content = new UIScrollView(new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));

			LoadMapButton ();
			LoadBanner ();
			LoadMap ();

			View.AddSubviews (content, map, Banner, map_button);
		}

		public override async void ViewDidAppear(bool animated)
		{
			await InitializeInterface ();

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
				MemoryUtility.ReleaseUIViewWithChildren (bt, true);
			}
			foreach (Marker mark in ListMapMarkers) {
				mark.Dispose ();
			}

			MemoryUtility.ReleaseUIViewWithChildren (map, true);

			ListMapMarkers = null;
			ListThumbs = null;

			Banner.UnsuscribeToEvents ();

			MemoryUtility.ReleaseUIViewWithChildren (View, true);
			GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced);
		}

		public async System.Threading.Tasks.Task InitializeInterface()
		{
			await LoadContent ();
		}

		private async System.Threading.Tasks.Task LoadContent()
		{
			thumbsize = AppDelegate.ScreenWidth / 3.5f;

			content.BackgroundColor = UIColor.White;

			var magazineBanner = new MagazineBanner ();
			AddChildViewController (magazineBanner); 
			content.AddSubview (magazineBanner.View);

			// GenerateBoardList()
			var boardList = await CloudController.GetUserBoards ();
			boardList = boardList.OrderBy(o=>o.GeolocatorObject.Neighborhood).ToList();

			LocationLabel.font = AppDelegate.Narwhal20;

			// starting point
			yposition = (float)Banner.Frame.Bottom + 10;

			content.ScrollEnabled = true;
			content.UserInteractionEnabled = true;

			content.ContentSize = new CGSize (AppDelegate.ScreenWidth, yposition + thumbsize + 5);

			/*content.Scrolled += (sender, e) => {

				if (content.ContentOffset.Y < magazineBanner.Frame.Bottom &&
					content.ContentOffset.Y + AppDelegate.ScreenHeight > magazineBanner.Frame.Top) {
					magazineBanner.ParallaxMove((float)content.ContentOffset.Y);
				}

			};*/
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
		}
			
		private void LoadMap()
		{
			firstLocationUpdate = false;

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

			InvokeOnMainThread (()=> map.MyLocationEnabled = true);
		}

		public override void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			if (!firstLocationUpdate) {
				firstLocationUpdate = true; 

				var location = change.ObjectForKey (NSValue.ChangeNewKey) as CoreLocation.CLLocation;
				map.Camera = CameraPosition.FromCamera (location.Coordinate, 15);

				if (!generatedMarkers) {
					GenerateMarkers ();
					generatedMarkers = true;
				}
			}
		}

		private void GenerateMarkers()
		{
			foreach (BoardThumb thumb in ListThumbs) {
				Marker marker = new Marker ();
				marker.AppearAnimation = MarkerAnimation.Pop;
				marker.Position = thumb.Board.GeolocatorObject.Coordinate;
				marker.Map = map;
				marker.Icon = CreateMarkerImage (thumb.Board.ImageView.Image);
				marker.Draggable = false;
				marker.Title = thumb.Board.Name;
				marker.Snippet = thumb.Board.GeolocatorObject.Address;
				marker.InfoWindowAnchor = new CGPoint (.5, .5);
				marker.Tappable = true;
				marker.UserData = new NSString(thumb.Board.Id);
				ListMapMarkers.Add (marker);
			}
		}

		private UIImage CreateMarkerImage(UIImage logo)
		{
			UIGraphics.BeginImageContextWithOptions (new CGSize (66, 96), false, 2f);

			using (UIImage container = UIImage.FromFile ("./screens/main/map/markercontainer.png")) {
				container.Draw (new CGRect (0, 0, 66, 96));
			}

			float autosize = 40;

			logo = logo.ImageScaledToFitSize (new CGSize(autosize,autosize));

			logo.Draw (new CGRect (33 - logo.Size.Width / 2, 33 - logo.Size.Height / 2, logo.Size.Width, logo.Size.Height));

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
		}

	}
}