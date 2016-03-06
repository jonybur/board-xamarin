using Board.Utilities;
using CoreGraphics;
using Foundation;
using System;

namespace Board.Schema
{
	public class Announcement : Content
	{
		public NSAttributedString Text;

		public Announcement() {}

		public Announcement(NSAttributedString text, float rotation, CGRect frame, string userid)
		{
			Text = text;
			Rotation = rotation;
			Frame = frame;
			UserId = userid;
		}
	}
}