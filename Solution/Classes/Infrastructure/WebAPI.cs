using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using ModernHttpClient;

namespace Board.Infrastructure
{
	public static class WebAPI
	{
		public static async System.Threading.Tasks.Task<string> GetJsonAsync(string uri, CancellationToken ct){
			string response;

			using (var httpClient = new HttpClient (new NativeMessageHandler ())) {
				var getTask = await httpClient.GetAsync (uri, ct);

				if (!getTask.IsSuccessStatusCode) {
					return ((int)getTask.StatusCode).ToString ();
				}

				response = await getTask.Content.ReadAsStringAsync();
			}

			return response;
		}

		public static async System.Threading.Tasks.Task<string> GetJsonAsync(string uri){
			string response;

			using (var httpClient = new HttpClient (new NativeMessageHandler ())) {
				var getTask = await httpClient.GetAsync (uri);

				if (!getTask.IsSuccessStatusCode) {
					return ((int)getTask.StatusCode).ToString ();
				}

				response = await getTask.Content.ReadAsStringAsync();
			}

			return response;
		}

		public static async System.Threading.Tasks.Task<string> PostJsonAsync(string uri, string json){
			string response;

			using (var httpClient = new HttpClient (new NativeMessageHandler ())) {
				httpClient.Timeout = new TimeSpan (0, 0, 8);
				var httpContent = new StringContent (json, Encoding.UTF8, "application/json");
				var httpResponse = await httpClient.PostAsync (uri, httpContent);
				response = await httpResponse.Content.ReadAsStringAsync();
			}
			return response;
		}

		public static async System.Threading.Tasks.Task<string> PutJsonAsync(string uri, string json = ""){
			string response;

			using (var httpClient = new HttpClient (new NativeMessageHandler ())) {
				httpClient.Timeout = new TimeSpan (0, 0, 8);
				var httpContent = new StringContent (json, Encoding.UTF8, "application/json");
				var httpResponse = await httpClient.PutAsync (uri, httpContent);
				response = await httpResponse.Content.ReadAsStringAsync();
			}

			return response;
		}

		public static string PutJsonSync(string uri, string json){
			string response;

			using (var httpClient = new HttpClient (new NativeMessageHandler ())) {
				httpClient.Timeout = new TimeSpan (0, 0, 8);
				var httpContent = new StringContent (json, Encoding.UTF8, "application/json");
				var postTask = httpClient.PutAsync (uri, httpContent);
				var result = postTask.Result;
				var responseTask = result.Content.ReadAsStringAsync();
				response = responseTask.Result;
			}
			return response;
		}

		class WebAPIResponse{
			int StatusCode;
			string Response;
		}

		public static string PostJsonSync(string uri, string json){
			string response;

			using (var httpClient = new HttpClient (new NativeMessageHandler ())) {
				var httpContent = new StringContent (json, Encoding.UTF8, "application/json");
				var postTask = httpClient.PostAsync (uri, httpContent);
				var result = postTask.Result;
				if (!result.IsSuccessStatusCode) {
					return ((int)result.StatusCode).ToString ();
				}
				var responseTask = result.Content.ReadAsStringAsync();
				response = responseTask.Result;
			}
			return response;
		}

		public static string GetJsonSync(string uri){
			string response = null;

			using (var httpClient = new HttpClient (new NativeMessageHandler ())) {
				var getTask = httpClient.GetAsync (uri);

				try{

					var result = getTask.Result;
					if (!result.IsSuccessStatusCode) {
						return ((int)result.StatusCode).ToString ();
					}
					var responseTask = result.Content.ReadAsStringAsync();
					response = responseTask.Result;

				} catch (Exception e) {

					Console.WriteLine(e.Message);
					Console.WriteLine(e.InnerException.Message);

				}

			}
			return response;
		}

		public static string DeleteJsonSync(string uri){
			string response;

			using (var httpClient = new HttpClient (new NativeMessageHandler ())) {
				httpClient.Timeout = new TimeSpan (0, 0, 8);
				var postTask = httpClient.DeleteAsync (uri);
				var result = postTask.Result;
				var responseTask = result.Content.ReadAsStringAsync();
				response = responseTask.Result;
			}
			return response;
		}

		public static string UploadStream(string uri, Stream data, string mimeType)
		{
			string response = null;

			using (var httpClient = new HttpClient (new NativeMessageHandler ())) {
				httpClient.Timeout = new TimeSpan (0, 1, 0);

				var httpContent = new StreamContent (data);
				httpContent.Headers.ContentType = new MediaTypeHeaderValue (mimeType);
				var postTask = httpClient.PutAsync (uri, httpContent);


				try{

					var result = postTask.Result;

					var absoluteURL = result.RequestMessage.RequestUri.AbsoluteUri;
					var indexOfParameter = absoluteURL.IndexOf ('?');

					if (indexOfParameter != -1) {
						absoluteURL = absoluteURL.Substring (0, indexOfParameter);
					}

					response = absoluteURL;

				} catch(Exception e) {

					Console.WriteLine(e.Message);
					Console.WriteLine(e.InnerException.Message);

				}

			}
			return response;
		}
	}
}

