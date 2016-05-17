﻿using System.Collections.Generic;
using Board.JsonResponses;
using Board.Schema;
using UIKit;
using Board.Infrastructure;
using Board.Utilities;

namespace Board.Facebook
{
	public static class FacebookPageImporter
	{
		public static void ImportPage (string pageId)
		{
			FacebookUtils.MakeGraphRequest (pageId, "?fields=name,location,about,cover,picture.type(large)", Completion);
		}

		private static async void Completion(List<FacebookElement> FacebookElements){
			if (FacebookElements.Count < 1) {
				return;
			}

			var importedBoard = (FacebookImportedPage)FacebookElements [0];
			var board = new Board.Schema.Board ();
			board.Name = importedBoard.Name;
			board.About = importedBoard.About;
			board.Image = await CommonUtils.DownloadUIImageFromURL (importedBoard.PictureUrl);
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
	}
}

