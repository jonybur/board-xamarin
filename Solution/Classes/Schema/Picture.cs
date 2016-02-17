using CoreGraphics;
using UIKit;

namespace Board.Schema
{
	public class Picture : Content
	{
		public string Id { get; set; }

		public UIImage Image { get; set; }

		public UIImage Thumbnail { get; set; }

		public Picture() {}

		public Picture(UIImage image, UIImage thumbnail, float rotation, CGRect frame, string userid)
		{
			Image = image;
			Thumbnail = thumbnail;
			Rotation = rotation;
			Frame = frame;
			UserId = userid;
		}
	}
}

