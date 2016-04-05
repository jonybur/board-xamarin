using System;
using System.Collections.Generic;
using System.Linq;
using BigTed;
using Foundation;
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

		public static Board.Schema.Board board;

		bool galleryActive;

		public UIBoardScroll BoardScroll;

		public static Dictionary<string, Content> DictionaryContent;
		public static Dictionary<string, Widget> DictionaryWidgets;

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
			AutomaticallyAdjustsScrollViewInsets = false;

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

		private void InitializeInterface()
		{
			View.BackgroundColor = board.MainColor;

			BoardScroll = new UIBoardScroll ();

			// generate the scrollview and the zoomingscrollview
			View.AddSubview (BoardScroll);

			// load buttons
			LoadButtons ();
		}

		public override void ViewDidDisappear(bool animated)
		{
			RemoveAllContent ();
		}

		public override void ViewDidAppear(bool animated)
		{
			if (firstLoad) {

				GenerateTestPictures ();

				GenerateWidgets ();

				firstLoad = false;

				BTProgressHUD.Dismiss ();
			}

			BoardScroll.SelectiveRendering ();

			if (Preview.IsAlive) {
				
				// shows the image preview so that the user can position the image
				BoardScroll.ScrollView.AddSubview (Preview.View);
			}
		}

		public void ExitBoard()
		{
			View.BackgroundColor = UIColor.Black;
			RemoveAndDisposeAllContent ();
			MemoryUtility.ReleaseUIViewWithChildren (BoardScroll, true);
			MemoryUtility.ReleaseUIViewWithChildren (View, true);
			ButtonInterface.DisableAllLayouts();
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

			BoardScroll.Alpha = alpha;

			if (enabled) {
				ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.NavigationBar);	
			}
			else{
				ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.Disable);
			}
		}


		/*
		 * 
		 * 
		DateTime lastOffsetCapture;
		CGPoint lastOffset;
		bool isScrollingFast;
		float scrollSpeed;
		
		private void InfiniteScroll(){
			float rightBound = (float)(scrollView.ContentOffset.X + AppDelegate.ScreenWidth);

			if (rightBound >= ScrollViewWidthSize) {
				scrollView.ContentOffset = new CGPoint (1, 0);
				if (scrollSpeed > 0) {
					scrollView.SetContentOffset (new CGPoint (scrollSpeed, 0), true);
					scrollSpeed = 0;
				}
					
			} else if (scrollView.ContentOffset.X <= 0) {
				scrollView.SetContentOffset (new CGPoint ((ScrollViewWidthSize - AppDelegate.ScreenWidth) - 1, 0), false);
			}
		}

		private void GetSpeed(){
			if (!scrollView.Dragging) {
				return;
			}

			var currentOffset = scrollView.ContentOffset;
		
			TimeSpan timeDiff = DateTime.Now.Subtract (lastOffsetCapture);
			if (timeDiff.TotalMilliseconds > 1) {
				float distance = (float)(currentOffset.X - lastOffset.X);

				distance = Math.Abs (distance);

				if (Math.Abs (distance) > 1000) {
					return;
				}

				// pixel per second? 
				scrollSpeed = ((float)distance * 10);

				//Console.WriteLine ("DI: " + distance);
				//Console.WriteLine ("SS: " + scrollSpeed);

				if (scrollSpeed <= 5) {
					scrollSpeed = 0f;
				}

				lastOffset = currentOffset;
				lastOffsetCapture = DateTime.Now;
			}
		}
		*/
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


	}
}