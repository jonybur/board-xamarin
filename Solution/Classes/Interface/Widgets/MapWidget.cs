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
			CreateMounting (container.mapView.Frame.Size);
			Frame = MountingView.Frame;
			AddSubviews (MountingView, container.mapView);

			

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
			}
		}

	}
}

