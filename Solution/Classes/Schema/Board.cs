using Board.Facebook;
using Board.JsonResponses;
using Board.Utilities;
using CoreGraphics;
using Foundation;
using Haneke;
using UIKit;

namespace Board.Schema
{
	public class Board
	{
		public UIImage Logo, CoverImage;
		public UIColor MainColor, SecondaryColor;
		public GoogleGeolocatorObject GeolocatorObject;
		public FacebookPage FBPage;
		public double Distance;

		public string Name, CreatorId, Id, About;
		public string LogoUrl, CoverImageUrl;

		public Board()
		{
			Id = CommonUtils.GenerateGuid ();
		}

		public Board(string id){
			Id = id;
		}

		public Board (string name, UIImage logo, UIColor mainColor, UIColor secondaryColor, GoogleGeolocatorObject geolocatorObject, string creatorId)
		{
			Id = CommonUtils.GenerateGuid ();
			Logo = logo;
			MainColor = mainColor;
			SecondaryColor = secondaryColor;
			GeolocatorObject = geolocatorObject;
			Name = name;
			CreatorId = creatorId;
		}
	}
}

