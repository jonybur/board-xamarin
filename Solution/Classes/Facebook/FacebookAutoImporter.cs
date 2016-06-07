using System.Collections.Generic;
using System.Linq;
using BigTed;
using Board.Infrastructure;
using Board.Interface;
using Board.JsonResponses;
using System;
using Board.Schema;
using Board.Utilities;
using CoreGraphics;
using UIKit;

namespace Board.Facebook
{
	public static class FacebookAutoImporter
	{
		static List<Content> ContentToImport;
		static CGPoint ItemLocation;

		public static void ImportPages(params string[] pageIds){
			foreach (var id in pageIds){
				ImportPage(id);
			}
		}

		public static void ImportPage (string pageId)
		{
			BTProgressHUD.Show ("Importing Board...");
			FacebookUtils.MakeGraphRequest (pageId, "?fields=name,location,about,cover,phone,category_list,picture.type(large)", async delegate (List<FacebookElement> FacebookElements) {
				if (FacebookElements.Count < 1) {
					BTProgressHUD.Dismiss ();
					return;
				}

				var importedBoard = (FacebookImportedPage)FacebookElements [0];
				var board = new Board.Schema.Board ();
				board.Name = importedBoard.Name;
				board.About = importedBoard.About;

				board.Logo = await CommonUtils.DownloadUIImageFromURL (importedBoard.PictureUrl);

				Console.WriteLine(importedBoard.PictureUrl);
				Console.WriteLine(board.Logo == null);

				board.CoverImage = await CommonUtils.DownloadUIImageFromURL (importedBoard.CoverUrl);

				Console.WriteLine(importedBoard.CoverUrl);
				Console.WriteLine(board.CoverImage == null);

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

				CloudController.CreateBoard (board);
				BTProgressHUD.Dismiss ();
			});
		}

		const float startX = 500;

		static string PageId;

		public static void ImportPageContent(string pageId){
			PageId = pageId;
			ItemLocation = new CGPoint(startX,100);
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

			// checks out the posts....
			// posts can be pictures or announcements
			foreach (FacebookPost fbPost in FacebookElements) {

				// if it has messages
				if (fbPost.Message != "<null>" && fbPost.Message != null) {

					// change location of new widget

					// goes check out if it has an image
					FacebookUtils.MakeGraphRequest (fbPost.Id, "?fields=full_picture", async delegate(List<FacebookElement> elementList) {
						
						if (elementList.Count > 0) {

							// WE HAVE A PICTURE

							// if it has an image, it adds it to the announcement (it searches in the announcement list for it)
							var cover = elementList [0] as FacebookFullPicture;

							if (!string.IsNullOrEmpty(cover.FullPicture)) {
								var image = await CommonUtils.DownloadUIImageFromURL(cover.FullPicture);
								var amazonUrl = CloudController.UploadToAmazon(image);

								var picture = new Picture (fbPost, amazonUrl, ItemLocation, CGAffineTransform.MakeIdentity());
								ContentToImport.Add(picture);

							}else{
								
								var announcement = new Announcement (fbPost, ItemLocation, CGAffineTransform.MakeIdentity());
								ContentToImport.Add(announcement);
							}

						} else {

							var announcement = new Announcement (fbPost, ItemLocation, CGAffineTransform.MakeIdentity());
							ContentToImport.Add(announcement);

						}

						// image or not, this announcement has been checked
						announcementsToLoad ++;
						ItemLocation.X += 150;

						// if i checked all the announcements
						if (announcementsToLoad == FacebookElements.Count){
							
							// goes to import events
							BTProgressHUD.Show("Importing Events...");
							FacebookUtils.MakeGraphRequest(PageId, "?fields=events.limit(3)", GetEvents);
						}
					});

				} else {
					// no message? next announcement please
					continue;
				}
			}

			// no announcements? goes seek events
			if (FacebookElements.Count == 0) {
				BTProgressHUD.Show("Importing Events...");
				FacebookUtils.MakeGraphRequest(PageId, "?fields=events.limit(3)", GetEvents);
			} 
		}

		static void GetEvents(List<FacebookElement> FacebookElements){
			// parses all events
			int coversToLoad = 0;
			var boardEvents = new List<BoardEvent> ();
			ItemLocation = new CGPoint (startX, 200);

			foreach (FacebookEvent fbEvent in FacebookElements) {
				
				var boardEvent = new BoardEvent (fbEvent, ItemLocation, CGAffineTransform.MakeIdentity());
				boardEvents.Add (boardEvent);

				FacebookUtils.MakeGraphRequest (fbEvent.Id, "?fields=cover,updated_time", async delegate(List<FacebookElement> elementList) {
					if (elementList.Count > 0) {
						var cover = elementList [0] as FacebookCoverUpdatedTime;
						if (cover != null) {
							var image = await CommonUtils.DownloadUIImageFromURL(cover.Source);
							var amazonUrl = CloudController.UploadToAmazon(image);
							boardEvents[coversToLoad].ImageUrl = amazonUrl;
						}
						boardEvents[coversToLoad].CreationDate = DateTime.Parse(cover.UpdatedTime);
					}

					coversToLoad++;
					ItemLocation.X += 150;

					if (coversToLoad == FacebookElements.Count){
						ContentToImport.AddRange(boardEvents);

						// gets albums
						BTProgressHUD.Show("Importing Videos...");
						FacebookUtils.MakeGraphRequest (PageId, "videos?fields=source,description,updated_time,thumbnails&limit=3", GetVideos);
					}
				});
			}

			if (FacebookElements.Count == 0) {
				// gets albums
				BTProgressHUD.Show("Importing Videos...");
				FacebookUtils.MakeGraphRequest (PageId, "videos?fields=source,description,updated_time,thumbnails&limit=3", GetVideos);
			}

		}

		static async void GetVideos(List<FacebookElement> FacebookElements){
			// parses all videos

			ItemLocation = new CGPoint (startX, 300);
			int i = 1;
			foreach (FacebookVideo fbVideo in FacebookElements) {

				var video = new Video (fbVideo, ItemLocation, CGAffineTransform.MakeIdentity());
				BTProgressHUD.Show("Importing Videos... " + i + "/" + FacebookElements.Count);
				var byteArray = await CommonUtils.DownloadByteArrayFromURL (fbVideo.Source);
				video.AmazonUrl = CloudController.UploadToAmazon (byteArray);

				ContentToImport.Add (video);
				ItemLocation.X += 150;
				i++;
			}

			UploadContent (); 
		}

		static void UploadContent(){
			var json = JsonUtilty.GenerateUpdateJson (ContentToImport);
			CloudController.UpdateBoard (UIBoardInterface.board.Id, json);
			BTProgressHUD.Dismiss();
		}


	}
}

