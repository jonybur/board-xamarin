using System;
using CoreGraphics;

namespace Board.Schema
{
	// The Picture class as it is uploaded on Azure
	// Board's way of storing its UIImageViews on the DB
	public class BoardEvent : Content
	{
		public string Id { get; set; }

		public string Name { get; set; }
	
		public DateTime Date { get; set; }

		public BoardEvent() {}

		public BoardEvent(string name, DateTime date, float rotation, CGRect frame, string userid)
		{
			Name = name;
			Date = date;
			Rotation = rotation;
			Frame = frame;
			UserId = userid;
		}
	}
}