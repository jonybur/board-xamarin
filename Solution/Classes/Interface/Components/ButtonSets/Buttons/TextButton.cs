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
	public class TextButton : Button
	{
		public TextButton (UINavigationController navigationController, UIScrollView scrollView, Action refreshContent)
		{
			UIImage uiImage;

			uiImage = UIImage.FromFile ("./boardscreen/buttons/text.png");
			uiButton = new UIButton (UIButtonType.Custom);
			uiButton.Alpha = 0f;
			uiButton.SetImage (uiImage, UIControlState.Normal);

			uiButton.Frame = new CGRect (0, 0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint ((BoardInterface.ScreenWidth + ButtonSize) / 2 +
			(BoardInterface.ScreenWidth - ButtonSize) / 8, BoardInterface.ScreenHeight - ButtonSize / 2);

			uiButton.TouchUpInside += async (object sender, EventArgs e) => {
				TextCreator tc = new TextCreator (navigationController, refreshContent);

				navigationController.PresentViewController (tc, true, null);
			};
		}
	}
}

