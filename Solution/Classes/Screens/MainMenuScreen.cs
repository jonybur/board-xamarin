using System;
using System.Collections.Generic;
using System.Linq;
using Board.Infrastructure;
using Board.Interface;
using Board.Screens.Controls;
using Board.Utilities;
using CoreGraphics;
using Facebook.CoreKit;
using Foundation;
using Google.Maps;
using MGImageUtilitiesBinding;
using UIKit;

namespace Board.Screens
{
	public partial class MainMenuScreen : UIViewController
	{
		UIMenuBanner Banner;
		UIScrollView ScrollView;
		List<Marker> ListMapMarkers;

		UIMagazine Magazine;
		UIButton map_button;
		EventHandler MapButtonEvent;
		MapView map;
		UIContentDisplay ContentDisplay;

		bool mapInfoTapped;
		bool firstLocationUpdate;
		bool generatedMarkers;

		public override void DidReceiveMemoryWarning ()
		{
			GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ListMapMarkers = new List<Marker> ();

			if (Profile.CurrentProfile == null) {
				AppDelegate.NavigationController.PopViewController (true);
			}

			ScrollView = new UIScrollView(new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));

			LoadMapButton ();
			LoadBanner ();
			LoadMap ();

			View.AddSubviews (ScrollView, map, Banner, map_button);
		}

		public override async void ViewDidAppear(bool animated)
		{
			await InitializeInterface ();

			// suscribe to observers, gesture recgonizers, events
			map.AddObserver (this, new NSString ("myLocation"), NSKeyValueObservingOptions.New, IntPtr.Zero);
			map_button.TouchUpInside += MapButtonEvent;	
			mapInfoTapped = false;
			ContentDisplay.SuscribeToEvents ();
			Banner.SuscribeToEvents ();
		}

		public override void ViewDidDisappear(bool animated)
		{
			// unsuscribe from observers, gesture recgonizers, events
			map.RemoveObserver (this, new NSString ("myLocation"));
			map_button.TouchUpInside -= MapButtonEvent;
			ContentDisplay.UnsuscribeToEvents ();
			foreach (Marker mark in ListMapMarkers) {
				mark.Dispose ();
			}

			MemoryUtility.ReleaseUIViewWithChildren (map);

			ListMapMarkers = null;

			Banner.UnsuscribeToEvents ();

			MemoryUtility.ReleaseUIViewWithChildren (View);
			GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced);
		}

		public async System.Threading.Tasks.Task InitializeInterface()
		{
			await LoadContent ();
		}

		private async System.Threading.Tasks.Task LoadContent()
		{
			ScrollView.BackgroundColor = UIColor.White;

			// GenerateBoardList()
			var boardList = await CloudController.GetUserBoards ();

			Magazine = new UIMagazine (boardList);

			ContentDisplay = Magazine.Pages [0].ContentDisplay;
			ScrollView.ContentSize = new CGSize (AppDelegate.ScreenWidth, ContentDisplay.Frame.Bottom);

			ScrollView.AddSubview (ContentDisplay);
			ScrollView.AddSubview (Magazine.Banner);

			ScrollView.ScrollEnabled = true;
			ScrollView.Scrolled += (sender, e) => {
				
				if (ScrollView.ContentOffset.Y < 0){
					Magazine.Banner.Center = new CGPoint(Magazine.Banner.Center.X,
						UIMenuBanner.MenuHeight + Magazine.Banner.Frame.Height / 2 + ScrollView.ContentOffset.Y);
				}

			};
		}

		private void LoadBanner()
		{
			Banner = new UIMenuBanner("./screens/main/banner/" + AppDelegate.PhoneVersion + ".jpg");

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					AppDelegate.containerScreen.BringSideMenuUp("main");
				}
			});

			Banner.AddTap (tap);
		}
			
		private void LoadMap()
		{
			firstLocationUpdate = false;

			var camera = CameraPosition.FromCamera (40, -100, -2);
			map = MapView.FromCamera (new CGRect (0, Banner.Frame.Bottom, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight - Banner.Frame.Height - map_button.Frame.Height), camera);
			map.Alpha = 0f;
			map.Settings.CompassButton = true;
			map.Settings.MyLocationButton = true;
			map.UserInteractionEnabled = true;

			map.InfoTapped += (sender, e) => {
				if (!mapInfoTapped)
				{
					/*UIBoardThumb bthumb = ListThumbs.Find(t=>t.Board.Id == ((NSString)e.Marker.UserData).ToString());
					AppDelegate.boardInterface = new BoardInterface(bthumb.Board, false);
					AppDelegate.NavigationController.PushViewController(AppDelegate.boardInterface, true);
					mapInfoTapped = true;*/
				}
			};

			InvokeOnMainThread (()=> map.MyLocationEnabled = true);
		}

		public override void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			if (!firstLocationUpdate) {
				firstLocationUpdate = true; 

				var location = change.ObjectForKey (NSValue.ChangeNewKey) as CoreLocation.CLLocation;
				map.Camera = CameraPosition.FromCamera (location.Coordinate, 15);

				if (!generatedMarkers) {
					GenerateMarkers ();
					generatedMarkers = true;
				}
			}
		}

		private void GenerateMarkers()
		{
			/*foreach (UIBoardThumb thumb in ListThumbs) {
				Marker marker = new Marker ();
				marker.AppearAnimation = MarkerAnimation.Pop;
				marker.Position = thumb.Board.GeolocatorObject.Coordinate;
				marker.Map = map;
				marker.Icon = CreateMarkerImage (thumb.Board.ImageView.Image);
				marker.Draggable = false;
				marker.Title = thumb.Board.Name;
				marker.Snippet = thumb.Board.GeolocatorObject.Address;
				marker.InfoWindowAnchor = new CGPoint (.5, .5);
				marker.Tappable = true;
				marker.UserData = new NSString(thumb.Board.Id);
				ListMapMarkers.Add (marker);
			}*/
		}

		public void PlaceNewScreen(UIContentDisplay newDisplay){
			ContentDisplay.UnsuscribeToEvents ();
			ContentDisplay.RemoveFromSuperview ();
			ContentDisplay = newDisplay;
			ScrollView.AddSubview (ContentDisplay);
			ScrollView.SendSubviewToBack (ContentDisplay);
			ScrollView.ContentSize = new CGSize (AppDelegate.ScreenWidth, newDisplay.Frame.Height);
			ContentDisplay.SuscribeToEvents ();
		} 

		private UIImage CreateMarkerImage(UIImage logo)
		{
			UIGraphics.BeginImageContextWithOptions (new CGSize (66, 96), false, 2f);

			using (UIImage container = UIImage.FromFile ("./screens/main/map/markercontainer.png")) {
				container.Draw (new CGRect (0, 0, 66, 96));
			}

			float autosize = 40;

			logo = logo.ImageScaledToFitSize (new CGSize(autosize,autosize));

			logo.Draw (new CGRect (33 - logo.Size.Width / 2, 33 - logo.Size.Height / 2, logo.Size.Width, logo.Size.Height));

			return UIGraphics.GetImageFromCurrentImageContext ();
		}

		private void LoadMapButton()
		{
			using (UIImage mapImage = UIImage.FromFile ("./screens/main/map/" + AppDelegate.PhoneVersion + ".jpg")) {
				map_button = new UIButton(new CGRect(0,AppDelegate.ScreenHeight - (mapImage.Size.Height / 2),
					mapImage.Size.Width / 2, mapImage.Size.Height / 2));
				map_button.SetImage(mapImage, UIControlState.Normal);
			}

			MapButtonEvent = (sender, e) => {
				
				if (map.Alpha == 0f)
				{ 
					map.Alpha = 1f; 

					using (UIImage listImage = UIImage.FromFile ("./screens/main/list/" + AppDelegate.PhoneVersion + ".jpg")) {
						map_button.SetImage(listImage, UIControlState.Normal);
					}
				} else {
					map.Alpha = 0f;

					using (UIImage mapImage = UIImage.FromFile ("./screens/main/map/" + AppDelegate.PhoneVersion + ".jpg")) {
						map_button.SetImage(mapImage, UIControlState.Normal);
					}
				} 
			};

			map_button.Alpha = .95f;
		}

	}
}