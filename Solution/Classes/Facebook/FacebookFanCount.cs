namespace Board.Facebook
{
	public class FacebookFanCount : FacebookElement
	{
		public string Id;
		public int Count;

		public FacebookFanCount(string id, string count)
		{
			Id = id;
			Count = int.Parse (count);
		}
	}
}

