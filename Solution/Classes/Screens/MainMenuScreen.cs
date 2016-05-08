using System;
using System.Collections.Generic;
using Board.Infrastructure;
using Board.Interface;
using Board.Screens.Controls;
using Board.Utilities;
using CoreGraphics;
using Facebook.CoreKit;
using Foundation;
using Google.Maps;
using MGImageUtilitiesBinding;
using UIKit;
using BigTed;

namespace Board.Screens
{
	public partial class MainMenuScreen : UIViewController
	{
		public MapView map;
		UIMenuBanner Banner;
		UIScrollView ScrollView;
		List<Marker> ListMapMarkers;

		UIMagazine Magazine;
		UIActionButton map_button;
		EventHandler MapButtonEvent;
		UIContentDisplay ContentDisplay;
		List<Board.Schema.Board> BoardList;

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

			ListMapMarkers = new List<Marker> ();

			if (Profile.CurrentProfile == null) {
				AppDelegate.NavigationController.PopViewController (true);
			}

			ScrollView = new UIScrollView(new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));

			LoadMapButton ();
			LoadBanner ();
			LoadMap ();

			View.AddSubviews (ScrollView, map, Banner, map_button);
		}

		public override async void ViewDidAppear(bool animated)
		{
			// suscribe to observers, gesture recgonizers, events
			map.AddObserver (this, new NSString ("myLocation"), NSKeyValueObservingOptions.New, IntPtr.Zero);
			map_button.TouchUpInside += MapButtonEvent;	
			mapInfoTapped = false;

			Banner.SuscribeToEvents ();
		}

		public override void ViewDidDisappear(bool animated)
		{
			// unsuscribe from observers, gesture recgonizers, events
			map.RemoveObserver (this, new NSString ("myLocation"));
			map_button.TouchUpInside -= MapButtonEvent;

			if (ContentDisplay != null) {
				ContentDisplay.UnsuscribeToEvents ();
			}

			foreach (Marker mark in ListMapMarkers) {
				mark.Dispose ();
			}

			MemoryUtility.ReleaseUIViewWithChildren (map);
			ListMapMarkers = null;
			Banner.UnsuscribeToEvents ();

			MemoryUtility.ReleaseUIViewWithChildren (View);
			GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced);
		}

		private async System.Threading.Tasks.Task LoadContent()
		{
			ScrollView.BackgroundColor = UIColor.White;

			//BoardList = await CloudController.GetAllBoards();
			BoardList = await CloudController.GetNearbyBoards (AppDelegate.UserLocation, 10000);

			Magazine = new UIMagazine (BoardList);

			ContentDisplay = Magazine.Pages [0].ContentDisplay;
			ScrollView.ContentSize = new CGSize (AppDelegate.ScreenWidth, ContentDisplay.Frame.Bottom);

			ScrollView.AddSubview (ContentDisplay);
			ScrollView.AddSubview (Magazine.Banner);

			ScrollView.ScrollEnabled = true;
			ScrollView.Scrolled += (sender, e) => {
				
				if (ScrollView.ContentOffset.Y < 0){
					Magazine.Banner.Center = new CGPoint(Magazine.Banner.Center.X,
						UIMenuBanner.Height + Magazine.Banner.Frame.Height / 2 + ScrollView.ContentOffset.Y);
				}

			};
		}

		private void LoadBanner()
		{
			Banner = new UIMenuBanner ("BOARD", "menu_left");

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
			map = MapView.FromCamera (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight), camera);
			map.Alpha = 0f;
			//	MapView.FromCamera (new CGRect (), camera);

			var edgeInsets = new UIEdgeInsets (UIMenuBanner.Height, 0, UIActionButton.Height, 0);

			map.Padding = edgeInsets;

			map.Settings.CompassButton = true;
			map.Settings.MyLocationButton = true;
			map.UserInteractionEnabled = true;

			map.InfoTapped += (sender, e) => {
				if (!mapInfoTapped) {
					var board = BoardList.Find(t=>t.Id == ((NSString)e.Marker.UserData).ToString());
					AppDelegate.BoardInterface = new UIBoardInterface(board);
					AppDelegate.NavigationController.PushViewController(AppDelegate.BoardInterface, true);
					mapInfoTapped = true;
				}
			};

			InvokeOnMainThread (()=> map.MyLocationEnabled = true);
		}

		public override async void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			if (!firstLocationUpdate) {
				
				BTProgressHUD.Show ();

				firstLocationUpdate = true; 

				var location = change.ObjectForKey (NSValue.ChangeNewKey) as CoreLocation.CLLocation;
				AppDelegate.UserLocation = location.Coordinate;
				map.Camera = CameraPosition.FromCamera (location.Coordinate, 15);

				await LoadContent ();
				ContentDisplay.SuscribeToEvents ();

				BTProgressHUD.Dismiss ();

				if (!generatedMarkers) {
					GenerateMarkers ();
					generatedMarkers = true;
				}
			}
		}

		private void GenerateMarkers()
		{
			foreach (Board.Schema.Board board in BoardList) {
				Marker marker = new Marker ();
				marker.AppearAnimation = MarkerAnimation.Pop;
				marker.Position = board.GeolocatorObject.Coordinate;
				marker.Map = map;
				marker.Icon = CreateMarkerImage (board.ImageView.Image);
				marker.Draggable = false;
				marker.Title = board.Name;
				marker.Snippet = board.GeolocatorObject.Address;
				marker.InfoWindowAnchor = new CGPoint (.5, .5);
				marker.Tappable = true;
				marker.UserData = new NSString(board.Id);
				ListMapMarkers.Add (marker);
			}
		}

		public void PlaceNewScreen(UIContentDisplay newDisplay){
			ContentDisplay.UnsuscribeToEvents ();
			ContentDisplay.RemoveFromSuperview ();

			// this updates distance to thumbs everytime user switches screen
			if (newDisplay is UIThumbsContentDisplay) {
				var listThumbs = ((UIThumbsContentDisplay)newDisplay).ListThumbComponents;
				if (listThumbs != null){
					foreach (var thumb in listThumbs) {
						thumb.UpdateDistanceLabel ();
					}
				}
			}
			ContentDisplay = newDisplay;

			ScrollView.AddSubview (ContentDisplay);
			ScrollView.SendSubviewToBack (ContentDisplay);
			ScrollView.ContentSize = new CGSize (AppDelegate.ScreenWidth, newDisplay.Frame.Height);
			ContentDisplay.SuscribeToEvents ();
		} 

		private UIImage CreateMarkerImage(UIImage logo)
		{
			UIGraphics.BeginImageContextWithOptions (new CGSize (66, 96), false, 2f);

			using (UIImage container = UIImage.FromFile ("./screens/main/map/markercontainer_black.png")) {
				container.Draw (new CGRect (0, 0, 66, 96));
			}

			float autosize = 40;

			logo = logo.ImageScaledToFitSize (new CGSize(autosize,autosize));

			logo.Draw (new CGRect (33 - logo.Size.Width / 2, 33 - logo.Size.Height / 2, logo.Size.Width, logo.Size.Height));

			return UIGraphics.GetImageFromCurrentImageContext ();
		}

		private void LoadMapButton()
		{
			map_button = new UIActionButton ("MAP");

			MapButtonEvent = (sender, e) => {
				map_button.Alpha = 1f;

				if (map.Alpha == 0f) { 
					map.Alpha = 1f; 
					map_button.ChangeTitle("LIST");
				} else {
					map.Alpha = 0f;
					map_button.ChangeTitle("MAP");
				} 
			};
		}

	}
}