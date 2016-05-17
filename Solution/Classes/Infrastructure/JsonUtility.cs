using System.Collections.Generic;
using Board.Utilities;
using Newtonsoft.Json;
using System;
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

		private static Dictionary<string, T> GenerateIdDictionary<T>(List<Content> listContent) where T : Content {
			var returnDictionary = new Dictionary<string, T> ();
			foreach (var content in listContent) {
				var TValue = content as T;
				if (TValue != null) {
					returnDictionary.Add (TValue.Id, TValue);
				}
			}
			return returnDictionary;
		}

		// figure this out
		private static void FillTypeHeader<T>(List<Content> listContent, ref Dictionary<string, object> typeHeader, string contentType) where T:Content{
			var contentDictionary = GenerateIdDictionary<T> (listContent);
			if (contentDictionary.Count > 0) {
				typeHeader.Add (contentType, contentDictionary);
			}
		}

		public static string GenerateUpdateJson(List<Content> listContent){
			
			var typeHeader = new Dictionary<string, object> ();

			FillTypeHeader<Announcement> (listContent, ref typeHeader, Announcement.Type);
			FillTypeHeader<BoardEvent> (listContent, ref typeHeader, BoardEvent.Type);
			FillTypeHeader<Poll> (listContent, ref typeHeader, Poll.Type);
			FillTypeHeader<Picture> (listContent, ref typeHeader, Picture.Type);
			FillTypeHeader<Video> (listContent, ref typeHeader, Video.Type);

			return AddTypeHeaderToFinalJson (typeHeader);;
		}

		// recibe un content, arma json de update
		public static string GenerateUpdateJson(Content content)
		{
			// ADDS ALL SAME-CONTENT TYPE ITEMS TO IDHEADER

			var idHeader = new Dictionary<string, Content> ();

			idHeader.Add (content.Id, content);

			// ADDS ALL IDHEADERS TO DICTIONARY THAT SAYS WHAT TYPE THEY ARE

			var typeHeader = new Dictionary<string, object> ();

			typeHeader.Add (GetContentType(content), idHeader);

			// ADDS ALL DICTIONARIES TO FINALJSON -EASY

			return AddTypeHeaderToFinalJson (typeHeader);
		}

		private static string GetContentType(Content content){
			string contentType;
			if (content is Announcement) {
				contentType = Announcement.Type;
			} else if (content is BoardEvent) {
				contentType = BoardEvent.Type;
			} else if (content is Poll) {
				contentType = Poll.Type;
			} else if (content is Picture) {
				contentType = Picture.Type;
			} else if (content is Video) {
				contentType = Video.Type;
			} else if (content is Sticker) {
				contentType = Sticker.Type;
			} else {
				contentType = string.Empty;
			}
			return contentType;
		}

		private static string AddTypeHeaderToFinalJson(Dictionary<string, object> typeHeader){
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

		public static string GenerateDeleteJson(string contentType, string contentId)
		{
			var InternalJson = new string[1];
			InternalJson[0] = contentType+"."+contentId;

			var FinalJson = new Dictionary<string, object> ();

			FinalJson.Add ("deletes", InternalJson);

			FinalJson.Add ("timestamp", CommonUtils.GetUnixTimeStamp ());

			return JsonConvert.SerializeObject (FinalJson);
		}

		public static string GenerateDeleteJson(params Content[] contents)
		{
			var InternalJson = new string[1];

			string contentType = GetContentType (contents [0]);

			InternalJson[0] = contentType+"."+contents[0].Id;

			var FinalJson = new Dictionary<string, object> ();

			FinalJson.Add ("deletes", InternalJson);

			FinalJson.Add ("timestamp", CommonUtils.GetUnixTimeStamp ());

			return JsonConvert.SerializeObject (FinalJson);
		}

	}
}

