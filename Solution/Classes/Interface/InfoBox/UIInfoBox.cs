using System;
using System.Collections.Generic;
using Board.Infrastructure;
using Board.Interface.LookUp;
using Board.Screens.Controls;
using Board.Utilities;
using CoreGraphics;
using CoreLocation;
using Google.Maps;
using MGImageUtilitiesBinding;
using UIKit;

namespace Board.Interface
{
	public sealed class UIInfoBox : UIScrollView
	{
		public const int XMargin = 10;

		MapContainer Container;
		UILabel NameLabel;
		TopBanner Banner;
		UITextView AboutBox;
		UIActionButtons ActionButtons;

		public UIInfoBox(Board.Schema.Board board){
			Frame = new CGRect (0, 0, AppDelegate.ScreenWidth - XMargin * 2, AppDelegate.ScreenHeight);
			Center = new CGPoint (XMargin + Frame.Width / 2, AppDelegate.ScreenHeight / 2);
			BackgroundColor = UIColor.White;
			ClipsToBounds = true;

			Banner = new TopBanner (board.Image, (float)Frame.Width);

			NameLabel = new UINameLabel (Banner.Bottom + 20, (float)Frame.Width);

			AboutBox = new UIAboutBox (board.About, (float)NameLabel.Frame.Bottom + 10, (float)Frame.Width);

			ActionButtons = new UIActionButtons (board, (float)AboutBox.Frame.Bottom + 10, (float)Frame.Width);

			Container = new MapContainer (Frame);

			AddSubviews (Banner, Container.button, NameLabel, AboutBox);
			foreach (var button in ActionButtons.ListActionButton) {
				AddSubview (button);
			}
		}

		class UIActionButtons{
			public List<UIActionButton> ListActionButton;

			public UIActionButtons(Board.Schema.Board board, float yposition, float infoboxWidth){
				ListActionButton = new List<UIActionButton>();

				ListActionButton.Add(CreateLikeButton());

				if (board.FBPage != null){
					ListActionButton.Add(CreateMessageButton(board.FBPage.Id));
				}

				ListActionButton.Add(CreateCallButton("test"));

				for (int i = 0; i < ListActionButton.Count; i++) {
					float xposition = (infoboxWidth / (ListActionButton.Count + 1)) * (i + 1);
					ListActionButton[i].Center = new CGPoint(xposition, yposition + ListActionButton[i].Frame.Height / 2);
				}
			}

			private UIActionButton CreateLikeButton(){
				var likeButton = new UIActionButton ("like", delegate {
					
				});
				return likeButton;
			}

			private UIActionButton CreateMessageButton(string facebookId){
				var messageButton = new UIActionButton ("message", delegate {
					if (AppsController.CanOpenFacebookMessenger ()) {
						AppsController.OpenFacebookMessenger (facebookId);
					}
				});
				return messageButton;
			}

			private UIActionButton CreateCallButton(string phoneNumber){
				var callButton = new UIActionButton ("call", delegate{
					
				});
				return callButton;
			}

			public class UIActionButton : UIButton{
				public UIActionButton(string buttonName, EventHandler touchUpInside){
					Frame = new CGRect (0, 0, 50, 50);
					//BackgroundColor = UIColor.Red;
					var imageView = new UIImageView();
					using (var image = UIImage.FromFile("./boardinterface/infobox/"+buttonName+".png")){
						imageView.Frame = new CGRect(0, 0, image.Size.Width * .6f, image.Size.Height * .6f);
						imageView.Image = image;
						//SetImage(image, UIControlState.Normal);
					}
					imageView.Center = new CGPoint(Frame.Width / 2, Frame.Height / 2);

					AddSubview(imageView);

					TouchUpInside += touchUpInside;
				}
			}

		}

		sealed class UIAboutBox : UITextView{
			public UIAboutBox(string about, float yposition, float infoboxwidth){
				Frame = new CGRect (UIInfoBox.XMargin, yposition, infoboxwidth - UIInfoBox.XMargin * 2, 100);
				Text = about;
				Font = UIFont.SystemFontOfSize (14);
				BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
				Editable = false;
				Selectable = false;
			}
		}

		sealed class UINameLabel : UILabel{
			public UINameLabel(float yposition, float infoboxwidth){
				Frame = new CGRect (UIInfoBox.XMargin, yposition, infoboxwidth - UIInfoBox.XMargin *2, 24);
				Font = AppDelegate.Narwhal20;
				TextColor = UIColor.Black;
				Text = UIBoardInterface.board.Name;
				TextAlignment = UITextAlignment.Center;
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

				BackgroundImage.ClipsToBounds = true;
				BannerPage = new UIBoardBannerPage (boardImage, width);

				AddSubviews(BackgroundImage, BannerPage);

				LoadCoverImage();
			}

			public async void LoadCoverImage(){
				var localBoard = UIBoardInterface.board;

				if (localBoard.CoverImage == null){
					localBoard.CoverImage = await CommonUtils.DownloadUIImageFromURL(localBoard.CoverImageUrl);

					if (localBoard.CoverImage != null) {
						var scaledImage = localBoard.CoverImage.ImageScaledToFitSize (new CGSize(BackgroundImage.Frame.Width, BackgroundImage.Frame.Width));

						var scaledImageView = new UIImageView (scaledImage);
						// hacer que la imagen esté en un subview de backgroundimage
						if (scaledImageView.Frame.Height < BackgroundImage.Frame.Height){
							scaledImageView.Frame = new CGRect(0,0, (BackgroundImage.Frame.Height * BackgroundImage.Frame.Width) / scaledImageView.Frame.Height, BackgroundImage.Frame.Height);
							scaledImageView.Center = BackgroundImage.Center;
						}

						BackgroundImage.AddSubview (scaledImageView);
					}
				}
			}
		}

		private sealed class MapContainer : UIViewController{
			const int ButtonHeight = 40;
			private MapView mapView;
			public UIButton uberButton, directionsButton;
			public UIButton button;
			UITapGestureRecognizer uberTap, directionsTap, MapTap;
			UIMapMarker mapMarker;

			public MapContainer(CGRect frame){
				CreateMap ((float)frame.Width);

				MapTap = new UITapGestureRecognizer(obj => {
					var lookUp = new MapLookUp(UIBoardInterface.board.GeolocatorObject);
					AppDelegate.PushViewLikePresentView(lookUp);
				});

				button.AddGestureRecognizer (MapTap);
				button.Center = new CGPoint (frame.Width / 2, frame.Height - button.Frame.Height / 2 - 10 - UIBoardScroll.ButtonBarHeight);
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

