using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using MGImageUtilitiesBinding;

namespace Board.Screens.Controls
{
	public class GalleryScrollView : UIScrollView {
		private List<UIButton> Pictures;
		float ButtonSize;

		public void SetImages (List<UIImage> listImages){
			var button = new UIButton(new CGRect (0, 0, ButtonSize, ButtonSize));

			foreach (var img in listImages){
				UIImage fixedImg = img.ImageCroppedToFitSize(button.Frame.Size);
				button.SetImage (fixedImg, UIControlState.Normal);
				Pictures.Add(button);
			}

		}

		public void SetImage (UIImage image){
			var button = new UIButton(new CGRect (0, 0, ButtonSize, ButtonSize));
			UIImage fixedImg = image.ImageCroppedToFitSize(button.Frame.Size);
			button.SetImage (fixedImg, UIControlState.Normal);
			Pictures.Add(button);
		}

		public void SetDemoImages(){
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

		public GalleryScrollView(float width, float height = 200) {
			Frame = new CGRect (0, 0, width, height);
			Pictures = new List<UIButton>();
			ScrollEnabled = true;
			UserInteractionEnabled = true;
			ButtonSize = (width / 4 - 2);
		}

		public void Fill(bool presentFromBottom, float yPosition){
			int x = 1; float y = 1;
			float lastBottom = 0;
			foreach (var button in Pictures) {
				button.Center = new CGPoint ((Frame.Width / 8) * x, (ButtonSize + 2) * y - ButtonSize / 2 + yPosition);

				x += 2;

				if (x >= 8) {
					x = 1;
					y ++;
				}

				AddSubview (button);
				lastBottom = (float)button.Frame.Bottom;
			}

			ContentSize = new CGSize (Frame.Width, (float)lastBottom + 1);

			if (presentFromBottom) {
				ContentOffset = new CGPoint (0, ContentSize.Height - Frame.Size.Height);
			} else {
				ContentOffset = new CGPoint (0, -21);
				Frame = new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);
			}
		}
	}

}

