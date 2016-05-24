using Foundation;
using CoreLocation;
using UIKit;

namespace Board.Infrastructure
{
	public static class AppsController
	{
		public static bool CanOpenUber(){
			NSUrl url = new NSUrl("uber://");
			return UIApplication.SharedApplication.CanOpenUrl (url);
		}

		public static bool CanOpenWaze(){
			NSUrl url = new NSUrl("waze://");
			return UIApplication.SharedApplication.CanOpenUrl (url);
		}

		public static bool CanOpenGoogleMaps(){
			NSUrl url = new NSUrl("comgooglemaps://");
			return UIApplication.SharedApplication.CanOpenUrl (url);
		}

		public static bool CanOpenFacebookMessenger(){
			NSUrl url = new NSUrl ("fb-messenger://");
			return UIApplication.SharedApplication.CanOpenUrl (url);
		}


		public static void OpenFacebookMessenger(string recieverId){
			NSUrl url = new NSUrl("fb-messenger://user-thread/"+recieverId);
			UIApplication.SharedApplication.OpenUrl (url);
		}

		public static void OpenUber(string productId, CLLocationCoordinate2D dropoff){
			NSUrl uberRequest = new NSUrl("uber://?" +
				"client_id=7-UVBjdHfUrKKeZU9nDlP_HktFs3iWVT&" +
				"product_id=" + productId + "&" +
				"action=setPickup&" +
				"pickup=my_location&" +
				"dropoff[latitude]=" + dropoff.Latitude + "&" +
				"dropoff[longitude]=" + dropoff.Longitude);

			UIApplication.SharedApplication.OpenUrl(uberRequest);
		}

		public static void OpenWaze(CLLocationCoordinate2D destination){
			NSUrl wazeRequest = new NSUrl("waze://?ll="+destination.Latitude+","+destination.Longitude+"&z=10&navigate=yes");
			UIApplication.SharedApplication.OpenUrl(wazeRequest);
		}

		public static void OpenFacebook(string facebookId, bool opensInApp){
			NSUrl url;
			if (opensInApp) {
				// opens in app
				url = new NSUrl("https://www.facebook.com/" + facebookId);
			} else {
				// opens in safari
				url = new NSUrl("https://facebook.com/" + facebookId);
			}
			UIApplication.SharedApplication.OpenUrl(url);
		}

		public static void OpenGoogleMaps(CLLocationCoordinate2D destination){
			NSUrl url = new NSUrl("comgooglemaps://?&daddr="+destination.Latitude+","+destination.Longitude+"&directionsmode=driving");
			UIApplication.SharedApplication.OpenUrl (url);
		}

		public static void OpenAppleMaps(CLLocationCoordinate2D destination){
			NSUrl url = new NSUrl ("http://maps.apple.com/?daddr="+destination.Latitude+","+destination.Longitude+"&dirflg=d&t=m");
			UIApplication.SharedApplication.OpenUrl (url);
		}
	}
}

