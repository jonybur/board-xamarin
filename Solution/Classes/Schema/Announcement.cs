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

		public Announcement(NSAttributedString text, CGPoint center, string creatorid, DateTime creationdate, CGAffineTransform transform)
		{
			Text = text.Value;
			AttributedText = text;
			Transform = transform;
			Center = center;
			CreatorId = creatorid;
			CreationDate = creationdate;
		}

		public Announcement(FacebookPost facebookPost, CGPoint center, CGAffineTransform transform){
			Text = facebookPost.Message;
			CreationDate = DateTime.Parse (facebookPost.CreatedTime);
			Transform = transform;
			Center = center;
			FacebookId = facebookPost.Id;
		}

		public Announcement(string text, CGPoint center, string creatorid, DateTime creationdate, CGAffineTransform transform)
		{
			Text = text;
			Transform = transform;
			Center = center;
			CreatorId = creatorid;
			CreationDate = creationdate;
		}
	}
}