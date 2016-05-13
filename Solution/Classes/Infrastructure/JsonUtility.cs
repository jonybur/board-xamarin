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
			var FinalJson = new Dictionary<string, object> ();

			FinalJson.Add ("updates", dictionary); 

			FinalJson.Add ("timestamp", CommonUtils.GetUnixTimeStamp());

			return JsonConvert.SerializeObject (FinalJson);
		}

		// recibe un content, arma json de update
		public static string GenerateUpdateJson(Content content)
		{
			var idHeader = new Dictionary<string, Content> ();

			idHeader.Add (content.Id, content);

			var typeHeader = new Dictionary<string, object> ();

			typeHeader.Add (content.Type, idHeader);

			var FinalJson = new Dictionary<string, object> ();

			FinalJson.Add ("updates", typeHeader);

			FinalJson.Add ("timestamp", CommonUtils.GetUnixTimeStamp ());

			return JsonConvert.SerializeObject (FinalJson);
		}

		private static Dictionary<string, Content> GenerateContentDictionary(Content content){
			return new Dictionary<string, Content> ();
		}

		private static Dictionary<string, Content> GenerateContentDictionary(List<Content> contents){
			return new Dictionary<string, Content> ();
		}

		public static string GenerateDeleteJson(params string[] contentids)
		{
			var FinalJson = new Dictionary<string, object> ();

			FinalJson.Add ("deletes", contentids);

			FinalJson.Add ("timestamp", CommonUtils.GetUnixTimeStamp ());

			return JsonConvert.SerializeObject (FinalJson);
		}

	}
}

