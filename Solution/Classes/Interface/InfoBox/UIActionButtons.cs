using System;
using System.Collections.Generic;
using Board.Infrastructure;
using UIKit;
using Haneke;
using Foundation;
using CoreGraphics;

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

		private UIActionButton CreateLikeButton(){
			var likeLabel = new UILabel ();
			likeLabel = new UILabel();
			likeLabel.Font = UIFont.SystemFontOfSize(20, UIFontWeight.Light);
			likeLabel.Text = "0";

			var likeButton = new UIActionButton ("emptylike", delegate { });
			bool isLiked = false;

			likeButton.TouchUpInside += (sender, e) => {
				if (!isLiked){
					likeButton.ChangeImage(fullHeart);
				} else {
					likeButton.ChangeImage(emptyHeart);
				}
				isLiked = !isLiked;
			};

			var sizeLikeLabel = likeLabel.Text.StringSize (likeLabel.Font);
			likeLabel.Frame = new CGRect(0, 0, sizeLikeLabel.Width, sizeLikeLabel.Height);
			likeLabel.Center = new CGPoint (likeButton.Frame.Right + likeLabel.Frame.Width / 2 + 5, likeButton.Center.Y);

			likeButton.AddSubview (likeLabel);

			return likeButton;
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

