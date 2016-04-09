using System;
using System.Collections.Generic;
using CoreGraphics;
using CoreLocation;
using Google.Maps;
using MGImageUtilitiesBinding;
using UIKit;
using Board.Schema;
using Board.Interface.LookUp;

namespace Board.Interface
{
	public sealed class InfoBox : UIView
	{
		UITextView DescriptionBox;
		MapContainer Container;
		UILabel NameLabel, AddressLabel, PhoneLabel, OpenLabel;
		UIActionButton[] ActionButtons;
		GalleryScrollView GallerySV;
		UITapGestureRecognizer MapTap;

		int CantActionButtons = 3;

		class UIActionButton : UIButton{
			public UIActionButton(string imageName){
				Frame = new CGRect (0, 0, 50, 50);

				using (UIImage img = UIImage.FromFile("./boardinterface/infobox/"+imageName+".png"))
				{
					SetImage(img, UIControlState.Normal);
				}
			}
		}

		public override void RemoveFromSuperview ()
		{
			Container.button.RemoveGestureRecognizer (MapTap);
			base.RemoveFromSuperview ();
		}
	
		public InfoBox (Board.Schema.Board board)
		{
			Frame = new CGRect (10, 0, AppDelegate.ScreenWidth - 20, AppDelegate.ScreenHeight - UIBoardScroll.BannerHeight - UIBoardScroll.ButtonBarHeight - 30);
			Center = new CGPoint (UIBoardScroll.ScrollViewWidthSize / 2, AppDelegate.ScreenHeight / 2 + (UIBoardScroll.BannerHeight - UIBoardScroll.ButtonBarHeight) / 2);
			BackgroundColor = UIColor.FromRGBA (0, 0, 0, 100);
			Layer.CornerRadius = 10;
			ClipsToBounds = true;

			NameLabel = new UILabel (new CGRect (10, 0, Frame.Width-20, 24));
			NameLabel.Font = AppDelegate.Narwhal24;
			NameLabel.TextColor = UIColor.White;
			NameLabel.TextAlignment = UITextAlignment.Center;
			NameLabel.Text = board.Name;
			NameLabel.AdjustsFontSizeToFitWidth = true;
			NameLabel.Center = new CGPoint (Frame.Width / 2, NameLabel.Frame.Height + 5);

			var Line1 = CreateLine (UIColor.White, (float)NameLabel.Frame.Bottom + 10);

			DescriptionBox = new UITextView (new CGRect(10, 0, Frame.Width - 20, 0));
			DescriptionBox.Editable = false;
			DescriptionBox.Selectable = false;
			DescriptionBox.ScrollEnabled = false;
			DescriptionBox.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
			DescriptionBox.DataDetectorTypes = UIDataDetectorType.Link;
			DescriptionBox.Text = "American Social Brickell overlooks the Miami River and invites guests to enjoy an endless selection of local and regional craft beers.";
			DescriptionBox.TextColor = UIColor.White;
			DescriptionBox.Font = AppDelegate.SystemFontOfSize16;
			DescriptionBox.SizeToFit ();
			DescriptionBox.LayoutIfNeeded ();
			DescriptionBox.Center = new CGPoint (Frame.Width / 2, Line1.Frame.Bottom + DescriptionBox.Frame.Height / 2 + 5);

			float actionButtonsY = (float)DescriptionBox.Frame.Bottom + 30;
			ActionButtons = new UIActionButton[CantActionButtons];
			ActionButtons[0] = new UIActionButton ("addstar4");
			ActionButtons [0].Center = new CGPoint ((Frame.Width / 8) * 1, actionButtonsY);
			ActionButtons[1] = new UIActionButton ("call3");
			ActionButtons [1].Center = new CGPoint ((Frame.Width / 8) * 4, actionButtonsY);
			ActionButtons[2] = new UIActionButton ("message2");
			ActionButtons [2].Center = new CGPoint ((Frame.Width / 8) * 7, actionButtonsY);
			AddSubviews (ActionButtons);

			GallerySV = new GalleryScrollView ((float)Frame.Width);
			GallerySV.BackgroundColor = UIColor.FromRGBA(0,0,0,50);
			GallerySV.Center = new CGPoint (Frame.Width / 2, actionButtonsY + GallerySV.Frame.Height / 2 + 35);
			GallerySV.Fill ();

			Container = new MapContainer ();
			Container.CreateMap ((float)Frame.Width);

			MapTap = new UITapGestureRecognizer(obj => {
				UILookUp lookUp = new MapLookUp(new Map());
				AppDelegate.PushViewLikePresentView(lookUp);
			});

			Container.button.AddGestureRecognizer (MapTap);
			Container.button.Center = new CGPoint (Frame.Width / 2, Frame.Height - Container.button.Frame.Height / 2);

			AddSubviews (NameLabel, Line1, DescriptionBox, Container.button, GallerySV);
		}

		protected class GalleryScrollView : UIScrollView {
			readonly List<UIButton> Pictures;
			float ButtonSize;

			public GalleryScrollView(float width) {
				Frame = new CGRect (0, 0, width, 200);
				Pictures = new List<UIButton>();
				ScrollEnabled = true;
				UserInteractionEnabled = true;
				ButtonSize = (width / 4 - 2);

				int j = 0;

				for (int i = 0; i < 15; i++) {

					var button = new UIButton(new CGRect (0, 0, ButtonSize, ButtonSize));
					button.BackgroundColor = UIColor.Black;

					using (UIImage img = UIImage.FromFile("./demo/pictures/"+j+".jpg")) {
						UIImage fixedImg = img.ImageCroppedToFitSize(button.Frame.Size);
						button.SetImage (fixedImg, UIControlState.Normal);
					}
					Pictures.Add(button);

					j++;

					if (7 <= j)
					{
						j = 0;
					}
				}
			}

			public void Fill(){
				int x = 1; float y = 1;
				float lastBottom = 0;
				foreach (var button in Pictures) {
					button.Center = new CGPoint ((Frame.Width / 8) * x, (ButtonSize + 2) * y - ButtonSize / 2);

					x += 2;

					if (x >= 8) {
						x = 1;
						y ++;
					}

					AddSubview (button);
					lastBottom = (float)button.Frame.Bottom;
				}
				ContentSize = new CGSize (Frame.Width, (float)lastBottom + 1);
				ContentOffset = new CGPoint (0, ContentSize.Height - Frame.Size.Height);
			}
		}

	
		private UIImageView CreateLine(UIColor color, float yposition){
			var line = new UIImageView (new CGRect (0, yposition, Frame.Width, 1));
			line.BackgroundColor = UIColor.White;
			line.Alpha = .5f;
			return line;
		}

		private class MapContainer : UIViewController{
			private MapView mapView;
			public UIButton button;
			bool firstLocationUpdate;

			public void CreateMap(float width) {
				var camera = CameraPosition.FromCamera (40, -100, -2);
				mapView = MapView.FromCamera (new CGRect (0, 0, width, 130), camera);
				mapView.UserInteractionEnabled = false;
				mapView.Layer.AllowsEdgeAntialiasing = true;
				CreateMarker (new CLLocationCoordinate2D());

				button = new UIButton (new CGRect (0, 0, mapView.Frame.Width, mapView.Frame.Height));
				button.AddSubview (mapView);
			}

			private void CreateMarker(CLLocationCoordinate2D location) {
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

