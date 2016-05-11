using Board.Screens.Controls;
using CoreGraphics;
using Google.Maps;
using MGImageUtilitiesBinding;
using UIKit;
using Board.Utilities;
using Board.Interface.LookUp;

namespace Board.Interface
{
	public sealed class UIInfoBox : UIScrollView
	{
		public const int XMargin = 10;

		UITextView DescriptionBox;
		MapContainer Container;
		UILabel NameLabel, PhoneLabel, OpenLabel;
		UIActionButton[] ActionButtons;
		UIGalleryScrollView GallerySV;
		TopBanner Banner;

		int CantActionButtons = 3;

		public UIInfoBox(Board.Schema.Board board){
			Frame = new CGRect (0, 0, AppDelegate.ScreenWidth - XMargin * 2, AppDelegate.ScreenHeight - UIBoardScroll.ButtonBarHeight);
			Center = new CGPoint (UIBoardScroll.ScrollViewWidthSize / 2 + XMargin, AppDelegate.ScreenHeight / 2 - UIBoardScroll.ButtonBarHeight / 2);
			BackgroundColor = UIColor.White;
			ClipsToBounds = true;

			Container = new MapContainer (Frame);

			Banner = new TopBanner (board.Image, (float)Frame.Width);

			AddSubviews (Banner, Container.button);
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
			private UIMapMarker mapMarker;
			public UIButton button;
			UITapGestureRecognizer MapTap;

			const int ButtonHeight = 40;
			public UIButton uberButton, directionsButton;

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
				directionsButton = new UIButton ();
				directionsButton.Frame = new CGRect (0, button.Frame.Height - ButtonHeight, button.Frame.Width / 2 - 1, ButtonHeight);
				directionsButton.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 200);
				directionsButton.SetTitle ("UBER", UIControlState.Normal);

				button.AddSubview (directionsButton);
			}

			private void CreateDirectionsButton(){
				directionsButton = new UIButton ();
				directionsButton.Frame = new CGRect (button.Frame.Width / 2 + 1, button.Frame.Height - ButtonHeight, button.Frame.Width / 2 - 2, ButtonHeight);
				directionsButton.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 200);
				directionsButton.SetTitle ("DIRECTIONS", UIControlState.Normal);

				button.AddSubview (directionsButton);
			}

			private void CreateMap(float width) {
				var camera = CameraPosition.FromCamera (40, -100, -2);
				button = new UIButton (new CGRect (20, 0, width-40, 200));
				mapView = MapView.FromCamera (new CGRect (0, 0, button.Frame.Width, button.Frame.Height), camera);
				mapView.UserInteractionEnabled = false;
				mapView.Layer.AllowsEdgeAntialiasing = true;
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
				button.RemoveGestureRecognizer(MapTap);
				mapMarker.Dispose ();
				MemoryUtility.ReleaseUIViewWithChildren (View);	
			}
		}
	}
}

