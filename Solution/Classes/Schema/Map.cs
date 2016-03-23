using CoreGraphics;
using System;

namespace Board.Schema
{
	public class Map : Content
	{
		public Map() {
		}

		public Map(float rotation, CGPoint center, string creatorid, DateTime creationdate)
		{
			Rotation = rotation;
			Center = center;
			CreatorId = creatorid;
			CreationDate = creationdate;
		}
	}
}

