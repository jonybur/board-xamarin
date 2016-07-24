using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Clubby.Infrastructure;
using Clubby.JsonResponses;
using Clubby.Schema;
using Newtonsoft.Json.Linq;

namespace Clubby.Screens.Controls
{
	public static class UIMagazine {
		
		// contains (content display)
		public static List<UIContentDisplay> Pages;
		public static bool TheresMagazine, TheresTimeline;

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
				var sw = new Stopwatch();
				sw.Start();

				// get from all venues
				ContentList = GetStoredTimeline();

				if (ContentList.Count == 0) {
					ContentList = await GetServerTimeline ();
					UpdatedTime = DateTime.Now;
				} else {
					UpdatedTime = StorageController.GetTimelineLastWriteTime();
				}

				sw.Stop();
				Console.WriteLine("A) Levantar el content: {0}",sw.Elapsed);

			}

			public static async System.Threading.Tasks.Task Update(List<Venue> boardList){
				if ((DateTime.Now - UpdatedTime).TotalMinutes > 5) {
					var timelineContent = (UITimelineContentDisplay)Pages [0];
					timelineContent.SetProgressView ();

					ContentList = await GetServerTimeline();
					UpdatedTime = DateTime.Now;

					// should update timeline
					timelineContent.UpdateTimeline(ContentList);
				}


			}
		}

		// generates the magazine headers
		public static async System.Threading.Tasks.Task GeneratePages(List<Venue> boardList){
			var sw = new Stopwatch();
			sw.Start();

			// --

			Pages = new List<UIContentDisplay> ();

			if (TimelineContent.ContentList == null || TimelineContent.UpdatedTime.TimeOfDay.TotalMinutes + 10 < DateTime.Now.TimeOfDay.TotalMinutes){
				await TimelineContent.Initialize ();
			}

			TheresTimeline = TimelineContent.ContentList.Count > 0;

			if (TheresTimeline) {
				Pages.Add (new UITimelineContentDisplay (boardList, TimelineContent.ContentList));
				TimelineContent.Update (boardList);
			}

			TheresMagazine = boardList.Count (x => x.FriendLikes > 0) > 0;

			if (TheresMagazine) {
				Pages.Add (new UICarouselContentDisplay (boardList));
			}

			Pages.Add (new UIThumbsContentDisplay (boardList, UIThumbsContentDisplay.OrderMode.Distance, 0, UIActionButton.Height));

			// --

			sw.Stop();
			Console.WriteLine("A+B+C+D+otros: {0}",sw.Elapsed);

		}
	}

}

