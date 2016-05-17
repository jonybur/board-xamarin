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
		public NSUrl AmazonNSUrl {
			get{ return NSUrl.FromString (AmazonUrl); }
			set{ AmazonUrl = value.AbsoluteString; }
		}

		public string AmazonUrl;

		[IgnoreDataMember]
		public NSUrl LocalNSUrl;

		[IgnoreDataMember]
		public UIImage Thumbnail;

		public const string Type = "videos";

		public string Description;

		public Video() { 
		}

		public NSUrl GetNSUrlForDisplay(){
			if (LocalNSUrl != null) {
				return LocalNSUrl;
			} else {
				return AmazonNSUrl;
			}
		}

		public Video(string amazonurl, UIImage thumbnail, float rotation, CGPoint center, string creatorid, DateTime creationdate)
		{
			AmazonUrl = amazonurl;
			Thumbnail = thumbnail;
			Rotation = rotation;
			Center = center;
			CreatorId = creatorid;
			CreationDate = creationdate;
		}
	}
}

