using System.Collections.Generic;
using Newtonsoft.Json;
using Foundation;

namespace Clubby.JsonResponses
{
	[Preserve(AllMembers = true)]
	public class PriceDetails
	{
		public List<object> service_fees { get; set; }
		public double cost_per_minute { get; set; }
		public string distance_unit { get; set; }
		public double minimum { get; set; }
		public double cost_per_distance { get; set; }
		public double @base { get; set; }
		public double cancellation_fee { get; set; }
		public string currency_code { get; set; }
	}

	[Preserve(AllMembers = true)]
	public class Product
	{
		public int capacity { get; set; }
		public string product_id { get; set; }
		public PriceDetails price_details { get; set; }
		public string image { get; set; }
		public string short_description { get; set; }
		public string display_name { get; set; }
		public string description { get; set; }
	}

	[Preserve(AllMembers = true)]
	public class UberProductResponse
	{
		public List<Product> products { get; set; }

		[JsonConstructor]
		public UberProductResponse(){}
	}
}

