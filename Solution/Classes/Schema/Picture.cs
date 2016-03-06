using CoreGraphics;
using UIKit;
using Board.Utilities;

namespace Board.Schema
{
	public class Picture : Content
	{
		public UIImageView ImageView;

		public UIImageView ThumbnailView;

		public Picture() {
		}

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

