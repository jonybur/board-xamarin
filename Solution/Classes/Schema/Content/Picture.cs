using System;
using Newtonsoft.Json;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;

namespace Solution
{
	// The Picture class as it is uploaded on Azure
	// Board's way of storing its UIImageViews on the DB
	public class Picture : Content
	{
		[JsonProperty(PropertyName = "id")]
		public string Id { get; set; }

		// Image is a JPEG saved as an enconded Base64
		[JsonProperty(PropertyName = "image")]
		public string Image { get; set; }

		// Thumbnail is a JPEG saved as an enconded Base64, with a low resolution, for the main board
		[JsonProperty(PropertyName = "thumbnail")]
		public string Thumbnail { get; set; }

		[JsonProperty(PropertyName = "rotation")]
		public float Rotation { get; set; }

		public UIImage GetImage()
		{
			NSData image = new NSData(Image, NSDataBase64DecodingOptions.None);
			return UIImage.LoadFromData(image);
		}

		public UIImage GetThumbnailImage()
		{
			NSData thumbnail = new NSData(Thumbnail, NSDataBase64DecodingOptions.None);
			return UIImage.LoadFromData(thumbnail);
		}

		public CGRect GetRectangleF()
		{
			return new CGRect (ImgX, ImgY, ImgW, ImgH);
		}

		public Picture() {}

		public Picture(string image, string thumbnail, float rotation, CGRect position, string userid)
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

