using CoreGraphics;

namespace Board.Schema
{
	// The Picture class as it is uploaded on Azure
	// Board's way of storing its UIImageViews on the DB
	public class Announcement : Content
	{
		public string Id { get; set; }

		public string Text { get; set; }

		public float Rotation { get; set; }

		public Announcement() {}

		public Announcement(string text, float rotation, CGRect frame, string userid)
		{
			Text = text;
			Rotation = rotation;
			Frame = frame;
			UserId = userid;
		}
	}
}