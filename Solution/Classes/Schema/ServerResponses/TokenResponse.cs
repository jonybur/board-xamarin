using System;
using Newtonsoft.Json;

namespace Board.JsonResponses
{
	public class TokenResponse
	{
		public string authToken { get; set; }

		public static TokenResponse Deserialize (string json)
		{
			try {
				return JsonConvert.DeserializeObject<TokenResponse>(json);
			}catch{
				return null;
			}
		}


	}
}

