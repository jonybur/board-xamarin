using System;
using Board.Schema;
using CoreGraphics;
using CoreLocation;
using Google.Maps;
using MGImageUtilitiesBinding;
using UIKit;

namespace Board.Interface.Widgets
{
	public class MapWidget : Widget
	{
		MapContainer container;

		public Map map
		{
			get { return (Map)content; }
		}

		public MapWidget(Map _map)
		{
			content = _map;

			container = new MapContainer ();
			container.CreateMap ();

			// mounting
			CreateMounting (container.mapView.Frame);
			View = new UIView(MountingView.Frame);
			View.AddSubviews (MountingView, container.mapView);

			EyeOpen = false;

			CreateGestures ();
		}

		private class MapContainer : UIViewController{
			
			public MapView mapView;

			bool firstLocationUpdate;

			public void CreateMap()
			{
				var camera = CameraPosition.FromCamera (40, -100, -2);
				mapView = MapView.FromCamera (new CGRect (10, 10, 250, 170), camera);
				mapView.UserInteractionEnabled = false;
				mapView.Layer.AllowsEdgeAntialiasing = true;
				CreateMarker (new CLLocationCoordinate2D());
			}

			private void CreateMarker(CLLocationCoordinate2D location)
			{
				Random rnd = new Random ();
				double lat = rnd.NextDouble () - .5;
				double lon = rnd.NextDouble () - .5;

				Marker marker = new Marker ();
				marker.AppearAnimation = MarkerAnimation.Pop;
				var markerLocation = new CLLocationCoordinate2D (25.792826, -80.12994);
				marker.Position = markerLocation;
				marker.Map = mapView;
				marker.Icon = CreateMarkerImage (UIBoardInterface.board.ImageView.Image);
				marker.Draggable = false;
				mapView.Camera = CameraPosition.FromCamera (new CLLocationCoordinate2D(25.792826, -80.12994), 16);
			}


			private UIImage CreateMarkerImage(UIImage logo)
			{
				UIGraphics.BeginImageContextWithOptions (new CGSize (44, 64), false, 2f);

				using (UIImage container = UIImage.FromFile ("./screens/main/map/markercontainer.png")) {
					container.Draw (new CGRect (0, 0, 44, 64));
				}

				float autosize = 25;

				logo = logo.ImageScaledToFitSize (new CGSize(autosize,autosize));

				logo.Draw (new CGRect (22 - logo.Size.Width / 2, 22 - logo.Size.Height / 2, logo.Size.Width, logo.Size.Height));

				return UIGraphics.GetImageFromCurrentImageContext ();
			}
		}

	}
}

