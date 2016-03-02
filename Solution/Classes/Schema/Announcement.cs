using CoreGraphics;
using Board.Utilities;

namespace Board.Schema
{
	// The Picture class as it is uploaded on Azure
	// Board's way of storing its UIImageViews on the DB
	public class Announcement : Content
	{
		public string Text { get; set; }

		public Announcement() {
			Id = CommonUtils.GenerateId ();
		}

		public Announcement(string text, float rotation, CGRect frame, string userid)
		{
			Id = CommonUtils.GenerateId ();
			Text = text;
			Rotation = rotation;
			Frame = frame;
			UserId = userid;
		}
	}
}