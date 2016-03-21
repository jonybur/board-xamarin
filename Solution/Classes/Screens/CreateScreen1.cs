using System;
using Board.JsonResponses;
using CoreGraphics;
using Foundation;
using Google.Maps;
using UIKit;
using Board.Screens.Controls;
using BigTed;

namespace Board.Screens
{
	public class CreateScreen1 : UIViewController
	{
		MenuBanner Banner;
		MapView map;
		Marker marker;
		UITextField addressView;
		UITextField nameView;
		UIImageView whiteRectangle;
		UIImageView orangeRectangle;
		Result resultAddress;

		bool nextEnabled;

		bool firstLocationUpdate;

		public override void ViewDidAppear (bool animated)
		{
			map.AddObserver (this, new NSString ("myLocation"), NSKeyValueObservingOptions.New, IntPtr.Zero);
			Banner.SuscribeToEvents ();
		}

		public override void ViewDidDisappear(bool animated)
		{
			map.RemoveObserver (this, new NSString ("myLocation"));
			Banner.UnsuscribeToEvents ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View.BackgroundColor = UIColor.White;

			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;

			InitializeInterface ();
		}

		private void InitializeInterface()
		{
			LoadBanner ();
			LoadContent ();
			LoadMap ();
		}

		private void LoadNameView()
		{
			nameView = new UITextField (new CGRect (30, 173, AppDelegate.ScreenWidth - 65, 26));
			nameView.BackgroundColor = UIColor.White;
			nameView.TextColor = AppDelegate.BoardBlue;
			nameView.Font = AppDelegate.SystemFontOfSize20;

			nameView.AutocapitalizationType = UITextAutocapitalizationType.Words;
			nameView.KeyboardType = UIKeyboardType.Default;
			nameView.ReturnKeyType = UIReturnKeyType.Done;
			nameView.EnablesReturnKeyAutomatically = true;

			nameView.ShouldReturn += textField => {
				textField.ResignFirstResponder();
				return true;
			};

			nameView.ShouldChangeCharacters = (textField, range, replacementString) => {
				var newLength = textField.Text.Length + replacementString.Length - range.Length;

				NextButtonEnabled(true);

				return newLength <= 30;
			};

			View.AddSubview (nameView);
		}


		private void LoadMap()
		{
			marker = new Marker ();
			marker.AppearAnimation = MarkerAnimation.Pop;

			var camera = CameraPosition.FromCamera (40, -100, -2);

			float mapSize = 0;

			if (AppDelegate.PhoneVersion == "6") {
				mapSize = 295;
			} else {
				mapSize = 350;
			}

			map = MapView.FromCamera (new CGRect (0, AppDelegate.ScreenHeight - mapSize, AppDelegate.ScreenWidth, mapSize), camera);

			map.Settings.CompassButton = true;
			map.Settings.MyLocationButton = true;
			map.UserInteractionEnabled = true;

			map.CoordinateLongPressed += (sender, e) => {
				marker.Position = new CoreLocation.CLLocationCoordinate2D (e.Coordinate.Latitude, e.Coordinate.Longitude);
				marker.Map = map;
				addressView.Text = string.Empty;

				string jsonobj = JsonHandler.GET("https://maps.googleapis.com/maps/api/geocode/json?address=" + e.Coordinate.Latitude + "," + e.Coordinate.Longitude + "&key=" + AppDelegate.GoogleMapsAPIKey);
				GoogleGeolocatorObject geolocatorObject = JsonHandler.DeserializeObject(jsonobj);

				try{
					if (geolocatorObject.results.Count > 0)
					{
						// displays address taken from coordinates
						addressView.Text += geolocatorObject.results[0].address_components[0].long_name + " " + 
							geolocatorObject.results[0].address_components[1].short_name;

						// saves result address, 
						resultAddress = geolocatorObject.results[0];

						// enables editing
						addressView.UserInteractionEnabled = true;

						NextButtonEnabled(true);

						whiteRectangle.Alpha = 0f;
					}
				}
				catch{
					Console.WriteLine("Error creating address");

					resultAddress = null;

					addressView.Text = string.Empty;
				}

			};

			View.AddSubview (map);

			InvokeOnMainThread (()=> map.MyLocationEnabled = true);
		}

		private void NextButtonEnabled(bool enabled)
		{
			if (nameView != null) {
				if (nameView.Text.Length < 5) {
					nextEnabled = false;
					orangeRectangle.Alpha = .5f;
					return;
				}
			}

			if (resultAddress == null) {
				nextEnabled = false;
				orangeRectangle.Alpha = .5f;
				return;
			}

			nextEnabled = enabled;

			if (nextEnabled) {
				orangeRectangle.Alpha = 0f;
			} else {
				orangeRectangle.Alpha = .5f;
			}
		}

		private UIImageView CreateColorSquare(CGSize size, CGPoint center, CGColor startcolor)
		{
			CGRect frame = new CGRect (0, 0, size.Width, size.Height);

			UIGraphics.BeginImageContext (new CGSize(frame.Size.Width, frame.Size.Height));
			CGContext context = UIGraphics.GetCurrentContext ();

			context.SetFillColor(startcolor);
			context.FillRect(frame);

			UIImageView uiv;
			using (UIImage img = UIGraphics.GetImageFromCurrentImageContext ()) {
				uiv = new UIImageView (img);
			}
			uiv.Center = center;

			return uiv;
		}

		public override void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			if (!firstLocationUpdate) {
				firstLocationUpdate = true; 

				var location = change.ObjectForKey (NSValue.ChangeNewKey) as CoreLocation.CLLocation;
				map.Camera = CameraPosition.FromCamera (location.Coordinate, 15);
			}
		}

		private void LoadAddressView()
		{
			addressView = new UITextField (new CGRect (30, 286, AppDelegate.ScreenWidth - 65, 26));
			addressView.BackgroundColor = UIColor.White;
			addressView.TextColor = AppDelegate.BoardBlue;
			addressView.Font = AppDelegate.SystemFontOfSize20;

			addressView.KeyboardType = UIKeyboardType.Default;
			addressView.ReturnKeyType = UIReturnKeyType.Done;

			addressView.UserInteractionEnabled = false;
			addressView.AdjustsFontSizeToFitWidth = true;

			addressView.ShouldReturn += textField => {
				EndsEditingStreet();
				return true;
			};

			// loads white box
			whiteRectangle = CreateColorSquare(new CGSize(AppDelegate.ScreenWidth, 80), new CGPoint(AppDelegate.ScreenWidth/2, addressView.Frame.Y), UIColor.White.CGColor);

			View.AddSubview (whiteRectangle);

			View.AddSubview(addressView);
		}

		private void EndsEditingStreet()
		{
			addressView.ResignFirstResponder();

			string addressToSend = addressView.Text;
			for (int i = 2; i < resultAddress.address_components.Count; i++)
			{
				addressToSend += ", " + resultAddress.address_components[i].short_name;
			}
			addressToSend = addressToSend.Replace(" ", "+");

			string jsonobj = JsonHandler.GET("https://maps.googleapis.com/maps/api/geocode/json?address=" + addressToSend + "&key=" + AppDelegate.GoogleMapsAPIKey);
			GoogleGeolocatorObject geolocatorObject = JsonHandler.DeserializeObject(jsonobj);

			if (geolocatorObject.results.Count == 0 || addressView.Text.Length == 0)
			{
				// if location doesnt get any results, or if text is left blank, kill marker and disable editing

				addressView.UserInteractionEnabled = false;
				addressView.Text = string.Empty;
				marker.Map = null;
				resultAddress = null;
				whiteRectangle.Alpha = 1f;

				NextButtonEnabled(false);

				UIAlertController alert = UIAlertController.Create("Street is out of reach", "The address is too far away from pin\nPlease pin your venue again", UIAlertControllerStyle.Alert);

				alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));

				NavigationController.PresentViewController (alert, true, null);

				return;
			}

			// if location returns an object, get the location, set the marker, save new resultaddress

			resultAddress = geolocatorObject.results [0];
			Location location = resultAddress.geometry.location;
			marker.Position = new CoreLocation.CLLocationCoordinate2D (location.lat, location.lng);

			return;
		}

		private void LoadContent()
		{
			UIImageView contentImageView;
			using (UIImage contentImage = UIImage.FromFile ("./screens/create/1/content/" + AppDelegate.PhoneVersion + ".jpg")) {
				contentImageView = new UIImageView (new CGRect(0, Banner.Frame.Bottom, contentImage.Size.Width / 2, contentImage.Size.Height / 2));
				contentImageView.Image = contentImage;
			}

			View.AddSubview (contentImageView);
			LoadNameView ();
			LoadAddressView ();
		}

		private void LoadBanner()
		{
			Banner = new MenuBanner ("./screens/create/1/banner/" + AppDelegate.PhoneVersion + ".jpg");

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					NavigationController.PopViewController(false);

				} else if (tg.LocationInView(this.View).X > (AppDelegate.ScreenWidth / 4) * 3 && nextEnabled){
					Board.Schema.Board newBoard = new Board.Schema.Board();
					newBoard.Location = resultAddress.formatted_address;
					newBoard.Name = nameView.Text;

					CreateScreen2 createScreen2 = new CreateScreen2(newBoard);
					NavigationController.PushViewController(createScreen2, false);
				}
			});

			Banner.AddTap (tap);

			orangeRectangle = CreateColorSquare (new CGSize (75, 60), 
				new CGPoint ((AppDelegate.ScreenWidth / 4) * 3 + 60, 25),
				AppDelegate.BoardOrange.CGColor);

			NextButtonEnabled(false);

			Banner.AddSubview (orangeRectangle);

			View.AddSubview (Banner);
		}
	}
}