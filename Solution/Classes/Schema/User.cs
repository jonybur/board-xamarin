using CoreLocation;
using UIKit;
using CoreGraphics;
using Haneke;
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
		public UIImageView ProfilePicture { get; set; }

		public void SetDefaultProfilePicture(){
			ProfilePicture = new UIImageView ();
			ProfilePicture.Frame = new CGRect (0, 0, 150, 150);
			ProfilePicture.ContentMode = UIViewContentMode.ScaleAspectFit;
			ProfilePicture.SetImage ("./DefaultUser.png");
		}

		public void SetProfilePictureFromURL(string url){
			ProfilePicture = new UIImageView ();
			ProfilePicture.Frame = new CGRect (0, 0, 150, 150);
			ProfilePicture.ContentMode = UIViewContentMode.ScaleAspectFit;
			ProfilePicture.SetImage (new NSUrl (url));
		}
	}
}