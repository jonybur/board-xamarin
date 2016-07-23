using System;

namespace Clubby.Schema
{
	public class Picture : Content
	{
		public string ImageUrl, ThumbnailImageUrl;

		public Picture (string id)
		{
			Id = id;
		}
	}
}

