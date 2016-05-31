using System;
using System.Collections.Generic;
using Board.Infrastructure;
using Board.Interface.LookUp;
using Board.Screens.Controls;
using Board.Utilities;
using UIKit;
using CoreGraphics;
using CoreLocation;
using Google.Maps;
using Haneke;

namespace Board.Interface
{
	class UIActionButtons
	{
		
		public List<UIActionButton> ListActionButton;

		public UIActionButtons(Board.Schema.Board board, float yposition, float infoboxWidth){
			ListActionButton = new List<UIActionButton>();

			ListActionButton.Add(CreateLikeButton());

			if (board.FBPage != null){
				ListActionButton.Add(CreateMessageButton(board.FBPage.Id));
			}

			ListActionButton.Add(CreateCallButton("test"));

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

		private UIActionButton CreateLikeButton(){
			var likeButton = new UIActionButton ("like", delegate {

			});
			return likeButton;
		}

		private UIActionButton CreateMessageButton(string facebookId){
			var messageButton = new UIActionButton ("message", delegate {
				if (AppsController.CanOpenFacebookMessenger ()) {
					AppsController.OpenFacebookMessenger (facebookId);
				}
			});
			return messageButton;
		}

		private UIActionButton CreateCallButton(string phoneNumber){
			var callButton = new UIActionButton ("call", delegate{

			});
			return callButton;
		}

		public class UIActionButton : UIButton{
			public UIActionButton(string buttonName, EventHandler touchUpInside){
				Frame = new CGRect (0, 0, 50, 50);
				var imageView = new UIImageView();
				using (var image = UIImage.FromFile("./boardinterface/infobox/"+buttonName+".png")){
					imageView.Frame = new CGRect(0, 0, image.Size.Width * .6f, image.Size.Height * .6f);
					imageView.Image = image;
					//SetImage(image, UIControlState.Normal);
				}
				imageView.Center = new CGPoint(Frame.Width / 2, Frame.Height / 2);

				AddSubview(imageView);

				TouchUpInside += touchUpInside;
			}
		}

	}
}

