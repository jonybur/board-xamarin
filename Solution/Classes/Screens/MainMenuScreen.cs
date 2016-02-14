using System;
using System.Drawing;
using System.Linq;

using CoreGraphics;
using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;

using System.Net.Http;

using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Facebook.CoreKit;
using Facebook.LoginKit;

using Google.Maps;

namespace Solution
{
	public class MainMenuScreen : UIViewController
	{
		UIImageView banner;
		UIImageView sidemenu;
		UIImageView map_button;
		bool sideMenuIsUp;
		ProfilePictureView profileView;
		UIScrollView content;
		UIImage circleImage;
		float thumbSize;
		MapView map;
		bool firstLocationUpdate = false;

		public MainMenuScreen () : base ("Board", null){
			
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

		public override void ViewDidAppear(bool animated)
		{
			map.AddObserver (this, new NSString ("myLocation"), NSKeyValueObservingOptions.New, IntPtr.Zero);
		}

		private void InitializeInterface()
		{
			LoadContent ();
			LoadBanner ();
			LoadMapButton ();
			LoadMap ();
			LoadSideMenu ();
		}


		private List<Board.Schema.Board> GenerateBoardList()
		{
			List<Board.Schema.Board> boardList = new List<Board.Schema.Board> ();

			boardList.Add(new Board.Schema.Board ("American Social", UIImage.FromFile("./logos/americansocial.jpeg"), UIColor.FromRGB (67, 15, 0), UIColor.FromRGB (221, 169, 91), "Brickell", Profile.CurrentProfile.UserID));
			boardList.Add(new Board.Schema.Board ("Dog House", UIImage.FromFile("./logos/doghouse.jpeg"), UIColor.FromRGB (35, 32, 35), UIColor.FromRGB (220, 31, 24), "Brickell", string.Empty));
			boardList.Add(new Board.Schema.Board ("Dolores Lolita", UIImage.FromFile("./logos/doloreslolita.jpg"), UIColor.FromRGB (185, 143, 6), UIColor.FromRGB (2, 0, 6), "Brickell", string.Empty));	
			boardList.Add(new Board.Schema.Board ("Blue Martini", UIImage.FromFile("./logos/bluemartini.png"), UIColor.FromRGB (140, 52, 50), UIColor.FromRGB (77, 185, 155), "Brickell", string.Empty));	
			boardList.Add(new Board.Schema.Board ("Taverna Opa", UIImage.FromFile("./logos/tavernopa.png"), UIColor.FromRGB (140, 52, 50), UIColor.FromRGB (77, 185, 155), "Brickell", string.Empty));	
			boardList.Add(new Board.Schema.Board ("Clevelander", UIImage.FromFile("./logos/clevelander.png"), UIColor.FromRGB (195, 27, 29), UIColor.FromRGB (2, 0, 6), "South Beach", string.Empty));
			boardList.Add(new Board.Schema.Board ("Fat Tuesdays", UIImage.FromFile("./logos/fattuesdays.jpg"), UIColor.FromRGB (185, 143, 6), UIColor.FromRGB (2, 0, 6), "South Beach", string.Empty));	
			boardList.Add(new Board.Schema.Board ("LIV", UIImage.FromFile("./logos/liv.jpg"), UIColor.FromRGB (195, 27, 29), UIColor.FromRGB (2, 0, 6), "South Beach", string.Empty));
			boardList.Add(new Board.Schema.Board ("Mangos", UIImage.FromFile("./logos/mangos.png"), UIColor.FromRGB (195, 27, 29), UIColor.FromRGB (2, 0, 6), "South Beach", string.Empty));
			boardList.Add(new Board.Schema.Board ("Mansion", UIImage.FromFile("./logos/mansion.jpg"), UIColor.FromRGB (195, 27, 29), UIColor.FromRGB (2, 0, 6), "South Beach", string.Empty));
			boardList.Add(new Board.Schema.Board ("Nikki Beach", UIImage.FromFile("./logos/nikki.jpg"), UIColor.FromRGB (195, 27, 29), UIColor.FromRGB (2, 0, 6), "South Beach", string.Empty));
			boardList.Add(new Board.Schema.Board ("Wet Willies", UIImage.FromFile("./logos/wetwillies.jpg"), UIColor.FromRGB (140, 52, 50), UIColor.FromRGB (77, 185, 155), "South Beach", string.Empty));	
			boardList.Add(new Board.Schema.Board ("Brickhouse", UIImage.FromFile("./logos/brickhouse.png"), UIColor.FromRGB (140, 52, 50), UIColor.FromRGB (77, 185, 155), "Wynwood", string.Empty));	
			boardList.Add(new Board.Schema.Board ("Coyo", UIImage.FromFile("./logos/coyo.jpeg"), UIColor.FromRGB (140, 52, 50), UIColor.FromRGB (77, 185, 155), "Wynwood", string.Empty));	
			boardList.Add(new Board.Schema.Board ("Panther Coffee", UIImage.FromFile("./logos/panther.JPG"), UIColor.FromRGB (140, 52, 50), UIColor.FromRGB (77, 185, 155), "Wynwood", string.Empty));	
			boardList.Add(new Board.Schema.Board ("Wood Tavern", UIImage.FromFile("./logos/wood.jpg"), UIColor.FromRGB (140, 52, 50), UIColor.FromRGB (77, 185, 155), "Wynwood", string.Empty));	
			boardList.Add(new Board.Schema.Board ("Electric Pickle", UIImage.FromFile("./logos/electricpickle.jpg"), UIColor.FromRGB (140, 52, 50), UIColor.FromRGB (77, 185, 155), "Wynwood", string.Empty));	

			return boardList;
		}  

		private UIImageView GenerateBoardThumb(Board.Schema.Board board, CGPoint ContentOffset)
		{
			float imgx, imgy, imgw, imgh;

			float autosize = thumbSize;
			float scale = (float)(board.Image.Size.Height/board.Image.Size.Width);

			if (scale > 1) {
				scale = (float)(board.Image.Size.Width/board.Image.Size.Height);
				imgh = autosize;
				imgw = autosize * scale;
			}
			else {
				imgw = autosize;
				imgh = autosize * scale;	
			}

			imgx = (float)(ContentOffset.X);

			if (imgx < AppDelegate.ScreenWidth / 2) {
				imgx -= autosize / 4;
			} else if (AppDelegate.ScreenWidth / 2 < imgx) {
				imgx += autosize / 4;
			}

			imgy = (float)(ContentOffset.Y);

			// launches the image preview
			UIImageView boardIcon = new UIImageView (new CGRect (0, 0, autosize, autosize));
			boardIcon.Center = new CGPoint (imgx, imgy);
			//boardIcon.BackgroundColor = UIColor.Black;

			UIImageView boardImage = new UIImageView(new CGRect (0, 0, imgw* .8f, imgh* .8f));
			boardImage.Center = new CGPoint (autosize/2, autosize/2);
			UIImage img = CommonUtils.ResizeImage (board.Image, boardIcon.Frame.Size);
			boardImage.Image = img;

			boardIcon.AddSubview (boardImage);

			UIImageView circle = new UIImageView (new CGRect(0,0,autosize,autosize));
			circle.Center = new CGPoint(autosize/2, autosize/2);
			circle.Image = circleImage;

			//boardIcon.AddSubview (circle);

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (sideMenuIsUp)
				{sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; return;}

				BoardInterface boardInterface = new BoardInterface(board, false);
				map.RemoveObserver (this, new NSString ("myLocation"));
				NavigationController.PushViewController(boardInterface, true);
			});

			boardIcon.AddGestureRecognizer (tap);
			boardIcon.UserInteractionEnabled = true;

			return boardIcon;
		}

		private void LoadContent()
		{
			thumbSize = AppDelegate.ScreenWidth / 4;
			circleImage = UIImage.FromFile ("./screems/home/circle.png");
			content = new UIScrollView(new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			List<Board.Schema.Board> boardList = GenerateBoardList ();
			boardList = boardList.OrderBy(o=>o.Location).ToList();

			int i = 1;

			string location = String.Empty;

			// starting point
			float yposition = 5;

			foreach (Board.Schema.Board b in boardList) {
				if (location != b.Location) {
					
					// draw new location string
					yposition += 70;
					UILabel lblLocation = new UILabel(new CGRect(30, yposition, AppDelegate.ScreenWidth - 40, 24));
					yposition += (float)lblLocation.Frame.Height + thumbSize / 2 + 10;
					lblLocation.Font = UIFont.FromName ("narwhal-bold", 20);
					lblLocation.TextColor = UIColor.FromRGB (241, 93, 74);
					location = b.Location;
					lblLocation.Text = location.ToUpper();
					content.AddSubview (lblLocation);
					i = 1;
				}
				 
				UIImageView igv = GenerateBoardThumb (b, new CGPoint ((AppDelegate.ScreenWidth/ 4) * i, yposition));
				i++;
				if (i >= 4) {
					i = 1;
					yposition += thumbSize + 6;
				}
				content.AddSubview (igv);
			}

			content.ScrollEnabled = true;
			content.UserInteractionEnabled = true;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (sideMenuIsUp)
				{ sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; return; }
			});

			content.AddGestureRecognizer (tap);
			content.UserInteractionEnabled = true;
			content.ContentSize = new CGSize (AppDelegate.ScreenWidth, yposition + thumbSize + 25);

			View.AddSubview (content);
		}

		public void HideSideMenu()
		{
			if (sideMenuIsUp)
			{ sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; }
		}

		private void LoadSideMenu()
		{
			UIImage bannerImage = UIImage.FromFile ("./screens/home/sidemenu/" + AppDelegate.PhoneVersion + ".png");

			sidemenu = new UIImageView(new CGRect(0,0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
			sidemenu.Image = bannerImage;

			float[] buttonLocations = new float[4];
			if (AppDelegate.PhoneVersion == "6") {
				buttonLocations [0] = 350;
				buttonLocations [1] = 440;
				buttonLocations [2] = 525;
				buttonLocations [3] = 605;
			} else {
				buttonLocations [0] = 390;
				buttonLocations [1] = 470;
				buttonLocations [2] = 550;
				buttonLocations [3] = 630;
			}

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (tg.LocationInView(this.View).Y > buttonLocations[0]-35 && tg.LocationInView(this.View).Y < buttonLocations[0]+35 ){
					BusinessScreen screen = new BusinessScreen();
					map.RemoveObserver (this, new NSString ("myLocation"));
					NavigationController.PushViewController(screen, false);
				}
				else if (tg.LocationInView(this.View).Y > buttonLocations[1]-35 && tg.LocationInView(this.View).Y < buttonLocations[1]+35){
					SettingsScreen screen = new SettingsScreen();
					map.RemoveObserver (this, new NSString ("myLocation"));
					NavigationController.PushViewController(screen, false);
				}
				else if (tg.LocationInView(this.View).Y > buttonLocations[2]-35 && tg.LocationInView(this.View).Y < buttonLocations[2]+35){
					SupportScreen screen = new SupportScreen();
					map.RemoveObserver (this, new NSString ("myLocation"));
					NavigationController.PushViewController(screen, false);
				}
				else if (tg.LocationInView(this.View).Y > buttonLocations[3]-35 && tg.LocationInView(this.View).Y < buttonLocations[3]+35){
					InviteScreen screen = new InviteScreen();
					map.RemoveObserver (this, new NSString ("myLocation"));
					NavigationController.PushViewController(screen, false);
				}
				HideSideMenu();
			});

			sidemenu.UserInteractionEnabled = true;
			sidemenu.AddGestureRecognizer (tap);
			sidemenu.Alpha = 0f;

			profileView = new ProfilePictureView (new CGRect (11, 78, 149, 149));
			profileView.ProfileId = Profile.CurrentProfile.UserID;
			profileView.Alpha = 0f;

			sideMenuIsUp = false;

			View.AddSubviews (profileView, sidemenu);

			UIFont namefont = UIFont.FromName("narwhal-bold", 20);
			UIFont lastnamefont = UIFont.FromName("narwhal-bold", 24);

			UILabel name = new UILabel (new CGRect(10, profileView.Frame.Bottom + 15, sidemenu.Frame.Width - 20, 20));
			name.Font = namefont;
			name.Text = Profile.CurrentProfile.FirstName;
			name.TextColor = UIColor.White;
			name.TextAlignment = UITextAlignment.Center;
			name.AdjustsFontSizeToFitWidth = true;
			sidemenu.AddSubview (name);

			UILabel lastname = new UILabel (new CGRect(10, name.Frame.Bottom + 3, sidemenu.Frame.Width - 20, 24));
			lastname.Font = lastnamefont;
			lastname.AdjustsFontSizeToFitWidth = true;
			lastname.Text = Profile.CurrentProfile.LastName;
			lastname.TextColor = UIColor.White;
			lastname.TextAlignment = UITextAlignment.Center;
			sidemenu.AddSubview (lastname);
		}

		private void LoadBanner()
		{
			UIImage bannerImage = UIImage.FromFile ("./screens/home/banner/" + AppDelegate.PhoneVersion + ".jpg");

			banner = new UIImageView(new CGRect(0,0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
			banner.Image = bannerImage;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (sideMenuIsUp)
				{ sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; return; }

				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					sidemenu.Alpha = 1f;
					profileView.Alpha = 1f;
					sideMenuIsUp = true;
				}
			});

			banner.UserInteractionEnabled = true;
			banner.AddGestureRecognizer (tap);
			banner.Alpha = .95f;
			View.AddSubview (banner);
		}
			
		private void LoadMap()
		{
			var camera = CameraPosition.FromCamera (latitude: 40, 
				longitude: -100, 
				zoom: -2);
			
			map = MapView.FromCamera (new CGRect (0, banner.Frame.Bottom, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight - banner.Frame.Height - map_button.Frame.Height), camera);
			map.Alpha = 0f;
			map.Settings.CompassButton = true;
			map.Settings.MyLocationButton = true;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (sideMenuIsUp)
				{ sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; return; }
			});

			map.UserInteractionEnabled = true;
			map.AddGestureRecognizer (tap);

			View.AddSubview (map);

			InvokeOnMainThread (()=> map.MyLocationEnabled = true);
		}

		public override void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			if (!firstLocationUpdate) {
				firstLocationUpdate = true; 

				var location = change.ObjectForKey (NSValue.ChangeNewKey) as CoreLocation.CLLocation;
				map.Camera = CameraPosition.FromCamera (location.Coordinate, 15);
			}
		}

		private void LoadMapButton()
		{
			UIImage mapImage = UIImage.FromFile ("./screens/home/map/" + AppDelegate.PhoneVersion + ".jpg");;
			UIImage listImage = UIImage.FromFile ("./screens/home/list/" + AppDelegate.PhoneVersion + ".jpg");;

			map_button = new UIImageView(new CGRect(0,AppDelegate.ScreenHeight - (mapImage.Size.Height / 2),
				mapImage.Size.Width / 2, mapImage.Size.Height / 2));
			map_button.Image = mapImage;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (sideMenuIsUp)
				{ sidemenu.Alpha = 0f; profileView.Alpha = 0f; sideMenuIsUp = false; return; }

				if (map.Alpha == 0f)
				{ map.Alpha = 1f; map_button.Image = listImage; } else { map.Alpha = 0f; map_button.Image = mapImage; } 
			});

			map_button.UserInteractionEnabled = true;
			map_button.AddGestureRecognizer (tap);
			map_button.Alpha = .95f;
			View.AddSubview (map_button);
		}


	}
}