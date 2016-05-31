using System.Collections.Generic;
using System.Linq;
using BigTed;
using Board.Infrastructure;
using Board.Interface;
using Board.JsonResponses;
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
		static string PageId;

		public static void ImportPage (string pageId)
		{
			PageId = pageId;
			BTProgressHUD.Show ("Importing Board...");
			FacebookUtils.MakeGraphRequest (pageId, "?fields=name,location,about,cover,picture.type(large)", GenerateBoard);
		}

		public static void ImportPageContent(string pageId){
			PageId = pageId;
			ItemLocation = new CGPoint(0,110);
			ContentToImport = new List<Content> ();
			FacebookUtils.MakeGraphRequest(PageId, "?fields=posts.limit(3)", GetAnnouncements);
		}

		private static async void GenerateBoard(List<FacebookElement> FacebookElements){
			if (FacebookElements.Count < 1) {
				return;
			}

			var importedBoard = (FacebookImportedPage)FacebookElements [0];
			var board = new Board.Schema.Board ();
			board.Name = importedBoard.Name;
			board.About = importedBoard.About;
			board.Logo = await CommonUtils.DownloadUIImageFromURL (importedBoard.PictureUrl);
			board.CoverImage = await CommonUtils.DownloadUIImageFromURL (importedBoard.CoverUrl);
			board.GeolocatorObject = new GoogleGeolocatorObject ();
			board.MainColor = UIColor.Black;
			board.SecondaryColor = UIColor.Black;

			board.GeolocatorObject.results = new List<Result> ();

			var result = new Result ();
			result.geometry = new Geometry ();
			result.geometry.location = new Location ();
			result.geometry.location.lat = importedBoard.Location.Latitude;
			result.geometry.location.lng = importedBoard.Location.Longitude;
			board.GeolocatorObject.results.Add (result);

			// - creates board -

			CloudController.CreateBoard (board);
		}



		static void GetAnnouncements(List<FacebookElement> FacebookElements){
			// parses all posts
			foreach (FacebookPost fbPost in FacebookElements) {

				if (fbPost.Message != "<null>" && fbPost.Message != null) {
					ItemLocation.X += 290;
					var announcement = new Announcement (fbPost, ItemLocation, CGAffineTransform.MakeIdentity());

					ContentToImport.Add (announcement);
				} else {
					continue;
				}
			}

			// gets 5 events
			BTProgressHUD.Show("Importing Events...");
			FacebookUtils.MakeGraphRequest(PageId, "?fields=events.limit(3)", GetEvents);
		}

		static void GetEvents(List<FacebookElement> FacebookElements){
			// parses all events
			int coversToLoad = 0;
			var boardEvents = new List<BoardEvent> ();
			ItemLocation = new CGPoint (0, 300);

			foreach (FacebookEvent fbEvent in FacebookElements) {

				ItemLocation.X += 290;
				var boardEvent = new BoardEvent (fbEvent, ItemLocation, CGAffineTransform.MakeIdentity());
				boardEvents.Add (boardEvent);

				FacebookUtils.MakeGraphRequest (fbEvent.Id, "?fields=cover", delegate(List<FacebookElement> elementList) {
					if (elementList.Count > 0) {
						var cover = elementList [0] as FacebookCover;
						if (cover != null) {
							boardEvents[coversToLoad].ImageUrl = cover.Source;
						}
					}

					coversToLoad++;

					if (coversToLoad == FacebookElements.Count){
						ContentToImport.AddRange(boardEvents);

						// gets albums
						BTProgressHUD.Show("Importing Photos...");
						//FacebookUtils.MakeGraphRequest(UIBoardInterface.board.FBPage.Id, "albums", GetAlbums);
						FacebookUtils.MakeGraphRequest(PageId, "photos/uploaded?limit=9", GetPhotos);
					}
				});
			}

			if (FacebookElements.Count == 0) {
				// gets albums
				BTProgressHUD.Show("Importing Photos...");
				//FacebookUtils.MakeGraphRequest(UIBoardInterface.board.FBPage.Id, "albums", GetAlbums);
				FacebookUtils.MakeGraphRequest(PageId, "photos/uploaded?limit=9", GetPhotos);
			}

		}

		static void GetAlbums(List<FacebookElement> elementList)
		{
			if (elementList.Count > 0) {
				var album = elementList [0] as FacebookAlbum;
				FacebookUtils.MakeGraphRequest(album.Id, "photos?limit=3", GetPhotos);
			}
		}

		static void GetPhotos(List<FacebookElement> FacebookElements){
			// parses all pictures
			var pictures = new List<Picture> ();
			int picturesToLoad = 0;
			ItemLocation = new CGPoint (0, 510);

			int photoNumber = 0;
			foreach (FacebookPhoto fbPhoto in FacebookElements) {
				if (photoNumber == 3) {
					ItemLocation = new CGPoint (1400, 310);
				} else if (photoNumber == 6) {
					ItemLocation = new CGPoint (1400, 500);
				}
				ItemLocation.X += 290;
				var picture = new Picture (fbPhoto, ItemLocation, CGAffineTransform.MakeIdentity());
				pictures.Add (picture);
				photoNumber++;
				FacebookUtils.MakeGraphRequest (fbPhoto.Id, "?fields=images", async delegate(List<FacebookElement> elementList) {
					if (elementList.Count > 0) {
						elementList = elementList.OrderByDescending(x => ((FacebookImage)x).Height).ToList();

						FacebookImage fbImage;
						if (elementList.Count > 2) {
							fbImage = elementList [2] as FacebookImage;
						} else if (elementList.Count > 1){
							fbImage = elementList [1] as FacebookImage;
						} else {
							fbImage = elementList [0] as FacebookImage;
						}

						if (fbImage != null) {
							var image = await CommonUtils.DownloadUIImageFromURL(fbImage.Source);
							var amazonUrl = CloudController.UploadToAmazon(image);
							pictures[picturesToLoad].ImageUrl = amazonUrl;
						}
					}

					picturesToLoad++;

					if (picturesToLoad == FacebookElements.Count){

						ContentToImport.AddRange(pictures);
						BTProgressHUD.Show("Importing Videos...");
						FacebookUtils.MakeGraphRequest (PageId, "videos?fields=source,description,updated_time,thumbnails&limit=3", GetVideos);
						//Test();
					}
				});
			}

			if (FacebookElements.Count == 0) {
				BTProgressHUD.Show("Importing Videos...");
				FacebookUtils.MakeGraphRequest (PageId, "videos?fields=source,description,updated_time,thumbnails&limit=3", GetVideos);
			}

			// gets 5 videos
		}

		static async void GetVideos(List<FacebookElement> FacebookElements){
			// parses all videos

			ItemLocation = new CGPoint (1700, 130);
			int i = 1;
			foreach (FacebookVideo fbVideo in FacebookElements) {

				var video = new Video (fbVideo, ItemLocation, CGAffineTransform.MakeIdentity());
				BTProgressHUD.Show("Importing Videos... " + i + "/" + FacebookElements.Count);
				var byteArray = await CommonUtils.DownloadByteArrayFromURL (fbVideo.Source);
				video.AmazonUrl = CloudController.UploadToAmazon (byteArray);

				ItemLocation.X += 290;
				ContentToImport.Add (video);
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

