using CoreGraphics;
using UIKit;
using System;
using Board.Utilities;

namespace Board.Schema
{
	public class Picture : Content
	{
		public UIImageView ImageView;

		public UIImageView ThumbnailView;

		public Picture() {
		}

		public Picture(UIImage image, UIImage thumbnail, float rotation, CGPoint position, string creatorid, DateTime creationdate)
		{
			ImageView = new UIImageView(image);
			ThumbnailView = new UIImageView(thumbnail);
			Rotation = rotation;
			Position = position;
			CreatorId = creatorid;
			CreationDate = creationdate;
		}
	}
}

