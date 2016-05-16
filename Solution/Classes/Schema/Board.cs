using Board.JsonResponses;
using Board.Facebook;
using UIKit;
using Board.Utilities;

namespace Board.Schema
{
	public class Board
	{
		public UIImage Image;
		public UIColor MainColor;
		public UIColor SecondaryColor;
		public GoogleGeolocatorObject GeolocatorObject;
		public string Name;
		public string CreatorId;
		public FacebookPage FBPage;
		public string Id;
		public double Distance;
		public string Description;

		public Board()
		{
			Id = CommonUtils.GenerateGuid ();
		}

		public Board(string id){
			Id = id;
		}

		public Board (string name, UIImage image, UIColor mainColor, UIColor secondaryColor, GoogleGeolocatorObject geolocatorObject, string creatorId)
		{
			Id = CommonUtils.GenerateGuid ();
			Image = image;
			MainColor = mainColor;
			SecondaryColor = secondaryColor;
			GeolocatorObject = geolocatorObject;
			Name = name;
			CreatorId = creatorId;
		}
	}
}

