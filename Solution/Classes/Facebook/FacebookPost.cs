namespace Board.Facebook
{
	public class FacebookPost : FacebookElement
	{
		public string Message;
		public string Story;
		public string CreatedTime;

		public FacebookPost(){
		}

		public FacebookPost(string id, string message, string story, string createdtime)
		{
			Message = message;
			Story = story;
			CreatedTime = createdtime;
			Id = id;
		}
	}
}

