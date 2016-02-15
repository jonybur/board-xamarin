using Foundation;
using UIKit;

using CoreAnimation;
using CoreGraphics;
using CoreText;

namespace Board.Schema
{
	// The Picture class as it is uploaded on Azure
	// Board's way of storing its UIImageViews on the DB
	public class Announcement : Content
	{
		public string Id { get; set; }

		public string Text { get; set; }

		public float Rotation { get; set; }

		public CGRect GetRectangleF()
		{
			return new CGRect (ImgX, ImgY, ImgW, ImgH);
		}

		public Announcement() {}

		public Announcement(string text, float rotation, CGRect position, string userid)
		{
			Text = text;
			Rotation = rotation;
			ImgX = (float)(position.X); ImgY = (float)(position.Y);
			ImgH = (float)(position.Height); ImgW = (float)(position.Width);
			UserId = userid;
		}
	}
}