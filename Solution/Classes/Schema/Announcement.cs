using Board.Utilities;
using CoreGraphics;
using Foundation;
using System;

namespace Board.Schema
{
	public class Announcement : Content
	{
		public NSAttributedString Text;

		public Announcement() {
			Id = CommonUtils.GenerateGuid ();
		}

		public Announcement(NSAttributedString text, float rotation, CGRect frame, string userid)
		{
			Id = CommonUtils.GenerateGuid ();
			Text = text;
			Rotation = rotation;
			Frame = frame;
			UserId = userid;
		}
	}
}