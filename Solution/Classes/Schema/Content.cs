using CoreGraphics;
using System.Collections.Generic;
using Board.Utilities;
using System;
using System.Runtime.Serialization;

namespace Board.Schema
{
	public class Content
	{
		[IgnoreDataMember]
		public string Id;
		public string CreatorId;
		public string FacebookId;

		public CGPoint Center;
		public DateTime CreationDate;

		[IgnoreDataMember]
		public List<int> SocialChannel;

		public float Rotation;

		public Content(){
			Id = CommonUtils.GenerateGuid ();
		}

	}
}

