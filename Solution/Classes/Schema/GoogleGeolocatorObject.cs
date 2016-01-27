using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Solution
{

	public class AddressComponent
	{
		public string long_name { get; set; }
		public string short_name { get; set; }
		public List<string> types { get; set; }
	}

	public class Location
	{
		public double lat { get; set; }
		public double lng { get; set; }
	}

	public class Northeast
	{
		public double lat { get; set; }
		public double lng { get; set; }
	}

	public class Southwest
	{
		public double lat { get; set; }
		public double lng { get; set; }
	}

	public class Viewport
	{
		public Northeast northeast { get; set; }
		public Southwest southwest { get; set; }
	}

	public class Northeast2
	{
		public double lat { get; set; }
		public double lng { get; set; }
	}

	public class Southwest2
	{
		public double lat { get; set; }
		public double lng { get; set; }
	}

	public class Bounds
	{
		public Northeast2 northeast { get; set; }
		public Southwest2 southwest { get; set; }
	}

	public class Geometry
	{
		public Location location { get; set; }
		public string location_type { get; set; }
		public Viewport viewport { get; set; }
		public Bounds bounds { get; set; }
	}

	public class Result
	{
		public List<AddressComponent> address_components { get; set; }
		public string formatted_address { get; set; }
		public Geometry geometry { get; set; }
		public string place_id { get; set; }
		public List<string> types { get; set; }
	}

	public class GoogleGeolocatorObject
	{
		public List<Result> results { get; set; }
		public string status { get; set; }
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
				WebResponse errorResponse = ex.Response;
				using (Stream responseStream = errorResponse.GetResponseStream())
				{
					StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
					String errorText = reader.ReadToEnd();
				}
				throw;
			}
		}
	}

}

