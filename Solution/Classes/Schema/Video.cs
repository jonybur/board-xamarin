using CoreGraphics;
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
			Id = CommonUtils.GenerateId ();
		}

		public Video(NSUrl url, UIImageView thumbnailView, float rotation, CGRect frame, string userid)
		{
			Id = CommonUtils.GenerateId ();
			Url = url;
			ThumbnailView = thumbnailView;
			Rotation = rotation;
			Frame = frame;
			UserId = userid;
		}
	}
}

