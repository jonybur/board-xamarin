using System;
using Board.Infrastructure;
using Board.Interface.Buttons;

using Board.Schema;

using CoreGraphics;
using UIKit;

namespace Board.Interface.LookUp
{
	public class ArchiveButton : Button
	{
		public ArchiveButton (Action<UIViewController, bool> presentViewController)
		{
			uiButton = new UIButton (UIButtonType.Custom);

			using (UIImage uiImage = UIImage.FromFile ("./boardinterface/lookup/archive.png")) {
				uiButton.SetImage (uiImage, UIControlState.Normal);
			}

			uiButton.Frame = new CGRect (0,0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint (ButtonSize/2 + ButtonSize * 2 + 10, ButtonSize/2 + 15);

			uiButton.TouchUpInside += (sender, e) => {
				UIAlertController alert = UIAlertController.Create("Are you sure you want to remove this picture from the board?", "The picture will still be visible in the gallery", UIAlertControllerStyle.ActionSheet);

				alert.AddAction (UIAlertAction.Create ("Accept", UIAlertActionStyle.Default, null));
				alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));

				presentViewController (alert, true);
			};
		}

	}
}

