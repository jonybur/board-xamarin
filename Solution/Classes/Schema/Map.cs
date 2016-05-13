using CoreGraphics;
using System;

namespace Board.Schema
{
	public class Map : Content
	{
		public Map() {
			Type = "maps";
		}

		public Map(float rotation, CGPoint center, string creatorid, DateTime creationdate)
		{
			Type = "maps";
			Rotation = rotation;
			Center = center;
			CreatorId = creatorid;
			CreationDate = creationdate;
		}
	}
}

