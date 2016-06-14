using System.Collections.Generic;
using System.Threading.Tasks;
using BigTed;
using Board.Infrastructure;
using Board.Interface;
using Board.JsonResponses;
using System;
using Board.Schema;
using System.Threading;
using Board.Utilities;
using CoreGraphics;
using UIKit;

namespace Board.Facebook
{
	public static class FacebookAutoImporter
	{
		static List<Content> ContentToImport;
		static CGPoint ItemLocation;
		static string PageId;

		public static void ImportPages(params string[] pageIds){
			foreach (var id in pageIds) {
				ImportPage(id);
			}
		}

		public static void ImportPage (string pageId)
		{
			BTProgressHUD.Show ("Importing Board...");
			FacebookUtils.MakeGraphRequest (pageId, "?fields=name,location,about,cover,phone,category_list,picture.type(large)", GenerateBoard);
		}

		private static async void GenerateBoard (List<FacebookElement> FacebookElements) { 
			if (FacebookElements.Count < 1) {
				BTProgressHUD.Dismiss ();
				return;
			}

			BTProgressHUD.Show ("Importing Board...");

			var importedBoard = (FacebookImportedPage)FacebookElements [0];
			var board = new Board.Schema.Board ();
			board.Name = importedBoard.Name;
			board.About = importedBoard.About;

			Console.WriteLine ("Recieved " + board.Name);

			board.Logo = await CommonUtils.DownloadUIImageFromURL (importedBoard.PictureUrl);
			board.CoverImage = await CommonUtils.DownloadUIImageFromURL (importedBoard.CoverUrl);
			board.GeolocatorObject = new GoogleGeolocatorObject ();
			board.MainColor = UIColor.Black;
			board.SecondaryColor = UIColor.Black;
			board.Phone = importedBoard.Phone;
			board.Category = importedBoard.Category;
			board.FacebookId = importedBoard.Id;

			board.GeolocatorObject.results = new List<Result> ();

			var result = new Result ();
			result.geometry = new Geometry ();
			result.geometry.location = new Location ();
			result.geometry.location.lat = importedBoard.Location.Latitude;
			result.geometry.location.lng = importedBoard.Location.Longitude;
			board.GeolocatorObject.results.Add (result);

			// - creates board -

			Console.WriteLine ("Sending " + board.Name + "...");

			CloudController.CreateBoard (board);

			BTProgressHUD.Dismiss ();
		}

		const float startX = 500;
		const float startTopY = 175;
		const float startLowerY = 475;
		const float xAggregate = 250;

		public static void ImportPageContent(string pageId){
			PageId = pageId;
			ItemLocation = new CGPoint(startX, startTopY);
			ContentToImport = new List<Content> ();
			BTProgressHUD.Show("Importing Latest Posts...");
			FacebookUtils.MakeGraphRequest(PageId, "?fields=posts.limit(9)", GetAnnouncements);
		}


		// GET POSTS -> IF IT HAS A PICTURE THEN ITS A PICTURE
		// 			 	IF IT DOESNT HAS A PICTURE THEN ITS AN ANNOUNCEMENT

		// THEN EVENTS, VIDEOS


		static void GetAnnouncements(List<FacebookElement> FacebookElements){
			// parses all posts
			int announcementsToLoad = 0;


			Console.WriteLine ("Got " + FacebookElements.Count + " posts");

			var facebookElementsNoStories = FacebookElements.FindAll (obj => ((FacebookPost)obj).Message != null && ((FacebookPost)obj).Message != null);

			// checks out the posts....
			// posts can be pictures or announcements
			foreach (FacebookPost fbPost in facebookElementsNoStories) {
				
				// change location of new widget

				// goes check out if it has an image
				FacebookUtils.MakeGraphRequest (fbPost.Id, "?fields=full_picture", async delegate(List<FacebookElement> elementList) {
					
					if (elementList.Count > 0) {

						// WE HAVE A PICTURE

						// if it has an image, it adds it to the announcement (it searches in the announcement list for it)
						var cover = elementList [0] as FacebookFullPicture;

						if (!string.IsNullOrEmpty(cover.FullPicture)) {


							Console.WriteLine ("Downloading image from Facebook...");
							var image = await CommonUtils.DownloadUIImageFromURL(cover.FullPicture);

							Console.WriteLine ("Uploading image to AWS...");
							var amazonUrl = CloudController.UploadToAmazon(image);

							var picture = new Picture (fbPost, amazonUrl, ItemLocation, CGAffineTransform.MakeIdentity());
							ContentToImport.Add(picture);

							Console.WriteLine ("Added a picture to import");

						}else{
							
							var announcement = new Announcement (fbPost, ItemLocation, CGAffineTransform.MakeIdentity());
							ContentToImport.Add(announcement);

							Console.WriteLine ("Added an announcement to import");
						}

					} else {

						var announcement = new Announcement (fbPost, ItemLocation, CGAffineTransform.MakeIdentity());
						ContentToImport.Add(announcement);

						Console.WriteLine ("Added an announcement to import");
					}

					// image or not, this announcement has been checked
					announcementsToLoad ++;
					ItemLocation.X += xAggregate;

					// if i checked all the announcements
					Console.WriteLine("Added " + announcementsToLoad + " of " + FacebookElements.Count);
					if (announcementsToLoad == facebookElementsNoStories.Count){
						
						// goes to import events
						Console.WriteLine ("Done importing posts");

						BTProgressHUD.Show("Importing Events...");
						FacebookUtils.MakeGraphRequest(PageId, "?fields=events.limit(3)", GetEvents);
					}
				});

			}

			// no announcements? goes seek events
			if (facebookElementsNoStories.Count == 0) {
				BTProgressHUD.Show("Importing Events...");
				FacebookUtils.MakeGraphRequest(PageId, "?fields=events.limit(3)", GetEvents);
			} 
		}

		static void GetEvents(List<FacebookElement> FacebookElements){
			// parses all events
			int coversToLoad = 0;
			var boardEvents = new List<BoardEvent> ();
			ItemLocation = new CGPoint (startX, startLowerY);

			foreach (FacebookEvent fbEvent in FacebookElements) {

				var boardEvent = new BoardEvent (fbEvent, ItemLocation, CGAffineTransform.MakeIdentity());
			
				if (DateTime.Compare (boardEvent.StartDate, DateTime.Now) < 0) {
					coversToLoad++;
					continue;
				}

				boardEvents.Add (boardEvent);

				FacebookUtils.MakeGraphRequest (fbEvent.Id, "?fields=cover,updated_time", async delegate(List<FacebookElement> elementList) {
					if (elementList.Count > 0) {
						var cover = elementList [0] as FacebookCoverUpdatedTime;
						if (cover != null) {
							Console.Write ("Uploading event image...");

							var image = await CommonUtils.DownloadUIImageFromURL(cover.Source);
							var amazonUrl = CloudController.UploadToAmazon(image);
							boardEvents[coversToLoad].ImageUrl = amazonUrl;

							Console.WriteLine(" done");
						}
						boardEvents[coversToLoad].CreationDate = DateTime.Parse(cover.UpdatedTime);
					}

					coversToLoad++;
					ItemLocation.X += xAggregate;

					if (coversToLoad == FacebookElements.Count){
						ContentToImport.AddRange(boardEvents);

						Console.WriteLine("Done importing events");
						// gets albums
						BTProgressHUD.Show("Importing Videos...");
						FacebookUtils.MakeGraphRequest (PageId, "videos?fields=source,description,updated_time,thumbnails&limit=3", GetVideos);
					}
				});
			}

			if (FacebookElements.Count == 0 || coversToLoad == FacebookElements.Count) {
				// gets albums
				BTProgressHUD.Show("Importing Videos...");
				FacebookUtils.MakeGraphRequest (PageId, "videos?fields=source,description,updated_time,thumbnails&limit=3", GetVideos);
			}

		}

		static async void GetVideos(List<FacebookElement> FacebookElements){
			// parses all videos
			int i = 1;
			foreach (FacebookVideo fbVideo in FacebookElements) {

				var video = new Video (fbVideo, ItemLocation, CGAffineTransform.MakeIdentity());

				BTProgressHUD.Show("Importing Videos... " + i + "/" + FacebookElements.Count);
				Console.WriteLine("Downloading video");
				var byteArray = await CommonUtils.DownloadByteArrayFromURL (fbVideo.Source);
				Console.WriteLine("Uploading video");
				video.AmazonUrl = CloudController.UploadToAmazon (byteArray);
				Console.WriteLine("Added video to import");
				ContentToImport.Add (video);
				ItemLocation.X += xAggregate;
				i++;
			}

			UploadContent (); 
		}

		static void UploadContent(){
			var json = JsonUtilty.GenerateUpdateJson (ContentToImport);

			Console.Write("Uploading all content...");
			CloudController.UpdateBoard (UIBoardInterface.board.Id, json);
			Console.WriteLine(" done");
			BTProgressHUD.Dismiss();
		}
	}
}

