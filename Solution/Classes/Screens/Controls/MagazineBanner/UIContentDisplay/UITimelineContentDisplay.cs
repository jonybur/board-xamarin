using System.Collections.Generic;
using System.Linq;
using Board.Schema;
using CoreGraphics;

namespace Board.Screens.Controls
{	
	public class UITimelineContentDisplay : UIContentDisplay {

		const float SeparationBetweenObjects = 30;
		public static Dictionary<string, UITimelineWidget> TimelineWidgets;

		public static void UpdateWidgetLikeCount(string contentId, int likeCount){
			if (TimelineWidgets != null) {
				if (TimelineWidgets.ContainsKey (contentId)) {
					TimelineWidgets [contentId].AddLikeCount (likeCount);
				}
			}
		}

		public UITimelineContentDisplay(List<Board.Schema.Board> boardList, List<Content> timelineContent) {
			
			float yposition = UIMagazineBannerPage.Height + UIMenuBanner.Height + 30;
			TimelineWidgets = new Dictionary<string, UITimelineWidget> ();

			foreach (var content in timelineContent){

				if (!(/*content is BoardEvent*/ content is Announcement || content is Picture)) {
					continue;
				}

				var board = boardList.FirstOrDefault (x => x.Id == content.boardId);

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
		}
	}
}

