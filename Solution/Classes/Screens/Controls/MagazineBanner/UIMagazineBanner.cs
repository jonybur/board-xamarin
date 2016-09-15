using System.Collections.Generic;
using System.Linq;
using Board.Infrastructure;
using Board.Schema;
using Board.Facebook;
using Board.JsonResponses;
using System;
using Newtonsoft.Json.Linq;
using CoreGraphics;
using MGImageUtilitiesBinding;
using UIKit;

namespace Board.Screens.Controls
{
	public class UIMagazine{
		// contains (page banner + content display)
		public static List<UIContentDisplay> Pages;

		public UIMagazine(List<Board.Schema.Board> boardList){
			GeneratePages (boardList);
			//Banner = new UIMagazineBanner ();
		}

		public static Dictionary<string, int> ContentLikes;
		public static Dictionary<string, bool> UserLikes;

		static MagazineResponse magazine;

		public static class TimelineContent{
			public static List<Content> ContentList;
			public static DateTime UpdatedTime;

			private static List<Content> GetStoredTimeline(){

				string timeline = StorageController.GetInstagramTimeline ();

				return TimelineToContentList (timeline);
			}

			private static async System.Threading.Tasks.Task<List<Content>> GetServerTimeline(){

				string timeline = await CloudController.GetInstagramTimeline();

				StorageController.StoreInstagramTimeline (timeline);

				return TimelineToContentList (timeline);
			}

			private static List<Content> TimelineToContentList(string instagramTimeline){

				if (string.IsNullOrEmpty (instagramTimeline)) {
					return new List<Content> ();
				}

				var instagramObject = JObject.Parse (instagramTimeline);

				var timelineContent = new List<Content> ();

				foreach (var publication in instagramObject["rows"].Select(x=>x["value"])) {

					var item = publication.ToObject<InstagramPageResponse.Item> ();

					var content = InstagramPageResponse.GenerateContent (item);
					if (content != null) {
						timelineContent.Add (content);
					}

				}

				return timelineContent;
			}

			public static async System.Threading.Tasks.Task Initialize(){
				// get from all venues
				ContentList = GetStoredTimeline();

				if (ContentList.Count == 0) {
					ContentList = await GetServerTimeline ();
					UpdatedTime = DateTime.Now;
				} else {
					UpdatedTime = StorageController.GetTimelineLastWriteTime();
				}
			}

			public static async System.Threading.Tasks.Task Update(List<Board.Schema.Board> boardList){
				if ((DateTime.Now - UpdatedTime).TotalMinutes > 5) {
					var timelineContent = (UITimelineContentDisplay)UIMagazine.Pages [0];
					timelineContent.SetProgressView ();

					ContentList = await GetServerTimeline();
					UpdatedTime = DateTime.Now;

					// should update timeline
					timelineContent.UpdateTimeline(ContentList);
				}


			}
		}

		public static void AddLikeToContent(string contentId){
			UIMagazine.ContentLikes [contentId]++;
			UIMagazine.UserLikes [contentId] = true;
		}

		public static void RemoveLikeToContent(string contentId){
			UIMagazine.ContentLikes [contentId]--;
			UIMagazine.UserLikes [contentId] = false;
		}

		public static bool TheresMagazine, TheresTimeline;

		// generates the magazine headers
		public static async System.Threading.Tasks.Task GeneratePages(List<Board.Schema.Board> boardList){

			Pages = new List<UIContentDisplay> ();

			if (TimelineContent.ContentList == null || TimelineContent.UpdatedTime.TimeOfDay.TotalMinutes + 10 < DateTime.Now.TimeOfDay.TotalMinutes){
				await TimelineContent.Initialize ();
			}

			TheresTimeline = TimelineContent.ContentList.Count > 0;

			if (TheresTimeline) {
				Pages.Add (new UITimelineContentDisplay (boardList, TimelineContent.ContentList));
				TimelineContent.Update (boardList);
			}

			if (magazine == null || !TheresMagazine || magazine.UpdatedTime.TimeOfDay.TotalMinutes + 60 < DateTime.Now.TimeOfDay.TotalMinutes) {
				Console.WriteLine ("Gets magazine"); 
				magazine = CloudController.GetMagazine (AppDelegate.UserLocation);
			}

			TheresMagazine = MagazineResponse.IsValidMagazine (magazine);

			if (TheresMagazine) {
				Pages.Add (new UICarouselContentDisplay (magazine));
			}

			Pages.Add (new UIThumbsContentDisplay (boardList, UIThumbsContentDisplay.OrderMode.Distance, 0, UIActionButton.Height));

		}
	}


}

