using CoreLocation;
using UIKit;
using Foundation;

namespace Board.Schema
{
	[Preserve(AllMembers = true)]
	public class User
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public object Age { get; set; }
		public int Gender { get; set; }
		public CLLocationCoordinate2D Location { get; set; }
		public string ProfilePictureURL { get; set; }
		public UIImage ProfilePictureUIImage { get; set; }
	}
}