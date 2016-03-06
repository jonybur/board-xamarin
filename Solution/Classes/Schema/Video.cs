﻿using CoreGraphics;
using UIKit;
using Board.Utilities;
using Foundation;

namespace Board.Schema
{
	public class Video : Content
	{
		public NSUrl Url { get; set; }

		public UIImageView ThumbnailView { get; set; }

		public Video() {
			Id = CommonUtils.GenerateGuid ();
		}

		public Video(NSUrl url, UIImageView thumbnailView, float rotation, CGRect frame, string userid)
		{
			Url = url;
			ThumbnailView = thumbnailView;
			Rotation = rotation;
			Frame = frame;
			UserId = userid;
		}
	}
}

