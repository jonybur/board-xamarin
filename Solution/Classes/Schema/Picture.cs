using CoreGraphics;
using UIKit;

namespace Board.Schema
{
	public class Picture : Content
	{
		public string Id;

		public UIImageView ImageView;

		public UIImageView ThumbnailView;

		public Picture() {}

		public Picture(UIImage image, UIImage thumbnail, float rotation, CGRect frame, string userid)
		{
			ImageView = new UIImageView(image);
			ThumbnailView = new UIImageView(thumbnail);
			Rotation = rotation;
			Frame = frame;
			UserId = userid;
		}
	}
}

