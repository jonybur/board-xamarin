using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Clubby.Schema;
using CoreGraphics;
using DACircularProgress;
using UIKit;

namespace Clubby.Screens.Controls
{	
	public class UITimelineContentDisplay : UIContentDisplay {

		const float SeparationBetweenObjects = 30;
		public static Dictionary<string, UITimelineWidget> TimelineWidgets;
		CircularProgressView progressView;

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

		public void UpdateTimeline(List<Venue> boardList, List<Content> updatedTimelineContent){
			
			//var result = updatedTimelineContent.Where(p => TimelineWidgets.ContainsKey(p.Id)).ToList();
			//FillTimeline (boardList, result);

			// refreshes all timeline

			foreach (var widget in TimelineWidgets) {
				widget.Value.RemoveFromSuperview ();
			}
			ListViews = new List<UIView> ();
			TimelineWidgets = new Dictionary<string, UITimelineWidget> ();

			var mainMenuScreen = AppDelegate.NavigationController.TopViewController as MainMenuScreen;
			mainMenuScreen.ScrollsUp (false);

			ForceSelectiveRendering (new CGPoint (0, 0));

			FillTimeline (boardList, updatedTimelineContent);

			DismissProgressView ();

			ForceSelectiveRendering (new CGPoint (0, 0));

		}

		public UITimelineContentDisplay(List<Venue> boardList, List<Content> timelineContent) {

			var sw = new Stopwatch();
			sw.Start();

			TimelineWidgets = new Dictionary<string, UITimelineWidget> ();
			FillTimeline (boardList, timelineContent);

			sw.Stop();
			Console.WriteLine("B) Pantalla 1 (Timeline): {0}",sw.Elapsed);
		}

		private void FillTimeline(List<Venue> boardList, List<Content> timelineContent){
			float yposition = UIMenuBanner.Height + 30;

			Console.Write ("Filling timeline... ");

			int timelineToLoad = timelineContent.Count < 50 ? timelineContent.Count : 50;

			//foreach (var content in timelineContent){
			for (int i = 0; i < timelineToLoad; i++) {
				var content = timelineContent [i];

				if (!(content is Picture)) {
					continue;
				}

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

