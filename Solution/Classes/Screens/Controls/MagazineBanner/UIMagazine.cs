using System;
using System.Collections.Generic;
using System.Linq;
using Clubby.Schema;
using UIKit;

namespace Clubby.Screens.Controls
{
	public static class UIMagazine{
		
		// contains (content display)
		public static List<UIContentDisplay> Pages;

		public static class TimelineContent{
			public static List<Content> ContentList;
			public static DateTime UpdatedTime;

			public static void Update(){
				// get from all venues
				ContentList = MainMenuScreen.FetchedVenues.GetTimeline();
				UpdatedTime = DateTime.Now;
			}
		}

		public static bool TheresMagazine, TheresTimeline;

		// generates the magazine headers
		public static void GeneratePages(List<Venue> boardList){

			Pages = new List<UIContentDisplay> ();

			if (TimelineContent.ContentList == null || TimelineContent.UpdatedTime.TimeOfDay.TotalMinutes + 10 < DateTime.Now.TimeOfDay.TotalMinutes){
				TimelineContent.Update ();
			}

			TheresTimeline = TimelineContent.ContentList.Count > 0;

			if (TheresTimeline) {
				Pages.Add (new UITimelineContentDisplay (boardList, TimelineContent.ContentList));
			}

			TheresMagazine = boardList.Count (x => x.FriendLikes > 0) > 0;

			if (TheresMagazine) {
				Pages.Add (new UICarouselContentDisplay (boardList));
			}

			Pages.Add (new UIThumbsContentDisplay (boardList, UIThumbsContentDisplay.OrderMode.Distance, 0, UIActionButton.Height));
		}
	}

}

