using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using CoreGraphics;
using System.Threading.Tasks;
using CoreLocation;
using Foundation;
using UIKit;

namespace Board.Utilities
{
	public static class CommonUtils
	{
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

		public static Int32 GetUnixTimeStamp(){
			return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
		}

		public static async Task<UIImage> DownloadUIImageFromURL(string webAddress)
		{
			var webClient = new WebClient ();
			var uri = new Uri (webAddress);
			byte[] bytes = null;
			try
			{
				bytes = await webClient.DownloadDataTaskAsync(uri);
				return CommonUtils.GetImagefromByteArray(bytes);
			}catch{
				return new UIImage ();
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

		public static UIImage ScaleAndRotateImage(UIImage imageIn, UIImageOrientation orIn) {
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

