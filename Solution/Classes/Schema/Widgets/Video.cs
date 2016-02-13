using System;
using Newtonsoft.Json;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;

namespace Solution
{
	public class Video : Content
	{
		public string Id { get; set; }

		public string Url { get; set; }

		public UIImage Thumbnail { get; set; }

		public float Rotation { get; set; }

		public CGRect GetRectangleF()
		{
			return new CGRect (ImgX, ImgY, ImgW, ImgH);
		}

		public Video() {}

		public Video(string url, UIImage thumbnail, float rotation, CGRect position, string userid)
		{
			Url = url;
			Thumbnail = thumbnail;
			Rotation = rotation;
			ImgX = (float)(position.X); ImgY = (float)(position.Y);
			ImgH = (float)(position.Height); ImgW = (float)(position.Width);
			UserId = userid;
		}
	}
}

