using CoreGraphics;
using UIKit;
using Board.Utilities;
using System.Runtime.Serialization;
using System;
using Foundation;

namespace Board.Schema
{
	public class Video : Content
	{
		[IgnoreDataMember]
		public NSUrl Url;

		public string UrlText;

		[IgnoreDataMember]
		public UIImageView ThumbnailView;

		public string Description;

		public Video() { }

		public Video(NSUrl url, UIImageView thumbnailView, float rotation, CGPoint center, string creatorid, DateTime creationdate)
		{
			Url = url;
			UrlText = url.AbsoluteString;
			ThumbnailView = thumbnailView;
			Rotation = rotation;
			Center = center;
			CreatorId = creatorid;
			CreationDate = creationdate;
		}
	}
}

