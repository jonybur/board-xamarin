using System;
using CoreGraphics;
using System.Linq;

using Foundation;
using UIKit;

using System.Collections.Generic;

using MediaPlayer;

using Facebook.CoreKit;
using Board.Interface.Buttons;
using Board.Interface.Widgets;
using Board.Interface;
using Board.Schema;

namespace Board.Interface
{
	// user interface - connects to the board controller
	// also called BoardView
	public class BoardInterface : UIViewController
	{
		private Gallery gallery;
		private NSObject orientationObserver;

		const float buttonBar = 90;

		public static UIImageView CenterLogo;
		public static Board.Schema.Board board;

		public static UIScrollView zoomingScrollView;
		public static UIScrollView scrollView;
		static float TempContentOffset;
		bool galleryActive = false;

		public enum Tags : byte {Background=1, Content};

		//private CloudController cloudController;

		public static int ScrollViewWidthSize = 2500;

		bool TestMode;

		public static List<Picture> ListPictures;
		public static List<Announcement> ListAnnouncements;
		public static List<Video> ListVideos;
		public static List<BoardEvent> ListEvents;

		public static List<Widget> ListWidgets;

		EventHandler scrolledEvent;

		bool firstLoad;

		public BoardInterface (Board.Schema.Board _board, bool _testMode) : base ("Board", null){
			board = _board;
			TestMode = _testMode;
			firstLoad = true;
		}

		public override void DidReceiveMemoryWarning ()
		{
			GC.Collect ();
		}

		public override void ViewDidLoad ()
		{
			// if it reaches this section, user has been logged in and authorized
			base.ViewDidLoad ();

			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;

			InitializeLists ();

			InitializeInterface ();
		}

		private void InitializeLists()
		{
			ListPictures = new List<Picture> ();
			ListVideos = new List<Video> ();
			ListAnnouncements = new List<Announcement> ();
			ListEvents = new List<BoardEvent> ();
			ListWidgets = new List<Widget> ();
		}

		public override void ViewDidAppear(bool animated)
		{
			if (firstLoad) {
				RefreshContent ();

				SuscribeToEvents ();

				firstLoad = false;
			}
		}

		public void ExitBoard()
		{
			foreach (Widget widget in ListWidgets) {
				if (widget is VideoWidget) {
					(widget as VideoWidget).KillLooper ();
				}

				widget.UnsuscribeToEvents ();
				widget.View.RemoveFromSuperview ();
			}
			ListWidgets = new List<Widget> ();
			ButtonInterface.DisableAllLayouts();
			UnsuscribeToEvents ();
		}

		private UIImageView CreateColorView(CGRect frame, CGColor color)
		{
			UIGraphics.BeginImageContext (new CGSize(frame.Size.Width, frame.Size.Height));
			CGContext context = UIGraphics.GetCurrentContext ();

			context.SetFillColor(color);
			context.FillRect(frame);

			UIImage orange = UIGraphics.GetImageFromCurrentImageContext ();
			UIImageView uiv = new UIImageView (orange);
			uiv.Frame = frame;

			return uiv;
		}

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

		// TODO: improve this method
		private void GenerateScrollViews()
		{
			scrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));

			scrolledEvent = (sender, e) => {
				// call from here "open eye" function

				if (!(ListWidgets == null || ListWidgets.Count == 0))
				{
					Widget wid = ListWidgets.Find(item => ((item.View.Frame.X + item.View.Frame.Width) > scrollView.ContentOffset.X) &&
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
			ButtonInterface.Initialize (delegate {
											RefreshContent ();
										});

			UIImageView buttonBackground = CreateColorView (new CGRect (0, 0, AppDelegate.ScreenWidth, 45), UIColor.White.CGColor);
			buttonBackground.Alpha = .95f;
			buttonBackground.Frame = new CGRect (0, AppDelegate.ScreenHeight - 45, AppDelegate.ScreenWidth, 45);
			this.View.AddSubview (buttonBackground);

			if (Profile.CurrentProfile.UserID == board.CreatorId) {
				this.View.AddSubviews (ButtonInterface.GetCreatorButtons());
			} else {
				this.View.AddSubviews (ButtonInterface.GetUserButtons ());
			}

			ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.NavigationBar);
		}

		public void RemoveAllContent()
		{
			foreach (Widget widget in ListWidgets) {
				widget.UnsuscribeToEvents ();
				widget.View.RemoveFromSuperview ();
			}

			ListWidgets = new List<Widget> ();
		}

		public void RefreshContent()
		{
			RemoveAllContent ();

			GenerateTestPictures ();

			foreach (Picture p in ListPictures) {
				DrawPictureWidget (p);
			}

			foreach (Video v in ListVideos) {
				DrawVideoWidget (v);
			}

			foreach (Announcement a in ListAnnouncements) {
				DrawAnnouncementWidget (a);
			}

			foreach (BoardEvent e in ListEvents) {
				DrawEventWidget (e);
			}

			ListWidgets = ListWidgets.OrderBy(o=>o.View.Frame.X).ToList();

			ButtonInterface.navigationButton.RefreshNavigationButtonText (ListWidgets.Count);
		}

		private void GenerateTestPictures()
		{
			using (UIImage img = UIImage.FromFile ("./demo/pictures/0.jpg")) {
				AddTestPicture (img, 40, 20, -.03f);
			}

			AddTestVideo ("./demo/videos/0.mp4", 15, 245, -.01f);

			using (UIImage img = UIImage.FromFile ("./demo/pictures/2.jpg")) {
				AddTestPicture (img, 310, 20, 0f);
			}
			using (UIImage img = UIImage.FromFile ("./demo/pictures/1.jpg")) {
				AddTestPicture (img, 330, 245, -.04f);
			}
		
			AddTestVideo ("./demo/videos/1.mp4", 580, 25, -.02f);

			using (UIImage img = UIImage.FromFile ("./demo/pictures/3.jpg")) {
				AddTestPicture (img, 610, 250, .05f);
			}

			BoardEvent bevent;
			using (UIImage img = UIImage.FromFile ("./demo/events/0.jpg")) {
				bevent = new BoardEvent ("La Roxtar", img, new DateTime (2016, 11, 10), 0, new CGRect (1500, 70, 0, 0), null);
			}
			ListEvents.Add (bevent);

			//AddTestPicture (UIImage.FromFile ("./demo/pictures/5.jpg"), , -.02f);
			//AddTestPicture (UIImage.FromFile ("./demo/pictures/4.jpg"), 25, 270, -.1f);

			//AddTestPicture (UIImage.FromFile ("./demo/pictures/6.jpg"), 25, 330, -.1f);
			//AddTestPicture (UIImage.FromFile ("./demo/pictures/7.jpg"), 650, 310, -.02f);
		}

		private void AddTestPicture(UIImage image, float imgx, float imgy, float rotation)
		{
			Picture pic = new Picture ();
			pic.Image = image;
			pic.Frame = new CGRect(imgx, imgy, 0, 0);
			pic.Rotation = rotation;
			ListPictures.Add (pic);
		}

		private void AddTestVideo(string url, float imgx, float imgy, float rotation)
		{
			Video vid = new Video ();

			MPMoviePlayerController moviePlayer = new MPMoviePlayerController (NSUrl.FromFilename(url));
			vid.Thumbnail = moviePlayer.ThumbnailImageAt (0, MPMovieTimeOption.Exact);
			moviePlayer.Pause ();
			moviePlayer.Dispose ();

			vid.Url = url;
			vid.Frame = new CGRect(imgx, imgy, 0, 0);
			vid.Rotation = rotation;
			ListVideos.Add (vid);
		}

		private void DrawVideoWidget(Video video)
		{
			VideoWidget widget = new VideoWidget (video);

			scrollView.AddSubview (widget.View);

			widget.SuscribeToEvents ();

			ListWidgets.Add (widget);
		}

		private void DrawPictureWidget(Picture picture)
		{
			PictureWidget widget = new PictureWidget (picture);

			scrollView.AddSubview (widget.View);

			widget.SuscribeToEvents ();

			ListWidgets.Add (widget);
		}

		private void DrawEventWidget(BoardEvent boardEvent)
		{
			EventWidget component = new EventWidget (boardEvent);

			UIView componentView = component.View;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (Preview.View != null) { return; }
			});

			componentView.AddGestureRecognizer (tap);
			componentView.UserInteractionEnabled = true;

			scrollView.AddSubview (component.View);
			ListWidgets.Add (component);
		}


		private void DrawAnnouncementWidget(Announcement ann)
		{
			AnnouncementWidget announcementWidget = new AnnouncementWidget (ann);

			scrollView.AddSubview (announcementWidget.View);

			ListWidgets.Add (announcementWidget);
		}

		private void LoadBackground()
		{	
			UIImage logo = board.ImageView.Image;
			LoadMainLogo (logo, new CGPoint(ScrollViewWidthSize/2, 0));

			scrollView.BackgroundColor = board.MainColor;

			UIImage circle1 = UIImage.FromFile ("./boardinterface/backgrounds/intern.png");
			UIImage circle2 = UIImage.FromFile ("./boardinterface/backgrounds/outer.png");

			circle1 = circle1.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
			circle2 = circle2.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);

			UIImageView circleTop = new UIImageView (circle1);
			circleTop.Frame = new CGRect (0, 0, ScrollViewWidthSize, AppDelegate.ScreenHeight);
			circleTop.Tag = (int)Tags.Background;

			UIImageView circleLower = new UIImageView (circle2);
			circleLower.Frame = new CGRect (0, 0, ScrollViewWidthSize, AppDelegate.ScreenHeight);
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


			UIImage circle3 = UIImage.FromFile ("./boardinterface/backgrounds/logobackground.png");
			UIImageView circleBackground = new UIImageView (circle3);
			circleBackground.Frame = new CGRect (0, 0, ScrollViewWidthSize, AppDelegate.ScreenHeight);
			circleBackground.Tag = (int)Tags.Background;

			UIImageView mainLogo = new UIImageView(new CGRect (imgx, imgy, imgw, imgh));
			mainLogo.Image = image.Scale (new CGSize (imgw, imgh));
			mainLogo.Tag = (int)Tags.Background;

			scrollView.AddSubviews (circleBackground, mainLogo);
		}
	}
}