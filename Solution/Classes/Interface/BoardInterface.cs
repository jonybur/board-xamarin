using System;
using System.Collections.Generic;
using System.Linq;
using BigTed;
using Board.Interface;
using Board.Interface.Buttons;
using Board.Interface.Widgets;
using Board.Schema;
using CoreGraphics;
using Facebook.CoreKit;
using Foundation;
using UIKit;

namespace Board.Interface
{
	// user interface - connects to the board controller
	// also called BoardView
	public partial class BoardInterface : UIViewController
	{
		private Gallery gallery;
		private NSObject orientationObserver;

		const float buttonBar = 90;

		public static UIImageView CenterLogo;
		public static Board.Schema.Board board;

		public static UIScrollView zoomingScrollView;
		public static UIScrollView scrollView;
		static float TempContentOffset;
		bool galleryActive;

		public enum Tags : byte {Background=1, Content};

		//private CloudController cloudController;

		public static int ScrollViewWidthSize = 2500;

		bool TestMode;

		public static Dictionary<string, Content> DictionaryContent;
		public static Dictionary<string, Widget> DictionaryWidgets;

		EventHandler scrolledEvent;

		bool firstLoad;

		public BoardInterface (Board.Schema.Board _board, bool _testMode){
			board = _board;
			TestMode = _testMode;
			firstLoad = true;
		}

		public override void DidReceiveMemoryWarning ()
		{
			AppDelegate.NavigationController.PopViewController (true);
		}

		public override void ViewDidLoad ()
		{
			// if it reaches this section, user has been logged in and authorized
			base.ViewDidLoad ();

			BTProgressHUD.Show();

			InitializeLists ();

			InitializeInterface ();

			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;

		}

		private void InitializeLists()
		{
			DictionaryContent = new Dictionary<string, Content> ();

			DictionaryWidgets = new Dictionary<string, Widget> ();
		}

		public override void ViewDidAppear(bool animated)
		{
			if (firstLoad) {
				
				GenerateTestPictures ();

				RefreshContent ();

				SuscribeToEvents ();

				firstLoad = false;

				BTProgressHUD.Dismiss ();
			}
		}

		public void ExitBoard()
		{
			RemoveAllContent ();
			ButtonInterface.DisableAllLayouts();
			UnsuscribeToEvents ();
		}

		public void RemoveAllContent()
		{
			foreach(KeyValuePair<string, Widget> widget in DictionaryWidgets)
			{
				if (widget.Value is VideoWidget) {
					(widget.Value as VideoWidget).KillVideo ();
				}

				widget.Value.UnsuscribeToEvents ();
				widget.Value.View.RemoveFromSuperview ();
			}

			DictionaryWidgets = null;
		}

		/*

		private UIImageView CreateColorView(CGRect frame, CGColor color)
		{
			UIGraphics.BeginImageContext (new CGSize(frame.Size.Width, frame.Size.Height));
			CGContext context = UIGraphics.GetCurrentContext ();

			context.SetFillColor(color);
			context.FillRect(frame);

			UIImageView uiv;
			using (UIImage img = UIGraphics.GetImageFromCurrentImageContext ()) {
				uiv = new UIImageView (img);
			}
			uiv.Frame = frame;

			return uiv;
		}*/

		// deprecated for now
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
			float alpha = enabled ? 1f : 0f;

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

			scrolledEvent = (sender, e) => {
				// call from here "open eye" function
				if (!scrollView.Dragging) { return; } 

				if (!(DictionaryWidgets == null || DictionaryWidgets.Count == 0))
				{
					// the ones at the left ;; the ones at the right ;; if it doesnt have an open eye
					Widget wid = DictionaryWidgets.Values.ToList().Find(item => ((item.View.Frame.X) > scrollView.ContentOffset.X) &&
						((item.View.Frame.X + item.View.Frame.Width) < (scrollView.ContentOffset.X + AppDelegate.ScreenWidth)) &&
						!item.EyeOpen);
					
					if (wid != null)
					{
						OpenEye(wid);
					}
				}

			};

			zoomingScrollView = new UIScrollView (new CGRect (0, 0, ScrollViewWidthSize, AppDelegate.ScreenHeight));
			zoomingScrollView.AddSubview (scrollView);

			View.AddSubview (zoomingScrollView);
		}


		private void OpenEye(Widget widget)
		{
			widget.OpenEye();
			ButtonInterface.navigationButton.SubtractNavigationButtonText();
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
			ButtonInterface.Initialize (RefreshContent);

			UIImageView buttonBackground = new UIImageView (new CGRect (0, 0, AppDelegate.ScreenWidth, 45));
			buttonBackground.BackgroundColor = UIColor.FromRGBA (255, 255, 255, 240);
			buttonBackground.Frame = new CGRect (0, AppDelegate.ScreenHeight - 45, AppDelegate.ScreenWidth, 45);
			this.View.AddSubview (buttonBackground);

			if (Profile.CurrentProfile.UserID == board.CreatorId) {
				this.View.AddSubviews (ButtonInterface.GetCreatorButtons());
			} else {
				this.View.AddSubviews (ButtonInterface.GetUserButtons ());
			}

			ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.NavigationBar);
		}

		public void RefreshContent()
		{
			//RemoveAllContent ();

			// looks for new keys in the DictionaryContent
			// draws new widget in case new content is found

			foreach (KeyValuePair<string, Content> c in DictionaryContent) {
				if (!DictionaryWidgets.ContainsKey (c.Key)) {
					DrawWidget (c.Value);
				}
			}

			//DictionaryWidgets = DictionaryWidgets.OrderBy(o=>o.View.Frame.X).ToList();

			ButtonInterface.navigationButton.RefreshNavigationButtonText (DictionaryWidgets.Count);
		}

		private void DrawWidget(Content content)
		{
			Widget widget;

			if (content is Video) {
				widget = new VideoWidget (content as Video);
			} else if (content is Picture) {
				widget = new PictureWidget (content as Picture);
			} else if (content is BoardEvent) {
				widget = new EventWidget (content as BoardEvent);
			} else if (content is Announcement) {
				widget = new AnnouncementWidget (content as Announcement);
			} else {
				widget = new Widget ();
			}

			scrollView.AddSubview (widget.View);
			widget.SuscribeToEvents ();
			DictionaryWidgets.Add (content.Id, widget);
		}

		private void LoadBackground()
		{	
			UIImage logo = board.ImageView.Image;
			LoadMainLogo (logo, new CGPoint(ScrollViewWidthSize/2, 0));

			scrollView.BackgroundColor = board.MainColor;

			UIImageView circleTop, circleLower;

			using (UIImage img = UIImage.FromFile ("./boardinterface/backgrounds/intern.png")){
				UIImage circle1 = img;
				circle1 = circle1.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				circleTop = new UIImageView (circle1);
			}

			using (UIImage img = UIImage.FromFile ("./boardinterface/backgrounds/outer.png")) {
				UIImage circle2 = img;
				circle2 = circle2.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				circleLower = new UIImageView (circle2);
			}


			circleTop.Frame = new CGRect (0, 0, ScrollViewWidthSize, AppDelegate.ScreenHeight);
			circleTop.Tag = (int)Tags.Background;

			circleLower.Frame = circleTop.Frame;
			circleLower.Tag = (int)Tags.Background;

			circleTop.TintColor = board.MainColor;
			circleLower.TintColor = board.SecondaryColor;

			this.AutomaticallyAdjustsScrollViewInsets = false;

			// this limits the size of the scrollview
			scrollView.ContentSize = new CGSize(ScrollViewWidthSize, AppDelegate.ScreenHeight);
			// sets the scrollview on the middle of the view
			scrollView.SetContentOffset (new CGPoint(ScrollViewWidthSize/2 - AppDelegate.ScreenWidth/2, 0), false);
			// adds the background
			scrollView.AddSubviews (circleTop, circleLower);

			zoomingScrollView.MaximumZoomScale = 1f;
			zoomingScrollView.MinimumZoomScale = .15f;

			zoomingScrollView.RemoveGestureRecognizer (zoomingScrollView.PinchGestureRecognizer);
		}

		private void SuscribeToEvents()
		{
			scrollView.Scrolled += scrolledEvent;
			zoomingScrollView.ViewForZoomingInScrollView += sv => zoomingScrollView.Subviews [0];
		}

		private void UnsuscribeToEvents()
		{
			scrollView.Scrolled -= scrolledEvent;
			zoomingScrollView.ViewForZoomingInScrollView -= sv => zoomingScrollView.Subviews [0];
		}

		private void LoadMainLogo(UIImage image, CGPoint ContentOffset)
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
			imgy = (float)(ContentOffset.Y + AppDelegate.ScreenHeight / 2 - imgh / 2);

			UIImageView circleBackground;
			using (UIImage img =UIImage.FromFile ("./boardinterface/backgrounds/logobackground.png"))
			{
				UIImage circle3 = img;
				circleBackground = new UIImageView (circle3);
			}

			circleBackground.Frame = new CGRect (0, 0, ScrollViewWidthSize, AppDelegate.ScreenHeight);
			circleBackground.Tag = (int)Tags.Background;

			UIImageView mainLogo = new UIImageView(new CGRect (imgx, imgy, imgw, imgh));
			mainLogo.Image = image.Scale (new CGSize (imgw, imgh));
			mainLogo.Tag = (int)Tags.Background;

			scrollView.AddSubviews (circleBackground, mainLogo);
		}
	}
}