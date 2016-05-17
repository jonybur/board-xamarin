using System;
using CoreLocation;

namespace Board.Facebook
{
	public class FacebookImportedPage : FacebookElement
	{
		public string Name;
		public CLLocationCoordinate2D Location;
		public string About;
		public string CoverUrl;
		public string PictureUrl;

		public FacebookImportedPage (string id, string name, CLLocationCoordinate2D location,
			string about, string coverurl, string pictureurl)
		{
			Id = id;
			Name = name;
			Location = location;
			About = about;
			CoverUrl = coverurl;
			PictureUrl = pictureurl;
		}
	}
}

