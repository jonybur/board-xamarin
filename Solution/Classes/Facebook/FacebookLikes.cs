namespace Clubby.Facebook
{
	public class FacebookLikes : FacebookElement
	{
		public string LikesData;

		public FacebookLikes(string id, string likesData)
		{
			Id = id;
			LikesData = likesData;
		}
	}
}

