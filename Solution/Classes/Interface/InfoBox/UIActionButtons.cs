﻿using System;
using System.Collections.Generic;
using Board.Infrastructure;
using CoreGraphics;
using Haneke;
using Board.Facebook;
using UIKit;

namespace Board.Interface
{
	class UIActionButtons
	{	
		public List<UIActionButton> ListActionButton;

		public UIActionButtons(Board.Schema.Board board, float yposition, float infoboxWidth){
			ListActionButton = new List<UIActionButton>();

			ListActionButton.Add(CreateLikeButton());

			if (board.FacebookId != null){
				ListActionButton.Add(CreateMessageButton(board.FacebookId));
			}

			if (board.Phone != null) {
				ListActionButton.Add (CreateCallButton (board.Phone));
			}

			float xposition = infoboxWidth / 8;

			switch (ListActionButton.Count){
			case 1:

				ListActionButton[0].Center = new CGPoint(xposition * 4, yposition + ListActionButton[0].Frame.Height / 2);
				break;

			case 2:

				ListActionButton[0].Center = new CGPoint(xposition * 2, yposition + ListActionButton[0].Frame.Height / 2);
				ListActionButton[1].Center = new CGPoint(xposition * 6, yposition + ListActionButton[1].Frame.Height / 2);
				break;

			case 3:

				ListActionButton[0].Center = new CGPoint(xposition * 1, yposition + ListActionButton[0].Frame.Height / 2);
				ListActionButton[1].Center = new CGPoint(xposition * 4, yposition + ListActionButton[1].Frame.Height / 2);
				ListActionButton[2].Center = new CGPoint(xposition * 7, yposition + ListActionButton[2].Frame.Height / 2);
				break;

			case 4:

				ListActionButton[0].Center = new CGPoint(xposition * 1, yposition + ListActionButton[0].Frame.Height / 2);
				ListActionButton[1].Center = new CGPoint(xposition * 3, yposition + ListActionButton[1].Frame.Height / 2);
				ListActionButton[3].Center = new CGPoint(xposition * 5, yposition + ListActionButton[2].Frame.Height / 2);
				ListActionButton[4].Center = new CGPoint(xposition * 7, yposition + ListActionButton[3].Frame.Height / 2);
				break;
			}
		}

		const string fullHeart = "fulllike", emptyHeart = "emptylike";

		UILabel likeLabel;
		int likes;

		private UIActionButton CreateLikeButton(){
			likeLabel = new UILabel ();
			likeLabel = new UILabel();
			likeLabel.Font = UIFont.SystemFontOfSize(20, UIFontWeight.Light);

			likes = UIBoardInterface.DictionaryLikes[UIBoardInterface.board.Id];
			likeLabel.Text = string.Empty;

			bool isLiked = UIBoardInterface.DictionaryUserLikes[UIBoardInterface.board.Id];
			var firstImage = isLiked ? fullHeart : emptyHeart;
			var likeButton = new UIActionButton (firstImage, delegate { });

			likeButton.TouchUpInside += (sender, e) => {
				if (!isLiked){
					CloudController.SendLike(UIBoardInterface.board.Id);
					likes ++;
					likeButton.ChangeImage(fullHeart);
				} else {
					CloudController.SendDislike(UIBoardInterface.board.Id);
					likes --;
					likeButton.ChangeImage(emptyHeart);
				}
				likeLabel.Text = likes.ToString();
				isLiked = !isLiked;
			};

			var sizeLikeLabel = likeLabel.Text.StringSize (likeLabel.Font);
			likeLabel.Frame = new CGRect(0, 0, sizeLikeLabel.Width + 20, sizeLikeLabel.Height);
			likeLabel.Center = new CGPoint (likeButton.Frame.Right + likeLabel.Frame.Width / 2 + 5, likeButton.Center.Y);

			likeButton.AddSubview (likeLabel);

			FacebookUtils.MakeGraphRequest (UIBoardInterface.board.FacebookId, "?fields=fan_count", LoadFanCount);

			return likeButton;
		}

		private void LoadFanCount(List<FacebookElement> obj){
			if (obj.Count > 0) {
				var fanCount = (FacebookFanCount)obj [0];
				likes += fanCount.Count;
			}
			likeLabel.Text = likes.ToString ();
			var sizeLikeLabel = likeLabel.Text.StringSize (likeLabel.Font);
			likeLabel.Frame = new CGRect (likeLabel.Frame.X, likeLabel.Frame.Y, sizeLikeLabel.Width + 20, sizeLikeLabel.Height);
		}

		private UIActionButton CreateMessageButton(string facebookId){
			var messageButton = new UIActionButton ("message", delegate {
				if (AppsController.CanOpenFacebookMessenger ()) {
					AppsController.OpenFacebookMessenger (facebookId);
				} else {
					var alert = UIAlertController.Create("No Facebook Messenger app installed", "To use this function please install Facebook Messenger", UIAlertControllerStyle.Alert);
					alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));
					AppDelegate.NavigationController.PresentViewController (alert, true, null);
				}
			});
			return messageButton;
		}

		private UIActionButton CreateCallButton(string phoneNumber){
			var callButton = new UIActionButton ("call", delegate{
				if (AppsController.CanOpenPhone()){
					phoneNumber = phoneNumber.Replace(" ", string.Empty);
					AppsController.OpenPhone(phoneNumber);
				}
			});
			return callButton;
		}

		public class UIActionButton : UIButton{
			UIImageView imageView;

			public UIActionButton(string buttonName, EventHandler touchUpInside){
				Frame = new CGRect (0, 0, 50, 50);

				imageView = new UIImageView();
				imageView.Frame = new CGRect(0, 0, Frame.Size.Width * .5f, Frame.Size.Height * .5f);
				imageView.SetImage("./boardinterface/infobox/"+buttonName+".png");
				imageView.Center = new CGPoint(Frame.Width / 2, Frame.Height / 2);

				AddSubview(imageView);

				TouchUpInside += touchUpInside;
			}

			public void ChangeImage(string newImage){
				imageView.SetImage("./boardinterface/infobox/"+newImage+".png");
			}
		}

	}
}

