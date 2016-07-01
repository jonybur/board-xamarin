using System.Collections.Generic;
using Clubby.Infrastructure;
using Clubby.Schema;
using CoreGraphics;
using Clubby.Interface.LookUp;
using Foundation;
using Haneke;
using UIKit;

namespace Clubby.Interface
{
	class UIInstagramGallery : UIScrollView {
		public static float ButtonSize;
		List<UIButton> InstagramPhotos;

		public UIInstagramGallery (float width, float yposition, List<Content> contents){
			ScrollEnabled = false;
			ButtonSize = width / 3 - 10;
			Frame = new CGRect (0, 0, width, ButtonSize * 2);

			InstagramPhotos = new List<UIButton> ();

			int imagesCount = (contents.Count > 11) ? 11 : contents.Count;

			if (imagesCount != 0) {
				foreach (Picture picture in contents) {
					SetImage (picture);
				}

				SetInstagramThumb ();
			}

			Fill ();

			Center = new CGPoint (Center.X, yposition + Frame.Height/2);
		}

		private void SetInstagramThumb(){
			var pictureButton = new UIButton(new CGRect (0, 0, ButtonSize, ButtonSize));
			var imageView = new UIImageView (pictureButton.Frame);
			imageView.SetImage ("./boardinterface/infobox/viewmore3.png");
			pictureButton.Alpha = .5f;
			pictureButton.AddSubview(imageView);
			pictureButton.Layer.CornerRadius = 10;
			pictureButton.ClipsToBounds = true;
			pictureButton.TouchUpInside += (sender, e) => {
				// opens instagram
				if (AppsController.CanOpenInstagram()){
					AppsController.OpenInstagram(UIVenueInterface.venue.InstagramId);
				}

				//location?id=LOCATION_ID
			};

			InstagramPhotos.Add(pictureButton);
		}

		private void SetImage (Picture picture){
			var pictureButton = new UIButton(new CGRect (0, 0, ButtonSize, ButtonSize));
			var imageView = new UIImageView (pictureButton.Frame);
			imageView.SetImage (new NSUrl(picture.ThumbnailImageUrl));
			pictureButton.AddSubview(imageView);
			pictureButton.Layer.CornerRadius = 10;
			pictureButton.ClipsToBounds = true;
			pictureButton.TouchUpInside += (sender, e) => {
				var lookUp = new PictureLookUp(picture);
				AppDelegate.PushViewLikePresentView(lookUp);
			};

			InstagramPhotos.Add(pictureButton);
		}

		private void Fill (){
			int x = 3; float y = 1;
			float lastBottom = 0;
			foreach (var button in InstagramPhotos) {
				button.Center = new CGPoint ((Frame.Width / 16) * x, (ButtonSize + 5) * y - ButtonSize / 2);

				x += 5;

				if (x >= 16) {
					x = 3;
					y ++;
				}

				AddSubview (button);
				lastBottom = (float)button.Frame.Bottom;
			}

			ContentSize = new CGSize (Frame.Width, (float)lastBottom + 1);
			ContentOffset = new CGPoint (0, 0);
			Frame = new CGRect (0, 0, Frame.Width, lastBottom);
		}
	}
}