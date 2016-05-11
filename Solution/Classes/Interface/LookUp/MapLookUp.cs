using Board.Schema;
using System;
using Board.Screens.Controls;
using Foundation;
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

		public MapLookUp(GoogleGeolocatorObject geolocatorObject)
		{	
			content = new Map ();

			GeolocatorObject = geolocatorObject;

			UIColor frontColor = UIColor.FromRGB(250,250,250);
			UIColor backColor = UIColor.Black;

			View.BackgroundColor = backColor;

			CreateButtons (UIColor.Black);

			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			ScrollView.UserInteractionEnabled = true;

			LoadMap ();
			ScrollView.AddSubview (mapView);

			View.AddSubviews (ScrollView, BackButton, UberButton);
		}

		public override void ViewDidDisappear(bool animated)
		{
			// unsuscribe from observers, gesture recgonizers, events
			base.ViewDidDisappear(animated);
			mapView.RemoveObserver (this, new NSString ("myLocation"));
			mapView = null;
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
	}
}

