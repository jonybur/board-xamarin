using Board.JsonResponses;
using Board.Facebook;
using CoreLocation;
using UIKit;
using Board.Utilities;

namespace Board.Schema
{
	// add udid
	public class Board
	{
		public UIImageView ImageView;
		public UIColor MainColor;
		public UIColor SecondaryColor;
		public GoogleGeolocatorObject GeolocatorObject;
		public string Name;
		public string CreatorId;
		public FacebookPage FBPage;
		public string Id;
		public double Distance;

		public Board()
		{
			Id = CommonUtils.GenerateGuid ();
		}

		public Board (string name, UIImageView imageview, UIColor mainColor, UIColor secondaryColor, GoogleGeolocatorObject geolocatorObject, string creatorId)
		{
			Id = CommonUtils.GenerateGuid ();
			ImageView = imageview;
			MainColor = mainColor;
			SecondaryColor = secondaryColor;
			GeolocatorObject = geolocatorObject;
			Name = name;
			CreatorId = creatorId;
		}
	}
}

