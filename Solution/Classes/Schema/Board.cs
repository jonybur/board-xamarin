using Board.JsonResponses;
using UIKit;

namespace Board.Schema
{
	public class Board
	{
		public UIImageView ImageView;
		public UIColor MainColor;
		public UIColor SecondaryColor;
		public GoogleGeolocatorObject GeolocatorObject;
		public string Location;
		public string Name;
		public string CreatorId;

		public Board()
		{
		}

		public Board (string name, UIImage image, UIColor mainColor, UIColor secondaryColor, string location, string creatorId)
		{
			ImageView = new UIImageView(image);
			MainColor = mainColor;
			SecondaryColor = secondaryColor;
			Location = location;
			Name = name;
			CreatorId = creatorId;
		}
	}
}

