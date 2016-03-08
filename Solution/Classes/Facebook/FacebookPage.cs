namespace Board.Facebook
{
	public class FacebookPage : FacebookElement
	{
		public string Name;
		public string Category;

		public FacebookPage(string id, string name, string category)
		{
			Name = name;
			Id = id;
			Category = category;
		}
	}
}

