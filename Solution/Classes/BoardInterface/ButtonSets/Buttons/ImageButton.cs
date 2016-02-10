using System;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;

using System.Net.Http;

using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace Solution
{
	public class ImageButton : Button
	{
		public ImageButton (UINavigationController navigationController, UIScrollView scrollView)
		{
			uiButton = new UIButton (UIButtonType.Custom);

			UIImage uiImage = UIImage.FromFile ("./boardscreen/buttons/picture6.png");

			uiButton = new UIButton (UIButtonType.Custom);
			uiButton.SetImage (uiImage, UIControlState.Normal);

			uiButton.Frame = new CGRect (0, 0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint ((AppDelegate.ScreenWidth - ButtonSize) / 8 * 3 - 5, AppDelegate.ScreenHeight - ButtonSize / 2 - 10);

			uiButton.TouchUpInside += (object sender, EventArgs e) => {
				ImagePicker ip = new ImagePicker (scrollView, UIImagePickerControllerSourceType.PhotoLibrary, navigationController);
				navigationController.PresentViewController (ip.UIImagePicker, true, null);
			};
		}
	}
}

