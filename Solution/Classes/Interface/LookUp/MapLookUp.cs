using Board.Schema;
using System;
using Board.Screens.Controls;
using Foundation;
using Board.Infrastructure;
using Google.Maps;
using Board.Utilities;
using Board.JsonResponses;
using CoreLocation;
using CoreGraphics;
using UIKit;

namespace Board.Interface.LookUp
{
	public class MapLookUp : UILookUp
	{
		MapView mapView;
		UIMapMarker mapMarker;
		bool firstLocationUpdate;
		GoogleGeolocatorObject GeolocatorObject;
		UIButton uberButton, directionsButton;
		UITapGestureRecognizer uberTap, directionsTap;
		const int ButtonHeight = 50;

		public MapLookUp(GoogleGeolocatorObject geolocatorObject)
		{	
			content = new Map ();

			GeolocatorObject = geolocatorObject;

			UIColor backColor = UIColor.Black;

			View.BackgroundColor = backColor;

			CreateButtons (UIColor.Black);

			LoadMap ();

			View.AddSubviews (mapView, BackButton);

			CreateUberButton ();
			CreateDirectionsButton ();
		}

		public override void ViewDidDisappear(bool animated)
		{
			// unsuscribe from observers, gesture recgonizers, events
			base.ViewDidDisappear(animated);
			mapView.RemoveObserver (this, new NSString ("myLocation"));
			mapView = null;

			uberButton.RemoveGestureRecognizer (uberTap);
			directionsButton.RemoveGestureRecognizer (directionsTap);

			MemoryUtility.ReleaseUIViewWithChildren (View);
		}

		private void LoadMap()
		{
			var camera = CameraPosition.FromCamera (new CLLocationCoordinate2D(GeolocatorObject.results [0].geometry.location.lat,
				GeolocatorObject.results [0].geometry.location.lng), 16);

			mapView = MapView.FromCamera (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight), camera);
			mapView.Settings.CompassButton = true;
			mapView.Settings.MyLocationButton = true;
			mapView.UserInteractionEnabled = true;
			mapView.AddObserver (this, new NSString ("myLocation"), NSKeyValueObservingOptions.New, IntPtr.Zero);

			var edgeInsets = new UIEdgeInsets (0, 0, ButtonHeight, 0);
			mapView.Padding = edgeInsets;

			InvokeOnMainThread (()=> mapView.MyLocationEnabled = true);
		}

		public override void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			if (!firstLocationUpdate) {
				firstLocationUpdate = true; 
				CreateMarker ();
			}
		}

		private void CreateMarker()
		{
			mapMarker = new UIMapMarker (UIBoardInterface.board, mapView, UIMapMarker.SizeMode.Normal);
		}
			
		private void CreateUberButton(){
			uberButton = new UIButton ();
			uberButton.Frame = new CGRect (0, mapView.Frame.Height - ButtonHeight, mapView.Frame.Width / 2 - 1, ButtonHeight);
			uberButton.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 200);
			uberButton.SetTitle ("UBER", UIControlState.Normal);

			uberButton.UserInteractionEnabled = true;

			uberTap = new UITapGestureRecognizer (tg => {

				if (AppsController.CanOpenUber()) {

					var location = new CLLocationCoordinate2D(UIBoardInterface.board.GeolocatorObject.results [0].geometry.location.lat,
						UIBoardInterface.board.GeolocatorObject.results [0].geometry.location.lng);

					var listproducts = CloudController.GetUberProducts(location);

					UIAlertController alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);
					foreach(var product in listproducts.products){
						alert.AddAction (UIAlertAction.Create (product.display_name, UIAlertActionStyle.Default, delegate {
							AppsController.OpenUber (product.product_id, location);
						}));	
					}
					alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));

					AppDelegate.NavigationController.PresentViewController(alert, true, null);
				}
				else {
					var alert = UIAlertController.Create("No Uber app installed", "To use this function please install Uber", UIAlertControllerStyle.Alert);
					alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));
					AppDelegate.NavigationController.PresentViewController (alert, true, null);
				}
			});

			uberButton.AddGestureRecognizer (uberTap);
			View.AddSubview (uberButton);
		}

		private void CreateDirectionsButton(){
			directionsButton = new UIButton ();
			directionsButton.Frame = new CGRect (mapView.Frame.Width / 2 + 1, mapView.Frame.Height - ButtonHeight, mapView.Frame.Width / 2 - 1, ButtonHeight);
			directionsButton.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 200);
			directionsButton.SetTitle ("DIRECTIONS", UIControlState.Normal);

			directionsButton.UserInteractionEnabled = true;

			directionsTap = new UITapGestureRecognizer (tg => {

				var location = new CLLocationCoordinate2D(UIBoardInterface.board.GeolocatorObject.results [0].geometry.location.lat,
					UIBoardInterface.board.GeolocatorObject.results [0].geometry.location.lng);

				UIAlertController alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);

				bool canOpenWaze = AppsController.CanOpenWaze();
				bool canOpenGoogleMaps = AppsController.CanOpenGoogleMaps();

				if (canOpenWaze || canOpenGoogleMaps){

					if (canOpenGoogleMaps){
						alert.AddAction (UIAlertAction.Create ("Google Maps", UIAlertActionStyle.Default, obj => AppsController.OpenGoogleMaps (location)));
					}
					alert.AddAction (UIAlertAction.Create ("Apple Maps", UIAlertActionStyle.Default, obj => AppsController.OpenAppleMaps (location)));
					if (canOpenWaze){
						alert.AddAction (UIAlertAction.Create ("Waze", UIAlertActionStyle.Default, obj => AppsController.OpenWaze (location)));
					}

					alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));

					AppDelegate.NavigationController.PresentViewController(alert, true, null);

				} else {
					AppsController.OpenAppleMaps (location);

				}
			});

			directionsButton.AddGestureRecognizer (directionsTap);
			View.AddSubview (directionsButton);
		}
	}
}

