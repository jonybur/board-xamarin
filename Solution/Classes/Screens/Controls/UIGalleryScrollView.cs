using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using MGImageUtilitiesBinding;
using System;

namespace Board.Screens.Controls
{
	public class UIGalleryScrollView : UIScrollView {
		private List<UIPictureButton> Pictures;
		public static float ButtonSize;
		float CornerRadius;

		private class UIPictureButton : UIButton{
			public EventHandler TouchUpInsideEvent;

			protected override void Dispose (bool disposing)
			{
				if (TouchUpInsideEvent != null) {
					TouchUpInside -= TouchUpInsideEvent;
				}
			}

			public UIPictureButton(CGRect frame){
				Frame = frame;
			}
		}

		public UIGalleryScrollView(float width, float height = 200, float imageCornerRadius = 0) {
			Frame = new CGRect (0, 0, width, height);
			Pictures = new List<UIPictureButton>();
			ScrollEnabled = true;
			UserInteractionEnabled = true;
			ButtonSize = (width / 4 - 2);
			CornerRadius = imageCornerRadius;
		}

		public void SetImage (UIImage image, EventHandler touchUpInsideEvent){
			if (image == null) {
				return;
			}

			var pictureButton = new UIPictureButton(new CGRect (0, 0, ButtonSize, ButtonSize));
			var fixedImg = image.ImageCroppedToFitSize(pictureButton.Frame.Size);
			pictureButton.SetImage (fixedImg, UIControlState.Normal);
			pictureButton.Layer.CornerRadius = CornerRadius;
			pictureButton.TouchUpInsideEvent = touchUpInsideEvent;
			pictureButton.TouchUpInside += touchUpInsideEvent;

			Pictures.Add(pictureButton);
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

