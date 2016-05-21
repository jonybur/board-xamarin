using System;
using System.Runtime.Serialization;
using Board.Facebook;
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

		public Video(string amazonurl, UIImage thumbnail, CGPoint center, string creatorid, DateTime creationdate, CGAffineTransform transform)
		{
			AmazonUrl = amazonurl;
			Thumbnail = thumbnail;
			Transform = transform;
			Center = center;
			CreatorId = creatorid;
			CreationDate = creationdate;
		}

		public Video(FacebookVideo fbVideo, CGPoint center, CGAffineTransform transform){
			Description = fbVideo.Description;
			CreationDate = DateTime.Parse(fbVideo.UpdatedTime);
			Center = center;
			Transform = transform;
		}
	}
}

