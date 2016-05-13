using System;
using System.Runtime.Serialization;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Board.Schema
{
	public class Video : Content
	{
		[IgnoreDataMember]
		public NSUrl Url;

		public string UrlText;

		[IgnoreDataMember]
		public UIImage Thumbnail;

		public string Description;

		public Video() { 
			Type = "videos";
		}

		public Video(NSUrl url, UIImage thumbnail, float rotation, CGPoint center, string creatorid, DateTime creationdate)
		{
			Type = "videos";
			Url = url;
			UrlText = url.AbsoluteString;
			Thumbnail = thumbnail;
			Rotation = rotation;
			Center = center;
			CreatorId = creatorid;
			CreationDate = creationdate;
		}
	}
}

