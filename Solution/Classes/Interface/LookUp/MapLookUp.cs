using Board.Schema;
using System;
using Foundation;
using Google.Maps;
using Board.Utilities;
using MGImageUtilitiesBinding;
using Board.JsonResponses;
using CoreLocation;
using CoreGraphics;
using UIKit;

namespace Board.Interface.LookUp
{
	public class MapLookUp : UILookUp
	{
		MapView mapView;
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
			Marker marker = new Marker ();
			marker.AppearAnimation = MarkerAnimation.Pop;
			var markerLocation = new CLLocationCoordinate2D(GeolocatorObject.results [0].geometry.location.lat,
				GeolocatorObject.results [0].geometry.location.lng);
			
			marker.Position = markerLocation;
			marker.Map = mapView;
			marker.Icon = CreateMarkerImage (BoardInterface.board.ImageView.Image);
			marker.Draggable = false;
			marker.Title = BoardInterface.board.Name;
			try{
				marker.Snippet = GeolocatorObject.results [0].address_components [0].long_name + " " +
					GeolocatorObject.results [0].address_components [1].short_name;
			}catch{
			}
			marker.InfoWindowAnchor = new CGPoint (.5, .5);
			marker.Tappable = true;
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

	}
}

