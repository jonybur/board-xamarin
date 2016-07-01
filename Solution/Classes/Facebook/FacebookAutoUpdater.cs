using System;
using System.Collections.Generic;
using System.Linq;
using BigTed;
using Board.Infrastructure;
using Board.Schema;
using Board.Utilities;
using UIKit;

namespace Board.Facebook
{
	public static class FacebookAutoUpdater
	{
		static List<Content> ContentToImport;
		static List<Content> CurrentContent;
		static int MaxTimestamp;
		static int ReplacedContents;
		static bool isRunning;
		static Board.Schema.Board currentBoard;

		public static void UpdateAllBoards(List<Board.Schema.Board> listBoards){

			var mainThread = new System.Threading.Thread (new System.Threading.ThreadStart(delegate {
				foreach (var board in listBoards) {
					Console.WriteLine("Updates " + board.Name);

					isRunning = true;
					currentBoard = board;

					var dictionaryContent = CloudController.GetBoardContent (board.Id);

					var uiview = new UIView();
					uiview.InvokeOnMainThread(delegate {
						UpdatePageContent (dictionaryContent.Values.ToList());
					});

					Console.WriteLine("On hold...");
					while (isRunning){}
					Console.WriteLine("Finishes loop");
				}
			}));
			mainThread.Start ();


		}

		public static void UpdatePageContent(List<Content> boardContent){

			if (boardContent.Count == 0) {
				Console.WriteLine ("IsRunning false");
				isRunning = false;
				return;
			}

			ReplacedContents = 0;
			CurrentContent = boardContent.OrderBy(x=>x.CreationDateTimestamp).ToList ();
			MaxTimestamp = CurrentContent.Max (x => x.CreationDateTimestamp);

			ContentToImport = new List<Content> ();

			BTProgressHUD.Show(currentBoard.Name + "\nImporting Latest Posts...");
			FacebookUtils.MakeGraphRequest(currentBoard.FacebookId, "?fields=posts.limit("+ CurrentContent.Count +")", GetPosts);
		}

		// GET POSTS -> IF IT HAS A PICTURE THEN ITS A PICTURE
		// 			 	IF IT DOESNT HAS A PICTURE THEN ITS AN ANNOUNCEMENT

		// THEN EVENTS, VIDEOS

		static void GetPosts(List<FacebookElement> FacebookElements){
			// parses all posts
			int announcementsToLoad = 0;

			Console.WriteLine ("Got " + FacebookElements.Count + " posts");

			var facebookElementsNoStories = FacebookElements.FindAll (obj =>
				((FacebookPost)obj).Message != "<null>" &&
				((FacebookPost)obj).Message != null &&
				((FacebookPost)obj).Timestamp > MaxTimestamp);

			Console.WriteLine (facebookElementsNoStories.Count + " are not stories and are new");

			// checks out the posts....
			// posts can be pictures or announcements

			int localReplace = -1;

			foreach (FacebookPost fbPost in facebookElementsNoStories) {
				// change location of new widget

				// goes check out if it has an image
				FacebookUtils.MakeGraphRequest (fbPost.Id, "?fields=full_picture", async delegate(List<FacebookElement> elementList) {

					if (localReplace >= CurrentContent.Count){
						return;
					}

					localReplace++;

					Console.WriteLine("Replacing " + localReplace);

					var contentToReplace = CurrentContent[localReplace];

					if (elementList.Count > 0) {

						// WE HAVE A PICTURE

						// if it has an image, it adds it to the announcement (it searches in the announcement list for it)
						var cover = elementList [0] as FacebookFullPicture;

						if (!string.IsNullOrEmpty(cover.FullPicture)) {
							
							Console.WriteLine ("Downloading image from Facebook...");

							var image = await CommonUtils.DownloadUIImageFromURL(cover.FullPicture);

							Console.WriteLine ("Uploading image to AWS...");

							var amazonUrl = CloudController.UploadToAmazon(image);
							var picture = new Picture (fbPost, amazonUrl, contentToReplace.Center, contentToReplace.Transform, currentBoard);
							ContentToImport.Add(picture);

							Console.WriteLine ("Added a picture to import");

						}else{

							var announcement = new Announcement (fbPost, contentToReplace.Center, contentToReplace.Transform, currentBoard);
							ContentToImport.Add(announcement);

							Console.WriteLine ("Added an announcement to import");
						}

					} else {

						var announcement = new Announcement (fbPost, contentToReplace.Center, contentToReplace.Transform, currentBoard);
						ContentToImport.Add(announcement);

						Console.WriteLine ("Added an announcement to import");
					}

					// image or not, this announcement has been checked
					announcementsToLoad ++;

					// if i checked all the announcements
					Console.WriteLine("Added " + announcementsToLoad + " of " + facebookElementsNoStories.Count);

					if (announcementsToLoad == facebookElementsNoStories.Count){

						// goes to import events
						Console.WriteLine ("Done importing posts");

						BTProgressHUD.Show(currentBoard.Name + "\nImporting Videos...");

						ReplacedContents = localReplace;

						UploadContent (); 
					}
				});
			}

			// no announcements? goes seek events
			if (facebookElementsNoStories.Count == 0) {
				UploadContent (); 
			} 
		}

		/*
		static void GetEvents(List<FacebookElement> FacebookElements){
			// parses all events
			int coversToLoad = 0;
			var boardEvents = new List<BoardEvent> ();

			var facebookEvents = FacebookElements.FindAll (x => 
				((FacebookEvent)x).StartTimestamp > CommonUtils.GetUnixTimeStamp ());
			Console.WriteLine ("Got " +facebookEvents +" events");

			foreach (FacebookEvent fbEvent in facebookEvents) {

				if (localReplace >= CurrentContent.Count){
					return;
				}

				var contentToReplace = CurrentContent[ReplacedContents];
				var boardEvent = new BoardEvent (fbEvent, contentToReplace.Center, contentToReplace.Transform, currentBoard);

				coversToLoad++;
				boardEvents.Add (boardEvent);

				FacebookUtils.MakeGraphRequest (fbEvent.Id, "?fields=cover,updated_time", async delegate(List<FacebookElement> elementList) {
					if (elementList.Count > 0) {
						var cover = elementList [0] as FacebookCoverUpdatedTime;
						if (cover != null) {
							Console.Write ("Uploading event image...");

							var image = await CommonUtils.DownloadUIImageFromURL(cover.Source);
							var amazonUrl = CloudController.UploadToAmazon(image);

							Console.WriteLine(boardEvents.Count + " " + coversToLoad);

							boardEvents[coversToLoad].ImageUrl = amazonUrl;

							Console.WriteLine(" done");
						}
						boardEvents[coversToLoad].CreationDate = DateTime.Parse(cover.UpdatedTime);
					}

					coversToLoad++;

					Console.WriteLine("Events covers loaded " + coversLoaded + " of " + boardEvents.Count);

					if (coversLoaded == boardEvents.Count){
						ContentToImport.AddRange(boardEvents);

						Console.WriteLine("Done importing events");
						// gets albums
						BTProgressHUD.Show(currentBoard.Name + "\nImporting Videos...");
						FacebookUtils.MakeGraphRequest (currentBoard.FacebookId, "videos?fields=source,description,updated_time,thumbnails&limit=3", GetVideos);
					}
				});
			}

			if (FacebookElements.Count == 0 || coversToLoad == FacebookElements.Count) {
				// gets albums
				BTProgressHUD.Show(currentBoard.Name + "\nImporting Videos...");
				FacebookUtils.MakeGraphRequest (currentBoard.FacebookId, "videos?fields=source,description,updated_time,thumbnails&limit=3", GetVideos);
			}
		}

		static async void GetVideos(List<FacebookElement> FacebookElements){
			// parses all videos
			int i = 1;

			var facebookNewVideos = FacebookElements.FindAll (obj =>
				((FacebookVideo)obj).Timestamp > MaxTimestamp);

			foreach (FacebookVideo fbVideo in facebookNewVideos) {

				if (ReplacedContents >= CurrentContent.Count){
					continue;
				}

				var contentToReplace = CurrentContent[ReplacedContents];

				var video = new Video (fbVideo, contentToReplace.Center, contentToReplace.Transform, currentBoard);

				BTProgressHUD.Show(currentBoard.Name + "\nImporting Videos... " + i + "/" + FacebookElements.Count);
				Console.WriteLine("Downloading video");
				var byteArray = await CommonUtils.DownloadByteArrayFromURL (fbVideo.Source);
				Console.WriteLine("Uploading video");
				video.AmazonUrl = CloudController.UploadToAmazon (byteArray);
				Console.WriteLine("Added video to import");
				ContentToImport.Add (video);
				i++;
			}

			UploadContent (); 
		}*/

		static void UploadContent(){

			Console.Write ("Deleting " + ReplacedContents + " old content... ");
			for (int i = 0; i < (ReplacedContents + 1); i++) {
				
				string deleteJson = JsonUtilty.GenerateDeleteJson (CurrentContent[i]);
				Console.WriteLine ("Deleting " + CurrentContent [i].Id);
				CloudController.UpdateBoard (currentBoard.Id, deleteJson);

			}
			Console.WriteLine ("Done deleting content");

			//double check all images are not null
			if (ContentToImport.Count > 0) {
				var json = JsonUtilty.GenerateUpdateJson (ContentToImport);
				Console.Write ("Uploading new content... ");
				CloudController.UpdateBoard (currentBoard.Id, json);
				Console.WriteLine ("done");
			}

			Console.WriteLine ("IsRunning false");
			isRunning = false;

			BTProgressHUD.Dismiss();
		}
	}
}

