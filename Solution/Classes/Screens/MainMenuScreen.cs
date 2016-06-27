using System;
using System.Collections.Generic;
using System.Threading;
using Board.Infrastructure;
using Board.Interface;
using Board.Screens.Controls;
using Board.Utilities;
using CoreGraphics;
using System.Linq;
using CoreLocation;
using Foundation;
using Google.Maps;
using UIKit;

namespace Board.Screens
{
	public class MainMenuScreen : UIViewController
	{
		public MapView map;
		UIMenuBanner Banner;
		UIScrollView ScrollView;
		List<UIMapMarker> ListMapMarkers;

		UIMagazine Magazine;
		UIActionButton map_button;
		EventHandler MapButtonEvent;
		UIContentDisplay ContentDisplay;

		static class FetchedBoards{
			public static List<Board.Schema.Board> BoardList;
			public static CLLocationCoordinate2D Location;

			public static void Update(){
				FetchedBoards.BoardList = CloudController.GetNearbyBoards (AppDelegate.UserLocation, 10000);
				FetchedBoards.Location = AppDelegate.UserLocation;
			}
		}

		const int ZoomLevel = 16;
		enum ScrollViewDirection { Up, Down };

		bool mapInfoTapped, generatedMarkers, hasLoaded, firstLocationUpdate;

		public override void DidReceiveMemoryWarning ()
		{
			GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ScrollView = new UIScrollView(new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			ScrollView.ScrollsToTop = true;

			ListMapMarkers = new List<UIMapMarker> ();

			LoadMapButton ();
			LoadBanner ();
			LoadMap ();

			var statusBarView = new UIView (new CGRect (0, 0, AppDelegate.ScreenWidth, 20));
			statusBarView.Alpha = .95f;
			statusBarView.BackgroundColor = AppDelegate.BoardOrange;

			View.AddSubviews (ScrollView, map, Banner, map_button, statusBarView);

			if (CLLocationManager.Status == CLAuthorizationStatus.NotDetermined) {
				
				var listenThread = new Thread (ListensToUndeterminedLocationService);
				listenThread.Start ();

			} else {

				CheckLocationServices ();
				if (AppDelegate.SimulatingNantucket) {
					BigTed.BTProgressHUD.Show ();
				}

			}
		}

		public override void ViewDidAppear(bool animated)
		{
			if (CLLocationManager.Status != CLAuthorizationStatus.NotDetermined) {
				if (AppDelegate.UserLocation.Latitude != 0 &&
					AppDelegate.UserLocation.Longitude != 0 &&
					!hasLoaded) {

					LoadContent ();
					ContentDisplaySuscribeToEvents (ContentDisplay);
					hasLoaded = true;

					if (AppDelegate.SimulatingNantucket) {
						map.Camera = CameraPosition.FromCamera (AppDelegate.UserLocation, ZoomLevel);
						GenerateMarkers ();
					}

					BigTed.BTProgressHUD.Dismiss ();
				}

				// suscribe to observers, gesture recgonizers, events 
				map.AddObserver (this, new NSString ("myLocation"), NSKeyValueObservingOptions.New, IntPtr.Zero);
				map_button.TouchUpInside += MapButtonEvent;	
				mapInfoTapped = false;
				Banner.SuscribeToEvents ();
			}
		}

		private void ListensToUndeterminedLocationService(){

			while (CLLocationManager.Status == CLAuthorizationStatus.NotDetermined) {
				Thread.Sleep (500);
			}

			InvokeOnMainThread(delegate {
				CheckLocationServices ();
				if (AppDelegate.SimulatingNantucket) {
					BigTed.BTProgressHUD.Show ();
				}

				ViewDidAppear (false);
			});
		}
			
		private void CheckLocationServices(){
			
			if (!CLLocationManager.LocationServicesEnabled ||
				CLLocationManager.Status == CLAuthorizationStatus.Denied ||
				CLLocationManager.Status == CLAuthorizationStatus.Restricted) {

				if (!AppDelegate.SimulatingNantucket) {

					var noContent = new UINoContent (UINoContent.Presets.LocationDisabled);
					ScrollView.AddSubview (noContent);
					map_button.Alpha = 0f;

					BigTed.BTProgressHUD.Dismiss ();
				}

			}
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

		private void ShowFirstTimeUseMessage(){
			var defaults = NSUserDefaults.StandardUserDefaults;
			const string key = "FirstTimeUse";
			if (!defaults.BoolForKey (key)) {
				// First launch
				NSUserDefaults.StandardUserDefaults.SetBool (true, key);
				defaults.Synchronize ();
				BigTed.BTProgressHUD.Show ("Setting up Board\nfor first time use...");
			} else { 
				BigTed.BTProgressHUD.Show ();
			}
		}

		public void SimulateNantucket(){
			AppDelegate.UserLocation = new CLLocationCoordinate2D(41.284558, -70.098572);

			AppDelegate.SimulatingNantucket = true;

			LoadContent ();
			ContentDisplaySuscribeToEvents (ContentDisplay);

			hasLoaded = true;
			map_button.Alpha = 1f;
			map.Camera = new CameraPosition (AppDelegate.UserLocation, ZoomLevel, 0, 0);

			GenerateMarkers ();

			BigTed.BTProgressHUD.Dismiss ();
		}

		public override void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{	
			if (!firstLocationUpdate) {

				ShowFirstTimeUseMessage ();

				firstLocationUpdate = true;
				var location = change.ObjectForKey (NSObject.ChangeNewKey) as CLLocation;

				if (!AppDelegate.SimulatingNantucket) {
					Console.WriteLine ("Gets user location from maps");

					AppDelegate.UserLocation = location.Coordinate;
					CloudController.LogSession ();

					map.Camera = CameraPosition.FromCamera (location.Coordinate, ZoomLevel);
				}

				if (!hasLoaded) {
					LoadContent ();
					ContentDisplaySuscribeToEvents (ContentDisplay);
					hasLoaded = true;
				}

				GenerateMarkers ();

				BigTed.BTProgressHUD.Dismiss ();
			}
		}

		private void ContentDisplaySuscribeToEvents(UIContentDisplay contentDisplay){
			if (contentDisplay != null) {
				contentDisplay.SuscribeToEvents ();
			}
		}

		bool addedScrollEvents;
		private void LoadContent()
		{
			ScrollView.BackgroundColor = UIColor.White;

			if (FetchedBoards.BoardList == null || FetchedBoards.BoardList.Count == 0 || CommonUtils.DistanceBetweenCoordinates (FetchedBoards.Location, AppDelegate.UserLocation) > 1) {
				Console.WriteLine ("Updates BoardList");
				FetchedBoards.Update ();
			}

			if (FetchedBoards.BoardList.Count > 0) {
				
				Magazine = new UIMagazine (FetchedBoards.BoardList);

				ContentDisplay = Magazine.Pages [0].ContentDisplay;
				ScrollView.ContentSize = new CGSize (AppDelegate.ScreenWidth, ContentDisplay.Frame.Bottom);

				ScrollView.AddSubview (ContentDisplay);
				ScrollView.AddSubview (Magazine.Banner);

			} else {

				var noContent = new UINoContent (UINoContent.Presets.NotInArea);
				ScrollView.AddSubview (noContent);
				map_button.Alpha = 0f;

			}

			ScrollView.ScrollEnabled = true;

			float previousOffset = 0;

			var direction = ScrollViewDirection.Up;

			if(!addedScrollEvents){
				addedScrollEvents = true;
				
				ScrollView.DraggingEnded += (sender, e) => {
					if (direction == ScrollViewDirection.Up){
						Banner.AnimateShow();
					} else if (direction == ScrollViewDirection.Down && ScrollView.ContentOffset.Y > Banner.Frame.Height) {
						Banner.AnimateHide();
					}
				};

				ScrollView.Scrolled += (sender, e) => {
					
					if (ScrollView.ContentOffset.Y < 0){
						
						Magazine.Banner.Center = new CGPoint(Magazine.Banner.Center.X,
							UIMenuBanner.Height + Magazine.Banner.Frame.Height / 2 + ScrollView.ContentOffset.Y);

						Banner.Frame = new CGRect(Banner.Frame.X, 0, Banner.Frame.Width, Banner.Frame.Height);


					} else if (ScrollView.ContentOffset.Y < ScrollView.ContentSize.Height - AppDelegate.ScreenHeight) {
						
						var diff = previousOffset - ScrollView.ContentOffset.Y;

						if (Banner.Frame.Y + diff > 0){
							Banner.Frame = new CGRect(Banner.Frame.X, 0, Banner.Frame.Width, Banner.Frame.Height);
						} else if (Banner.Frame.Y + diff < -Banner.Frame.Height){
							Banner.Frame = new CGRect(Banner.Frame.X, -Banner.Frame.Height, Banner.Frame.Width, Banner.Frame.Height);
						} else {
							Banner.Frame = new CGRect(Banner.Frame.X, Banner.Frame.Y + diff, Banner.Frame.Width, Banner.Frame.Height);
						}
					}

					if (previousOffset < ScrollView.ContentOffset.Y) { 
						direction = ScrollViewDirection.Down;
					}else{
						direction = ScrollViewDirection.Up;
					}

					previousOffset = (float)ScrollView.ContentOffset.Y;

				};

			}
		}

		private void LoadBanner()
		{
			Banner = new UIMenuBanner ("BOARD", "menu_left");

			bool taps = false;
			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					if (!taps){
						taps = true;
						Facebook.FacebookAutoUpdater.UpdateAllBoards(FetchedBoards.BoardList);
					}
					//AppDelegate.containerScreen.BringSideMenuUp("main");
				}
			});

			Banner.AddTap (tap);
		}

		private void LoadMap()
		{
			var camera = CameraPosition.FromCamera (40, -100, -2);
			map = MapView.FromCamera (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight), camera);
			map.Alpha = 0f;

			var edgeInsets = new UIEdgeInsets (UIMenuBanner.Height, 0, UIActionButton.Height, 0);

			map.Padding = edgeInsets;

			map.Settings.CompassButton = true;
			map.Settings.MyLocationButton = true;
			map.UserInteractionEnabled = true;

			map.InfoTapped += (sender, e) => {
				if (!mapInfoTapped) {
					var board = FetchedBoards.BoardList.Find(t => t.Id == ((NSString)e.Marker.UserData).ToString());
					AppDelegate.BoardInterface = new UIBoardInterface (board);
					AppDelegate.NavigationController.PushViewController (AppDelegate.BoardInterface, true);
					mapInfoTapped = true;
				}
			};

			InvokeOnMainThread (() => map.MyLocationEnabled = true);
		}

		private void GenerateMarkers()
		{
			if (!generatedMarkers) {
				foreach (Board.Schema.Board board in FetchedBoards.BoardList) {
					var marker = new UIMapMarker (board, map, UIMapMarker.SizeMode.Normal);
					ListMapMarkers.Add (marker);
				}
				generatedMarkers |= FetchedBoards.BoardList.Count > 0;
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
			ContentDisplaySuscribeToEvents (ContentDisplay);
		} 

		private void LoadMapButton()
		{
			map_button = new UIActionButton ("MAP");

			MapButtonEvent = (sender, e) => {
				map_button.Alpha = 1f;

				if (map.Alpha == 0f) { 
					map.Alpha = 1f;

					// stops scrollview
					ScrollView.SetContentOffset(ScrollView.ContentOffset, false);

					// animates banner to be shown
					Banner.AnimateShow();

					map_button.ChangeTitle("LIST");
				} else {
					map.Alpha = 0f;
					map_button.ChangeTitle("MAP");
				} 
			};
		}

	}
}