using CoreGraphics;
using System.Collections.Generic;
using Board.Utilities;
using System;
using System.Runtime.Serialization;

namespace Board.Schema
{
	public class Content
	{
		public string Id;
		public string CreatorId;
		public string FacebookId;
		public string Type;

		[IgnoreDataMember]
		public CGPoint Center;

		public float CenterX {
			set { Center = new CGPoint (value, Center.Y); }
			get { return (float)Center.X; }
		}

		public float CenterY {
			set { Center = new CGPoint (Center.X, value); }
			get { return (float)Center.Y; }
		}

		public DateTime CreationDate;

		[IgnoreDataMember]
		public List<int> SocialChannel;

		public float Rotation;

		public Content(){
			Id = CommonUtils.GenerateGuid ();
		}

	}
}

