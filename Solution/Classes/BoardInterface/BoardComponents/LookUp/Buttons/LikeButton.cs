using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;

namespace Solution
{
	public class LikeButton : Button
	{
		private UIImage likeImage;
		private UIImage unlikeImage;
		private Like like;

		public LikeButton (string contentId, Action<string> updateLabelText)
		{
			uiButton = new UIButton (UIButtonType.Custom);

			UIImage likeImage = UIImage.FromFile ("./boardscreen/lookup/like.png");
			UIImage unlikeImage = UIImage.FromFile ("./boardscreen/lookup/unlike.png");

			like = StorageController.LookupLike (CloudController.BoardUser.Id, contentId);

			if (like.Id == null) {
				// there's no like
				uiButton.SetImage (likeImage, UIControlState.Normal);
			} else {
				uiButton.SetImage (unlikeImage, UIControlState.Normal);
			}

			uiButton.Frame = new CGRect (0,0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint (ButtonSize/2 + 10, ButtonSize/2 + 15);

			uiButton.TouchUpInside += async (object sender, EventArgs e) => {
				if (like.Id == null)
				{
					// add the like
					like = await AppDelegate.CloudController.InsertLikeAsync(contentId);
				}
				else 
				{
					// delete the like
					like = await AppDelegate.CloudController.RemoveLikeAsync(like);
				}


				if (like.Id == null) {
					uiButton.SetImage (likeImage, UIControlState.Normal);
				} else {
					uiButton.SetImage (unlikeImage, UIControlState.Normal);
				}

				updateLabelText(contentId);
			};
		}

		private void UpdateLabel()
		{

		}
	}
}
