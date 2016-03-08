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

		public Announcement(NSAttributedString text, float rotation, CGRect frame, string creatorid, DateTime creationdate)
		{
			Text = text;
			Rotation = rotation;
			Frame = frame;
			CreatorId = creatorid;
			CreationDate = creationdate;
		}
	}
}