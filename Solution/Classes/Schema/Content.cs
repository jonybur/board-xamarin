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
		public string FacebookId;

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

		public Content(){
			Id = CommonUtils.GenerateGuid ();
		}

		public DateTime CreationDate;
		public float Rotation;

		[IgnoreDataMember]
		public List<int> SocialChannel;

		[IgnoreDataMember]
		public string CreatorId;
	}
}

