using System;
using System.Collections.Generic;
using System.Globalization;
using Clubby.JsonResponses;
using Clubby.Facebook;
using Clubby.Schema;
using Clubby.Utilities;
using CoreLocation;
using Newtonsoft.Json;

namespace Clubby.Infrastructure
{
	// https://api.instagram.com/v1/locations/search?lat=-34.5885886&lng=-58.426003&access_token=2292871863.37fcdb1.cdc6ab03abfa4a8db4a2da022ec5d3c2

	public static class CloudController
	{

		public static void LogSession(){
			if (AppDelegate.HasLoggedSession){
				return;
			}

			string userName = string.Empty;
			if (AppDelegate.BoardUser == null) {
				userName = "EmailUser";
			} else {
				userName = AppDelegate.BoardUser.FirstName + " " + AppDelegate.BoardUser.LastName;
			}


			string json = "{\"latitude\": \"" + AppDelegate.UserLocation.Latitude.ToString(CultureInfo.InvariantCulture) + "\", " +
				"\"longitude\": \"" + AppDelegate.UserLocation.Longitude.ToString(CultureInfo.InvariantCulture) + "\", " + 
				"\"timestamp\": \"" + CommonUtils.GetUnixTimeStamp() + "\", " + 
				"\"name\": \"" + userName + "\" }";

			WebAPI.PostJsonAsync ("https://admin.boardack.com/log-user", json);
			AppDelegate.HasLoggedSession = true;
		}

		public static async System.Threading.Tasks.Task<List<Venue>> GetNearbyVenues(CLLocationCoordinate2D location, int meterRadius){
			var venueList = new List<Venue> ();

			venueList.Add (await CreateVenue("ClubSpace", "clubspacemiami"));
			venueList.Add (await CreateVenue("109273159180982", "tavernaopabrickell"));
			venueList.Add (await CreateVenue("woodtavern", "woodtavern"));
			venueList.Add (await CreateVenue("BROKENSHAKERMIAMI", "brokenshakemiami"));
			venueList.Add (await CreateVenue("323.Treehouse.Miami", "treehousemiami"));
			venueList.Add (await CreateVenue("LIVMiami", "livmiami"));
			venueList.Add (await CreateVenue("11Miami", "11miami"));
			venueList.Add (await CreateVenue("iconsobe", "icon.miami"));
			venueList.Add (await CreateVenue("SHOTSMiami", "shotsmiami"));
			venueList.Add (await CreateVenue("grampsmiami", "grampswynwood"));
			venueList.Add (await CreateVenue("Brickmia", "brickmia"));
			venueList.Add (await CreateVenue("electricpicklemiami", "picklemiami"));
			venueList.Add (await CreateVenue("nikkibeach", "nikkibeachmiami"));
			venueList.Add (await CreateVenue("blackbirdordinary", "blackbirdordinary"));
			venueList.Add (await CreateVenue("heartnightclub", "heartnightclub"));
			venueList.Add (await CreateVenue("storymiami", "storymiami"));
			venueList.Add (await CreateVenue("mokaimiami", "mokaiofficial"));
			venueList.Add (await CreateVenue("bardotmiami", "bardot_miami"));
			venueList.Add (await CreateVenue("BasementMiami", "basementmiami"));
			venueList.Add (await CreateVenue("setmiami", "setmiami"));
			venueList.Add (await CreateVenue("studio23miami", "studio23miami"));
			venueList.Add (await CreateVenue("coyotacowynwood", "coyotaco"));
			venueList.Add (await CreateVenue("MyntLoungeUSA", "myntloungeusa"));
			venueList.Add (await CreateVenue("SpazioNightclub", "blumenightclub"));
			venueList.Add (await CreateVenue("fdrsobe", "fdratdelano"));
			venueList.Add (await CreateVenue("RecRoomies", "RECROOMOFFICIAL"));
			venueList.Add (await CreateVenue("334543100240", "purdylounge"));
			venueList.Add (await CreateVenue("killyouridolmiami", "kyimiami"));

			return venueList;
		}


		private static async System.Threading.Tasks.Task<Venue> CreateVenue(string fbID, string instaID)
		{	
			Console.WriteLine("- Starts creating " +fbID);
			var newVenue = new Venue (fbID, instaID);
			await newVenue.Initialize ();
			return newVenue;
		}


		public static async System.Threading.Tasks.Task<string> AsyncGraphAPIRequest(string pageId, string element){

			string token = FacebookUtils.GetFacebookAccessToken ();
			string url = "https://graph.facebook.com/v2.6/" + pageId + "/" + element + "&access_token=" + token + "&format=json&include_headers=false&sdk=ios";
			string jsonobj = await WebAPI.GetJsonAsync (url);

			return jsonobj;
		}

		public static async System.Threading.Tasks.Task<GoogleGeolocatorObject> LoadGeolocatorObject(CLLocationCoordinate2D location){
			string url = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" +
			             location.Latitude.ToString (CultureInfo.InvariantCulture) + "," + location.Longitude.ToString (CultureInfo.InvariantCulture) + "&key=" + AppDelegate.GoogleMapsAPIKey;
			string jsonobj = await WebAPI.GetJsonAsync (url);
			
			return JsonHandler.DeserializeObject (jsonobj);
		}

		public static async System.Threading.Tasks.Task<InstagramPageResponse> GetInstagramPage(string instagramId){
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
			AppDelegate.BoardUser = null;
		}


	}
}

