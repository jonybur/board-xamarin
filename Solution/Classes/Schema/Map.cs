using CoreGraphics;

namespace Board.Schema
{
	public class Map : Content
	{
		public const string Type = "maps";

		public Map() {
		}

		public Map(CGAffineTransform transform, CGPoint center, string creatorid)
		{
			Transform = transform;
			Center = center;
			CreatorId = creatorid;
		}
	}
}

