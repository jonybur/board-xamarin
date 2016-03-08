using CoreGraphics;
using System.Collections.Generic;
using Board.Utilities;
using System;

namespace Board.Schema
{
	public class Content
	{
		public string Id;

		public CGRect Frame;

		public string CreatorId;

		public List<int> SocialChannel;

		public float Rotation;

		public DateTime CreationDate; 

		public Content(){
			Id = CommonUtils.GenerateGuid ();
		}

	}
}

