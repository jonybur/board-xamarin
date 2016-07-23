namespace Clubby.Schema
{
	public class Video : Content
	{
		public string VideoUrl, ImageUrl;

		public Video (string id)
		{
			Id = id;
		}
	}
}