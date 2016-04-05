using System;
using System.Collections.Generic;
using System.Linq;
using BigTed;
using Board.Interface;
using System.Threading;
using Board.Interface.Buttons;
using Board.Interface.Widgets;
using Board.Utilities;
using Board.Schema;
using MGImageUtilitiesBinding;
using CoreGraphics;
using Facebook.CoreKit;
using UIKit;

namespace Board.Interface
{
	// user interface - connects to the board controller
	// also called BoardView
	public partial class BoardInterface : UIViewController
	{
		private Gallery gallery;

		public const int BannerHeight = 66;
		public const int ButtonBarHeight = 45;

		private static UIImageView CenterLogo, TopBanner;
		public static Board.Schema.Board board;

		public static UIScrollView zoomingScrollView;
		public static UIScrollView scrollView;
		static float TempContentOffset; 
		bool galleryActive;

		public enum Tags : byte { Background = 1, Content };

		public static int ScrollViewWidthSize = 2600;

		public static Dictionary<string, Content> DictionaryContent;
		public static Dictionary<string, Widget> DictionaryWidgets;
		List<Widget> DrawnWidgets;

		EventHandler scrolledEvent;
		bool firstLoad;

		bool TestMode;

		public BoardInterface (Board.Schema.Board _board, bool _testMode){
			board = _board;
			TestMode = _testMode;
			firstLoad = true;
		}

		public override void DidReceiveMemoryWarning ()
		{
			GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced);
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

			DrawnWidgets = new List<Widget> ();
		}

		public override void ViewDidDisappear(bool animated)
		{
			RemoveAllContent ();
		}

		public override void ViewDidAppear(bool animated)
		{
			if (firstLoad) {

				//GenerateTestPictures ();

				GenerateWidgets ();

				SuscribeToEvents ();

				firstLoad = false;

				var infoBox = new InfoBox (board);
				scrollView.AddSubview (infoBox);

				BTProgressHUD.Dismiss ();
			}

			SelectiveRendering ();

			if (Preview.IsAlive) {
				scrollView.AddSubview (Preview.View);
			}
		}

		public void ExitBoard()
		{
			View.BackgroundColor = UIColor.Black;
			RemoveAndDisposeAllContent ();
			MemoryUtility.ReleaseUIViewWithChildren (this.View, true);
			ButtonInterface.DisableAllLayouts();
			UnsuscribeToEvents ();
		}

		public void RemoveAllContent()
		{
			foreach(KeyValuePair<string, Widget> widget in DictionaryWidgets)
			{
				if (widget.Value is VideoWidget) {
					Thread killVideoThread = new Thread (new ThreadStart((widget.Value as VideoWidget).KillVideo));
					killVideoThread.Start ();
				}

				widget.Value.View.RemoveFromSuperview ();
			}
		}

		public void RemoveAndDisposeAllContent()
		{
			foreach(KeyValuePair<string, Widget> widget in DictionaryWidgets)
			{
				if (widget.Value is VideoWidget) {
					Thread killVideoThread = new Thread (new ThreadStart((widget.Value as VideoWidget).KillVideo));
					killVideoThread.Start ();
				}

				widget.Value.UnsuscribeToEvents ();
				widget.Value.View.RemoveFromSuperview ();
				MemoryUtility.ReleaseUIViewWithChildren (widget.Value.View, true);
			}

			DictionaryWidgets = new Dictionary<string, Widget>();
		}

		// deprecated for now
		private void InitializeGallery() {
			UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();

			gallery = new Gallery ();
			galleryActive = false;
			View.AddSubview (gallery.GetScrollView ());

			/*orientationObserver = UIDevice.Notifications.ObserveOrientationDidChange ((s, e) => {
				
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
			});*/
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

				SelectiveRendering();
			};

			zoomingScrollView = new UIScrollView (new CGRect (0, 0, ScrollViewWidthSize, AppDelegate.ScreenHeight));

			LoadMainLogo ();

			zoomingScrollView.AddSubview (scrollView);

			View.AddSubview (zoomingScrollView);
		}

		public void SelectiveRendering(){
			if (!(DictionaryWidgets == null || DictionaryWidgets.Count == 0))
			{
				List<Widget> WidgetsToDraw = DictionaryWidgets.Values.ToList().FindAll(item => ((item.View.Frame.X) > (scrollView.ContentOffset.X - AppDelegate.ScreenWidth)) &&
					((item.View.Frame.X + item.View.Frame.Width) < (scrollView.ContentOffset.X + AppDelegate.ScreenWidth + AppDelegate.ScreenWidth)));

				// takes wids that are close
				DrawWidgets(WidgetsToDraw);

				// the ones at the left ;; the ones at the right ;; if it doesnt have an open eye
				// checks only on the wids that are drawn
				List<Widget> WidgetsToOpenEyes = WidgetsToDraw.FindAll(item => ((item.View.Frame.X) > scrollView.ContentOffset.X) &&
					((item.View.Frame.X + item.View.Frame.Width) < (scrollView.ContentOffset.X + AppDelegate.ScreenWidth)) &&
					!item.EyeOpen);

				if (WidgetsToOpenEyes != null && WidgetsToOpenEyes.Count > 0)
				{
					foreach (var wid in WidgetsToOpenEyes) {
						OpenEye(wid);
					}
				}
			}
		}

		private void OpenEye(Widget widget)
		{
			widget.OpenEye();
			ButtonInterface.navigationButton.SubtractNavigationButtonText();
		}

		public static void ZoomScrollview()
		{
			zoomingScrollView.AddSubview (TopBanner);
			zoomingScrollView.SetZoomScale(1f, true);
			scrollView.Frame = new CGRect(0, 0, AppDelegate.ScreenWidth, scrollView.Frame.Height);
			scrollView.SetContentOffset (new CGPoint (TempContentOffset, 0), false);
			zoomingScrollView.SendSubviewToBack (TopBanner);
		}

		public static void UnzoomScrollview()
		{
			TopBanner.RemoveFromSuperview ();

			// TODO: remove hardcode and programatically derive the correct zooming value (.15f is current)
			TempContentOffset = (float)scrollView.ContentOffset.X;
			scrollView.Frame = new CGRect(0, AppDelegate.ScreenHeight/2 - 70, ScrollViewWidthSize, scrollView.Frame.Height);
			zoomingScrollView.SetZoomScale(.15f, true);
		}

		private void LoadButtons()
		{
			ButtonInterface.Initialize ();

			UIImageView buttonBackground = new UIImageView (new CGRect (0, AppDelegate.ScreenHeight - 45, AppDelegate.ScreenWidth, ButtonBarHeight));
			buttonBackground.BackgroundColor = UIColor.FromRGBA (255, 255, 255, 240);

			View.AddSubview (buttonBackground);

			if (Profile.CurrentProfile.UserID == board.CreatorId) {
				View.AddSubviews (ButtonInterface.GetCreatorButtons().ToArray());
			} else {
				View.AddSubviews (ButtonInterface.GetUserButtons (board.FBPage != null).ToArray());
			}

			ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.NavigationBar);
		}

		public void DrawWidgets(List<Widget> WidgetsToDraw)
		{
			List<Widget> WidgetsToRemove = DrawnWidgets.FindAll (item => !WidgetsToDraw.Contains (item));

			foreach (var wid in WidgetsToRemove) {
				wid.View.RemoveFromSuperview ();
				DrawnWidgets.Remove (wid);
			}

			foreach (var wid in WidgetsToDraw) {
				// if the widget hasnt been drawn
				if (wid.View.Superview != scrollView) {
					// draw it!
					scrollView.AddSubview (wid.View);
					DrawnWidgets.Add (wid);
				}
			}

		}

		public void GenerateWidgets()
		{
			BTProgressHUD.Show ();

			// looks for new keys in the DictionaryContent
			// draws new widget in case new content is found

			foreach (KeyValuePair<string, Content> c in DictionaryContent) {
				if (!DictionaryWidgets.ContainsKey (c.Key)) {
					AddWidgetToDictionaryFromContent (c.Value);
				}
			}

			//DictionaryWidgets = DictionaryWidgets.OrderBy(o=>o.View.Frame.X).ToList();
			ButtonInterface.navigationButton.RefreshNavigationButtonText (DictionaryWidgets.Count);

			BTProgressHUD.Dismiss ();
		}

		public void AddWidgetToDictionaryFromContent(Content content)
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
			} else if (content is Poll) {
				widget = new PollWidget (content as Poll);
			} else if (content is Map) {
				widget = new MapWidget (content as Map);
			} else {
				widget = new Widget ();
			}

			widget.SuscribeToEvents ();
			DictionaryWidgets.Add (content.Id, widget);
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

		private void LoadBackground()
		{	
			scrollView.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);

			AutomaticallyAdjustsScrollViewInsets = false;

			// this limits the size of the scrollview
			scrollView.ContentSize = new CGSize(ScrollViewWidthSize, AppDelegate.ScreenHeight);
			// sets the scrollview on the middle of the view
			scrollView.SetContentOffset (new CGPoint(ScrollViewWidthSize/2 - AppDelegate.ScreenWidth/2, 0), false);

			zoomingScrollView.MaximumZoomScale = 1f;
			zoomingScrollView.MinimumZoomScale = .15f;

			zoomingScrollView.RemoveGestureRecognizer (zoomingScrollView.PinchGestureRecognizer);
		}

		private void LoadMainLogo()
		{
			TopBanner = new UIImageView (new CGRect(0, 0, AppDelegate.ScreenWidth, BannerHeight));
			TopBanner.BackgroundColor = UIColor.White;

			float autosize = (float)TopBanner.Frame.Height - 25;

			UIImage logo = board.ImageView.Image;
			UIImage logoImage = logo.ImageScaledToFitSize  (new CGSize (autosize, autosize));
			UIImageView mainLogo = new UIImageView(logoImage);
			mainLogo.Center = new CGPoint (TopBanner.Frame.Width / 2, TopBanner.Frame.Height / 2 + 10);

			TopBanner.Tag = (int)Tags.Background;

			TopBanner.AddSubview (mainLogo);

			zoomingScrollView.AddSubview (TopBanner);
		}
	}
}