﻿using Board.Schema;
using CoreGraphics;
using UIKit;

namespace Board.Interface.LookUp
{
	public class AnnouncementLookUp : UILookUp
	{
		public AnnouncementLookUp(Announcement announcement)
		{
			content = announcement;

			UIColor frontColor = UIColor.White;
			UIColor backColor = UIColor.Black;

			View.BackgroundColor = backColor;

			CreateButtons (frontColor);

			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			ScrollView.UserInteractionEnabled = true;

			UITextView textView = LoadTextView (announcement, frontColor);

			ScrollView.AddSubview (textView);

			View.AddSubviews (ScrollView, BackButton, LikeButton, FacebookButton, ShareButton, TrashButton, EditButton);
		}

		private UITextView LoadTextView(Announcement announcement, UIColor color){

			UITextView textView = new UITextView(new CGRect (10,
				TrashButton.Frame.Bottom,
				AppDelegate.ScreenWidth - 20,
				AppDelegate.ScreenHeight - TrashButton.Frame.Bottom - LikeButton.Frame.Height));

			if (announcement.AttributedText != null) {
				textView.AttributedText = announcement.AttributedText;
			} else {
				textView.Text = announcement.Text;
			}
			textView.Editable = false;
			textView.ScrollEnabled = true;
			textView.Selectable = true;
			textView.DataDetectorTypes = UIDataDetectorType.Link;
			textView.UserInteractionEnabled = true;
			textView.TextColor = color;
			textView.BackgroundColor = UIColor.FromRGBA (250, 250, 250, 0);

			return textView;

		}
	}
}

