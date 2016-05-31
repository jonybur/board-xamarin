using System;
using System.Collections.Generic;
using BigTed;
using Board.Infrastructure;
using Board.Interface;
using Board.Interface.Buttons;
using Board.Interface.Widgets;
using Board.Schema;
using Board.Utilities;
using CoreGraphics;
using UIKit;

namespace Board.Interface
{
	// user interface - connects to the board controller
	// also called BoardView
	public partial class UIBoardInterface : UIViewController
	{
		public const int BannerHeight = 66;

		public static bool UserCanEditBoard;
		public static Board.Schema.Board board;

		public UIBoardScroll BoardScroll;

		public static Dictionary<string, Content> DictionaryContent;
		public static Dictionary<string, Widget> DictionaryWidgets;
		public static Dictionary<string, UISticker> DictionaryStickers;

		bool firstLoad;

		public UIBoardInterface (Board.Schema.Board _board){
			board = _board;
			board.FBPage = new Board.Facebook.FacebookPage ("camelotwestpalm", null, null);
			firstLoad = true;
		}

		public override void DidReceiveMemoryWarning  ()
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

			UserCanEditBoard = false;//CloudController.UserCanEditBoard (board.Id);

			// gets content, puts it in dictionarycontent
			DictionaryContent = CloudController.GetBoardContent (board.Id);

			InitializeInterface ();

			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;
		}

		private void InitializeLists()
		{
			DictionaryContent = new Dictionary<string, Content> ();

			DictionaryWidgets = new Dictionary<string, Widget> ();

			DictionaryStickers = new Dictionary<string, UISticker> ();
		}

		private void InitializeInterface()
		{
			// This was main color
			View.BackgroundColor = UIColor.FromRGB(170, 183, 192);//board.MainColor;

			BoardScroll = new UIBoardScroll ();
			BoardScroll.Frame = new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);

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
			ButtonInterface.DisableAllLayouts();
			MemoryUtility.ReleaseUIViewWithChildren (BoardScroll);
			MemoryUtility.ReleaseUIViewWithChildren (View);
		}

		public void RemoveAllContent()
		{
			foreach(KeyValuePair<string, Widget> widget in DictionaryWidgets)
			{
				widget.Value.RemoveFromSuperview ();
			}
		}

		public void RemoveAndDisposeAllContent()
		{
			foreach(KeyValuePair<string, Widget> widget in DictionaryWidgets)
			{
				widget.Value.UnsuscribeFromEditingEvents ();
				widget.Value.UnsuscribeFromUsabilityEvents ();
				widget.Value.RemoveFromSuperview ();

				MemoryUtility.ReleaseUIViewWithChildren (widget.Value);
			}

			DictionaryWidgets = new Dictionary<string, Widget>();
		}

		private void LoadButtons()
		{
			ButtonInterface.Initialize ();

			var buttonBackground = new UIImageView (new CGRect (0, AppDelegate.ScreenHeight - 45, AppDelegate.ScreenWidth, ButtonInterface.ButtonBarHeight));
			buttonBackground.BackgroundColor = UIColor.FromRGBA(255, 255, 255, 220);
			View.AddSubview (buttonBackground);

			if (UserCanEditBoard) {
				View.AddSubviews (ButtonInterface.GetCreatorButtons().ToArray());
			} else {
				View.AddSubviews (ButtonInterface.GetUserButtons ().ToArray());
			}

			ButtonInterface.SwitchButtonLayout (ButtonInterface.ButtonLayout.NavigationBar);
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

		public void AddStickerToDictionaryFromContent(Sticker content){
			var sticker = new UISticker (content);
			DictionaryStickers.Add (content.Id, sticker);
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
				((EventWidget)widget).Initialize ();

			} else if (content is Announcement) {
				widget = new AnnouncementWidget (content as Announcement);
			} else if (content is Poll) {
				widget = new PollWidget (content as Poll);
			} else if (content is Map) {
				widget = new MapWidget (content as Map);
			} else {
				widget = new Widget ();
			}

			widget.SuscribeToUsabilityEvents ();
			DictionaryWidgets.Add (content.Id, widget);
		}


	}
}