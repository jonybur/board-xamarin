using Foundation;
using CoreLocation;
using UIKit;
using System.Globalization;

namespace Board.Infrastructure
{
	public static class AppsController
	{
		public static bool CanOpenPhone(){
			return CanOpenGeneric ("telprompt://");
		}

		public static bool CanOpenInstagram(){
			return CanOpenGeneric ("instagram://");
		}

		public static bool CanOpenUber(){
			return CanOpenGeneric ("uber://");
		}

		public static bool CanOpenWaze(){
			return CanOpenGeneric ("waze://");
		}

		public static bool CanOpenGoogleMaps(){
			return CanOpenGeneric ("comgooglemaps://");
		}

		public static bool CanOpenFacebookMessenger(){
			return CanOpenGeneric ("fb-messenger://");
		}

		private static bool CanOpenGeneric(string link){
			NSUrl url = new NSUrl (link);
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

		public static void OpenInstagram(string locationId){
			NSUrl url = new NSUrl("instagram://location?id="+locationId);
			UIApplication.SharedApplication.OpenUrl (url);
		}

		public static void OpenGoogleMaps(CLLocationCoordinate2D destination){
			NSUrl url = new NSUrl("comgooglemaps://?&daddr="+destination.Latitude+","+destination.Longitude+"&directionsmode=driving");
			UIApplication.SharedApplication.OpenUrl (url);
		}

		public static void OpenAppleMaps(CLLocationCoordinate2D destination){
			NSUrl url = new NSUrl ("http://maps.apple.com/?daddr="+destination.Latitude+","+destination.Longitude+"&dirflg=d&t=m");
			UIApplication.SharedApplication.OpenUrl (url);
		}

		public static void OpenPhone(string phoneNumber){
			NSUrl url = new NSUrl ("telprompt://"+phoneNumber.ToString(CultureInfo.InvariantCulture));
			UIApplication.SharedApplication.OpenUrl (url);
		}

		public static void OpenWebsite(string url){
			NSUrl nsurl = new NSUrl (url);
			UIApplication.SharedApplication.OpenUrl (nsurl);
		}
	}
}

