using Board.Facebook;
using Board.JsonResponses;
using Board.Utilities;
using UIKit;

namespace Board.Schema
{
	public class Board
	{
		public UIImage Logo, CoverImage;
		public UIColor MainColor, SecondaryColor;
		public GoogleGeolocatorObject GeolocatorObject;
		public double Distance;

		public string Id, Name, About, Phone, Category, FacebookId, CreatorId;
		public string LogoUrl, CoverImageUrl;

		public Board()
		{
			Id = CommonUtils.GenerateGuid ();
		}

		public Board(string id){
			Id = id;
		}

		public Board (string name, UIImage logo, UIColor mainColor, UIColor secondaryColor, GoogleGeolocatorObject geolocatorObject)
		{
			Id = CommonUtils.GenerateGuid ();
			Logo = logo;
			MainColor = mainColor;
			SecondaryColor = secondaryColor;
			GeolocatorObject = geolocatorObject;
			Name = name;
		}
	}
}

