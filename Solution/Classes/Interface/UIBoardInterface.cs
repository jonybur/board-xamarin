using System;
using System.Collections.Generic;
using BigTed;
using Board.Infrastructure;
using Board.Interface;
using Board.Interface.Buttons;
using Facebook.CoreKit;
using Board.Interface.Widgets;
using Board.Schema;
using Board.Utilities;
using System.Threading;
using CoreGraphics;
using UIKit;
using System.Linq;

namespace Board.Interface
{
	// user interface - connects to the board controller
	// also called BoardView
	public class UIBoardInterface : UIViewController
	{
		public const int BannerHeight = 66;

		public static bool UserCanEditBoard;
		public static Board.Schema.Board board;

		public UIBoardScroll BoardScroll;

		// includes board likes
		public static Dictionary<string, bool> DictionaryUserLikes;
		public static Dictionary<string, int> DictionaryLikes;

		public static Dictionary<string, Content> DictionaryContent;
		public static Dictionary<string, Widget> DictionaryWidgets;
		public static Dictionary<string, UISticker> DictionaryStickers;
		public static CancellationTokenSource DownloadCancellation;

		bool firstLoad;

		public UIBoardInterface (Board.Schema.Board _board){
			board = _board;
			DownloadCancellation = new CancellationTokenSource();
			firstLoad = true;
		}

		public override void DidReceiveMemoryWarning  ()
		{
			GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced);
		}

		public override void ViewDidLoad ()
		{
			AppEvents.LogEvent ("entersBoard");

			//var json = JsonUtilty.GenerateDeleteAllJson ();
			//CloudController.UpdateBoard (board.Id, json);

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

			DictionaryStickers = new Dictionary<string, UISticker> ();
		}

		private void InitializeInterface()
		{
			// This was main color
			View.BackgroundColor = board.MainColor;

			BoardScroll = new UIBoardScroll ();
			BoardScroll.Frame = new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);

			// generate the scrollview and the zoomingscrollview
			View.AddSubview (BoardScroll);

			var statusBarView = new UIView (new CGRect (0, 0, AppDelegate.ScreenWidth, 20));
			statusBarView.Alpha = .6f;
			statusBarView.BackgroundColor = board.MainColor;

			View.AddSubview (statusBarView);

			// load buttons
			LoadButtons ();
		}

		public override void ViewDidDisappear(bool animated) {
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
				if (widget.Value is PictureWidget) {
					((PictureWidget)widget.Value).CancelSetImage ();
				} else if (widget.Value is AnnouncementWidget) {
					((AnnouncementWidget)widget.Value).CancelSetImage ();
				}

				widget.Value.UnsuscribeFromEditingEvents ();
				widget.Value.UnsuscribeFromUsabilityEvents ();
				widget.Value.RemoveFromSuperview ();

				MemoryUtility.ReleaseUIViewWithChildren (widget.Value);
			}

			DictionaryWidgets = new Dictionary<string, Widget>();
		}

		private async void LoadButtons()
		{
			ButtonInterface.Initialize ();

			var buttonBackground = new UIImageView (new CGRect (0, AppDelegate.ScreenHeight - 45, AppDelegate.ScreenWidth, ButtonInterface.ButtonBarHeight));
			buttonBackground.BackgroundColor = UIColor.FromRGBA(255, 255, 255, 220);
			View.AddSubview (buttonBackground);

			try{
				UserCanEditBoard = await CloudController.UserCanEditBoardAsync (board.Id, DownloadCancellation.Token);

				if (UserCanEditBoard) {
					View.AddSubviews (ButtonInterface.GetCreatorButtons().ToArray());
				} else {
					View.AddSubviews (ButtonInterface.GetUserButtons ().ToArray());
				}

				ButtonInterface.SwitchButtonLayout (ButtonInterface.ButtonLayout.NavigationBar);
			} catch (OperationCanceledException) {
				Console.WriteLine ("Task got cancelled");
			}
		}


		public async void GenerateWidgets()
		{
			// looks for new keys in the DictionaryContent
			// draws new widget in case new content is found

			try{
				DictionaryContent = await CloudController.GetBoardContentAsync (DownloadCancellation.Token, board.Id);
				
				var listContentIds = DictionaryContent.Values.Select (x => x.Id).ToList ();

				listContentIds.Add (UIBoardInterface.board.Id);
				var contentIds = listContentIds.ToArray ();

				DictionaryLikes = await CloudController.GetLikesAsync(DownloadCancellation.Token, contentIds);

				DictionaryUserLikes = await CloudController.GetUserLikesAsync(DownloadCancellation.Token, contentIds);

				foreach (KeyValuePair<string, Content> c in DictionaryContent) {
					if (!DictionaryWidgets.ContainsKey (c.Key)) {
						AddWidgetToDictionaryFromContent (c.Value);
					}
				}

				var newContentCount = DictionaryWidgets.Values.ToList ().Count (widget => !widget.EyeOpen);
				ButtonInterface.navigationButton.RefreshNavigationButtonText (newContentCount);

				BoardScroll.RecalculateBoardSize();
			}catch (OperationCanceledException){
				Console.WriteLine ("Task got cancelled");
			}
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