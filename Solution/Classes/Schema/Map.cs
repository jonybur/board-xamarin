using CoreGraphics;
using System;

namespace Board.Schema
{
	public class Map : Content
	{
		public Map() {
		}

		public Map(float rotation, CGPoint position, string creatorid, DateTime creationdate)
		{
			Rotation = rotation;
			Position = position;
			CreatorId = creatorid;
			CreationDate = creationdate;
		}
	}
}

