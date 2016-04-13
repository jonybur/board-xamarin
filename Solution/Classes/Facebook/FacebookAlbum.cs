using Board.Facebook;

namespace Board
{
	public class FacebookAlbum : FacebookElement
	{
		public string Name;
		public string CreatedTime;

		public FacebookAlbum(){
		}

		public FacebookAlbum(string id, string name, string createdtime)
		{
			Id = id;
			Name = name;
			CreatedTime = createdtime;
		}
	}
}

