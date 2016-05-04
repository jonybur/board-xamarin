using System.IO;
using System.Net;
using CoreLocation;
using System.Threading.Tasks;
using Board.JsonResponses;
using Board.Utilities;
using Facebook.CoreKit;
using System.Collections.Generic;
using Newtonsoft.Json;
using UIKit;

namespace Board.Infrastructure
{
	public static class CloudController
	{
		public static bool GetAmazonS3Ticket(){
			if (AccessToken.CurrentAccessToken == null) {
				return false;
			}

			string result = JsonGETRequest ("http://"+AppDelegate.APIAddress+"/api/media/ticket?authToken="+AppDelegate.EncodedBoardToken);

			try{
				AppDelegate.AmazonS3Ticket = JsonConvert.DeserializeObject<AmazonS3TicketResponse>(result);
				return true;
			} catch {
				AppDelegate.AmazonS3Ticket = null;
				return false;
			}
		}

		public static bool UserCanEditBoard(Board.Schema.Board board){
			if (AccessToken.CurrentAccessToken == null) {
				return false;
			}

			string result = JsonGETRequest ("http://"+AppDelegate.APIAddress+"/api/board/"+board.Id+"/edit?authToken="+AppDelegate.EncodedBoardToken);

			if (result == "200" || result == string.Empty) {
				return true;
			} else {
				return false;
			}
		}

		public static bool LogIn(){
			if (AccessToken.CurrentAccessToken == null) {
				return false;
			}

			string json = "{\"accessToken\": \"" + AccessToken.CurrentAccessToken.TokenString + "\", " +
				"\"userId\": \"" + AccessToken.CurrentAccessToken.UserID + "\" }";
			
			string result = JsonPOSTRequest ("http://"+AppDelegate.APIAddress+"/api/account/login", json);

			TokenResponse tk;
			try{
				tk = JsonConvert.DeserializeObject<TokenResponse> (result);
			} catch {
				// TODO: handle connect failure or something
				tk = null;
			}

			if (tk != null && tk.authToken != null & tk.authToken != string.Empty) {
				AppDelegate.BoardToken = tk.authToken;
				AppDelegate.EncodedBoardToken = WebUtility.UrlEncode(AppDelegate.BoardToken);
				return true;
			} else {
				return false;
			}
		}

		public static void UploadObject(string url)
		{
			ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

			HttpWebRequest httpRequest = WebRequest.Create(url) as HttpWebRequest;
			httpRequest.Method = "PUT";
			httpRequest.ContentType = "application/octet-stream";
			using (Stream dataStream = httpRequest.GetRequestStream())
			{
				byte[] buffer = new byte[8000];
				using (FileStream fileStream = new FileStream("./logos/americansocial.png", FileMode.Open, FileAccess.Read))
				{
					int bytesRead = 0;
					while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
					{
						dataStream.Write(buffer, 0, bytesRead);
					}
				}
			}

			HttpWebResponse response = httpRequest.GetResponse() as HttpWebResponse;
		}


		public static bool CreateBoard(Board.Schema.Board board){

			GetAmazonS3Ticket ();

			if (AppDelegate.AmazonS3Ticket != null) {
				UploadObject (AppDelegate.AmazonS3Ticket.url);
			}
			
			string json = "{\"uuid\": \"" + board.Id  + "\", " + 
				"\"latitude\": \"" + board.GeolocatorObject.Coordinate.Latitude  + "\", " +
				"\"longitude\": \"" + board.GeolocatorObject.Coordinate.Longitude  + "\", " +
				"\"name\": \"" + board.Name + "\", " +
				"\"description\": \"" + board.Description + "\", " +
				"\"mainColorCode\": \"" + CommonUtils.UIColorToHex(board.MainColor)  + "\", " +
				"\"secondaryColorCode\": \"" + CommonUtils.UIColorToHex(board.SecondaryColor) + "\", " +
				"\"logoURL\": \"" + "http://www.getonboard.us/wp-content/uploads/2016/02/orange_60.png" + "\" }";

			string result = JsonPOSTRequest ("http://"+AppDelegate.APIAddress+"/api/board?authToken=" + AppDelegate.EncodedBoardToken, json);

			if (result == "200" || result == string.Empty) {
				return true;
			} else {
				return false;
			}
		}

		public static async Task<List<Board.Schema.Board>> GetNearbyBoards(CLLocationCoordinate2D location, int meterRadius){

			string result = JsonGETRequest ("http://" + AppDelegate.APIAddress + "/api/boards/nearby?" +
				"authToken="+AppDelegate.EncodedBoardToken+ "&latitude=" + location.Latitude + "&longitude=" + location.Longitude + "&radiusInMeters="+meterRadius);

			BoardResponse response = BoardResponse.Deserialize (result);

			// array hotfix
			result = "{ data: " + result + "}";

			var boards = await GenerateBoardListFromBoardResponse (response);

			return boards;
		}

		// hernan lo tiene que arreglar
		public static async Task<List<Board.Schema.Board>> GetUserBoards(){
			
			string result = JsonGETRequest ("http://" + AppDelegate.APIAddress + "/api/user/boards?authToken=" + AppDelegate.EncodedBoardToken);

			BoardResponse response = BoardResponse.Deserialize (result);

			var boards = await GenerateBoardListFromBoardResponse (response);

			return boards;
		}

		private static async Task<List<Board.Schema.Board>> GenerateBoardListFromBoardResponse(BoardResponse response){
			var boards = new List<Board.Schema.Board> ();

			if (response != null) {
				foreach (BoardResponse.Datum r in response.data) {

					UIImage boardImage;
					GoogleGeolocatorObject geolocatorObject;

					Board.Schema.Board board = StorageController.BoardIsStored(r.uuid);

					if (board == null) {
						board = new Board.Schema.Board ();

						// gets image and location from cloud
						boardImage = await CommonUtils.DownloadUIImageFromURL (r.logoURL);

						string jsonobj = JsonHandler.GET ("https://maps.googleapis.com/maps/api/geocode/json?latlng=" + r.latitude + "," + r.longitude + "&key=" + AppDelegate.GoogleMapsAPIKey);
						geolocatorObject = JsonHandler.DeserializeObject (jsonobj);

						// saves it to storage
						board.ImageView = new UIImageView (boardImage);
						board.GeolocatorObject = geolocatorObject;

						board.Id = r.uuid;

						StorageController.StoreBoard (board, jsonobj);
					}

					// finishes compiling board
					board.Name = r.name;
					board.MainColor = CommonUtils.HexToUIColor (r.mainColorCode);
					board.SecondaryColor = CommonUtils.HexToUIColor (r.secondaryColorCode);
					board.CreatorId = r.userId;

					boards.Add (board);

				}
			}

			return boards;
		}

		public static string GetUberProduct(double lat, double lng){
			string result = JsonGETRequest("https://api.uber.com/v1/products?latitude="+lat+"&longitude="+lng+"&server_token="+AppDelegate.UberServerToken);

			var productResponse = JsonConvert.DeserializeObject<UberProductResponse> (result);

			if (productResponse != null) {
				if (productResponse.products.Count > 0) {
					// always gets first product (luckily it will always be uberX)
					return productResponse.products [0].product_id;
				}
			}

			return string.Empty;
		}

		public static void LogOut(){
			AppDelegate.BoardToken = null;
			AppDelegate.EncodedBoardToken = null;
		}

		private static string JsonGETRequest(string url, string contentType = "application/json")
		{
			var httpWebRequest = (HttpWebRequest)WebRequest.Create (url);
			httpWebRequest.ContentType = contentType;
			httpWebRequest.Method = "GET";
			httpWebRequest.Timeout = 8000;

			try{
				var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse ();
				string result = string.Empty;
				using (var streamReader = new StreamReader (httpResponse.GetResponseStream ())) {
					result = streamReader.ReadToEnd ();
				}
				return result;
			} catch (WebException e) {

				if (e.Status == WebExceptionStatus.ProtocolError) 
				{
					return ((HttpWebResponse)e.Response).StatusCode.ToString();
				}

				return e.Status.ToString();
			}
		}

		private static string JsonPOSTRequest(string url, string json)
		{
			var httpWebRequest = (HttpWebRequest)WebRequest.Create (url);
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "POST";
			httpWebRequest.Timeout = 8000;

			try{
				using (var streamWriter = new StreamWriter (httpWebRequest.GetRequestStream ())) {
					streamWriter.Write (json);
					streamWriter.Flush ();
					streamWriter.Close ();
				}

				var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse ();
				string result = string.Empty;
				using (var streamReader = new StreamReader (httpResponse.GetResponseStream ())) {
					result = streamReader.ReadToEnd ();
				}
				return result;
			} catch (WebException e) {

				if (e.Status == WebExceptionStatus.ProtocolError) 
				{
					return ((HttpWebResponse)e.Response).StatusCode.ToString();
				}

				return e.Status.ToString();

			}
		}
	}
}

