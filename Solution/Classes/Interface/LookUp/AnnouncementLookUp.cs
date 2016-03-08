﻿using Board.Interface.Buttons;

using Board.Schema;

using CoreGraphics;
using Facebook.CoreKit;
using UIKit;

namespace Board.Interface.LookUp
{
	public class AnnouncementLookUp : LookUp
	{
		public AnnouncementLookUp(Announcement announcement)
		{
			this.content = announcement;

			View.BackgroundColor = UIColor.FromRGB(250,250,250);

			CreateButtons (BoardInterface.board.MainColor);

			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			ScrollView.UserInteractionEnabled = true;

			UITextView textView = LoadTextView (announcement);

			ScrollView.AddSubview (textView);

			View.AddSubviews (ScrollView, BackButton, LikeButton, FacebookButton, ShareButton, TrashButton);
		}

		private UITextView LoadTextView(Announcement announcement){

			UITextView textView = new UITextView(new CGRect (10,
				TrashButton.Frame.Bottom,
				AppDelegate.ScreenWidth - 20,
				AppDelegate.ScreenHeight - TrashButton.Frame.Bottom - LikeButton.Frame.Height));
			
			textView.AttributedText = announcement.Text;
			textView.Editable = false;
			textView.Selectable = true;
			textView.ScrollEnabled = true;
			textView.TextColor = BoardInterface.board.MainColor;
			textView.BackgroundColor = UIColor.FromRGB (250, 250, 250);

			return textView;

		}
	}
}

