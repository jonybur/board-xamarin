namespace Board.Facebook
{
	public class FacebookVideo : FacebookElement
	{
		public string Description;
		public string UpdatedTime;

		public FacebookVideo (string description, string updatedTime, string id)
		{
			Description = description;
			UpdatedTime = updatedTime;
			Id = id;
		}
	}

	public class FacebookVideoSource : FacebookElement{
		public string Source;
		public string ThumbnailUrl;

		public FacebookVideoSource(string id, string source, string thumburl){
			Id = id;
			Source = source;
			ThumbnailUrl = thumburl;
		}
		
	}
}

