﻿using System.Collections.Generic;
using Board.Facebook;
using Board.Infrastructure;
using Board.Interface.Buttons;
using Board.Utilities;
using CoreAnimation;
using CoreGraphics;
using Foundation;
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
			TimeStamp.Text = CommonUtils.GetFormattedTimeDifference (content.CreationDate);
			TimeStamp.TextAlignment = UITextAlignment.Center;
			TimeStamp.TextColor = UIColor.FromRGB (140, 140, 140);
		}

		protected void CreateEye()
		{
			EyeView = new UIImageView(new CGRect (MountingView.Frame.Width - IconSize - SideMargin - 5,
										MountingView.Frame.Height - IconSize - 5, IconSize, IconSize));

			if (StorageController.WasContentSeen(content.Id)){
				EyeView.Image = OpenEyeImageView.Image;
				EyeView.TintColor = AppDelegate.BoardOrange;
				EyeOpen = true;
			}else{
				EyeView.Image = ClosedEyeImageView.Image;
				EyeView.TintColor = WidgetGrey;
				EyeOpen = false;
			}

		}

		public void OpenEye()
		{
			// widget hasnt been drawn yet
			if (EyeView == null) {
				return;
			}

			StorageController.SetContentAsSeen (content.Id);

			EyeView.Image = OpenEyeImageView.Image;
			EyeView.TintColor = AppDelegate.BoardOrange;
			EyeOpen = true;

			var scale = new CAKeyFrameAnimation ();
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

		private void CreateLikeComponent()
		{
			LikeComponent = new UIImageView (new CGRect (SideMargin,
				MountingView.Frame.Height - 40, 80, 40));
			
			if (UIBoardInterface.DictionaryUserLikes.ContainsKey (content.Id)) {
				liked = UIBoardInterface.DictionaryUserLikes [content.Id];
			}

			likeView = CreateLike (LikeComponent.Frame);
			LikeLabel = CreateLikeLabel (likeView.Center);

			if (liked) {
				likeView.TintColor = AppDelegate.BoardOrange;
				LikeLabel.TextColor = AppDelegate.BoardOrange;
			} else {
				likeView.TintColor = WidgetGrey;
				LikeLabel.TextColor = WidgetGrey;
			}

			LikeComponent.AddSubviews (likeView, LikeLabel);
		}

		public void Like()
		{
			if (!liked)
			{
				CloudController.SendLike(content.Id);
				
				likes ++;
				LikeLabel.Text = likes.ToString();

				likeView.TintColor = AppDelegate.BoardOrange;
				LikeLabel.TextColor = AppDelegate.BoardOrange;

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
			} else {
				CloudController.SendDislike(content.Id);

				likes --;
				LikeLabel.Text = likes.ToString();

				likeView.TintColor = WidgetGrey;
				LikeLabel.TextColor = WidgetGrey;

				liked = false;
			}
		}
			
		protected UIImageView CreateLike(CGRect frame)
		{
			var likeView = new UIImageView(new CGRect(0, 0, IconSize, IconSize));

			using (var img = UIImage.FromFile ("./boardinterface/widget/like.png")) {
				likeView.Image = img;
				likeView.Image = likeView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
			}

			likeView.Center = new CGPoint (IconSize / 2, frame.Height / 2);
			return likeView;
		}

		protected UILabel CreateLikeLabel(CGPoint center)
		{
			int fontSize = 14;

			UIFont likeFont = UIFont.SystemFontOfSize (fontSize);

			CGSize likeLabelSize = likes.ToString().StringSize (likeFont);

			UILabel likeLabel = new UILabel();
			likeLabel.Frame = new CGRect (IconSize, center.Y - likeLabelSize.Height / 2, likeLabelSize.Width + 20, likeLabelSize.Height);

			likeLabel.Font = likeFont;
			if (UIBoardInterface.DictionaryLikes.ContainsKey (content.Id)) {
				likes = UIBoardInterface.DictionaryLikes [content.Id];
			}
			likeLabel.Text = likes.ToString();
			//likeLabel.Center = new CGPoint (center.X + likeLabel.Frame.Width / 2 + 5, center.Y);
			likeLabel.TextAlignment = UITextAlignment.Left;

			FacebookUtils.MakeGraphRequest (content.FacebookId, "?fields=likes", LoadFacebookLike);

			return likeLabel;
		}

		private void LoadFacebookLike(List<FacebookElement> obj){

			if (obj.Count > 0) {
				int facebookLikeCount = 0;
				var fblikes = (FacebookLikes)obj [0];

				if (fblikes.LikesData != null) {
					facebookLikeCount = CommonUtils.CountStringOccurrences (fblikes.LikesData, "id");
				}

				likes += facebookLikeCount;
				LikeLabel.Text = likes.ToString ();
			}
		}


	}
}

