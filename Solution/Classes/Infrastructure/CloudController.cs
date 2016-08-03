using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Clubby.Facebook;
using Clubby.JsonResponses;
using Clubby.Schema;
using Clubby.Utilities;
using CoreLocation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Clubby.Infrastructure
{
	public static class CloudController
	{
		private static async Task<string> LoadName(){
			string json = await CloudController.AsyncGraphAPIRequest ("me", "?fields=name");

			if (string.IsNullOrEmpty (json)) {
				return string.Empty;
			}

			var jobject = JObject.Parse (json);

			return jobject ["name"].ToString();
		}

		public static async void LogSession(){
			if (AppDelegate.HasLoggedSession){
				return;
			}

			string userName = string.Empty;
			userName = await LoadName ();

			string json = "{\"latitude\": \"" + AppDelegate.UserLocation.Latitude.ToString(CultureInfo.InvariantCulture) + "\", " +
				"\"longitude\": \"" + AppDelegate.UserLocation.Longitude.ToString(CultureInfo.InvariantCulture) + "\", " + 
				"\"timestamp\": \"" + CommonUtils.GetUnixTimeStamp() + "\", " + 
				"\"name\": \"" + userName + "\" }";

			WebAPI.PostJsonAsync ("https://admin.boardack.com/log-user", json);
			AppDelegate.HasLoggedSession = true;
		}

		private static string TryGetJsonValue(JValue jvalue){
			try{
				return jvalue.ToString();
			}catch{
				return string.Empty;
			}
		}

		class FacebookPagesResponse{

			public class FetchedVenue
			{
				public string id { get; set; }
				public string facebookPage { get; set; }
				public string instagramPage { get; set; }
			}

			public class Value
			{
				public List<FetchedVenue> venues { get; set; }
			}

			public class Row
			{
				public string id { get; set; }
				public string key { get; set; }
				public Value value { get; set; }
			}

			public int total_rows { get; set; }
			public List<Row> rows { get; set; }
		}

		public static async Task<string> GetInstagramTimeline(){

			string instagramTimeline = await WebAPI.GetJsonAsync ("http://api.goclubby.com:8092/default/_design/instagram/_view/timeline?connection_timeout=60000&descending=true&inclusive_end=true&skip=0&stale=false&limit=200");

			return instagramTimeline;
		}

		public static async Task<List<Venue>> GetNearbyVenues(CLLocationCoordinate2D location, int meterRadius){
			var sw = new Stopwatch();
			sw.Start();

			var venueList = new List<Venue> ();


			Console.Write ("Getting facebook pages...");
			var facebookPagesJson = await WebAPI.GetJsonAsync ("http://api.goclubby.com:8092/pages/_design/pages/_view/pages?connection_timeout=60000");
			if (string.IsNullOrEmpty (facebookPagesJson)) {
				return new List<Venue> ();
			}

			var facebookPages = JsonConvert.DeserializeObject<FacebookPagesResponse> (facebookPagesJson);

			if (facebookPages.rows != null && facebookPages.rows.Count > 0) {
				var rows = facebookPages.rows [0];
				if (rows.value.venues != null && rows.value.venues.Count > 0) {
					foreach (var fetchedVenue in rows.value.venues) {
						var venue = new Venue(fetchedVenue.facebookPage, fetchedVenue.instagramPage, fetchedVenue.id);
						venueList.Add (venue);
					}
				}
			}
			Console.WriteLine (" done: {0}",sw.Elapsed);

			string userToken = FacebookUtils.GetFacebookAccessToken ();
			var graphApiClient = new GraphAPIClient (userToken);

			var facebookPagesToFetch = new List<string> ();
			var allFacebookPages = new Dictionary<string, dynamic> ();

			foreach (var venue in venueList) {
				// returns null or Dictionary w one key,value
				var fbpage = StorageController.GetFacebookPage (venue.FacebookId);

				if (fbpage == null) {
					// has to fetch this facebook page!
					facebookPagesToFetch.Add (venue.FacebookId);
				} else {
					// it is really only 1 page...
					foreach (var x in fbpage) {
						allFacebookPages.Add (x.Key, x.Value);
					}
				}
			}

			// gets facebook pages from da cloud
			if (facebookPagesToFetch.Count > 0) {
				Console.Write ("Fetching new facebook pages...");
				var fetchedPages = await graphApiClient.GetPages (facebookPagesToFetch);
				foreach (var page in fetchedPages) {
					// stores page
					StorageController.StoreFacebookPage (page.Key, page.Value.ToString ());
					allFacebookPages.Add (page.Key, page.Value);
				}
				Console.WriteLine (" done: {0}",sw.Elapsed);
			}

			Console.Write ("Loading pages...");
			foreach (var page in allFacebookPages) {
				var pageContent = page.Value;
				var facebookImportedPage = FacebookUtils.ReadFacebookResponse(pageContent);

				var venue = venueList.Find (x => x.FacebookId == page.Key);
				if (venue != null) {
					await venue.LoadFacebookDatum (facebookImportedPage);
				}
			}
			Console.WriteLine (" done: {0}", sw.Elapsed);

			sw.Stop();
			Console.WriteLine("Levantar venues de Facebook toma: {0}",sw.Elapsed);

			return venueList;
		}

		public static async Task<string> AsyncGraphAPIRequest(string pageId, string element){

			string token = FacebookUtils.GetFacebookAccessToken ();
			string url = "https://graph.facebook.com/v2.6/" + pageId + "/" + element + "&access_token=" + token + "&format=json&include_headers=false&sdk=ios";
			return await WebAPI.GetJsonAsync (url);;
		}

		public static async Task<string> GetGeolocatorJson(CLLocationCoordinate2D location){
			string url = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" +
			             location.Latitude.ToString (CultureInfo.InvariantCulture) + "," + location.Longitude.ToString (CultureInfo.InvariantCulture) + "&key=" + AppDelegate.GoogleMapsAPIKey;
			string jsonobj = await WebAPI.GetJsonAsync (url);
			
			return jsonobj;
		}

		public static async Task<InstagramPageResponse> GetInstagramPage(string instagramId){
			string result = await WebAPI.GetJsonAsync("https://www.instagram.com/"+instagramId+"/media/");

			return JsonConvert.DeserializeObject<InstagramPageResponse> (result);
		}

		public static UberProductResponse GetUberProducts(CLLocationCoordinate2D location){

			string result = WebAPI.GetJsonSync("https://api.uber.com/v1/products?latitude="+location.Latitude.ToString(CultureInfo.InvariantCulture)
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
		}


	}
}