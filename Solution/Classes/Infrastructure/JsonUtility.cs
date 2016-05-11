using System.Collections.Generic;
using Board.Utilities;
using Newtonsoft.Json;
using Board.Schema;

namespace Board.Infrastructure
{
	public static class JsonUtilty{

		// recibe un diccionario de contents, arma al json
		public static string GenerateUpdateJson(Dictionary<string, Content> dictionary)
		{
			Dictionary<string, object> FinalJson = new Dictionary<string, object> ();

			FinalJson.Add ("updates", dictionary);

			FinalJson.Add ("timestamp", CommonUtils.GetUnixTimeStamp());

			return JsonConvert.SerializeObject (FinalJson);
		}

		// recibe un content, arma json de update
		public static string GenerateUpdateJson(Content content)
		{
			Dictionary<string, Content> singleContent = new Dictionary<string, Content> ();
			singleContent.Add (content.Id, content);

			Dictionary<string, object> FinalJson = new Dictionary<string, object> ();

			FinalJson.Add ("updates", singleContent);

			FinalJson.Add ("timestamp", CommonUtils.GetUnixTimeStamp ());

			return JsonConvert.SerializeObject (FinalJson);
		}

		public static string GenerateDeleteJson(params string[] contentids)
		{
			Dictionary<string, object> FinalJson = new Dictionary<string, object> ();

			FinalJson.Add ("deletes", contentids);

			FinalJson.Add ("timestamp", CommonUtils.GetUnixTimeStamp ());

			return JsonConvert.SerializeObject (FinalJson);
		}

	}
}

