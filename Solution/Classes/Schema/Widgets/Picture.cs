using System;
using Newtonsoft.Json;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;

namespace Board.Schema
{
	public class Picture : Content
	{
		public string Id { get; set; }

		public UIImage Image { get; set; }

		public UIImage Thumbnail { get; set; }

		public float Rotation { get; set; }

		public CGRect GetRectangleF()
		{
			return new CGRect (ImgX, ImgY, ImgW, ImgH);
		}

		public Picture() {}

		public Picture(UIImage image, UIImage thumbnail, float rotation, CGRect position, string userid)
		{
			Image = image;
			Thumbnail = thumbnail;
			Rotation = rotation;
			ImgX = (float)(position.X); ImgY = (float)(position.Y);
			ImgH = (float)(position.Height); ImgW = (float)(position.Width);
			UserId = userid;
		}
	}
}

