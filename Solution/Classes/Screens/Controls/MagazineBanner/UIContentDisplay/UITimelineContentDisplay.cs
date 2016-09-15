using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Board.Schema;
using CoreGraphics;
using DACircularProgress;
using UIKit;

namespace Board.Screens.Controls
{	
	public class UITimelineContentDisplay : UIContentDisplay {

		const float SeparationBetweenObjects = 30;
		public static Dictionary<string, UITimelineWidget> TimelineWidgets;
		CircularProgressView progressView;
		List<Board.Schema.Board> boardList; List<Content> timelineContent; public static List<string> VideosToMute;
		float yposition;

		public UITimelineContentDisplay(List<Board.Schema.Board> _boardList, List<Content> _timelineContent) {

			var sw = new Stopwatch();
			sw.Start();

			VideosToMute = new List<string> ();
			TimelineWidgets = new Dictionary<string, UITimelineWidget> ();

			boardList = _boardList;
			timelineContent = _timelineContent;

			FillTimeline ();

			sw.Stop();
			Console.WriteLine("B) Pantalla 1 (Timeline): {0}",sw.Elapsed);
		}

		public void MuteVideos(float scrollOffsetY){

			var widgets = TimelineWidgets.Where (x => VideosToMute.Contains (x.Key));
			var toMute = widgets.Where (x => x.Value.Frame.Bottom < scrollOffsetY || 
				x.Value.Frame.Top > scrollOffsetY + AppDelegate.ScreenHeight);

			foreach (var m in toMute) {
				((UITimelineWidget)m.Value).timelineVideo.playerLayer.Player.Muted = true;
				VideosToMute.Remove (m.Key);
			}

		}

		public static void UpdateWidgetLikeCount(string contentId, int likeCount){
			if (TimelineWidgets != null) {
				if (TimelineWidgets.ContainsKey (contentId)) {
					TimelineWidgets [contentId].AddLikeCount (likeCount);
				}
			}
		}

		public void SetProgressView(){
			progressView = new CircularProgressView ();
			progressView.Progress = 0.35f;
			progressView.IndeterminateDuration = 1.0f;
			progressView.Indeterminate = true;
			progressView.Frame = new CGRect (0, 0, 40, 40);
			progressView.Center = new CGPoint (AppDelegate.ScreenWidth / 2, UIMenuBanner.Height + 40);

			foreach (var widget in TimelineWidgets) {
				widget.Value.MoveDown ();
			}

			AddSubview (progressView);
		}

		public void DismissProgressView(){
			progressView.RemoveFromSuperview ();
		}

		public void UpdateTimeline(List<Content> updatedTimelineContent){
			timelineContent = updatedTimelineContent;

			// refreshes all timeline
			foreach (var widget in TimelineWidgets) {
				widget.Value.RemoveFromSuperview ();
			}

			ListViews = new List<UIView> ();
			TimelineWidgets = new Dictionary<string, UITimelineWidget> ();

			var mainMenuScreen = AppDelegate.NavigationController.TopViewController as MainMenuScreen;
			mainMenuScreen.ScrollsUp (false);

			ForceSelectiveRendering (new CGPoint (0, 0));

			FillTimeline ();

			DismissProgressView ();

			ForceSelectiveRendering (new CGPoint (0, 0));
		}

		public void FillMoreTimeline(){

			// cuantos widgets hay?
			int widgetCount = TimelineWidgets.Count;

			if (timelineContent.Count <= widgetCount) {
				return;
			}

			// timelinecontent is api's timeline
			if (TimelineWidgets.ContainsKey(timelineContent[widgetCount].Id)){
				widgetCount++;
			}

			// bueno, de i = widget count a 15, agarrar los proximos 15 widgets
			for (int i = widgetCount; i < widgetCount + 15; i++) {

				var content = timelineContent [i];

				var board = boardList.FirstOrDefault (x => x.InstagramId == content.InstagramId);

				if (board == null) {
					continue;
				}

				var timelineWidget = new UITimelineWidget (board, content);
				timelineWidget.Center = new CGPoint (AppDelegate.ScreenWidth / 2, yposition + timelineWidget.Frame.Height / 2);

				ListViews.Add (timelineWidget);
				TimelineWidgets.Add (content.Id, timelineWidget);

				yposition += (float)timelineWidget.Frame.Height + SeparationBetweenObjects;
			}

			var size = new CGSize (AppDelegate.ScreenWidth, yposition + UIActionButton.Height * 2);
			Frame = new CGRect (0, 0, size.Width, size.Height);

		}

		private void FillTimeline(){
			yposition = UIMenuBanner.Height + 30;

			Console.Write ("Filling timeline... ");

			int timelineToLoad = timelineContent.Count < 10 ? timelineContent.Count : 10;

			for (int i = 0; i < timelineToLoad; i++) {
				var content = timelineContent [i];

				var board = boardList.FirstOrDefault (x => x.InstagramId == content.InstagramId);

				if (board == null) {
					continue;
				}

				var timelineWidget = new UITimelineWidget (board, content);
				timelineWidget.Center = new CGPoint (AppDelegate.ScreenWidth / 2, yposition + timelineWidget.Frame.Height / 2);

				ListViews.Add (timelineWidget);
				TimelineWidgets.Add (content.Id, timelineWidget);

				yposition += (float)timelineWidget.Frame.Height + SeparationBetweenObjects;
			}

			var size = new CGSize (AppDelegate.ScreenWidth, yposition + UIActionButton.Height * 2);
			Frame = new CGRect (0, 0, size.Width, size.Height);
			UserInteractionEnabled = true;

			Console.WriteLine ("done");

		}
	}
}
