using CoreGraphics;
using System.Collections.Generic;
using Board.Utilities;
using System;

namespace Board.Schema
{
	public class Content
	{
		public string Id;
		public string CreatorId;
		public string FacebookId;

		public CGRect Frame;
		public DateTime CreationDate; 
		public List<int> SocialChannel;

		public float Rotation;

		public Content(){
			Id = CommonUtils.GenerateGuid ();
		}

	}
}

