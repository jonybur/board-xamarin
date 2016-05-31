using Board.Infrastructure;
using Board.Interface.LookUp;
using Board.Screens.Controls;
using Board.Utilities;
using CoreGraphics;
using CoreLocation;
using Google.Maps;
using UIKit;

namespace Board.Interface
{
	public class UIMapContainer : UIViewController
	{
		const int ButtonHeight = 40;
		private MapView mapView;
		public UIButton uberButton, directionsButton;
		public UIButton Map;
		UITapGestureRecognizer uberTap, directionsTap, MapTap;
		UIMapMarker mapMarker;

		public UIMapContainer(CGRect frame){
			CreateMap ((float)frame.Width);

			MapTap = new UITapGestureRecognizer(obj => {
				var lookUp = new MapLookUp(UIBoardInterface.board.GeolocatorObject);
				AppDelegate.PushViewLikePresentView(lookUp);
			});

			Map.AddGestureRecognizer (MapTap);
			Map.Center = new CGPoint (frame.Width / 2, frame.Height - Map.Frame.Height / 2 - 10 - UIBoardScroll.ButtonBarHeight * 2);
			Map.ClipsToBounds = true;

			CreateDirectionsButton();
			CreateUberButton();
		}

		private void CreateUberButton(){
			uberButton = new UIButton ();
			uberButton.Frame = new CGRect (0, Map.Frame.Height - ButtonHeight, Map.Frame.Width / 2 - 1, ButtonHeight);
			uberButton.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 200);
			uberButton.SetTitle ("UBER", UIControlState.Normal);

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
					UIAlertController alert = UIAlertController.Create("No Uber app installed", "To use this function please install Uber", UIAlertControllerStyle.Alert);
					alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));
					AppDelegate.NavigationController.PresentViewController (alert, true, null);
				}
			});

			uberButton.AddGestureRecognizer (uberTap);
			Map.AddSubview (uberButton);
		}

		private void CreateDirectionsButton(){
			directionsButton = new UIButton ();
			directionsButton.Frame = new CGRect (Map.Frame.Width / 2 + 1, Map.Frame.Height - ButtonHeight, Map.Frame.Width / 2 - 2, ButtonHeight);
			directionsButton.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 200);
			directionsButton.SetTitle ("DIRECTIONS", UIControlState.Normal);

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
						alert.AddAction (UIAlertAction.Create ("Waze Maps", UIAlertActionStyle.Default, obj => AppsController.OpenWaze (location)));
					}

					alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));

					AppDelegate.NavigationController.PresentViewController(alert, true, null);

				} else {
					AppsController.OpenAppleMaps (location);

				}
			});

			directionsButton.AddGestureRecognizer (directionsTap);
			Map.AddSubview (directionsButton);
		}

		private void CreateMap(float width) {
			var camera = CameraPosition.FromCamera (40, -100, -2);
			Map = new UIButton (new CGRect (20, 0, width-40, 160));
			mapView = MapView.FromCamera (new CGRect (0, 0, Map.Frame.Width, Map.Frame.Height), camera);
			mapView.UserInteractionEnabled = false;
			mapView.Layer.AllowsEdgeAntialiasing = true;
			mapView.MyLocationEnabled = true;
			mapView.Camera = CameraPosition.FromCamera (UIBoardInterface.board.GeolocatorObject.Coordinate, 16);
			mapMarker = new UIMapMarker (UIBoardInterface.board, mapView);

			var edgeInsets = new UIEdgeInsets (0, 0, ButtonHeight, 0);
			mapView.Padding = edgeInsets;

			Map.Layer.CornerRadius = 10;
			mapView.Layer.CornerRadius = 10;
			View.Layer.CornerRadius = 10;

			Map.AddSubview (mapView);
		}

		public override void ViewDidUnload ()
		{
			uberButton.RemoveGestureRecognizer (uberTap);
			directionsButton.RemoveGestureRecognizer (directionsTap);
			Map.RemoveGestureRecognizer(MapTap);
			mapMarker.Dispose ();
			MemoryUtility.ReleaseUIViewWithChildren (View);	
		}
	}
}

