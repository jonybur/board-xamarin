using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Clubby.Infrastructure;
using Clubby.Interface.VenueInterface;
using Clubby.Schema;
using Clubby.Screens.Controls;
using Clubby.Utilities;
using CoreGraphics;
using CoreLocation;
using DACircularProgress;
using Foundation;
using Google.Maps;
using UIKit;

namespace Clubby.Screens
{
	public static class FetchedVenues{
		public static List<Venue> VenueList;
		public static CLLocationCoordinate2D Location;

		public static async System.Threading.Tasks.Task Update(){
			FetchedVenues.VenueList = await CloudController.GetNearbyVenues (AppDelegate.UserLocation, 10000);
			FetchedVenues.Location = AppDelegate.UserLocation;
		}
	}

	public class MainMenuScreen : UIViewController
	{
		public MapView map;
		UIMenuBanner Banner;
		UIScrollView ScrollView;
		List<UIMapMarker> ListMapMarkers;

		UIMultiActionButtons LowerButtons;
		UIContentDisplay ContentDisplay;

		enum ScrollViewDirection { Up, Down };
		enum SubScreens { Featured, Timeline, Directory, Map };

		const int ZoomLevel = 16;
		bool mapInfoTapped, generatedMarkers, hasLoaded, firstLocationUpdate, addedScrollEvents;

		static class LastScreenStatus{
			public static SubScreens CurrentScreen = SubScreens.Timeline;
			public static CGPoint ContentOffset = new CGPoint(0, 0);
		}

		public override void DidReceiveMemoryWarning () {
			GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced);
		}

		public static CircularProgressView progressView;
		public static UITextView firstTimeLabel;

		public static void StopCircularProgress(){
			progressView.RemoveFromSuperview();
			firstTimeLabel.RemoveFromSuperview();
		}

		private void CheckForNotifications(){
			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				var pushSettings = UIUserNotificationSettings.GetSettingsForTypes (
					UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
					new NSSet ());

				UIApplication.SharedApplication.RegisterUserNotificationSettings (pushSettings);
				UIApplication.SharedApplication.RegisterForRemoteNotifications ();
			} else {
				UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
				UIApplication.SharedApplication.RegisterForRemoteNotificationTypes (notificationTypes);
			}
		}

		private void LoadCircularProgress(){
			progressView = new CircularProgressView ();
			progressView.Progress = 0.35f;
			progressView.IndeterminateDuration = 1.0f;
			progressView.Indeterminate = true;
			progressView.Frame = new CGRect (0, 0, 60, 60);
			progressView.Center = new CGPoint (AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight / 2);

			firstTimeLabel = new UITextView ();

			var defaults = NSUserDefaults.StandardUserDefaults;
			const string key = "FirstTimeUse";
			if (!defaults.BoolForKey (key)) {
				// First launch
				NSUserDefaults.StandardUserDefaults.SetBool (true, key);
				defaults.Synchronize ();

				firstTimeLabel.Frame = new CGRect (0, 0, AppDelegate.ScreenWidth, 0);
				firstTimeLabel.Text = "Welcome to Clubby \ud83c\udf89\nSetting up the app for first time use\nThis might take a moment";
				firstTimeLabel.Font = UIFont.SystemFontOfSize (16, UIFontWeight.Light);
				firstTimeLabel.TextAlignment = UITextAlignment.Center;
				firstTimeLabel.Editable = false;
				firstTimeLabel.Selectable = false;
				firstTimeLabel.ScrollEnabled = false;
				firstTimeLabel.TextColor = UIColor.White;
				firstTimeLabel.BackgroundColor = UIColor.Clear;

				var size = firstTimeLabel.SizeThatFits (firstTimeLabel.Frame.Size);

				firstTimeLabel.Frame = new CGRect (firstTimeLabel.Frame.X, progressView.Frame.Bottom + 16, size.Width, size.Height);
				firstTimeLabel.Center = new CGPoint (AppDelegate.ScreenWidth / 2, firstTimeLabel.Center.Y);

			} else { 
				firstTimeLabel.Alpha = 0f;
			}

			View.AddSubviews (progressView, firstTimeLabel);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var sw = new Stopwatch();
			sw.Start();

			View.BackgroundColor = AppDelegate.ClubbyBlack;

			NavigationController.SetNavigationBarHidden (true, false);

			ScrollView = new UIScrollView(new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			ScrollView.ScrollsToTop = true;

			ListMapMarkers = new List<UIMapMarker> ();

			LoadBanner ();
			LoadMap ();

			var statusBarView = new UIStatusBar ();

			View.AddSubviews (ScrollView, map, Banner, statusBarView);

			CheckForNotifications ();

			if (CLLocationManager.Status == CLAuthorizationStatus.NotDetermined) {
				
				var listenThread = new Thread (ListensToUndeterminedLocationService);
				listenThread.Start ();

			} else {

				CheckLocationServices ();

			}

			sw.Stop();
			Console.WriteLine("ViewDidLoad: {0}",sw.Elapsed);

		}

		public override void ViewDidAppear(bool animated)
		{
			var sw = new Stopwatch();
			sw.Start();

			if (CLLocationManager.Status != CLAuthorizationStatus.NotDetermined) {
				if (AppDelegate.UserLocation.Latitude != 0 &&
					AppDelegate.UserLocation.Longitude != 0) {

					LoadContent ();
					ContentDisplaySuscribeToEvents (ContentDisplay);

				}

				// suscribe to observers, gesture recgonizers, events 
				map.AddObserver (this, new NSString ("myLocation"), NSKeyValueObservingOptions.New, IntPtr.Zero);
				mapInfoTapped = false;
				Banner.SuscribeToEvents ();

				NavigationController.InteractivePopGestureRecognizer.Enabled = false;
				NavigationController.InteractivePopGestureRecognizer.Delegate = null;
			}

			sw.Stop();
			Console.WriteLine("ViewDidAppear: {0}",sw.Elapsed);
		}

		private void ListensToUndeterminedLocationService(){

			while (CLLocationManager.Status == CLAuthorizationStatus.NotDetermined) {
				Thread.Sleep (500);
			}

			InvokeOnMainThread(delegate {
				CheckLocationServices ();

				ViewDidAppear (false);
			});

		}

		private void CheckLocationServices(){

			if (!CLLocationManager.LocationServicesEnabled ||
			    CLLocationManager.Status == CLAuthorizationStatus.Denied ||
			    CLLocationManager.Status == CLAuthorizationStatus.Restricted) {

				var noContent = new UINoContent (UINoContent.Presets.LocationDisabled);
				ScrollView.AddSubview (noContent);

			} else {
				LoadCircularProgress ();
			}
		}

		public override void ViewDidDisappear(bool animated)
		{
			// unsuscribe from observers, gesture recgonizers, events
			try{
				map.RemoveObserver (this, new NSString ("myLocation"));
			}catch{ }

			if (ContentDisplay != null) {
				ContentDisplay.UnsuscribeToEvents ();
			}

			Banner.UnsuscribeToEvents ();

			GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced);
		}

		public override void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{				
			if (!firstLocationUpdate) {

				firstLocationUpdate = true;
				var location = change.ObjectForKey (NSObject.ChangeNewKey) as CLLocation;

				Console.WriteLine ("Gets user location from maps");

				AppDelegate.UserLocation = location.Coordinate;
				CloudController.LogSession ();

				map.Camera = CameraPosition.FromCamera (location.Coordinate, ZoomLevel);

				if (!hasLoaded) {
					LoadContent ();
					ContentDisplaySuscribeToEvents (ContentDisplay);
					hasLoaded = true;
				}

			}
		}

		private void ContentDisplaySuscribeToEvents(UIContentDisplay contentDisplay){
			if (contentDisplay != null) {
				contentDisplay.SuscribeToEvents ();
			}
		}

		private async void LoadContent()
		{			
			var sw = new Stopwatch();
			sw.Start();

			ScrollView.BackgroundColor = AppDelegate.ClubbyBlack;

			if (FetchedVenues.VenueList == null || FetchedVenues.VenueList.Count == 0 || CommonUtils.DistanceBetweenCoordinates (FetchedVenues.Location, AppDelegate.UserLocation) > 1) {
				
				Console.WriteLine ("Updates venues list");

				// gets venues from FB
				await FetchedVenues.Update ();

				// generates pages
				await UIMagazine.GeneratePages (FetchedVenues.VenueList);
			}

			Console.WriteLine("LoadContent 1: {0}",sw.Elapsed);
			sw.Restart ();

			StopCircularProgress ();

			if (LowerButtons == null) {
				LowerButtons = new UIMultiActionButtons ();
				View.AddSubview (LowerButtons);
			}

			if (FetchedVenues.VenueList.Count > 0) {
				
				ScrollView.SetContentOffset (LastScreenStatus.ContentOffset, false);

				switch (LastScreenStatus.CurrentScreen) {
				case SubScreens.Timeline:
					LowerButtons.ListButtons [0].SetFullImage ();
					ContentDisplay = UIMagazine.Pages [0];
					ContentDisplay.SelectiveRendering (ScrollView.ContentOffset);
					break;
				case SubScreens.Featured:
					LowerButtons.ListButtons [1].SetFullImage ();
					ContentDisplay = UIMagazine.Pages [1];
					ContentDisplay.SelectiveRendering (ScrollView.ContentOffset);
					break;
				case SubScreens.Directory:
					LowerButtons.ListButtons [2].SetFullImage ();
					ContentDisplay = UIMagazine.Pages [2];
					((UIThumbsContentDisplay)ContentDisplay).SelectiveThumbsRendering (ScrollView.ContentOffset);
					break;
				case SubScreens.Map:
					LowerButtons.ListButtons [3].SetFullImage ();
					ShowMap ();
					break;
				}

				ScrollView.ContentSize = new CGSize (AppDelegate.ScreenWidth, ContentDisplay.Frame.Bottom);

				ScrollView.AddSubview (ContentDisplay);
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

					LastScreenStatus.ContentOffset = ScrollView.ContentOffset;

					if (ContentDisplay is UIThumbsContentDisplay){
						((UIThumbsContentDisplay)ContentDisplay).SelectiveThumbsRendering(ScrollView.ContentOffset);
					} else {
						ContentDisplay.SelectiveRendering(ScrollView.ContentOffset);
					}

					if (ScrollView.ContentOffset.Y < 0) {

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

					if (ContentDisplay is UITimelineContentDisplay){
						
						if ((ScrollView.ContentOffset.Y + AppDelegate.ScreenHeight) >= ScrollView.ContentSize.Height){
							
							((UITimelineContentDisplay)ContentDisplay).FillMoreTimeline();
							ScrollView.ContentSize = new CGSize (AppDelegate.ScreenWidth, ContentDisplay.Frame.Bottom);

						}

						((UITimelineContentDisplay)ContentDisplay).MuteVideos((float)ScrollView.ContentOffset.Y);
					}

					direction = previousOffset < ScrollView.ContentOffset.Y ? ScrollViewDirection.Down : ScrollViewDirection.Up;

					previousOffset = (float)ScrollView.ContentOffset.Y;

				};

			}
			Console.WriteLine("LoadContent 2: {0}",sw.Elapsed);
			sw.Restart ();
		}

		private void LoadBanner()
		{
			Banner = new UIMenuBanner ("", "full_user");
			Banner.SetMainTitle ();

			Banner.AddLeftTap (delegate {
				AppDelegate.PushViewLikePresentView(new UserScreen());
			});
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
					var board = FetchedVenues.VenueList.Find(t => t.Id == ((NSString)e.Marker.UserData).ToString());
					var venueInterface = new UIVenueInterface (board);
					AppDelegate.NavigationController.PushViewController (venueInterface, true);
					mapInfoTapped = true;
				}
			};

			InvokeOnMainThread (() => map.MyLocationEnabled = true);
		}

		private void GenerateMarkers()
		{
			if (!generatedMarkers) {
				foreach (Venue board in FetchedVenues.VenueList) {
					var marker = new UIMapMarker (board, map, UIMapMarker.SizeMode.Normal);
					ListMapMarkers.Add (marker);
				}
				generatedMarkers |= FetchedVenues.VenueList.Count > 0;

			}
		}

		public void PlaceNewScreen(UIContentDisplay newDisplay){
			map.Alpha = 0f;

			ContentDisplay.UnsuscribeToEvents ();
			ContentDisplay.RemoveFromSuperview ();

			// this updates distance to thumbs everytime user switches screen
			if (newDisplay is UIThumbsContentDisplay) {
				((UIThumbsContentDisplay)newDisplay).GenerateList ();

				var listThumbs = ((UIThumbsContentDisplay)newDisplay).ListThumbComponents;
				if (listThumbs != null) {
					foreach (var thumb in listThumbs) {
						thumb.UpdateDistanceLabel ();
					}
				}

				((UIThumbsContentDisplay)newDisplay).SelectiveThumbsRendering (ScrollView.ContentOffset);
				LastScreenStatus.CurrentScreen = SubScreens.Directory;
			} else if (newDisplay is UICarouselContentDisplay) {
				LastScreenStatus.CurrentScreen = SubScreens.Featured;
				newDisplay.SelectiveRendering (ScrollView.ContentOffset);
			} else if (newDisplay is UITimelineContentDisplay) {
				LastScreenStatus.CurrentScreen = SubScreens.Timeline;
				newDisplay.SelectiveRendering (ScrollView.ContentOffset);
			}
			ContentDisplay = newDisplay;

			ScrollView.AddSubview (ContentDisplay);
			ScrollView.SendSubviewToBack (ContentDisplay);
			ScrollView.ContentSize = new CGSize (AppDelegate.ScreenWidth, newDisplay.Frame.Height);
			ContentDisplaySuscribeToEvents (ContentDisplay);
			// animates banner to be shown

			ScrollView.SetContentOffset (new CGPoint (0, 0), false);

			Banner.SetMainTitle ();

			Banner.AnimateShow();
		}

		public void PlaceNewScreen(UIContentDisplay newDisplay, string screenName, UIFont newFont){
			PlaceNewScreen (newDisplay, screenName, newFont, UIColor.White);
		}

		// 3 lower buttons
		public void PlaceNewScreen(UIContentDisplay newDisplay, string screenName, UIFont newFont, UIColor newColor){
			map.Alpha = 0f;

			ContentDisplay.UnsuscribeToEvents ();
			ContentDisplay.RemoveFromSuperview ();

			// this updates distance to thumbs everytime user switches screen
			if (newDisplay is UIThumbsContentDisplay) {
				((UIThumbsContentDisplay)newDisplay).GenerateList ();

				var listThumbs = ((UIThumbsContentDisplay)newDisplay).ListThumbComponents;
				if (listThumbs != null) {
					foreach (var thumb in listThumbs) {
						thumb.UpdateDistanceLabel ();
					}
				}

				((UIThumbsContentDisplay)newDisplay).SelectiveThumbsRendering (ScrollView.ContentOffset);
				LastScreenStatus.CurrentScreen = SubScreens.Directory;
			} else if (newDisplay is UICarouselContentDisplay) {
				LastScreenStatus.CurrentScreen = SubScreens.Featured;
				newDisplay.SelectiveRendering (ScrollView.ContentOffset);
			} else if (newDisplay is UITimelineContentDisplay) {
				LastScreenStatus.CurrentScreen = SubScreens.Timeline;
				newDisplay.SelectiveRendering (ScrollView.ContentOffset);
			}
			ContentDisplay = newDisplay;

			ScrollView.AddSubview (ContentDisplay);
			ScrollView.SendSubviewToBack (ContentDisplay);
			ScrollView.ContentSize = new CGSize (AppDelegate.ScreenWidth, newDisplay.Frame.Height);
			ContentDisplaySuscribeToEvents (ContentDisplay);
			// animates banner to be shown

			var zeroPoint = new CGPoint (0, 0);
			ScrollView.SetContentOffset (zeroPoint, false);
			ContentDisplay.ForceSelectiveRendering (zeroPoint);

			Banner.ChangeTitle (screenName, newFont, newColor);

			Banner.AnimateShow();
		} 

		public void ScrollsUp(bool animated){
			ScrollView.SetContentOffset (new CGPoint (0, 0), animated);
		}

		// map button
		public void ShowMap(){
			LastScreenStatus.CurrentScreen = SubScreens.Map;

			map.Alpha = 1f;
			GenerateMarkers ();

			// stops scrollview
			ScrollView.SetContentOffset (new CGPoint (0, 0), false);

			// animates banner to be shown
			Banner.AnimateShow();

			Banner.ChangeTitle ("Map", UIFont.SystemFontOfSize(20, UIFontWeight.Medium));

		}
	}
}