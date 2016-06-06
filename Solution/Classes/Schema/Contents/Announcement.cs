using CoreGraphics;
using Foundation;
using System.Runtime.Serialization;
using System;
using Board.Facebook;
using Board.Interface;

namespace Board.Schema
{
	public class Announcement : Content
	{
		[IgnoreDataMember]
		public NSAttributedString AttributedText;

		public string Text;

		public string Type {
			get { return "announcements"; }
		}

		public Announcement() {
			CreationDate = DateTime.Now;
		}

		public Announcement(NSAttributedString text, CGPoint center, string creatorid, CGAffineTransform transform)
		{
			CreationDate = DateTime.Now;
			Text = text.Value;
			AttributedText = text;
			Transform = transform;
			Center = center;
			CreatorId = creatorid;
		}

		public Announcement(FacebookPost facebookPost, CGPoint center, CGAffineTransform transform){
			Text = facebookPost.Message;
			CreationDate = DateTime.Parse (facebookPost.CreatedTime);
			Transform = transform;
			Center = center;
			FacebookId = facebookPost.Id;

			var boardCoordinate = UIBoardInterface.board.GeolocatorObject.Coordinate;
			latitude = boardCoordinate.Latitude;
			longitude = boardCoordinate.Longitude;

			boardId = UIBoardInterface.board.Id;
		}

		public Announcement(string text, CGPoint center, string creatorid, DateTime creationdate, CGAffineTransform transform)
		{
			CreationDate = DateTime.Now;
			Text = text;
			Transform = transform;
			Center = center;
			CreatorId = creatorid;
			CreationDate = creationdate;
		}
	}
}