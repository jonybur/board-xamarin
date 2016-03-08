using System;
using UIKit;
using CoreGraphics;
using Board.Utilities;

namespace Board.Schema
{
	// The Picture class as it is uploaded on Azure
	// Board's way of storing its UIImageViews on the DB
	public class BoardEvent : Content
	{
		public string Name;
	
		public string Description;

		public UIImageView ImageView;

		public UIImageView Thumbnail;

		public DateTime StartDate;

		public DateTime EndDate;
	
		public BoardEvent() {
		}

		public BoardEvent(string name, UIImage image, DateTime startdate, DateTime enddate, float rotation, CGRect frame, string creatorid, DateTime creationdate)
		{
			ImageView = new UIImageView(image);
			Name = name;
			StartDate = startdate;
			EndDate = enddate;
			Rotation = rotation;
			Frame = frame;
			CreatorId = creatorid;
			CreationDate = creationdate;
		}
	}
}