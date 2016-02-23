using System;
using UIKit;
using CoreGraphics;

namespace Board.Schema
{
	// The Picture class as it is uploaded on Azure
	// Board's way of storing its UIImageViews on the DB
	public class BoardEvent : Content
	{
		public string Name { get; set; }
	
		public UIImage Image { get; set; }

		public UIImage Thumbnail { get; set; }

		public DateTime Date { get; set; }

		public BoardEvent() {}

		public BoardEvent(string name, UIImage image, DateTime date, float rotation, CGRect frame, string userid)
		{
			Image = image;
			Name = name;
			Date = date;
			Rotation = rotation;
			Frame = frame;
			UserId = userid;
		}
	}
}