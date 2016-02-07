using System;
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

namespace Solution
{
	// user interface - connects to the board controller
	// also called BoardView
	public class BoardInterface : UIViewController
	{
		private Gallery gallery;

		const float primaryBar = Button.ButtonSize + 20;
		const float secondaryBar = primaryBar / 2;

		public static UIImageView CenterLogo;
		Board board;

		private NSObject orientationObserver;
		public static UIScrollView zoomingScrollView;
		public static UIScrollView scrollView;
		static float TempContentOffset;
		bool galleryActive = false;

		public enum Tags : byte {Background=1, Content};

		//private CloudController cloudController;
		private ButtonInterface buttonInterface;

		public static int ScrollViewWidthSize = 2500;

		bool TestMode;

		public static List<Picture> ListPictures;
		public static List<PictureComponent> ListPictureComponents;

		public static List<TextBox> ListTextboxes;

		public BoardInterface (Board _board, bool _testMode) : base ("Board", null){
			board = _board;
			TestMode = _testMode;
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		public override void ViewDidLoad ()
		{
			// if it reaches this section, user has been logged in and authorized
			base.ViewDidLoad ();

			//StorageController.Initialize ();

			//GetFromLocalDB ();

			// updates the board
			//RefreshContent ();

			// initialices interface and place the local pictures on board
			InitializeInterface ();

			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;

			// downloads new Board content into the local DB
			//await StorageController.UpdateLocalDB ();

			// updates the board
			RefreshContent ();

			// initializes the gallery
			//InitializeGallery ();

			/*TextBox tb = new TextBox (CloudController.BoardUser.Id, "Hello, world!");
			
			TextBoxComponent tbc = new TextBoxComponent (tb, NavigationController.PushViewController);

			await tbc.LoadTextBoxComponent ();

			View.AddSubview (tbc.GetUIView ());*/

		}


		private static UIImageView CreateColorSquare(CGSize size, CGPoint center, CGColor startcolor)
		{
			CGRect frame = new CGRect (0, 0, size.Width, size.Height);

			CGRect frame2 = frame;

			UIGraphics.BeginImageContext (new CGSize(frame.Size.Width, frame.Size.Height));
			CGContext context = UIGraphics.GetCurrentContext ();

			context.SetFillColor(startcolor);
			context.FillRect(frame);

			UIImage orange = UIGraphics.GetImageFromCurrentImageContext ();
			UIImageView uiv = new UIImageView (orange);
			uiv.Center = center;

			return uiv;
		}


		private void GetFromLocalDB()
		{
			//ListPictures = StorageController.ReturnAllStoredPictures (false);
			//ListTextboxes = StorageController.ReturnAllStoredTextboxes (false);
		}

		private void InitializeGallery() {
			UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();

			gallery = new Gallery ();
			galleryActive = false;
			View.AddSubview (gallery.GetScrollView ());

			orientationObserver = UIDevice.Notifications.ObserveOrientationDidChange ((s, e) => {
				
				if (galleryActive)
				{
					if (UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.Portrait)
					{
						BoardEnabled(true);
						gallery.SetAlpha(0f);
						galleryActive = false;
					}	
				}

				else
				{
					if (UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeLeft)
					{
						gallery.SetOrientation(false);
						ActivateGallery();
					}
					else if (UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeRight)
					{
						gallery.SetOrientation(true);
						ActivateGallery();
					}
				}
			});
		}

		private void ActivateGallery()
		{
			if (gallery.LoadGallery ()) {
				BoardEnabled(false);
				gallery.SetAlpha (1f);
				galleryActive = true;
			}
		}

		private void BoardEnabled(bool enabled)
		{
			float alpha;
			if (enabled) {
				alpha = 1f;
			} else {
				alpha = 0f;
			}
			scrollView.Alpha = alpha;
			zoomingScrollView.Alpha = alpha;

			if (enabled) {
				ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.NavigationBar);	
			}
			else{
				ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.Disable);
			}
		}

		private void InitializeInterface()
		{
			this.View.BackgroundColor = board.MainColor;

			// generate the scrollview and the zoomingscrollview
			GenerateScrollViews ();

			// create our image view
			LoadBackground ();

			// load buttons
			LoadButtons ();
		}

		private void GenerateScrollViews()
		{
			scrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));

			scrollView.Scrolled += (object sender, EventArgs e) => {
				// call from here "open eye" function
			};

			zoomingScrollView = new UIScrollView (new CGRect (0, 0, ScrollViewWidthSize, AppDelegate.ScreenHeight));
			zoomingScrollView.AddSubview (scrollView);
			View.AddSubview (zoomingScrollView);
		}

		public static void ZoomScrollview()
		{
			zoomingScrollView.SetZoomScale(1f, true);
			scrollView.Frame = new CGRect(0, 0, AppDelegate.ScreenWidth, scrollView.Frame.Height);
			scrollView.SetContentOffset (new CGPoint (TempContentOffset, 0), false);
		}

		public static void UnzoomScrollview()
		{
			// TODO: remove hardcode and programatically derive the correct zooming value (.15f is current)
			TempContentOffset = (float)scrollView.ContentOffset.X;
			scrollView.Frame = new CGRect(0, AppDelegate.ScreenHeight/2 - 70,
												ScrollViewWidthSize, scrollView.Frame.Height);

			zoomingScrollView.SetZoomScale(.15f, true);
		}

		private void LoadButtons()
		{
			buttonInterface = new ButtonInterface (RefreshContent, scrollView, NavigationController, board.SecondaryColor);

			if (Profile.CurrentProfile.UserID == board.CreatorId) {
				this.View.AddSubviews (buttonInterface.GetCreatorButtons());
			} else {
				this.View.AddSubviews (buttonInterface.GetUserButtons ());
			}
		}

		public void RemoveAllContent()
		{
			foreach (UIView iv in scrollView) { 
				if (iv.Tag != (int)Tags.Background) {
					iv.RemoveFromSuperview ();
					iv.Dispose ();
				}
			}
		}

		public void RefreshContent()
		{
			RemoveAllContent();

			ListPictures = new List<Picture> ();
			ListPictureComponents = new List<PictureComponent> ();

			ListTextboxes = new List<TextBox> ();

			//GetFromLocalDB ();

			GenerateTestPictures ();


			foreach (Picture p in ListPictures) {
				DrawPictureComponent (p);
			}

			foreach (TextBox tb in ListTextboxes) {
				DrawTextbox (tb);
			}
		}

		private void GenerateTestPictures()
		{
			AddTestPicture (UIImage.FromFile ("./boardscreen/testpictures/0.jpg"), 40, 40, -.03f);
			AddTestPicture (UIImage.FromFile ("./boardscreen/testpictures/1.jpg"), 25, 330, -.1f);
			AddTestPicture (UIImage.FromFile ("./boardscreen/testpictures/2.jpg"), 290, 20, 0f);
			AddTestPicture (UIImage.FromFile ("./boardscreen/testpictures/3.jpg"), 330, 280, -.04f);
			AddTestPicture (UIImage.FromFile ("./boardscreen/testpictures/4.jpg"), 710, 50, .05f);
			AddTestPicture (UIImage.FromFile ("./boardscreen/testpictures/5.jpg"), 650, 310, -.02f);
		}

		private void AddTestPicture(UIImage image, float imgx, float imgy, float rotation)
		{
			Picture pic = new Picture ();
			pic.Image = image;
			pic.ImgX = imgx;
			pic.ImgY = imgy;
			pic.Rotation = rotation;
			ListPictures.Add (pic);
		}

		private void DrawPictureComponent(Picture picture)
		{
			PictureComponent component = new PictureComponent (picture);

			UIView componentView = component.GetUIView ();

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				LookUp lookUp = new LookUp(picture);
				NavigationController.PushViewController(lookUp, true);
			});

			componentView.AddGestureRecognizer (tap);
			componentView.UserInteractionEnabled = true;

			scrollView.AddSubview (component.GetUIView());
			ListPictureComponents.Add (component);
		}

		private async void DrawTextbox(TextBox tb)
		{
			TextBoxComponent textBoxComponent = new TextBoxComponent (tb);

			await textBoxComponent.Load (NavigationController, RefreshContent);

			UIView uiv = textBoxComponent.GetUIView ();

			scrollView.AddSubview (uiv);
		}

		private void LoadBackground()
		{	
			UIImage pattern = UIImage.FromFile ("./boardscreen/backgrounds/branco.jpg");

			UIImageView boardView = new UIImageView (pattern);
			boardView.Frame = new CGRect (0, 0, ScrollViewWidthSize, AppDelegate.ScreenHeight);
			boardView.Tag = (int)Tags.Background;
			this.AutomaticallyAdjustsScrollViewInsets = false;

			// this limits the size of the scrollview
			scrollView.ContentSize = new CGSize(ScrollViewWidthSize, AppDelegate.ScreenHeight);
			// sets the scrollview on the middle of the view
			scrollView.SetContentOffset (new CGPoint(ScrollViewWidthSize/2 - AppDelegate.ScreenWidth/2, 0), false);
			// adds the background
			scrollView.AddSubview (boardView);

			zoomingScrollView.MaximumZoomScale = 1f;
			zoomingScrollView.MinimumZoomScale = .15f;

			zoomingScrollView.ViewForZoomingInScrollView += (UIScrollView sv) => {
				return zoomingScrollView.Subviews[0];
			};

			zoomingScrollView.RemoveGestureRecognizer (zoomingScrollView.PinchGestureRecognizer);

			UIImageView secondary = CreateColorSquare(new CGSize(ScrollViewWidthSize,secondaryBar), new CGPoint(ScrollViewWidthSize/2,AppDelegate.ScreenHeight - primaryBar - secondaryBar / 2), board.SecondaryColor.CGColor);
			secondary.Tag = (int)Tags.Background;
			scrollView.AddSubview (secondary);

			UIImageView primary = CreateColorSquare(new CGSize(ScrollViewWidthSize,primaryBar), new CGPoint(ScrollViewWidthSize/2, AppDelegate.ScreenHeight - primaryBar / 2), board.MainColor.CGColor);
			primary.Tag = (int)Tags.Background;
			scrollView.AddSubview (primary);

			UIImage logo = board.Image;
			UIImageView mainLogo = LoadMainLogo (logo, new CGPoint(ScrollViewWidthSize/2,-20));
			scrollView.AddSubview (mainLogo);

			if (TestMode) {
				UIImageView democontent = new UIImageView (UIImage.FromFile ("./boardscreen/backgrounds/contentdemo2.png"));
				democontent.Frame = boardView.Frame;
				scrollView.AddSubview (democontent);
			}
		}

		private UIImageView LoadMainLogo(UIImage image, CGPoint ContentOffset)
		{
			// the image is uploadable
			// so now launch image preview to choose position in the board
			float imgx, imgy, imgw, imgh;
			float autosize = AppDelegate.ScreenWidth;

			float scale = (float)(image.Size.Width/image.Size.Height);

			if (scale >= 1) {
				imgw = autosize * scale;
				imgh = autosize;

				if (imgw > AppDelegate.ScreenWidth) {
					scale = (float)(image.Size.Height/image.Size.Width);
					imgw = AppDelegate.ScreenWidth;
					imgh = imgw * scale;
				}
			} else {
				scale = (float)(image.Size.Height / image.Size.Width);
				imgw = autosize;
				imgh = autosize * scale;
			}

			imgx = (float)(ContentOffset.X - imgw / 2);
			imgy = (float)(ContentOffset.Y + AppDelegate.ScreenHeight / 2 - imgh / 2 - Button.ButtonSize / 2);

			// launches the image preview

			CGRect frame = new CGRect (imgx, imgy, imgw, imgh);
			UIImageView mainLogo = new UIImageView(frame);

			UIImageView uiImageView =  new UIImageView (new CGRect(0,0,frame.Width, frame.Height));

			UIImage thumbImg = image.Scale (new CGSize (imgw, imgh));
			uiImageView.Image = thumbImg;

			mainLogo.Tag = (int)Tags.Background;
			mainLogo.AddSubviews(uiImageView);

			return mainLogo;
		}
	}
}