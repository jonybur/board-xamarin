using Board.Utilities;
using CoreGraphics;
using Foundation;
using System.Collections.Generic;

namespace Board.Schema
{
	// The Picture class as it is uploaded on Azure
	// Board's way of storing its UIImageViews on the DB
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