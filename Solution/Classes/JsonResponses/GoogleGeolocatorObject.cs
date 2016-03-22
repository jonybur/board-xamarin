using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using Foundation;

namespace Board.JsonResponses
{
	[Preserve(AllMembers = true)]
	public class AddressComponent
	{
		public string long_name { get; set; }
		public string short_name { get; set; }
		public List<string> types { get; set; }
	}

	[Preserve(AllMembers = true)]
	public class Location
	{
		public double lat { get; set; }
		public double lng { get; set; }
	}

	[Preserve(AllMembers = true)]
	public class Northeast
	{
		public double lat { get; set; }
		public double lng { get; set; }
	}

	[Preserve(AllMembers = true)]
	public class Southwest
	{
		public double lat { get; set; }
		public double lng { get; set; }
	}

	[Preserve(AllMembers = true)]
	public class Viewport
	{
		public Northeast northeast { get; set; }
		public Southwest southwest { get; set; }
	}

	[Preserve(AllMembers = true)]
	public class Northeast2
	{
		public double lat { get; set; }
		public double lng { get; set; }
	}

	[Preserve(AllMembers = true)]
	public class Southwest2
	{
		public double lat { get; set; }
		public double lng { get; set; }
	}

	[Preserve(AllMembers = true)]
	public class Bounds
	{
		public Northeast2 northeast { get; set; }
		public Southwest2 southwest { get; set; }
	}

	[Preserve(AllMembers = true)]
	public class Geometry
	{
		public Location location { get; set; }
		public string location_type { get; set; }
		public Viewport viewport { get; set; }
		public Bounds bounds { get; set; }
	}

	[Preserve(AllMembers = true)]
	public class Result
	{
		public List<AddressComponent> address_components { get; set; }
		public string formatted_address { get; set; }
		public Geometry geometry { get; set; }
		public string place_id { get; set; }
		public List<string> types { get; set; }
	}

	[Preserve(AllMembers = true)]
	public class GoogleGeolocatorObject
	{
		public List<Result> results { get; set; }
		public string status { get; set; }

		[JsonConstructor]
		public GoogleGeolocatorObject(){}
	}

	public static class JsonHandler
	{
		public static GoogleGeolocatorObject DeserializeObject (string json)
		{
			return JsonConvert.DeserializeObject<GoogleGeolocatorObject>(json);
		}

		// Returns JSON string
		public static string GET(string url) 
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			try {
				WebResponse response = request.GetResponse();
				using (Stream responseStream = response.GetResponseStream()) {
					StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
					return reader.ReadToEnd();
				}
			}
			catch (WebException ex) {
				Console.WriteLine (ex);

				return null;
			}
		}
	}

}

