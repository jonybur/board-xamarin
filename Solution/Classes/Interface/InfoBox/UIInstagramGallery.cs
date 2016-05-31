using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using MGImageUtilitiesBinding;

namespace Board.Interface
{
	class UIInstagramGallery : UIScrollView {
		public static float ButtonSize;
		List<UIButton> InstagramPhotos;

		public UIInstagramGallery (float width, float yposition, List<UIImage> images){
			ScrollEnabled = false;
			ButtonSize = width / 3 - 10;
			Frame = new CGRect (0, 0, width, ButtonSize * 2);

			InstagramPhotos = new List<UIButton> ();

			int imagesCount = (images.Count > 11) ? 11 : images.Count;

			if (imagesCount != 0) {
				for (int i = 0; i < imagesCount; i++) {
					SetImage (images [i]);
				}
				SetInstagramThumb ();
			}

			Fill ();

			Center = new CGPoint (Center.X, yposition + Frame.Height/2);
		}

		private void SetInstagramThumb(){
			var image = UIImage.FromFile ("./boardinterface/infobox/viewmore2.png");
			var pictureButton = new UIButton(new CGRect (0, 0, ButtonSize, ButtonSize));
			var fixedImg = image.ImageCroppedToFitSize(pictureButton.Frame.Size);
			var imageView = new UIImageView (fixedImg);
			pictureButton.Alpha = .75f;
			pictureButton.AddSubview(imageView);
			pictureButton.Layer.CornerRadius = 10;
			pictureButton.ClipsToBounds = true;
			pictureButton.TouchUpInside += (sender, e) => {
				// opens instagram
			};

			InstagramPhotos.Add(pictureButton);
		}

		private void SetImage (UIImage image){
			if (image == null) {
				return;
			}

			var pictureButton = new UIButton(new CGRect (0, 0, ButtonSize, ButtonSize));
			var fixedImg = image.ImageCroppedToFitSize(pictureButton.Frame.Size);
			var imageView = new UIImageView (fixedImg);
			pictureButton.AddSubview(imageView);
			pictureButton.Layer.CornerRadius = 10;
			pictureButton.ClipsToBounds = true;
			pictureButton.TouchUpInside += (sender, e) => {
				var picture = new Board.Schema.Picture(image, "", new CGPoint(), "", CGAffineTransform.MakeIdentity());
				var pictureLookUp = new Board.Interface.LookUp.PictureLookUp(picture);
				AppDelegate.PushViewLikePresentView(pictureLookUp);
			};

			InstagramPhotos.Add(pictureButton);
		}

		private void Fill(){
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