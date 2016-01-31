using System;
using System.Drawing;
using System.IO;
using System.Net;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;

using System.Net.Http;

using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Facebook.CoreKit;
using Facebook.LoginKit;

namespace Solution
{
	public static class CommonUtils
	{
		public static UIImage ResizeImageView(UIImage imageToResize, CGSize desiredSize)
		{
			float scale = (float)(desiredSize.Width / imageToResize.Size.Width) + .1f ;

			UIImageView imageView = new UIImageView (imageToResize);
			UIGraphics.BeginImageContextWithOptions (imageView.Frame.Size, false, scale);
			imageView.DrawRect (new CGRect (0, 0, imageView.Frame.Width, imageView.Frame.Height), new UIViewPrintFormatter());
			UIImage image = UIGraphics.GetImageFromCurrentImageContext ();
			return image;
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

		public static void JsonRequest(string url, string json)
		{
			var httpWebRequest = (HttpWebRequest)WebRequest.Create (url);
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "POST";

			using (var streamWriter = new StreamWriter (httpWebRequest.GetRequestStream ())) {
				streamWriter.Write (json);
				streamWriter.Flush ();
				streamWriter.Close ();
			}

			var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse ();
			using (var streamReader = new StreamReader (httpResponse.GetResponseStream ())) {
				var result = streamReader.ReadToEnd ();
			}
		}


	}
}

