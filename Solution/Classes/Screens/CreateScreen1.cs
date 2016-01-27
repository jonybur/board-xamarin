using System;
using System.Drawing;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;

using System.Net.Http;
using System.Net;

using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Facebook.CoreKit;
using Facebook.LoginKit;

using Geolocator.Plugin;
using Google.Maps;

namespace Solution
{
	public class CreateScreen1 : UIViewController
	{
		UIImageView banner;
		MapView map;
		UITextField addressView;
		UITextField nameView;
		bool firstLocationUpdate = false;
		const string APIKey = "AIzaSyAUO-UX9QKVWK421yjXqoo02N5TYrG_hY8";

		public CreateScreen1 () : base ("Board", null){

		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;

			InitializeInterface ();
		}

		private void InitializeInterface()
		{
			// loads center button
			LoadBanner ();
			LoadContent ();
			LoadMap ();
		}

		private void LoadNameView()
		{
			nameView = new UITextField (new CGRect (30, 173, AppDelegate.ScreenWidth - 65, 26));
			nameView.BackgroundColor = UIColor.White;
			nameView.TextColor = AppDelegate.CityboardBlue;
			nameView.Font = UIFont.FromName ("roboto-regular", 20);

			nameView.KeyboardType = UIKeyboardType.Default;
			nameView.ReturnKeyType = UIReturnKeyType.Continue;
			nameView.EnablesReturnKeyAutomatically = true;

			nameView.ShouldChangeCharacters = (textField, range, replacementString) => {
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return newLength <= 30;
			};

			UITapGestureRecognizer tapGesture= new UITapGestureRecognizer ((tg) => {
				nameView.ResignFirstResponder();
			});

			View.AddGestureRecognizer (tapGesture);
			View.AddSubview (nameView);
		}

		Marker marker;


		private async void LoadMap()
		{
			marker = new Marker ();

			Geocoder geocoder = new Geocoder ();

			var camera = CameraPosition.FromCamera (latitude: 40, 
				longitude: -100, 
				zoom: -2);

			map = MapView.FromCamera (new CGRect (0, AppDelegate.ScreenHeight - 295, AppDelegate.ScreenWidth, 296), camera);

			map.AddObserver (this, new NSString ("myLocation"), NSKeyValueObservingOptions.New, IntPtr.Zero);

			map.Settings.CompassButton = true;
			map.Settings.MyLocationButton = true;
			map.UserInteractionEnabled = true;

			map.CoordinateLongPressed += (object sender, GMSCoordEventArgs e) => {
				marker.Position = new CoreLocation.CLLocationCoordinate2D (e.Coordinate.Latitude, e.Coordinate.Longitude);
				marker.Map = map;
				geocoder.ReverseGeocodeCord (new CoreLocation.CLLocationCoordinate2D (e.Coordinate.Latitude, e.Coordinate.Longitude), HandleReverseGeocodeCallback);

				using(WebClient client = new WebClient()) {

					string url = "https://maps.googleapis.com/maps/api/geocode/json?address=" + e.Coordinate.Latitude.ToString() + "," + e.Coordinate.Longitude.ToString() + "&key=" + APIKey;

					string s = client.DownloadString(url);
				}

			};

			View.AddSubview (map);

			InvokeOnMainThread (()=> map.MyLocationEnabled = true);
		}

		public override void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			if (!firstLocationUpdate) {
				firstLocationUpdate = true; 

				var location = change.ObjectForKey (NSValue.ChangeNewKey) as CoreLocation.CLLocation;
				map.Camera = CameraPosition.FromCamera (location.Coordinate, 16);
			}
		}


		private void LoadAddressView()
		{
			addressView = new UITextField (new CGRect (30, 286, AppDelegate.ScreenWidth - 65, 26));
			addressView.BackgroundColor = UIColor.White;
			addressView.TextColor = AppDelegate.CityboardBlue;
			addressView.Font = UIFont.FromName ("roboto-regular", 20);
			addressView.UserInteractionEnabled = false;
			addressView.AdjustsFontSizeToFitWidth = true;

			View.AddSubview(addressView);
		}

		private void LoadContent()
		{
			UIImage contentImage = UIImage.FromFile ("./createscreens/screen1/content6.jpg");
			UIImageView contentImageView = new UIImageView (new CGRect(0, banner.Frame.Bottom, contentImage.Size.Width / 2, contentImage.Size.Height / 2));
			contentImageView.Image = contentImage;
			View.AddSubview (contentImageView);
			LoadNameView ();
			LoadAddressView ();
		}

		void HandleReverseGeocodeCallback (ReverseGeocodeResponse response, NSError error)
		{
			Address ad = response.FirstResult;

			addressView.Text = ad.Thoroughfare + ", " + ad.SubLocality;
		}

		private void LoadBanner()
		{
			UIImage bannerImage = UIImage.FromFile ("./createscreens/screen1/banner.jpg");

			banner = new UIImageView(new CGRect(0,0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
			banner.Image = bannerImage;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 3){
					NavigationController.PopViewController(false);
				} else if (tg.LocationInView(this.View).X > (AppDelegate.ScreenWidth / 3) * 2){
					CreateScreen2 createScreen2 = new CreateScreen2();
					NavigationController.PushViewController(createScreen2, false);
				}
			});

			banner.UserInteractionEnabled = true;
			banner.AddGestureRecognizer (tap);
			banner.Alpha = .95f;
			View.AddSubview (banner);
		}
	}
}