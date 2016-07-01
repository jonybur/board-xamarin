using System.Collections.Generic;
using Newtonsoft.Json;
using Foundation;

namespace Clubby.JsonResponses
{
	[Preserve(AllMembers = true)]
	public class AmazonS3TicketResponse
	{
		public string duration { get; set; }
		public string url { get; set; }
	}
}

