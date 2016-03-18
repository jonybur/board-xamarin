using System;
using UIKit;
using CoreGraphics;
using System.Runtime.Serialization;

namespace Board.Schema
{
	// The Picture class as it is uploaded on Azure
	// Board's way of storing its UIImageViews on the DB
	public class BoardEvent : Content
	{
		public string Name;
	
		public string Description;

		[IgnoreDataMember]
		public UIImageView ImageView;

		[IgnoreDataMember]
		public UIImageView Thumbnail;

		public DateTime StartDate;

		public DateTime EndDate;

		public string ImageUrl;
	
		public BoardEvent() {
		}

		public BoardEvent(string name, UIImage image, DateTime startdate, DateTime enddate, float rotation, CGPoint position, string creatorid, DateTime creationdate)
		{
			ImageView = new UIImageView(image);
			Name = name;
			StartDate = startdate;
			EndDate = enddate;
			Rotation = rotation;
			Position = position;
			CreatorId = creatorid;
			CreationDate = creationdate;
		}
	}
}