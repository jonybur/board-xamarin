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

		static class TimelineContent{
			public static List<Content> ContentList;
			public static DateTime UpdatedTime;

			public static void Update(){
				// get from all venues
				ContentList = MainMenuScreen.FetchedVenues.GetTimeline();
				UpdatedTime = DateTime.Now;
			}
		}

		// generates the magazine headers
		public static void GeneratePages(List<Venue> boardList){

			Pages = new List<UIContentDisplay> ();

			if (TimelineContent.ContentList == null || TimelineContent.UpdatedTime.TimeOfDay.TotalMinutes + 10 < DateTime.Now.TimeOfDay.TotalMinutes){
				TimelineContent.Update ();
			}

			bool theresTimeline = TimelineContent.ContentList.Count > 0;

			if (theresTimeline) {
				Pages.Add (new UITimelineContentDisplay (boardList, TimelineContent.ContentList));
			}

			bool theresMagazine = boardList.Count (x => x.FriendLikes > 0) > 0;

			if (theresMagazine) {
				Pages.Add (new UICarouselContentDisplay (boardList));
			}

			Pages.Add (new UIThumbsContentDisplay (boardList, UIThumbsContentDisplay.OrderMode.Distance, 0, UIActionButton.Height));
		}
	}

}

