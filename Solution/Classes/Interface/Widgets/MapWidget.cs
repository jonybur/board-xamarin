using Google.Maps;
using UIKit;
using CoreLocation;
using CoreGraphics;
using System;
using Board.Schema;

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

			View.Frame = new CGRect (_map.Position.X, _map.Position.Y, MountingView.Frame.Width, MountingView.Frame.Height);
			View.Transform = CGAffineTransform.MakeRotation(_map.Rotation);

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
				CreateMarker (new CLLocationCoordinate2D());
			}

			private void CreateMarker(CLLocationCoordinate2D location)
			{
				Random rnd = new Random ();
				double lat = rnd.NextDouble () - .5;
				double lon = rnd.NextDouble () - .5;

				Marker marker = new Marker ();
				marker.AppearAnimation = MarkerAnimation.Pop;
				CLLocationCoordinate2D markerLocation = new CLLocationCoordinate2D (25.792826, -80.12994);
				marker.Position = markerLocation;
				marker.Map = mapView;
				marker.Icon = CreateMarkerImage (BoardInterface.board.ImageView.Image);
				marker.Draggable = false;
				mapView.Camera = CameraPosition.FromCamera (new CLLocationCoordinate2D(25.792826, -80.12994), 16);
			}

			// this one just creates a color square
			private UIImage CreateMarkerImage(UIImage logo)
			{
				UIGraphics.BeginImageContext (new CGSize(44, 64));

				using (UIImage circle = UIImage.FromFile ("./screens/main/map/marker_blue.png")) {
					circle.Draw (new CGRect (0, 0, 44, 64));
				}

				float imgw, imgh;

				float scale = (float)(logo.Size.Height/logo.Size.Width);
				imgw = 25;
				imgh = imgw * scale;

				logo.Draw (new CGRect (22 - imgw / 2, 22 - imgh / 2, imgw, imgh));

				return UIGraphics.GetImageFromCurrentImageContext ();
			}
		}


	}
}

