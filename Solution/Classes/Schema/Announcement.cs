using CoreGraphics;
using Foundation;
using System.Runtime.Serialization;
using System;
using Board.Facebook;

namespace Board.Schema
{
	public class Announcement : Content
	{
		[IgnoreDataMember]
		public NSAttributedString AttributedText;

		// TODO: keep this until i sort out how to serialize a nsattributedstring
		public string Text;

		public const string Type = "announcements";

		public Announcement() {
		}

		public Announcement(NSAttributedString text, float rotation, CGPoint center, string creatorid, DateTime creationdate)
		{
			Text = text.Value;
			AttributedText = text;
			Rotation = rotation;
			Center = center;
			CreatorId = creatorid;
			CreationDate = creationdate;
		}

		public Announcement(FacebookPost facebookPost, CGPoint center, float rotation){
			Text = facebookPost.Message;
			CreationDate = DateTime.Parse (facebookPost.CreatedTime);
			Rotation = rotation;
			Center = center;
			FacebookId = facebookPost.Id;
		}

		public Announcement(string text, float rotation, CGPoint center, string creatorid, DateTime creationdate)
		{
			Text = text;
			Rotation = rotation;
			Center = center;
			CreatorId = creatorid;
			CreationDate = creationdate;
		}
	}
}