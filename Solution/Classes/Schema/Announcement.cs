using CoreGraphics;
using Foundation;
using System.Runtime.Serialization;
using System;

namespace Board.Schema
{
	public class Announcement : Content
	{
		[IgnoreDataMember]
		public NSAttributedString AttributedText;

		// TODO: keep this until i sort out how to serialize a nsattributedstring
		public string Text;

		public Announcement() {}

		public Announcement(NSAttributedString text, float rotation, CGPoint center, string creatorid, DateTime creationdate)
		{
			Text = text.Value;
			AttributedText = text;
			Rotation = rotation;
			Center = center;
			CreatorId = creatorid;
			CreationDate = creationdate;
		}
	}
}