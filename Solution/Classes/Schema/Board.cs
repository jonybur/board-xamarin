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
		public string Neighborhood{
			get {
				string hood = "<ERROR>";
				try{
					hood = GeolocatorObject.results [0].address_components [2].long_name; 
				} catch {
					hood = "<ERROR>";
				}
				return hood;
			}	
		}
		public string Address{
			get {
				string hood = "<ERROR>";
				try{
					hood = GeolocatorObject.results [0].address_components [0].long_name + " " +
						GeolocatorObject.results [0].address_components [1].short_name; 
				} catch {
					hood = "<ERROR>";
				}
				return hood;
			}	
		}
		public string FullAddress{
			get {
				string hood = "<ERROR>";
				try{
					hood = GeolocatorObject.results [0].formatted_address; 
				} catch {
					hood = "<ERROR>";
				}
				return hood;
			}	
		}
		public CLLocationCoordinate2D Coordinate{
			get {
				return new CLLocationCoordinate2D (GeolocatorObject.results [0].geometry.location.lat,
					GeolocatorObject.results [0].geometry.location.lng);
			}
		}

		public string Name;
		public string CreatorId;
		public FacebookPage FBPage;
		public string Id;

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

