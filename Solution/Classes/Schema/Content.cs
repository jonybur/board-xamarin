using System;
using Clubby.Utilities;

namespace Clubby.Schema
{
	// bridge class for instagram item
	public class Content
	{
		public string Id, InstagramId, Description, Name;
		public int Likes, Timestamp;
		public DateTime CreationDate;

		public Content ()
		{
			Id = CommonUtils.GenerateGuid ();
		}
	}
}

