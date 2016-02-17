using CoreGraphics;
using UIKit;

namespace Board.Schema
{
	public class Video : Content
	{
		public string Id { get; set; }

		public string Url { get; set; }

		public UIImage Thumbnail { get; set; }

		public Video() {}

		public Video(string url, UIImage thumbnail, float rotation, CGRect frame, string userid)
		{
			Url = url;
			Thumbnail = thumbnail;
			Rotation = rotation;
			Frame = frame;
			UserId = userid;
		}
	}
}

