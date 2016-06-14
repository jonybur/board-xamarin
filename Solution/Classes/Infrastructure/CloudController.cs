using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Http.Headers;
using Board.JsonResponses;
using Board.Schema;
using Board.Utilities;
using CoreLocation;
using Facebook.CoreKit;
using Foundation;
using ModernHttpClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIKit;
using System.Net.Http;

namespace Board.Infrastructure
{
	// https://api.instagram.com/v1/locations/search?lat=-34.5885886&lng=-58.426003&access_token=2292871863.37fcdb1.cdc6ab03abfa4a8db4a2da022ec5d3c2

	public static class CloudController
	{
		public static void GetUserProfile(){
			string result = GetJsonSync ("https://" + AppDelegate.APIAddress + "/api/user?authToken=" + AppDelegate.EncodedBoardToken);

			if (result == "Timeout" || result == "InternalServerError") {
				return;
			}

			AppDelegate.BoardUser = JsonConvert.DeserializeObject<User> (result);

			AppDelegate.BoardUser.SetProfilePictureFromURL (AppDelegate.BoardUser.ProfilePictureURL);
		}

		public static Dictionary<string, bool> GetUserLikes(params string[] publicationIds){
			string publicationsToRequest = string.Empty;

			foreach (var id in publicationIds) {
				publicationsToRequest += "publicationId=" + id + "&";
			}
			string result = GetJsonSync ("https://"+AppDelegate.APIAddress+"/api/user/likes?"+publicationsToRequest+"authToken="+AppDelegate.EncodedBoardToken);

			if (result == "Timeout" || result == "InternalServerError") {
				return new Dictionary<string, bool> ();
			}

			var jobject = JObject.Parse (result);

			var dictionaryLikes = new Dictionary<string, bool> ();
			foreach (var id in publicationIds) {
				var likes = jobject [id];

				if (likes.Type == JTokenType.Boolean) {
					bool boolValue = Boolean.Parse (likes.ToString ());

					dictionaryLikes.Add (id, boolValue);
				}
			}

			return dictionaryLikes;
		}

		public static bool GetUserLike(string publicationId){
			string request = "https://" + AppDelegate.APIAddress + "/api/user/likes?publicationId=" + publicationId +
			                 "&authToken=" + AppDelegate.EncodedBoardToken;
			string result = GetJsonSync (request);

			var jobject = JObject.Parse (result);

			var isLiked = jobject [publicationId];

			if (isLiked.Type == JTokenType.Boolean) {
				return Boolean.Parse (isLiked.ToString ());
			}

			return false;
		}

		public static void SendLike(string idToLike){
			PutJsonAsync ("https://"+AppDelegate.APIAddress+"/api/publications/"+idToLike+"/like?authToken="+AppDelegate.EncodedBoardToken+
				"&time="+CommonUtils.GetUnixTimeStamp());
		}

		public static void SendDislike(string idToDislike){
			PutJsonAsync ("https://"+AppDelegate.APIAddress+"/api/publications/"+idToDislike+"/dislike?authToken="+AppDelegate.EncodedBoardToken+
				"&time="+CommonUtils.GetUnixTimeStamp());
		}

		public static int GetLike(string id){
			string result = GetJsonSync ("https://"+AppDelegate.APIAddress+"/api/publications/likes?publicationId=" + id + "&authToken="+AppDelegate.EncodedBoardToken);

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

			string result = GetJsonSync ("https://"+AppDelegate.APIAddress+"/api/publications/likes?"+publicationsToRequest+"authToken="+AppDelegate.EncodedBoardToken);

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
			string result = PostJsonSync ("https://"+AppDelegate.APIAddress+"/api/board/"+boardId+"/updates?authToken="+AppDelegate.EncodedBoardToken, json);

			if (result == "200" || result == string.Empty) {
				return true;
			}
			return false;
		}

		public static List<Content> GetTimeline(CLLocationCoordinate2D location){
			string request = "https://" + AppDelegate.APIAddress + "/api/boards/timeline?latitude=" +
			             location.Latitude.ToString (CultureInfo.InvariantCulture) + "&longitude=" + location.Longitude.ToString (CultureInfo.InvariantCulture) +
			             "&authToken=" + AppDelegate.EncodedBoardToken;
			string result = GetJsonSync (request);

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

			string result = GetJsonSync ("https://"+AppDelegate.APIAddress+"/api/magazines/nearest?latitude="+
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
			string result = GetJsonSync ("https://"+AppDelegate.APIAddress+"/api/board/"+boardId+"/snapshot?authToken="+AppDelegate.EncodedBoardToken);

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
			string result = GetJsonSync ("https://"+AppDelegate.APIAddress+"/api/media/ticket?authToken="+AppDelegate.EncodedBoardToken+"&contentType="+mimeType);

			try{
				AppDelegate.AmazonS3Ticket = JsonConvert.DeserializeObject<AmazonS3TicketResponse>(result);
				return true;
			} catch {
				AppDelegate.AmazonS3Ticket = null;
				return false;
			}
		}

		public static bool GetAmazonS3Ticket(){
			string result = GetJsonSync ("https://"+AppDelegate.APIAddress+"/api/media/ticket?authToken="+AppDelegate.EncodedBoardToken);

			try{
				AppDelegate.AmazonS3Ticket = JsonConvert.DeserializeObject<AmazonS3TicketResponse>(result);
				return true;
			} catch {
				AppDelegate.AmazonS3Ticket = null;
				return false;
			}
		}

		public static bool DeleteBoard(string boardId){
			
			string result = DeleteJsonSync ("https://" + AppDelegate.APIAddress + "/api/board/" + boardId + "?authToken=" + AppDelegate.EncodedBoardToken);

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

			string url = "https://" + AppDelegate.APIAddress + "/api/board/" + boardId + "/edit?authToken=" + AppDelegate.EncodedBoardToken;
			string result = GetJsonSync (url);

			if (result == "200" || result == string.Empty) {
				return true;
			} else {
				return false;
			}
		}

		public static void LogSession(){
			if (AppDelegate.HasLoggedSession){
				return;
			}

			string json = "{\"latitude\": \"" + AppDelegate.UserLocation.Latitude.ToString(CultureInfo.InvariantCulture) + "\", " +
				"\"longitude\": \"" + AppDelegate.UserLocation.Longitude.ToString(CultureInfo.InvariantCulture) + "\", " + 
				"\"timestamp\": \"" + CommonUtils.GetUnixTimeStamp() + "\", " + 
				"\"name\": \"" + AppDelegate.BoardUser.FirstName + " " + AppDelegate.BoardUser.LastName +"\" }";

			PostJsonAsync ("https://admin.boardack.com/log-user", json);
			AppDelegate.HasLoggedSession = true;
		}

		public static bool LogIn(){
			if (AccessToken.CurrentAccessToken == null) {
				return false;
			}

			string json = "{\"accessToken\": \"" + AccessToken.CurrentAccessToken.TokenString + "\", " +
				"\"userId\": \"" + AccessToken.CurrentAccessToken.UserID + "\" }";

			string result = PostJsonSync ("https://" + AppDelegate.APIAddress + "/api/account/login", json);

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
			}
			return false;
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

		public static bool CreateBoard(Schema.Board board){
			string logoURL = string.Empty, coverURL = string.Empty;

			if (board.Logo != null) {
				Console.Write ("Uploading logo to AWS... ");
				logoURL = UploadToAmazon (board.Logo);
				Console.WriteLine ("done - link: " + logoURL);
			}
			if (board.CoverImage != null) {
				Console.Write ("Uploading cover image to AWS... ");
				coverURL = UploadToAmazon (board.CoverImage);
				Console.WriteLine ("done - link: " + coverURL);
			}

			// HAS TO HAVE A LOGO
			if (string.IsNullOrEmpty(logoURL)) {
				Console.WriteLine ("No logo, cancelling operation");
				return false;
			}

			Console.WriteLine ("Finished uploading media to AWS");

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

			Console.WriteLine ("Sending " + board.Name + " to server...");
			string result = PostJsonSync ("https://" + AppDelegate.APIAddress + "/api/board?authToken=" + AppDelegate.EncodedBoardToken, json);
			Console.WriteLine ("Sent!");

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

			string result = PutJsonSync ("https://" + AppDelegate.APIAddress + "/api/board/"+ board.Id +"?authToken=" + AppDelegate.EncodedBoardToken, json);

			if (result == "200" || result == string.Empty) {
				return true;
			}
			return false;
		}

		public static List<Board.Schema.Board> GetNearbyBoards(CLLocationCoordinate2D location, int meterRadius){
			string request = "https://" + AppDelegate.APIAddress + "/api/boards/nearby?" +
			                 "authToken=" + AppDelegate.EncodedBoardToken + "&latitude=" + 
				location.Latitude.ToString(CultureInfo.InvariantCulture) + "&longitude=" +
				location.Longitude.ToString(CultureInfo.InvariantCulture) + "&radiusInMeters=" + meterRadius;
			
			Console.WriteLine ("Getting nearby Boards... ");
			string result = GetJsonSync (request);

			Console.WriteLine ("Deserializing Boards...");
			var response = BoardResponse.Deserialize (result);

			Console.WriteLine ("Generating Board List...");
			var boards = GenerateBoardListFromBoardResponse (response);

			Console.WriteLine ("Done! - got " + boards.Count + " Boards");

			return boards;
		}

		public static List<Board.Schema.Board> GetAllBoards(){
			string result = GetJsonSync ("https://" + AppDelegate.APIAddress + "/api/boards?authToken=" + AppDelegate.EncodedBoardToken);

			BoardResponse response = BoardResponse.Deserialize (result);

			var boards = GenerateBoardListFromBoardResponse (response);

			return boards;
		}

		public static List<Board.Schema.Board> GetUserBoards(){
			
			string result = GetJsonSync ("https://" + AppDelegate.APIAddress + "/api/user/boards?authToken=" + AppDelegate.EncodedBoardToken);

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

			Schema.Board board = StorageController.BoardIsStored(datum.uuid);


			if (board == null) {
				Console.WriteLine (datum.name + " is not stored");

				board = new Schema.Board ();

				Console.WriteLine ("Getting location information from Google");
				string jsonobj = GetJsonSync ("https://maps.googleapis.com/maps/api/geocode/json?latlng=" +
					datum.latitude.ToString (CultureInfo.InvariantCulture) + "," + datum.longitude.ToString (CultureInfo.InvariantCulture) + "&key=" + AppDelegate.GoogleMapsAPIKey);

				Console.WriteLine ("Deserializing Google geolocation");
				geolocatorObject = JsonHandler.DeserializeObject (jsonobj);

				board.GeolocatorObject = geolocatorObject;
				board.Id = datum.uuid;

				StorageController.StoreBoard (board, jsonobj);
			} else {
				Console.WriteLine (datum.name + " is stored");
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
			string result = GetJsonSync("https://api.instagram.com/v1/locations/"+locationId+"/media/recent?access_token="+AppDelegate.InstagramServerToken);

			var instagramResponse = JsonConvert.DeserializeObject<InstagramMediaResponse> (result);

			if (instagramResponse != null) {
				Console.WriteLine ("stop");
			}

			return null;
		}

		public static UberProductResponse GetUberProducts(CLLocationCoordinate2D location){

			string result = GetJsonSync("https://api.uber.com/v1/products?latitude="+location.Latitude.ToString(CultureInfo.InvariantCulture)
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

		private static async System.Threading.Tasks.Task<string> PostJsonAsync(string uri, string json){
			string response;

			using (var httpClient = new HttpClient (new NativeMessageHandler ())) {
				httpClient.Timeout = new TimeSpan (0, 0, 8);
				var httpContent = new StringContent (json, Encoding.UTF8, "application/json");
				var httpResponse = await httpClient.PostAsync (uri, httpContent);
				response = await httpResponse.Content.ReadAsStringAsync();
			}
			return response;
		}

		private static async System.Threading.Tasks.Task<string> PutJsonAsync(string uri, string json = ""){
			string response;

			using (var httpClient = new HttpClient (new NativeMessageHandler ())) {
				httpClient.Timeout = new TimeSpan (0, 0, 8);
				var httpContent = new StringContent (json, Encoding.UTF8, "application/json");
				var httpResponse = await httpClient.PutAsync (uri, httpContent);
				response = await httpResponse.Content.ReadAsStringAsync();
			}
			return response;
		}

		private static string PutJsonSync(string uri, string json){
			string response;

			using (var httpClient = new HttpClient (new NativeMessageHandler ())) {
				httpClient.Timeout = new TimeSpan (0, 0, 8);
				var httpContent = new StringContent (json, Encoding.UTF8, "application/json");
				var postTask = httpClient.PutAsync (uri, httpContent);
				var result = postTask.Result;
				var responseTask = result.Content.ReadAsStringAsync();
				response = responseTask.Result;
			}
			return response;
		}

		private static string PostJsonSync(string uri, string json){
			string response;

			using (var httpClient = new HttpClient (new NativeMessageHandler ())) {
				//httpClient.Timeout = new TimeSpan (0, 0, 8);
				var httpContent = new StringContent (json, Encoding.UTF8, "application/json");
				var postTask = httpClient.PostAsync (uri, httpContent);
				var result = postTask.Result;
				var responseTask = result.Content.ReadAsStringAsync();
				response = responseTask.Result;
			}
			return response;
		}

		private static string GetJsonSync(string uri){
			string response = string.Empty;

			using (var httpClient = new HttpClient (new NativeMessageHandler ())) {
				//httpClient.Timeout = new TimeSpan (0, 0, 8);
				var postTask = httpClient.GetAsync (uri);

				try{
					
					var result = postTask.Result;
					var responseTask = result.Content.ReadAsStringAsync();
					response = responseTask.Result;

				} catch (Exception e) {

					Console.WriteLine(e.Message);
					Console.WriteLine(e.InnerException.Message);

				}

			}
			return response;
		}

		private static string DeleteJsonSync(string uri){
			string response;

			using (var httpClient = new HttpClient (new NativeMessageHandler ())) {
				httpClient.Timeout = new TimeSpan (0, 0, 8);
				var postTask = httpClient.DeleteAsync (uri);
				var result = postTask.Result;
				var responseTask = result.Content.ReadAsStringAsync();
				response = responseTask.Result;
			}
			return response;
		}

		private static string UploadStream(string uri, Stream data, string mimeType)
		{
			string response = string.Empty;

			using (var httpClient = new HttpClient (new NativeMessageHandler ())) {
				httpClient.Timeout = new TimeSpan (0, 1, 0);

				var httpContent = new StreamContent (data);
				httpContent.Headers.ContentType = new MediaTypeHeaderValue (mimeType);
				var postTask = httpClient.PutAsync (uri, httpContent);


				try{
					
					var result = postTask.Result;

					var absoluteURL = result.RequestMessage.RequestUri.AbsoluteUri;
					var indexOfParameter = absoluteURL.IndexOf ('?');

					if (indexOfParameter != -1) {
						absoluteURL = absoluteURL.Substring (0, indexOfParameter);
					}

					response = absoluteURL;

				} catch(Exception e) {
					
					Console.WriteLine(e.Message);
					Console.WriteLine(e.InnerException.Message);

				}

			}
			return response;
		}

	}
}

