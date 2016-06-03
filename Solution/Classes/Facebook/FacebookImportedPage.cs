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
		public string Phone;
		public string Category;

		public FacebookImportedPage (string id, string name, CLLocationCoordinate2D location,
			string about, string coverurl, string pictureurl, string phone, string category)
		{
			Id = id;
			Name = name;
			Location = location;
			About = about;
			CoverUrl = coverurl;
			PictureUrl = pictureurl;
			Phone = phone;
			Category = category;
		}
	}
}

