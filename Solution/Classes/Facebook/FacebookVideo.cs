namespace Board.Facebook
{
	public class FacebookVideo : FacebookElement
	{
		string Description;
		string UpdatedTime;

		public FacebookVideo (string description, string updatedTime, string id)
		{
			Description = description;
			UpdatedTime = updatedTime;
			Id = id;
		}
	}

	public class FacebookVideoSource : FacebookElement{
		string Source;

		public FacebookVideoSource(string source, string id){
			Source = source;
			Id = id;
		}
		
	}
}

