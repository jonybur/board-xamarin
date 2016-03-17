using CoreGraphics;
using Foundation;
using System;

namespace Board.Schema
{
	public class Announcement : Content
	{
		public NSAttributedString Text;

		public Announcement() {}

		public Announcement(NSAttributedString text, float rotation, CGPoint position, string creatorid, DateTime creationdate)
		{
			Text = text;
			Rotation = rotation;
			Position = position;
			CreatorId = creatorid;
			CreationDate = creationdate;
		}
	}
}