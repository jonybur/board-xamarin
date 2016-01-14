using System;
using System.Drawing;
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
	}
}

