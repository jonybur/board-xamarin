namespace Clubby.Facebook
{
	public class FacebookElement
	{
		public string Id;
	}

	public class FacebookFullPicture : FacebookElement{
		public string FullPicture;

		public FacebookFullPicture(string id, string fullPicture){
			FullPicture = fullPicture;
			Id = Id;
		}
	}
}

