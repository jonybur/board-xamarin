using CoreGraphics;
using UIKit;
using System.Runtime.Serialization;
using System;

namespace Board.Schema
{
	public class Picture : Content
	{
		[IgnoreDataMember]
		public UIImageView ImageView;

		[IgnoreDataMember]
		public UIImageView ThumbnailView;

		public string ImageUrl;

		public Picture() {
		}

		public Picture(UIImage image, UIImage thumbnail, float rotation, CGPoint center, string creatorid, DateTime creationdate)
		{
			ImageView = new UIImageView(image);
			ThumbnailView = new UIImageView(thumbnail);
			Rotation = rotation;
			Center = center;
			CreatorId = creatorid;
			CreationDate = creationdate;
		}
	}
}

