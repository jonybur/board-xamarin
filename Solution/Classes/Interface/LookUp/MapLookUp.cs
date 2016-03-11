using Board.Schema;
using System;
using Foundation;
using Google.Maps;
using CoreLocation;
using CoreGraphics;
using UIKit;

namespace Board.Interface.LookUp
{
	public class MapLookUp : LookUp
	{
		MapView mapView;
		bool firstLocationUpdate;

		public MapLookUp(Map map)
		{
			this.content = map;

			UIColor backColor = UIColor.FromRGB(250,250,250);
			UIColor frontColor = AppDelegate.BoardBlack;

			View.BackgroundColor = backColor;

			CreateButtons (frontColor);

			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			ScrollView.UserInteractionEnabled = true;

			LoadMap ();
			ScrollView.AddSubview (mapView);

			View.AddSubviews (ScrollView, BackButton, LikeButton, UberButton, ShareButton, TrashButton);
		}

		public override void ViewDidDisappear(bool animated)
		{
			// unsuscribe from observers, gesture recgonizers, events
			base.ViewDidDisappear(animated);
			mapView.RemoveObserver (this, new NSString ("myLocation"));
		}

		private void LoadMap()
		{
			var camera = CameraPosition.FromCamera (40, -100, -2);

			mapView = MapView.FromCamera (new CGRect (10,
				TrashButton.Frame.Bottom,
				AppDelegate.ScreenWidth - 20,
				AppDelegate.ScreenHeight - TrashButton.Frame.Bottom - LikeButton.Frame.Height), camera);
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

				var location = change.ObjectForKey (NSObject.ChangeNewKey) as CLLocation;
				CreateMarker (location.Coordinate);
			}
		}

		private void CreateMarker(CLLocationCoordinate2D location)
		{
			Random rnd = new Random ();
			double lat = rnd.NextDouble () - .5;
			double lon = rnd.NextDouble () - .5;

			Marker marker = new Marker ();
			marker.AppearAnimation = MarkerAnimation.Pop;
			CLLocationCoordinate2D markerLocation = new CLLocationCoordinate2D (25.792826, -80.129943);
			marker.Position = markerLocation;
			marker.Map = mapView;
			marker.Icon = CreateMarkerImage (BoardInterface.board.ImageView.Image);
			marker.Draggable = false;
			marker.Title = BoardInterface.board.Name;
			marker.Snippet = "2 Cooper Street, Wynwood, FL 33880";
			marker.InfoWindowAnchor = new CGPoint (.5, .5);
			marker.Tappable = true;
			mapView.Camera = CameraPosition.FromCamera (new CLLocationCoordinate2D(25.792826, -80.129953), 16);
		}

		// this one just creates a color square
		private UIImage CreateMarkerImage(UIImage logo)
		{
			UIGraphics.BeginImageContext (new CGSize(66, 96));

			using (UIImage circle = UIImage.FromFile ("./screens/home/map/marker_blue.png")) {
				circle.Draw (new CGRect (0, 0, 66, 96));
			}

			float imgw, imgh;

			float scale = (float)(logo.Size.Height/logo.Size.Width);
			imgw = 40;
			imgh = imgw * scale;

			logo.Draw (new CGRect (33 - imgw / 2, 33 - imgh / 2, imgw, imgh));

			return UIGraphics.GetImageFromCurrentImageContext ();
		}
	}
}

