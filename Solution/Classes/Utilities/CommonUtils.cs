using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

using CoreGraphics;

using Foundation;
using UIKit;

namespace Board.Utilities
{
	public static class CommonUtils
	{
		public static List<string> NSObjectToString(string fetch, NSObject obj)
		{
			NSString nsString = new NSString (fetch);

			NSArray array = (NSArray)obj.ValueForKeyPath (nsString);
			List<string> list = new List<string> ();

			for (int i = 0; i < (int)array.Count; i++) {
				var item = array.GetItem<NSObject> ((nuint)i);
				list.Add(item.ToString());
			}

			return list;
		}

		// instead of nsdictionary, UIStringAttributes con su .Dictionary como el NSDictionary
		public static Dictionary<NSRange, NSDictionary> GetFormatDictionaries(NSAttributedString attributedString)
		{
			// seteo un rango inicial
			NSRange range = new NSRange (0, attributedString.Length);

			// armo el diccionario que va a contener a los diccionarios de formato y a los rangos de c/u
			Dictionary<NSRange, NSDictionary> dictionaries = new Dictionary<NSRange, NSDictionary> ();

			// recorro al string
			int i = 0;
			while (i < attributedString.Length) {
				// obtengo los atributos, y de out, el rango de ese set de atributos
				NSDictionary attributeDictionary = attributedString.GetAttributes (i, out range);

				// agrego al diccionario de atributos a mi diccionario total
				dictionaries.Add (range, attributeDictionary);

				// adelanto al i asi no tengo que iterar por todo el string, voy directo al final de string o al siguiente diccionario
				i = (int)(range.Location + range.Length);
			}

			return dictionaries;
		}

		public static NSAttributedString GenerateAttributedString(string text, Dictionary<NSRange, NSDictionary> dictionaries)
		{
			NSMutableAttributedString attstring = new NSMutableAttributedString (text);

			// le pongo todos los diccionarios de formato
			foreach (KeyValuePair<NSRange, NSDictionary> dic in dictionaries) {
				attstring.SetAttributes (dic.Value, dic.Key);
			}

			return attstring;
		}

		public static UIImage ResizeImage(UIImage sourceImage, CGSize newSize)
		{
			UIGraphics.BeginImageContext(newSize);
			sourceImage.Draw(new CGRect(0, 0, newSize.Width, newSize.Height));
			var resultImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return resultImage;
		}

		public static UIColor HexToUIColor(string hex)
		{
			if (hex.Length != 6) {
				return UIColor.White;
			}

			try {
				UIColor color = UIColor.FromRGB (
					int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
					int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
					int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));

				return color;
			}
			catch{
				return UIColor.White;
			}
		}

		public static string UIColorToHex(UIColor color)
		{
			nfloat red; nfloat green; nfloat blue; nfloat alpha; 
			color.GetRGBA (out red, out green, out blue, out alpha);

			int ired = (int)(red * 255);
			int igreen = (int)(green * 255);
			int iblue = (int)(blue * 255);

			string hex = ired.ToString("x2") + igreen.ToString("x2") + iblue.ToString("x2");
			return hex;
		}

		public static string JsonGETRequest(string url)
		{
			var httpWebRequest = (HttpWebRequest)WebRequest.Create (url);
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "GET";
			httpWebRequest.Timeout = 8000;

			try{
				var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse ();
				string result = string.Empty;
				using (var streamReader = new StreamReader (httpResponse.GetResponseStream ())) {
					result = streamReader.ReadToEnd ();
				}
				return result;
			} catch (WebException e) {

				if (e.Status == WebExceptionStatus.ProtocolError) 
				{
					return ((HttpWebResponse)e.Response).StatusCode.ToString();
				}

				return e.Status.ToString();

			}
		}

		public static string JsonPOSTRequest(string url, string json)
		{
			var httpWebRequest = (HttpWebRequest)WebRequest.Create (url);
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "POST";
			httpWebRequest.Timeout = 8000;
				
			try{
				using (var streamWriter = new StreamWriter (httpWebRequest.GetRequestStream ())) {
					streamWriter.Write (json);
					streamWriter.Flush ();
					streamWriter.Close ();
				}

				var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse ();
				string result = string.Empty;
				using (var streamReader = new StreamReader (httpResponse.GetResponseStream ())) {
					result = streamReader.ReadToEnd ();
				}
				return result;
			} catch (WebException e) {
				
				if (e.Status == WebExceptionStatus.ProtocolError) 
				{
					return ((HttpWebResponse)e.Response).StatusCode.ToString();
				}

				return e.Status.ToString();

			}
		}

		public static UIImage GetImagefromByteArray (byte[] imageBuffer)
		{
			NSData imageData = NSData.FromArray(imageBuffer);
			UIImage image = UIImage.LoadFromData (imageData);
			return image;
		}

		public static string GenerateGuid()
		{
			Guid guid = Guid.NewGuid ();
			return guid.ToString ();
		}
	}
}

