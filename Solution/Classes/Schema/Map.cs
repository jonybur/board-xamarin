using CoreGraphics;
using System;

namespace Board.Schema
{
	public class Map : Content
	{
		public Map() {
		}

		public Map(float rotation, CGRect frame, string creatorid, DateTime creationdate)
		{
			Rotation = rotation;
			Frame = frame;
			CreatorId = creatorid;
			CreationDate = creationdate;
		}
	}
}

