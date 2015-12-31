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

namespace Solution
{
	// user interface - connects to the board controller
	// also called BoardView
	public partial class BoardInterface : UIViewController
	{
		public static float ScreenWidth, ScreenHeight;

		private Gallery gallery;

		public static UIColor InterfaceColor = UIColor.FromRGB(196,25,23);

		private NSObject orientationObserver;
		public static UIScrollView zoomingScrollView;
		public static UIScrollView scrollView;
		static float TempContentOffset;
		bool galleryActive = false;

		public enum Tags : byte {Background=1, Content};

		//private CloudController cloudController;
		private ButtonInterface buttonInterface;

		public static int ScrollViewWidthSize = 2500;

		public static List<Picture> ListPictures;
		public static List<TextBox> ListTextboxes;

		public BoardInterface () : base ("Board", null){}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		public override void ViewDidLoad ()
		{
			// if it reaches this section, user has been logged in and authorized
			base.ViewDidLoad ();

			ScreenWidth = (float)UIScreen.MainScreen.Bounds.Width;
			ScreenHeight = (float)UIScreen.MainScreen.Bounds.Height;

			//StorageController.Initialize ();

			// initialices interface and place the local pictures on board
			InitializeInterface ();

			//GetFromLocalDB ();

			// updates the board
			//RefreshContent ();

			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;

			// downloads new Board content into the local DB
			//await StorageController.UpdateLocalDB ();

			// updates the board
			//RefreshContent ();

			// initializes the gallery
			//InitializeGallery ();

			/*TextBox tb = new TextBox (CloudController.BoardUser.Id, "Hello, world!");
			
			TextBoxComponent tbc = new TextBoxComponent (tb, NavigationController.PushViewController);

			await tbc.LoadTextBoxComponent ();

			View.AddSubview (tbc.GetUIView ());*/

		}

		private void GetFromLocalDB()
		{
			ListPictures = StorageController.ReturnAllStoredPictures (false);
			ListTextboxes = StorageController.ReturnAllStoredTextboxes (false);
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
			else
			{
				ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.Disable);
			}
		}

		private void InitializeInterface()
		{
			this.View.BackgroundColor = InterfaceColor;
			//this.View.BackgroundColor = UIColor.FromRGB(189,34,58);

			// generate the scrollview and the zoomingscrollview
			GenerateScrollViews ();

			// create our image view
			LoadBackground ();

			// load buttons
			LoadButtons ();
		}

		private void GenerateScrollViews()
		{
			// board orange color
			/*UIImage background = UIImage.FromFile ("./boardscreen/backgrounds/interfacebackground.png");
			UIImageView boardBack = new UIImageView (background);
			boardBack.Frame = new RectangleF (0, 0, ScreenWidth, ScreenHeight);
			View.AddSubview (boardBack);*/

			scrollView = new UIScrollView (new CGRect (0, 0, ScreenWidth, ScreenHeight));
			zoomingScrollView = new UIScrollView (new CGRect (0, 0, ScrollViewWidthSize, ScreenHeight));
			zoomingScrollView.AddSubview (scrollView);
			View.AddSubview (zoomingScrollView);
		}

		public static void ZoomScrollview()
		{
			zoomingScrollView.SetZoomScale(1f, true);
			scrollView.Frame = new CGRect(0, 0, ScreenWidth, scrollView.Frame.Height);
			scrollView.SetContentOffset (new CGPoint (TempContentOffset, 0), false);
		}

		public static void UnzoomScrollview()
		{
			// TODO: remove hardcode and programatically derive the correct zooming value (.15f is current)
			TempContentOffset = (float)scrollView.ContentOffset.X;
			scrollView.Frame = new CGRect(0, ScreenHeight/2 - 70,
												ScrollViewWidthSize, scrollView.Frame.Height);

			zoomingScrollView.SetZoomScale(.15f, true);
		}

		private void LoadButtons()
		{
			buttonInterface = new ButtonInterface (RefreshContent, scrollView, NavigationController);

			this.View.AddSubviews (buttonInterface.GetAllViews());
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

			GetFromLocalDB ();

			foreach (Picture p in ListPictures) {
				DrawPicture (p);
			}

			foreach (TextBox tb in ListTextboxes) {
				DrawTextbox (tb);
			}
		}
			
		public static bool CheckRectangleFCollision(CGRect prev)
		{
			foreach (Picture pic in ListPictures) {
				CGRect aux = pic.GetRectangleF();
				if (prev.IntersectsWith (aux)) {
					return true;
				}
			}

			foreach (TextBox tb in ListTextboxes) {
				CGRect aux = tb.GetRectangleF();
				if (prev.IntersectsWith (aux)) {
					return true;
				}
			}
			return false;
		}

		private void DrawPicture(Picture p)
		{
			UIImageView imageView = new UIImageView (new CGRect (0, 0, p.ImgW, p.ImgH));
			imageView.Transform = CGAffineTransform.MakeRotation (p.Rotation);
			imageView.Center = new CGPoint (p.ImgX, p.ImgY);
			imageView.Image = p.GetThumbnailImage ();
			imageView.UserInteractionEnabled = true;
			imageView.Tag = (int)Tags.Content;

			// creates the imageview which contains the corresponding tap gesture
			UITapGestureRecognizer tapGesture= new UITapGestureRecognizer (async (tg) => {
				// there was a tap on the board's image so it is brought full-screen
				LookUp lookup = new LookUp(p, p.GetImage(), RefreshContent, NavigationController);
				await lookup.CreateNameLabel(p.UserId);
				NavigationController.PushViewController(lookup, true);
				ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.Disable);
			});

			imageView.AddGestureRecognizer (tapGesture);

			scrollView.AddSubview (imageView);
		}

		private async void DrawTextbox(TextBox tb)
		{
			TextBoxComponent textBoxComponent = new TextBoxComponent (tb);

			await textBoxComponent.LoadTextBoxComponent (NavigationController, RefreshContent);

			UIView uiv = textBoxComponent.GetUIView ();

			scrollView.AddSubview (uiv);
		}

		private void LoadBackground()
		{
			UIImage pattern = UIImage.FromFile ("./boardscreen/backgrounds/mangos.jpg");

			UIImageView boardView = new UIImageView (pattern);
			boardView.Frame = new CGRect (0, 0, ScrollViewWidthSize, ScreenHeight);
			boardView.Tag = (int)Tags.Background;
			this.AutomaticallyAdjustsScrollViewInsets = false;

			// this limits the size of the scrollview
			scrollView.ContentSize = new CGSize(ScrollViewWidthSize, ScreenHeight);
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
		}
	}
}