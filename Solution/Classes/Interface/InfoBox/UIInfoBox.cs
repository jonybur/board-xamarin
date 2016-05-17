using Board.Screens.Controls;
using CoreGraphics;
using Google.Maps;
using MGImageUtilitiesBinding;
using UIKit;
using Board.Utilities;
using Board.Interface.LookUp;
using BigTed;
using Board.Infrastructure;
using Foundation;
using CoreLocation;

namespace Board.Interface
{
	public sealed class UIInfoBox : UIScrollView
	{
		public const int XMargin = 10;

		MapContainer Container;
		UILabel NameLabel, PhoneLabel, OpenLabel;
		UIActionButton[] ActionButtons;
		UIGalleryScrollView GallerySV;
		TopBanner Banner;
		UITextView AboutBox;

		int CantActionButtons = 3;

		public UIInfoBox(Board.Schema.Board board){
			Frame = new CGRect (0, 0, AppDelegate.ScreenWidth - XMargin * 2, AppDelegate.ScreenHeight - UIBoardScroll.ButtonBarHeight);
			Center = new CGPoint (UIBoardScroll.ScrollViewWidthSize / 2 + XMargin, AppDelegate.ScreenHeight / 2 - UIBoardScroll.ButtonBarHeight / 2);
			BackgroundColor = UIColor.White;
			ClipsToBounds = true;

			Banner = new TopBanner (board.Image, (float)Frame.Width);

			NameLabel = new UILabel ();
			NameLabel.Frame = new CGRect (0, Banner.Bottom + 15, Frame.Width, 24);
			NameLabel.Font = UIFont.SystemFontOfSize (22);
			NameLabel.TextColor = UIColor.Black;
			NameLabel.Text = UIBoardInterface.board.Name;
			NameLabel.TextAlignment = UITextAlignment.Center;

			Container = new MapContainer (Frame);

			AboutBox = new UITextView ();
			AboutBox.Frame = new CGRect (0, NameLabel.Frame.Bottom + 15, Frame.Width, 100);
			AboutBox.Text = board.About;
			AboutBox.Font = UIFont.SystemFontOfSize (16);
			AboutBox.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
			AboutBox.Editable = false;
			AboutBox.Selectable = false;

			AddSubviews (Banner, Container.button, NameLabel, AboutBox);
		}

		class UIActionButton : UIButton{
			public UIActionButton(string imageName){
				Frame = new CGRect (0, 0, 50, 50);

				using (UIImage img = UIImage.FromFile("./boardinterface/infobox/"+imageName+".png"))
				{
					SetImage(img, UIControlState.Normal);
				}
			}
		}

		private class TopBanner : UIView {
			UIImageView BackgroundImage;
			UIBoardBannerPage BannerPage;

			public float Bottom{
				get { 
					return (float)BackgroundImage.Frame.Bottom;
				}
			}


			public TopBanner(UIImage boardImage, float width){
				BackgroundImage = new UIImageView (new CGRect(0, 0, width, UIMagazineBannerPage.Height));

				using (UIImage img = UIImage.FromFile ("./demo/infobox/american_cover.png")) {
					var scaledImage = img.ImageScaledToFitSize (BackgroundImage.Frame.Size);
					BackgroundImage.Image = scaledImage;
				}

				BannerPage = new UIBoardBannerPage (boardImage);

				AddSubviews(BackgroundImage, BannerPage);
			}
		}

		private class MapContainer : UIViewController{
			private MapView mapView;
			const int ButtonHeight = 40;
			public UIButton uberButton, directionsButton;
			UITapGestureRecognizer uberTap, directionsTap, MapTap;
			public UIButton button;
			UIMapMarker mapMarker;

			public MapContainer(CGRect frame){
				CreateMap ((float)frame.Width);

				MapTap = new UITapGestureRecognizer(obj => {
					var lookUp = new MapLookUp(UIBoardInterface.board.GeolocatorObject);
					AppDelegate.PushViewLikePresentView(lookUp);
				});

				button.AddGestureRecognizer (MapTap);
				button.Center = new CGPoint (frame.Width / 2, frame.Height - button.Frame.Height / 2 - 10);
				button.ClipsToBounds = true;

				CreateDirectionsButton();
				CreateUberButton();
			}


			private void CreateUberButton(){
				uberButton = new UIButton ();
				uberButton.Frame = new CGRect (0, button.Frame.Height - ButtonHeight, button.Frame.Width / 2 - 1, ButtonHeight);
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
				button.AddSubview (uberButton);
			}

			private void CreateDirectionsButton(){
				directionsButton = new UIButton ();
				directionsButton.Frame = new CGRect (button.Frame.Width / 2 + 1, button.Frame.Height - ButtonHeight, button.Frame.Width / 2 - 2, ButtonHeight);
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
							alert.AddAction (UIAlertAction.Create ("Waze", UIAlertActionStyle.Default, obj => AppsController.OpenWaze (location)));
						}

						alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));

						AppDelegate.NavigationController.PresentViewController(alert, true, null);

					} else {
						AppsController.OpenAppleMaps (location);
						
					}
				});

				directionsButton.AddGestureRecognizer (directionsTap);
				button.AddSubview (directionsButton);
			}

			private void CreateMap(float width) {
				var camera = CameraPosition.FromCamera (40, -100, -2);
				button = new UIButton (new CGRect (20, 0, width-40, 200));
				mapView = MapView.FromCamera (new CGRect (0, 0, button.Frame.Width, button.Frame.Height), camera);
				mapView.UserInteractionEnabled = false;
				mapView.Layer.AllowsEdgeAntialiasing = true;
				mapView.MyLocationEnabled = true;
				mapView.Camera = CameraPosition.FromCamera (UIBoardInterface.board.GeolocatorObject.Coordinate, 16);
				mapMarker = new UIMapMarker (UIBoardInterface.board, mapView);

				var edgeInsets = new UIEdgeInsets (0, 0, ButtonHeight, 0);
				mapView.Padding = edgeInsets;

				button.Layer.CornerRadius = 10;
				mapView.Layer.CornerRadius = 10;
				View.Layer.CornerRadius = 10;

				button.AddSubview (mapView);
			}

			public override void ViewDidUnload ()
			{
				uberButton.RemoveGestureRecognizer (uberTap);
				directionsButton.RemoveGestureRecognizer (directionsTap);
				button.RemoveGestureRecognizer(MapTap);
				mapMarker.Dispose ();
				MemoryUtility.ReleaseUIViewWithChildren (View);	
			}
		}
	}
}

