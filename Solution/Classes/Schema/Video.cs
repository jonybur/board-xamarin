using CoreGraphics;
using UIKit;
using Board.Utilities;
using System;
using Foundation;

namespace Board.Schema
{
	public class Video : Content
	{
		public NSUrl Url;

		public UIImageView ThumbnailView;

		public Video() {
			Id = CommonUtils.GenerateGuid ();
		}

		public Video(NSUrl url, UIImageView thumbnailView, float rotation, CGPoint position, string creatorid, DateTime creationdate)
		{
			Url = url;
			ThumbnailView = thumbnailView;
			Rotation = rotation;
			Position = position;
			CreatorId = creatorid;
			CreationDate = creationdate;
		}
	}
}

