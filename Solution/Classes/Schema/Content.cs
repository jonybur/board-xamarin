using CoreGraphics;
using System.Collections.Generic;

namespace Board.Schema
{
	public class Content
	{
		public CGRect Frame { get; set; }

		public string UserId { get; set; }

		public List<int> SocialChannel { get; set; }

		public float Rotation { get; set; }
	}
}

