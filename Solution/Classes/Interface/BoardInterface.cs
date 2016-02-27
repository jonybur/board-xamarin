using System;
using CoreGraphics;
using System.Linq;

using Foundation;
using UIKit;

using System.Collections.Generic;
using System.Threading;

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

		const float buttonBar = 90;

		public static UIImageView CenterLogo;
		public static Board.Schema.Board board;

		private NSObject orientationObserver;
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

		public static List<PictureWidget> ListPictureWidgets;
		public static List<AnnouncementWidget> ListAnnouncementWidgets;
		public static List<VideoWidget> ListVideoWidgets;
		public static List<EventWidget> ListEventWidgets;


		public BoardInterface (Board.Schema.Board _board, bool _testMode) : base ("Board", null){
			board = _board;
			TestMode = _testMode;
		}

		public override void ViewDidAppear(bool animated)
		{
			RefreshContent();
		}

		public override void ViewDidLoad ()
		{
			// if it reaches this section, user has been logged in and authorized
			base.ViewDidLoad ();

			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;

			ListPictures = new List<Picture> ();
			ListVideos = new List<Video> ();
			ListAnnouncements = new List<Announcement> ();
			ListEvents = new List<BoardEvent> ();

			BoardEvent bevent = new BoardEvent ("La Roxtar", UIImage.FromFile("./demo/events/0.jpg"), new DateTime(2016, 11, 10),0, new CGRect (1500, 70, 0, 0), null);
			ListEvents.Add (bevent);

			InitializeInterface ();

			//StorageController.Initialize ();

			//GetFromLocalDB ();

			// updates the board
			//RefreshContent ();

			// initialices interface and place the local pictures on board

			// updates the board
			//RefreshContent ();


			// downloads new Board content into the local DB
			//await StorageController.UpdateLocalDB ();


			// initializes the gallery
			//InitializeGallery ();

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

		// TODO: improve this method
		private void GenerateScrollViews()
		{
			scrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));

			scrollView.Scrolled += (object sender, EventArgs e) => {
				// call from here "open eye" function

				if (!(ListPictureWidgets == null || ListPictureWidgets.Count == 0))
				{
					PictureWidget pic = ListPictureWidgets.Find(item => (item.View.Frame.X + item.View.Frame.Width) > scrollView.ContentOffset.X &&
												(item.View.Frame.X + item.View.Frame.Width) < (scrollView.ContentOffset.X + AppDelegate.ScreenWidth) &&
												!item.EyeOpen);

					if (pic != null)
					{
						Thread thread = new Thread(() => OpenEye(pic));
						thread.Start();
					}
				}

				if (!(ListVideoWidgets == null || ListVideoWidgets.Count == 0))
				{
					VideoWidget vid = ListVideoWidgets.Find(item => (item.View.Frame.X + item.View.Frame.Width) > scrollView.ContentOffset.X &&
						(item.View.Frame.X + item.View.Frame.Width) < (scrollView.ContentOffset.X + AppDelegate.ScreenWidth) &&
						!item.EyeOpen);

					if (vid != null)
					{
						Thread thread = new Thread(() => OpenEye(vid));
						thread.Start();
					}
				}

				if (!(ListAnnouncementWidgets == null || ListAnnouncementWidgets.Count == 0))
				{
					AnnouncementWidget ann = ListAnnouncementWidgets.Find(item => (item.View.Frame.X + item.View.Frame.Width) > scrollView.ContentOffset.X &&
						(item.View.Frame.X + item.View.Frame.Width) < (scrollView.ContentOffset.X + AppDelegate.ScreenWidth) &&
						!item.EyeOpen);

					if (ann != null)
					{
						Thread thread = new Thread(() => OpenEye(ann));
						thread.Start();
					}
				}

				if (!(ListEventWidgets == null || ListEventWidgets.Count == 0))
				{
					EventWidget ev = ListEventWidgets.Find(item => (item.View.Frame.X + item.View.Frame.Width) > scrollView.ContentOffset.X &&
						(item.View.Frame.X + item.View.Frame.Width) < (scrollView.ContentOffset.X + AppDelegate.ScreenWidth) &&
						!item.EyeOpen);

					if (ev != null)
					{
						Thread thread = new Thread(() => OpenEye(ev));
						thread.Start();
					}
				}

			};

			zoomingScrollView = new UIScrollView (new CGRect (0, 0, ScrollViewWidthSize, AppDelegate.ScreenHeight));
			zoomingScrollView.AddSubview (scrollView);

			View.AddSubview (zoomingScrollView);
		}

		private void OpenEye(PictureWidget picWidget)
		{
			Thread.Sleep (750);
			InvokeOnMainThread(picWidget.OpenEye);
			InvokeOnMainThread(ButtonInterface.navigationButton.RefreshNavigationButtonText);
		}

		private void OpenEye(VideoWidget vidWidget)
		{
			Thread.Sleep (750);
			InvokeOnMainThread(vidWidget.OpenEye);
			InvokeOnMainThread(ButtonInterface.navigationButton.RefreshNavigationButtonText);
		}

		private void OpenEye(AnnouncementWidget annWidget)
		{
			Thread.Sleep (750);
			InvokeOnMainThread(annWidget.OpenEye);
			InvokeOnMainThread(ButtonInterface.navigationButton.RefreshNavigationButtonText);
		}

		private void OpenEye(EventWidget evWidget)
		{
			Thread.Sleep (750);
			InvokeOnMainThread(evWidget.OpenEye);
			InvokeOnMainThread(ButtonInterface.navigationButton.RefreshNavigationButtonText);
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
			ButtonInterface.Initialize (RefreshContent, board.MainColor);

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
			foreach (UIView iv in scrollView) { 
				if (iv.Tag != (int)Tags.Background) {
					iv.RemoveFromSuperview ();
					iv.Dispose ();
				}
			}
		}

		public void RefreshContent()
		{
			RemoveAllContent ();

			GenerateTestPictures ();

			ListPictureWidgets = new List<PictureWidget> ();
			ListVideoWidgets = new List<VideoWidget> ();
			ListAnnouncementWidgets = new List<AnnouncementWidget> ();
			ListEventWidgets = new List<EventWidget> ();

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

			ListPictureWidgets = ListPictureWidgets.OrderBy(o=>o.View.Frame.X).ToList();
			ListVideoWidgets = ListVideoWidgets.OrderBy(o=>o.View.Frame.X).ToList();
			ListAnnouncementWidgets = ListAnnouncementWidgets.OrderBy(o=>o.View.Frame.X).ToList();
			ListEventWidgets = ListEventWidgets.OrderBy(o=>o.View.Frame.X).ToList();
		}

		private void GenerateTestPictures()
		{
			AddTestPicture (UIImage.FromFile ("./demo/pictures/0.jpg"), 40, 20, -.03f);
			AddTestVideo ("./demo/videos/0.mp4", 15, 245, -.01f);

			AddTestPicture (UIImage.FromFile ("./demo/pictures/2.jpg"), 310, 20, 0f);
			AddTestPicture (UIImage.FromFile ("./demo/pictures/1.jpg"), 330, 245, -.04f);

			AddTestVideo ("./demo/videos/1.mp4", 580, 25, -.02f);
			AddTestPicture (UIImage.FromFile ("./demo/pictures/3.jpg"), 610, 250, .05f);


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
			VideoWidget component = new VideoWidget (video);

			UIView componentView = component.View;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (Preview.View != null) { return; }

				MPMoviePlayerController moviePlayer = new MPMoviePlayerController (NSUrl.FromFilename (video.Url));

				View.AddSubview (moviePlayer.View);
				moviePlayer.SetFullscreen (true, false);
				moviePlayer.Play ();
			});

			componentView.AddGestureRecognizer (tap);
			componentView.UserInteractionEnabled = true;

			scrollView.AddSubview (component.View);
			ListVideoWidgets.Add (component);
		}

		private void DrawPictureWidget(Picture picture)
		{
			PictureWidget component = new PictureWidget (picture);

			UIView componentView = component.View;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (Preview.View != null) { return; }

				LookUp lookUp = new LookUp(picture);
				NavigationController.PushViewController(lookUp, true);
			});

			componentView.AddGestureRecognizer (tap);
			componentView.UserInteractionEnabled = true;

			scrollView.AddSubview (component.View);
			ListPictureWidgets.Add (component);
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
			ListEventWidgets.Add (component);
		}


		private void DrawAnnouncementWidget(Announcement ann)
		{
			AnnouncementWidget announcementWidget = new AnnouncementWidget (ann);

			scrollView.AddSubview (announcementWidget.View);

			ListAnnouncementWidgets.Add (announcementWidget);
		}

		private void LoadBackground()
		{	
			UIImage logo = board.Image;
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

			zoomingScrollView.ViewForZoomingInScrollView += sv => {
				return zoomingScrollView.Subviews [0];
			};

			zoomingScrollView.RemoveGestureRecognizer (zoomingScrollView.PinchGestureRecognizer);
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