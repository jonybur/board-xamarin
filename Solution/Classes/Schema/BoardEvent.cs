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
	
		public UIImageView ImageView;

		public UIImageView Thumbnail;

		public DateTime Date;

		public BoardEvent() {
			Id = CommonUtils.GenerateGuid ();
		}

		public BoardEvent(string name, UIImage image, DateTime date, float rotation, CGRect frame, string userid)
		{
			Id = CommonUtils.GenerateGuid ();
			ImageView = new UIImageView(image);
			Name = name;
			Date = date;
			Rotation = rotation;
			Frame = frame;
			UserId = userid;
		}
	}
}