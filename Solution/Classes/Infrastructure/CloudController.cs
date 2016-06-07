using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Board.JsonResponses;
using System.Globalization;
using Board.Schema;
using Board.Utilities;
using CoreLocation;
using Facebook.CoreKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using Foundation;
using UIKit;

namespace Board.Infrastructure
{
	// https://api.instagram.com/v1/locations/search?lat=-34.5885886&lng=-58.426003&access_token=2292871863.37fcdb1.cdc6ab03abfa4a8db4a2da022ec5d3c2

	public static class CloudController
	{
		public static void GetUserProfile(){
			string result = JsonGETRequest ("http://" + AppDelegate.APIAddress + "/api/user?authToken=" + AppDelegate.EncodedBoardToken);

			if (result == "Timeout" || result == "InternalServerError") {
				return;
			}

			AppDelegate.BoardUser = JsonConvert.DeserializeObject<User> (result);

			AppDelegate.BoardUser.SetProfilePictureFromURL (AppDelegate.BoardUser.ProfilePictureURL);
		}

		public static bool UserLikesPublication(string publicationId){
			string request = "http://" + AppDelegate.APIAddress + "/api/user/likes?publicationId=" + publicationId +
			                 "&authToken=" + AppDelegate.EncodedBoardToken;
			string result = JsonGETRequest (request);

			var jobject = JObject.Parse (result);

			var isLiked = jobject [publicationId];

			if (isLiked.Type == JTokenType.Boolean) {
				return Boolean.Parse (isLiked.ToString ());
			}

			return false;

		}

		public static bool SendLike(string idToLike){
			string result = JsonPUTRequest ("http://"+AppDelegate.APIAddress+"/api/publications/"+idToLike+"/like?authToken="+AppDelegate.EncodedBoardToken+
				"&time="+CommonUtils.GetUnixTimeStamp());

			if (result == "200" || result == string.Empty) {
				return true;
			}
			return false;

		}

		public static bool SendDislike(string idToDislike){
			string result = JsonPUTRequest ("http://"+AppDelegate.APIAddress+"/api/publications/"+idToDislike+"/dislike?authToken="+AppDelegate.EncodedBoardToken+
				"&time="+CommonUtils.GetUnixTimeStamp());

			if (result == "200" || result == string.Empty) {
				return true;
			}
			return false;
		}

		public static int GetLike(string id){
			string result = JsonGETRequest ("http://"+AppDelegate.APIAddress+"/api/publications/likes?publicationId=" + id + "&authToken="+AppDelegate.EncodedBoardToken);

			if (result == "Timeout" || result == "InternalServerError") {
				return -1;
			}

			var jobject = JObject.Parse (result);

			var likes = jobject [id];

			if (likes.Type == JTokenType.Integer) {
				int likeNumber = Int32.Parse (likes.ToString ());

				return likeNumber;
			}

			return -1;
		}

		public static Dictionary<string, int> GetLikes(params string[] ids){
			string publicationsToRequest = string.Empty;

			foreach (var id in ids) {
				publicationsToRequest += "publicationId=" + id + "&";
			}

			string result = JsonGETRequest ("http://"+AppDelegate.APIAddress+"/api/publications/likes?"+publicationsToRequest+"authToken="+AppDelegate.EncodedBoardToken);

			if (result == "Timeout" || result == "InternalServerError") {
				return new Dictionary<string, int> ();
			}

			var jobject = JObject.Parse (result);

			// TODO: see if this can get simplified using just json.deserialize or such
			var dictionaryLikes = new Dictionary<string, int> ();
			foreach (var id in ids) {
				var likes = jobject [id];

				if (likes.Type == JTokenType.Integer) {
					int likeNumber = Int32.Parse (likes.ToString ());

					dictionaryLikes.Add (id, likeNumber);
				}
			}

			return dictionaryLikes;
		}

		public static bool UpdateBoard(string boardId, string json){
			string result = JsonPOSTRequest ("http://"+AppDelegate.APIAddress+"/api/board/"+boardId+"/updates?authToken="+AppDelegate.EncodedBoardToken, json);

			if (result == "200" || result == string.Empty) {
				return true;
			}
			return false;
		}

		public static List<Content> GetTimeline(CLLocationCoordinate2D location){
			string request = "http://" + AppDelegate.APIAddress + "/api/boards/timeline?latitude=" +
			             location.Latitude.ToString (CultureInfo.InvariantCulture) + "&longitude=" + location.Longitude.ToString (CultureInfo.InvariantCulture) +
			             "&authToken=" + AppDelegate.EncodedBoardToken;
			string result = JsonGETRequest (request);

			if (result == "Timeout" || result == "InternalServerError") {
				return new List<Content> ();
			}

			var jobject = JObject.Parse(result);

			var datum = jobject ["data"];

			var timeline = new List<Content> ();

			foreach (var jsonContent in datum) {
				switch (jsonContent ["Type"].ToString()) {
				case "pictures":
					timeline.Add (jsonContent.ToObject<Picture>());
					break;
				case "announcements":
					timeline.Add (jsonContent.ToObject<Announcement>());
					break;
				case "videos":
					timeline.Add (jsonContent.ToObject<Video>());
					break;
				case "polls":
					timeline.Add (jsonContent.ToObject<Poll>());
					break;
				case "events":
					timeline.Add (jsonContent.ToObject<BoardEvent>());
					break;
				}
			}

			return timeline;
		}

		public static MagazineResponse GetMagazine(CLLocationCoordinate2D location){

			string result = JsonGETRequest ("http://"+AppDelegate.APIAddress+"/api/magazines/nearest?latitude="+
				location.Latitude.ToString(CultureInfo.InvariantCulture)+"&longitude="+location.Longitude.ToString(CultureInfo.InvariantCulture)+
				"&authToken="+AppDelegate.EncodedBoardToken);

			if (result == "Timeout" || result == "InternalServerError") {
				return new MagazineResponse ();
			}

			var magazine = MagazineResponse.Deserialize (result);

			if (MagazineResponse.IsValidMagazine(magazine)){
				magazine.data.entries = magazine.data.entries.OrderBy (x => x.section).ToList ();
			}

			return magazine;
		}

		public static Dictionary<string, Content> GetBoardContent(string boardId){
			string result = JsonGETRequest ("http://"+AppDelegate.APIAddress+"/api/board/"+boardId+"/snapshot?authToken="+AppDelegate.EncodedBoardToken);

			if (result == "Timeout" || result == "InternalServerError") {
				return new Dictionary<string, Content> ();
			}

			var fullJson = JsonConvert.DeserializeObject<Dictionary<string, object>> (result);

			var compiledDictionary = new Dictionary<string, Content> ();
			if (fullJson.ContainsKey ("updates")) {
				string updatesJson = fullJson ["updates"].ToString ();

				var contentsLevel = JsonConvert.DeserializeObject<Dictionary<string, object>> (updatesJson);

				FillDictionary<Picture> (contentsLevel, "pictures", ref compiledDictionary);
				FillDictionary<Announcement> (contentsLevel, "announcements", ref compiledDictionary);
				FillDictionary<Poll> (contentsLevel, "polls", ref compiledDictionary);
				FillDictionary<Video> (contentsLevel, "videos", ref compiledDictionary);
				FillDictionary<BoardEvent> (contentsLevel, "events", ref compiledDictionary);
			}
			return compiledDictionary;
		}

		private static void FillDictionary<T>(Dictionary<string, object> contentsLevel, 
			string jsonTypeName,
			ref Dictionary<string, Content> dictionaryToFill) where T : Content
		{
			
			if (contentsLevel.ContainsKey (jsonTypeName)) {
				string contents = contentsLevel [jsonTypeName].ToString ();
				var contentsDictionary = JsonConvert.DeserializeObject<Dictionary<string, T>> (contents);
				
				foreach (var cnt in contentsDictionary) {
					dictionaryToFill.Add (cnt.Key, cnt.Value);
				}
			}
		}

		public static bool GetAmazonS3Ticket(string mimeType){
			string result = JsonGETRequest ("http://"+AppDelegate.APIAddress+"/api/media/ticket?authToken="+AppDelegate.EncodedBoardToken+"&contentType="+mimeType);

			try{
				AppDelegate.AmazonS3Ticket = JsonConvert.DeserializeObject<AmazonS3TicketResponse>(result);
				return true;
			} catch {
				AppDelegate.AmazonS3Ticket = null;
				return false;
			}
		}

		public static bool GetAmazonS3Ticket(){
			string result = JsonGETRequest ("http://"+AppDelegate.APIAddress+"/api/media/ticket?authToken="+AppDelegate.EncodedBoardToken);

			try{
				AppDelegate.AmazonS3Ticket = JsonConvert.DeserializeObject<AmazonS3TicketResponse>(result);
				return true;
			} catch {
				AppDelegate.AmazonS3Ticket = null;
				return false;
			}
		}

		public static bool DeleteBoard(string boardId){
			
			string result = JsonGETRequest ("http://" + AppDelegate.APIAddress + "/api/board/" + boardId + "?authToken=" + AppDelegate.EncodedBoardToken, "DELETE");

			if (result == "200" || result == string.Empty) {
				return true;
			} else {
				return false;
			}
		}

		public static bool UserCanEditBoard(string boardId){
			if (AccessToken.CurrentAccessToken == null) {
				return false;
			}

			string url = "http://" + AppDelegate.APIAddress + "/api/board/" + boardId + "/edit?authToken=" + AppDelegate.EncodedBoardToken;
			string result = JsonGETRequest (url);

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

		private static string UploadStream(string amazonUrl, Stream data, string mimeType)
		{
			HttpWebRequest httpRequest = WebRequest.Create(amazonUrl) as HttpWebRequest;
			httpRequest.Method = "PUT";
			httpRequest.ContentType = mimeType;

			using (Stream dataStream = httpRequest.GetRequestStream()) {
				data.CopyTo (dataStream);
			}

			var response = httpRequest.GetResponse() as HttpWebResponse;
			var absoluteURL = response.ResponseUri.AbsoluteUri;
			var indexOfParameter = absoluteURL.IndexOf ('?');

			if (indexOfParameter != -1) {
				absoluteURL = absoluteURL.Substring (0, indexOfParameter);
			}

			return absoluteURL;
		}

		public static string UploadToAmazon(byte[] byteArray, string mime = "video/mp4"){
			GetAmazonS3Ticket (mime);

			string url;
			if (AppDelegate.AmazonS3Ticket != null) {
				var stream = new MemoryStream(byteArray);
				url = UploadStream (AppDelegate.AmazonS3Ticket.url, stream, mime);
			} else {
				return null;
			}
			return url;
		}

		public static string UploadToAmazon(NSUrl localnsurl, string mime = "video/mp4"){
			GetAmazonS3Ticket (mime);

			string url;
			if (AppDelegate.AmazonS3Ticket != null) {
				var byteArray = File.ReadAllBytes(localnsurl.Path);
				var stream = new MemoryStream(byteArray);
				url = UploadStream (AppDelegate.AmazonS3Ticket.url, stream, mime);
			} else {
				return null;
			}
			return url;
		}

		public static string UploadToAmazon(UIImage image, string mime = "image/jpeg"){
			GetAmazonS3Ticket (mime);

			string url;
			if (AppDelegate.AmazonS3Ticket != null) {
				url = UploadStream (AppDelegate.AmazonS3Ticket.url, image.AsJPEG().AsStream(), mime);
			} else {
				return null;
			}
			return url;
		}

		public static bool CreateBoard(Board.Schema.Board board){
			string logoURL = string.Empty, coverURL = string.Empty;

			if (board.Logo != null) {
				logoURL = UploadToAmazon (board.Logo);
			}
			if (board.CoverImage != null) {
				coverURL = UploadToAmazon (board.CoverImage);
			}

			// HAS TO HAVE A LOGO
			if (string.IsNullOrEmpty(logoURL)) {
				return false;
			}
			
			string json = "{\"uuid\": \"" + board.Id  + "\", " + 
				"\"latitude\": \"" + board.GeolocatorObject.Coordinate.Latitude  + "\", " +
				"\"longitude\": \"" + board.GeolocatorObject.Coordinate.Longitude  + "\", " +
				"\"name\": \"" + board.Name + "\", " +
				"\"about\": " + JsonConvert.ToString(board.About) + ", " +
				"\"mainColorCode\": \"" + CommonUtils.UIColorToHex(board.MainColor)  + "\", " +
				"\"secondaryColorCode\": \"" + CommonUtils.UIColorToHex(board.SecondaryColor) + "\", " +
				"\"phoneNumber\": \"" + board.Phone + "\", " + 
				"\"facebookID\": \"" + board.FacebookId + "\", " + 
				"\"categoryName\": \"" + board.Category + "\", " + 
				"\"logoURL\": \"" + logoURL + "\", " + 
				"\"coverURL\": \"" + coverURL + "\" }";

			string result = JsonPOSTRequest ("http://" + AppDelegate.APIAddress + "/api/board?authToken=" + AppDelegate.EncodedBoardToken, json);

			if (result == "200" || result == string.Empty) {
				return true;
			}
			return false;
		}

		public static bool EditBoard(Board.Schema.Board board){

			string json = "{\"latitude\": \"" + board.GeolocatorObject.Coordinate.Latitude  + "\", " +
				"\"longitude\": \"" + board.GeolocatorObject.Coordinate.Longitude  + "\", " +
				"\"name\": \"" + board.Name + "\", " +
				"\"about\": " + JsonConvert.ToString(board.About) + ", " +
				"\"mainColorCode\": \"" + CommonUtils.UIColorToHex(board.MainColor)  + "\", " +
				"\"secondaryColorCode\": \"" + CommonUtils.UIColorToHex(board.SecondaryColor) + "\", " +
				"\"phoneNumber\": \"" + board.Phone + "\", " + 
				"\"facebookID\": \"" + board.FacebookId + "\", " + 
				"\"categoryName\": \"" + board.Category + "\", " + 
				"\"logoURL\": \"" + board.LogoUrl + "\", " + 
				"\"coverURL\": \"" + board.CoverImageUrl + "\" }";

			string result = JsonPUTRequest ("http://" + AppDelegate.APIAddress + "/api/board/"+ board.Id +"?authToken=" + AppDelegate.EncodedBoardToken, json);

			if (result == "200" || result == string.Empty) {
				return true;
			}
			return false;
		}

		public static List<Board.Schema.Board> GetNearbyBoards(CLLocationCoordinate2D location, int meterRadius){
			
			string request = "http://" + AppDelegate.APIAddress + "/api/boards/nearby?" +
			                 "authToken=" + AppDelegate.EncodedBoardToken + "&latitude=" + 
				location.Latitude.ToString(CultureInfo.InvariantCulture) + "&longitude=" +
				location.Longitude.ToString(CultureInfo.InvariantCulture) + "&radiusInMeters=" + meterRadius;

			string result = JsonGETRequest (request);

			var response = BoardResponse.Deserialize (result);

			var boards = GenerateBoardListFromBoardResponse (response);

			return boards;
		}

		public static List<Board.Schema.Board> GetAllBoards(){
			string result = JsonGETRequest ("http://" + AppDelegate.APIAddress + "/api/boards?authToken=" + AppDelegate.EncodedBoardToken);

			BoardResponse response = BoardResponse.Deserialize (result);

			var boards = GenerateBoardListFromBoardResponse (response);

			return boards;
		}

		public static List<Board.Schema.Board> GetUserBoards(){
			
			string result = JsonGETRequest ("http://" + AppDelegate.APIAddress + "/api/user/boards?authToken=" + AppDelegate.EncodedBoardToken);

			BoardResponse response = BoardResponse.Deserialize (result);

			var boards = GenerateBoardListFromBoardResponse (response);

			return boards;
		}

		private static List<Board.Schema.Board> GenerateBoardListFromBoardResponse(BoardResponse response){
			var boards = new List<Board.Schema.Board> ();

			if (response != null) {
				foreach (BoardResponse.Datum r in response.data) {
					boards.Add(GenerateBoardFromBoardResponse (r));
				}
			}

			return boards;
		}

		public static Board.Schema.Board GenerateBoardFromBoardResponse(BoardResponse.Datum datum){
			GoogleGeolocatorObject geolocatorObject;

			Board.Schema.Board board = StorageController.BoardIsStored(datum.uuid);

			if (board == null) {
				board = new Board.Schema.Board ();

				string jsonobj = JsonHandler.GET ("https://maps.googleapis.com/maps/api/geocode/json?latlng=" +
					datum.latitude.ToString(CultureInfo.InvariantCulture) + "," + datum.longitude.ToString(CultureInfo.InvariantCulture) + "&key=" + AppDelegate.GoogleMapsAPIKey);
				geolocatorObject = JsonHandler.DeserializeObject (jsonobj);

				board.GeolocatorObject = geolocatorObject;
				board.Id = datum.uuid;

				StorageController.StoreBoard (board, jsonobj);
			}

			// finishes compiling board
			board.Name = datum.name;
			board.About = datum.about;
			board.LogoUrl = datum.logoURL;
			board.CoverImageUrl = datum.coverURL;
			board.MainColor = CommonUtils.HexToUIColor (datum.mainColorCode);
			board.SecondaryColor = CommonUtils.HexToUIColor (datum.secondaryColorCode);
			board.CreatorId = datum.userId;
			board.Phone = datum.phoneNumber;
			board.Category = datum.categoryName;
			board.FacebookId = datum.facebookID;

			return board;
		}

		public static InstagramMediaResponse GetInstagramMedia(string locationId){
			string result = JsonGETRequest("https://api.instagram.com/v1/locations/"+locationId+"/media/recent?access_token="+AppDelegate.InstagramServerToken);

			var instagramResponse = JsonConvert.DeserializeObject<InstagramMediaResponse> (result);

			if (instagramResponse != null) {
				Console.WriteLine ("stop");
			}

			return null;
		}

		public static UberProductResponse GetUberProducts(CLLocationCoordinate2D location){

			string result = JsonGETRequest("https://api.uber.com/v1/products?latitude="+location.Latitude.ToString(CultureInfo.InvariantCulture)
				+"&longitude="+location.Longitude.ToString(CultureInfo.InvariantCulture)+"&server_token="+AppDelegate.UberServerToken);

			var productResponse = JsonConvert.DeserializeObject<UberProductResponse> (result);

			if (productResponse != null) {
				if (productResponse.products.Count > 0) {
					// always gets first product (luckily it will always be uberX)
					return productResponse;
				}
			}

			return null;
		}

		public static void LogOut(){
			AppDelegate.BoardToken = null;
			AppDelegate.EncodedBoardToken = null;
			AppDelegate.BoardUser = null;
		}

		private static string JsonGETRequest(string url, string method = "GET", string contentType = "application/json")
		{
			var httpWebRequest = (HttpWebRequest)WebRequest.Create (url);
			httpWebRequest.ContentType = contentType;
			httpWebRequest.Method = method;
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

		private static string JsonPUTRequest(string url)
		{
			var httpWebRequest = (HttpWebRequest)WebRequest.Create (url);
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "PUT";
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

		private static string JsonPUTRequest(string url, string json)
		{
			var httpWebRequest = (HttpWebRequest)WebRequest.Create (url);
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "PUT";
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

