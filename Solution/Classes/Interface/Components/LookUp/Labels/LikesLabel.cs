using System;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;

namespace Solution
{
	public class LikesLabel : UILabel
	{
		public LikesLabel (string contentid)
		{		
			const int LabelHeight = 21;

			Frame = new CGRect (0, AppDelegate.ScreenHeight - LabelHeight - 25, AppDelegate.ScreenWidth, LabelHeight);
			TextAlignment = UITextAlignment.Center;
			BackgroundColor = UIColor.Clear;
			TextColor = UIColor.White;

			UpdateText (contentid);
		}

		public void UpdateText(string contentid)
		{
			int numberofLikes = StorageController.ReturnNumberOfLikes (contentid);

			if (numberofLikes > 1) {
				this.Text = numberofLikes + " Likes";
			} else if (numberofLikes == 1){
				this.Text = numberofLikes + " Like";
			} else if (numberofLikes == 0) {
				this.Text = "No likes";
			}
		}
	}
}

