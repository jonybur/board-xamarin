using CoreGraphics;
using UIKit;
using System;
using Clubby.Schema;
using Plugin.Share;

namespace Clubby.Interface.LookUp
{
	public class UILookUp : UIViewController
	{
		public Content content;

		protected UIScrollView ScrollView;
		protected UIImageView BackButton;
		protected UIImageView LikeButton;

		UITapGestureRecognizer backTap;
		UITapGestureRecognizer likeTap;

		UIWindow window;

		public UILookUp()
		{
			AutomaticallyAdjustsScrollViewInsets = false;
			View = new UIView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
		}

		public void CreateButtons(UIColor buttonColor)
		{
			CreateBackButton (buttonColor);

			if (content != null) {
				CreateLikeButton (buttonColor);
			}
		}

		public override void ViewDidAppear(bool animated)
		{
			BackButton.AddGestureRecognizer (backTap);
			if (LikeButton != null) {
				LikeButton.AddGestureRecognizer (likeTap);
			}
		}

		public override void ViewDidDisappear(bool animated)
		{
			BackButton.RemoveGestureRecognizer (backTap);
			if (LikeButton != null) {
				LikeButton.RemoveGestureRecognizer (likeTap);
			}
		}

		protected void CreateBackButton(UIColor buttonColor)
		{
			using (UIImage img = UIImage.FromFile ("./boardinterface/lookup/cancel.png")) {
				BackButton = new UIImageView(new CGRect(0,0,img.Size.Width * 2,img.Size.Height * 2));

				UIImageView subView = new UIImageView (new CGRect (0, 0, img.Size.Width / 2, img.Size.Height / 2));
				subView.Image = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				subView.Center = new CGPoint (BackButton.Frame.Width / 2, BackButton.Frame.Height / 2);
				subView.TintColor = buttonColor;

				BackButton.AddSubview (subView);
				BackButton.Center = new CGPoint (img.Size.Width / 2 + 10, 35);
			}
			 
			BackButton.UserInteractionEnabled = true;

			backTap = new UITapGestureRecognizer (tg => {
				// user tapped on "Done" button
				AppDelegate.PopViewControllerLikeDismissView();
			});
		}

		protected void CreateLikeButton(UIColor buttonColor)
		{
			UIImageView subView;
			UILabel lblLikes;

			int likes = content.Likes;

			using (UIImage img = UIImage.FromFile ("./boardinterface/lookup/like.png")) {
				UIFont font = UIFont.SystemFontOfSize (14);

				LikeButton = new UIImageView(new CGRect(0, 0, img.Size.Width + likes.ToString().StringSize(font).Width + 5, img.Size.Height * 2));

				subView = new UIImageView (new CGRect (0, 0, img.Size.Width / 2, img.Size.Height / 2));
				subView.Image = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				subView.Center = new CGPoint (img.Size.Width / 2, BackButton.Frame.Height / 2);

				lblLikes = new UILabel (new CGRect (0, 0, likes.ToString().StringSize(font).Width + 10, 14));
				lblLikes.Font = font;
				lblLikes.Text = likes.ToString();
				lblLikes.Center = new CGPoint (subView.Center.X + lblLikes.Frame.Width / 2 + 15, subView.Center.Y);

				LikeButton.AddSubviews(subView, lblLikes);
				LikeButton.Center = new CGPoint (LikeButton.Frame.Width / 2 + 10, AppDelegate.ScreenHeight - 25);
			}

			bool isLiked = false;

			if (isLiked) {
				subView.TintColor = AppDelegate.ClubbyOrange;
				lblLikes.TextColor = AppDelegate.ClubbyOrange;
			} else {
				subView.TintColor = buttonColor;
				lblLikes.TextColor = buttonColor;
			}

			LikeButton.UserInteractionEnabled = true;

			bool liked = false;

			likeTap = new UITapGestureRecognizer (tg => {
				if (!liked)
				{
					likes++;
					lblLikes.Text = likes.ToString();
					subView.TintColor = AppDelegate.ClubbyOrange;
					lblLikes.TextColor = AppDelegate.ClubbyOrange;
					liked = true;
				}
				else 
				{
					likes--;
					lblLikes.Text = likes.ToString();
					subView.TintColor = buttonColor;
					lblLikes.TextColor = buttonColor;
					liked = false;
				}
			});
		}
	}
}

