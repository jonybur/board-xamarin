using System;
using System.Runtime.Serialization;
using Board.Facebook;
using CoreGraphics;
using Board.Interface;
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

		public string Type {
			get { return "videos"; }
		}

		public string Description;

		public Video() {
			CreationDate = DateTime.Now;
		}

		public NSUrl GetNSUrlForDisplay(){
			if (LocalNSUrl != null) {
				return LocalNSUrl;
			} else {
				return AmazonNSUrl;
			}
		}

		public Video(string amazonurl, UIImage thumbnail, CGPoint center, string creatorid, CGAffineTransform transform)
		{
			CreationDate = DateTime.Now;
			AmazonUrl = amazonurl;
			Thumbnail = thumbnail;
			Transform = transform;
			Center = center;
			CreatorId = creatorid;
		}

		public Video(FacebookVideo fbVideo, CGPoint center, CGAffineTransform transform){
			CreationDate = DateTime.Now;
			Description = fbVideo.Description;
			CreationDate = DateTime.Parse(fbVideo.UpdatedTime);
			Center = center;
			Transform = transform;

			var boardCoordinate = UIBoardInterface.board.GeolocatorObject.Coordinate;
			latitude = boardCoordinate.Latitude;
			longitude = boardCoordinate.Longitude;

			boardId = UIBoardInterface.board.Id;
		}
	}
}

