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
	public class TextBox : Content
	{
		[JsonProperty(PropertyName = "id")]
		public string Id { get; set; }

		[JsonProperty(PropertyName = "text")]
		public string Text { get; set; }

		[JsonProperty(PropertyName = "rotation")]
		public float Rotation { get; set; }

		public CGRect GetRectangleF()
		{
			return new CGRect (ImgX, ImgY, ImgW, ImgH);
		}

		public TextBox(string id, string userid, string text, CGRect bounds, float rotation)
		{
			Id = id;
			ImgX = (float)(bounds.X); ImgY = (float)(bounds.Y);
			ImgH = (float)(bounds.Height); ImgW = (float)(bounds.Width);
			UserId = userid;
			Text = text;
			Rotation = rotation;
		}

		public void SetPosition(CGPoint position)
		{
			ImgX = (float)position.X; ImgY = (float)position.Y;
		}

		public void SetRotation(float rotation)
		{
			Rotation = rotation;
		}

	}
}

