using System;
using Clubby.Schema;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using CoreGraphics;
using System.Globalization;
using System.Threading.Tasks;
using CoreLocation;
using Foundation;
using UIKit;

namespace Clubby.Utilities
{
	public class TypeSwitch
	{
		Dictionary<Type, Action<object>> matches = new Dictionary<Type, Action<object>>();
		public TypeSwitch Case<T>(Action<T> action) { matches.Add(typeof(T), (x) => action((T)x)); return this; } 
		public void Switch(object x) { matches[x.GetType()](x); }
	}

	public static class CommonUtils
	{
		public static List<int> AllIndexesOf(string str, string value) {
			if (String.IsNullOrEmpty(value))
				throw new ArgumentException("the string to find may not be empty", "value");
			List<int> indexes = new List<int>();
			for (int index = 0;; index += value.Length) {
				index = str.IndexOf(value, index);
				if (index == -1)
					return indexes;
				indexes.Add(index);
			}
		}

		public static string FirstLetterOfEveryWordToUpper (string s) { 
			return Regex.Replace (s, @"(^\w)|(\s\w)", m => m.Value.ToUpper ());
		}

		public static string LimitStringToWidth (string textToLimit, UIFont forFont, float toWidth)
		{
			string limitedText = textToLimit;

			int textLength = textToLimit.Length;

			while (limitedText.StringSize (forFont).Width > toWidth) {

				limitedText = textToLimit.Substring (0, textLength) + "...";
				textLength--;

			}

			return limitedText;
		}

		public static bool IsStringAllUpper (string input)
		{
			for (int i = 0; i < input.Length; i++) {
				if (Char.IsLetter (input [i]) && !Char.IsUpper (input [i]))
					return false;
			}
			return true;
		}

		public static string GetFormattedTimeDifference(DateTime creationTime){
			string result = string.Empty;

			var timeDifference = DateTime.Now.Subtract (creationTime);

			if (timeDifference.TotalSeconds < 60) {
				result = timeDifference.Seconds + "s";
			} else if (timeDifference.TotalMinutes < 60) {
				result = timeDifference.Minutes + "m";
			} else if (timeDifference.TotalHours < 24) {
				result = timeDifference.Hours + "h";
			} else if (timeDifference.TotalDays < 7) {
				result = timeDifference.Days + "d";
			} else {
				result = (timeDifference.Days/7) + "w";
			}

			return result;
		}

		public static string GetFormattedDistance(double distance){
			string distanceString, distanceTotalString;

			if (distance < .18) {

				distance *= 5280;
				distanceString = ((int)distance).ToString ();
				distanceTotalString = distanceString + " feet away";

				return distanceTotalString;

			} else {

				string farAway;
				if (distance != 1) {
					farAway = " miles away";
				} else {
					farAway = " mile away";
				}

				distanceString = distance.ToString ("F1", CultureInfo.InvariantCulture);
				if (distanceString.EndsWith (".0")) {
					distanceString = distanceString.Substring (0, distanceString.Length - 2);
				}

				distanceTotalString = distanceString + farAway;

				return distanceTotalString;
			}
		}

		public static double GetDistanceFromUserToBoard(Venue board){
			var location = AppDelegate.UserLocation;
			double distance = 0;

			if (location.IsValid ()) {
				distance = DistanceBetweenCoordinates (board.GeolocatorObject.Coordinate, location, 'M');
				board.Distance = distance;
			}
			return distance;
		}

		public static int CountStringOccurrences(string text, string pattern)
		{
			// Loop through all instances of the string 'text'.
			int count = 0;
			int i = 0;
			while ((i = text.IndexOf(pattern, i)) != -1)
			{
				i += pattern.Length;
				count++;
			}
			return count;
		}

		public static double DistanceBetweenCoordinates(CLLocationCoordinate2D location1, CLLocationCoordinate2D location2, char unit = 'K')
		{
			double rlat1 = Math.PI*location1.Latitude/180;
			double rlat2 = Math.PI*location2.Latitude/180;
			double theta = location1.Longitude - location2.Longitude;
			double rtheta = Math.PI*theta/180;
			double dist =
				Math.Sin(rlat1)*Math.Sin(rlat2) + Math.Cos(rlat1)*
				Math.Cos(rlat2)*Math.Cos(rtheta);
			dist = Math.Acos(dist);
			dist = dist*180/Math.PI;
			dist = dist*60*1.1515;

			switch (unit)
			{
			case 'K': //Kilometers -> default
				return dist*1.609344;
			case 'N': //Nautical Miles 
				return dist*0.8684;
			case 'M': //Miles
				return dist;
			}

			return dist;
		}

		public static DateTime UnixTimeStampToDateTime(int unixTimeStamp) {
			var dtDateTime = new DateTime(1970,1,1,0,0,0,0,DateTimeKind.Utc);
			return dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
		}


		public static Int32 GetUnixTimeStamp(){
			return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
		}

		public static Int32 GetUnixTimeStamp(DateTime time){
			return (Int32)(time.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
		}

		public static async Task<UIImage> DownloadUIImageFromURL(string webAddress)
		{
			if (webAddress == null) {
				return null;
			}

			var webClient = new WebClient ();
			var uri = new Uri (webAddress);

			try
			{
				var bytes = await webClient.DownloadDataTaskAsync (uri);
				var image = GetImagefromByteArray (bytes);
				return image;
			}catch (Exception ex){
				Console.WriteLine (ex.Message);
				return null;
			}
		}

		public static async Task<byte[]> DownloadByteArrayFromURL(string webAddress)
		{
			var webClient = new WebClient ();
			var uri = new Uri (webAddress);
			byte[] bytes = null;
			try
			{
				bytes = await webClient.DownloadDataTaskAsync(uri);
				return bytes;
			}catch{
				return null;
			}
		}

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

		public static UIImage RotateImage(UIImage imageIn, UIImageOrientation orIn) {
			int kMaxResolution = 2048;

			CGImage imgRef = imageIn.CGImage;
			float width = imgRef.Width;
			float height = imgRef.Height;
			CGAffineTransform transform = CGAffineTransform.MakeIdentity ();
			CGRect bounds = new CGRect( 0, 0, width, height );

			if ( width > kMaxResolution || height > kMaxResolution )
			{
				float ratio = width/height;

				if (ratio > 1)
				{
					bounds.Width  = kMaxResolution;
					bounds.Height = bounds.Width / ratio;
				}
				else
				{
					bounds.Height = kMaxResolution;
					bounds.Width  = bounds.Height * ratio;
				}
			}

			float scaleRatio = (float)bounds.Width / width;
			CGSize imageSize = new CGSize( width, height);
			UIImageOrientation orient = orIn;
			float boundHeight;

			switch(orient)
			{
			case UIImageOrientation.Up:                                        //EXIF = 1
				transform = CGAffineTransform.MakeIdentity();
				break;

			case UIImageOrientation.UpMirrored:                                //EXIF = 2
				transform = CGAffineTransform.MakeTranslation (imageSize.Width, 0f);
				transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
				break;

			case UIImageOrientation.Down:                                      //EXIF = 3
				transform = CGAffineTransform.MakeTranslation (imageSize.Width, imageSize.Height);
				transform = CGAffineTransform.Rotate(transform, (float)Math.PI);
				break;

			case UIImageOrientation.DownMirrored:                              //EXIF = 4
				transform = CGAffineTransform.MakeTranslation (0f, imageSize.Height);
				transform = CGAffineTransform.MakeScale(1.0f, -1.0f);
				break;

			case UIImageOrientation.LeftMirrored:                              //EXIF = 5
				boundHeight = (float)bounds.Height;
				bounds.Height = bounds.Width;
				bounds.Width = boundHeight;
				transform = CGAffineTransform.MakeTranslation (imageSize.Height, imageSize.Width);
				transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
				transform = CGAffineTransform.Rotate(transform, 3.0f * (float)Math.PI/ 2.0f);
				break;

			case UIImageOrientation.Left:                                      //EXIF = 6
				boundHeight = (float)bounds.Height;
				bounds.Height = bounds.Width;
				bounds.Width = boundHeight;
				transform = CGAffineTransform.MakeTranslation (0.0f, imageSize.Width);
				transform = CGAffineTransform.Rotate(transform, 3.0f * (float)Math.PI / 2.0f);
				break;

			case UIImageOrientation.RightMirrored:                             //EXIF = 7
				boundHeight = (float)bounds.Height;
				bounds.Height = bounds.Width;
				bounds.Width = boundHeight;
				transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
				transform = CGAffineTransform.Rotate(transform, (float)Math.PI / 2.0f);
				break;

			case UIImageOrientation.Right:                                     //EXIF = 8
				boundHeight = (float)bounds.Height;
				bounds.Height = bounds.Width;
				bounds.Width = boundHeight;
				transform = CGAffineTransform.MakeTranslation(imageSize.Height, 0.0f);
				transform = CGAffineTransform.Rotate(transform, (float)Math.PI  / 2.0f);
				break;

			default:
				throw new Exception("Invalid image orientation");
			}

			UIGraphics.BeginImageContext (bounds.Size);//, false, 2);

			CGContext context = UIGraphics.GetCurrentContext ();

			if ( orient == UIImageOrientation.Right || orient == UIImageOrientation.Left )
			{
				context.ScaleCTM(-scaleRatio, scaleRatio);
				context.TranslateCTM(-height, 0);
			}
			else
			{
				context.ScaleCTM(scaleRatio, -scaleRatio);
				context.TranslateCTM(0, -height);
			}

			context.ConcatCTM(transform);
			context.DrawImage (new CGRect (0, 0, width, height), imgRef);

			UIImage imageCopy = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();

			return imageCopy;
		}
	}
}

