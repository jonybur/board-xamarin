using System;
using System.Collections.Generic;
using Board.Infrastructure;
using Board.Interface.Buttons;
using Board.Interface.LookUp;
using Board.Schema;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using MGImageUtilitiesBinding;
using UIKit;

namespace Board.Interface.Widgets
{
	public partial class Widget
	{
		protected void CreateDeleteButton(){
			DeleteButton = new UIButton ();
			DeleteButton.Frame = new CGRect (0, 0, 80, 80);

			using (var image = UIImage.FromFile ("./boardinterface/widget/deletebut.png")) {
				var imageView = new UIImageView ();
				imageView.Frame = new CGRect (0, 0, image.Size.Width / 2, image.Size.Height / 2);
				imageView.Image = image;
				imageView.Center = new CGPoint (DeleteButton.Frame.Width / 2, DeleteButton.Frame.Height / 2);

				DeleteButton.AddSubview (imageView); 
				//DeleteButton.SetImage(image, UIControlState.Normal);
			}

			DeleteButton.TouchUpInside += (sender, e) => {

				var alert = UIAlertController.Create("Delete widget?", null, UIAlertControllerStyle.Alert);

				alert.AddAction (UIAlertAction.Create ("Delete", UIAlertActionStyle.Default, RemoveWidget));
				alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));	

				AppDelegate.NavigationController.PresentViewController(alert, true, null);
			};

			DeleteButton.Center = new CGPoint (0, 0);
		}

		private void CreateTimeStamp()
		{
			TimeStamp = new UILabel (new CGRect (0, 0, MountingView.Frame.Width, 40));
			TimeStamp.Font = UIFont.SystemFontOfSize (14);
			TimeStamp.Center = new CGPoint (MountingView.Frame.Width / 2, LikeComponent.Center.Y);

			TimeSpan timeDifference = DateTime.Now.Subtract (content.CreationDate);

			if (timeDifference.TotalSeconds < 60) {
				TimeStamp.Text = timeDifference.Seconds + "s";
			} else if (timeDifference.TotalMinutes < 60) {
				TimeStamp.Text = timeDifference.Minutes + "m";
			} else if (timeDifference.TotalHours < 24) {
				TimeStamp.Text = timeDifference.Hours + "h";
			} else if (timeDifference.TotalDays < 7) {
				TimeStamp.Text = timeDifference.Days + "d";
			} else {
				TimeStamp.Text = (timeDifference.Days/7) + "w";
			}

			TimeStamp.TextAlignment = UITextAlignment.Center;
			TimeStamp.TextColor = UIColor.FromRGB (140, 140, 140);
		}

		protected void CreateEye()
		{
			EyeView = new UIImageView(new CGRect (MountingView.Frame.Width - IconSize - SideMargin - 5, MountingView.Frame.Height - IconSize - 5, IconSize, IconSize));
			EyeView.Image = ClosedEyeImageView.Image;
			EyeView.TintColor = WidgetGrey;
		}

		private void CreateLikeComponent()
		{
			LikeComponent = new UIImageView (new CGRect (SideMargin,
				MountingView.Frame.Height - 40, 80, 40));

			likeView = CreateLike (LikeComponent.Frame);
			likeLabel = CreateLikeLabel (likeView.Center);

			LikeComponent.AddSubviews (likeView, likeLabel);
		}

		public void Like()
		{
			if (!liked)
			{
				likes ++;
				likeLabel.Text = likes.ToString();
				likeView.TintColor = AppDelegate.BoardOrange;
				likeLabel.TextColor = AppDelegate.BoardOrange;

				CAKeyFrameAnimation scale = new CAKeyFrameAnimation ();
				scale.KeyPath = "transform";

				var identity = CATransform3D.MakeScale(1f, 1f, 1f);
				var scaled = CATransform3D.MakeScale (1.3f, 1.3f, 1.3f);

				scale.Values = new NSObject[]{ 
					NSValue.FromCATransform3D (identity),
					NSValue.FromCATransform3D (scaled),
					NSValue.FromCATransform3D (identity)
				};

				scale.KeyTimes = new NSNumber[]{0, 1/2, 1};
				scale.Additive = true;
				scale.Duration = .5f;
				scale.RemovedOnCompletion = false;

				likeView.Layer.AddAnimation (scale, "highlight");

				liked = true;
			}
			else {
				likes --;
				likeLabel.Text = likes.ToString();
				likeView.TintColor = WidgetGrey;
				likeLabel.TextColor = WidgetGrey;
				liked = false;
			}
		}

		public void OpenEye()
		{
			// widget hasnt been drawn yet
			if (EyeView == null) {
				return;
			}
			
			EyeView.Image = OpenEyeImageView.Image;
			EyeView.TintColor = AppDelegate.BoardOrange;
			EyeOpen = true;

			CAKeyFrameAnimation scale = new CAKeyFrameAnimation ();
			scale.KeyPath = "transform";

			var identity = CATransform3D.MakeScale(1f, 1f, 1f);
			var scaled = CATransform3D.MakeScale (1.3f, 1.3f, 1.3f);

			scale.Values = new NSObject[]{ 
				NSValue.FromCATransform3D (identity),
				NSValue.FromCATransform3D (scaled),
				NSValue.FromCATransform3D (identity)
			};

			scale.KeyTimes = new NSNumber[]{0, 1/2, 1};
			scale.Duration = .5f;
			scale.RemovedOnCompletion = false;

			EyeView.Layer.AddAnimation (scale, "highlight");

			ButtonInterface.navigationButton.SubtractNavigationButtonText();
		}

		protected UIImageView CreateLike(CGRect frame)
		{
			var likeView = new UIImageView(new CGRect(0, 0, IconSize, IconSize));

			using (UIImage img = UIImage.FromFile ("./boardinterface/widget/like.png")) {
				likeView.Image = img;
				likeView.Image = likeView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
			}

			likeView.TintColor = WidgetGrey;
			likeView.Center = new CGPoint (IconSize / 2, frame.Height / 2);
			return likeView;
		}

		protected UILabel CreateLikeLabel(CGPoint center)
		{
			int fontSize = 14;

			UIFont likeFont = UIFont.SystemFontOfSize (fontSize);

			CGSize likeLabelSize = likes.ToString().StringSize (likeFont);

			UILabel likeLabel = new UILabel(new CGRect(0, 0, likeLabelSize.Width + 20, likeLabelSize.Height));
			likeLabel.TextColor = WidgetGrey;
			likeLabel.Font = likeFont;
			likeLabel.Text = likes.ToString();
			likeLabel.Center = new CGPoint (center.X + likeLabel.Frame.Width / 2 + 5, center.Y);
			likeLabel.TextAlignment = UITextAlignment.Center;

			return likeLabel;
		}

	}
}

