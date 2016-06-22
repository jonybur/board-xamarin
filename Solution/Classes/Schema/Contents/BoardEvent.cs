using System;
using UIKit;
using CoreGraphics;
using System.Threading.Tasks;
using Board.Utilities;
using System.Globalization;
using CoreLocation;
using Board.Interface;
using MGImageUtilitiesBinding;
using Board.Interface.Widgets;
using Board.Facebook;
using System.Runtime.Serialization;

namespace Board.Schema
{
	// The Picture class as it is uploaded on Azure
	// Board's way of storing its UIImageViews on the DB
	public class BoardEvent : Content
	{
		[IgnoreDataMember]
		private UIImage _image;

		[IgnoreDataMember]
		public UIImage Image{
			get { 
				return _image;
			}
		}

		[IgnoreDataMember]
		private UIImage _thumbnail;

		[IgnoreDataMember]
		public UIImage Thumbnail{
			get { 
				return _thumbnail;
			}
		}

		[IgnoreDataMember]
		public bool CoverLoaded;

		[IgnoreDataMember]
		public DateTime StartDate;

		[IgnoreDataMember]
		public DateTime EndDate;

		public string Name;
		public string Description;

		public int StartDateTimestamp{
			get { return CommonUtils.GetUnixTimeStamp(StartDate); }
			set { StartDate = CommonUtils.UnixTimeStampToDateTime(value);  }
		}

		public int EndDateTimestamp{
			get { return CommonUtils.GetUnixTimeStamp(EndDate); }
			set { EndDate = CommonUtils.UnixTimeStampToDateTime(value);  }
		}

		public string ImageUrl;

		public string Type {
			get { return "events"; }
		}

		public void SetImageFromUIImage(UIImage image){
			_image = image;
			SetThumbnail ();
		}

		public async Task GetImageFromUrl(string imageUrl){
			ImageUrl = imageUrl;
			_image = await CommonUtils.DownloadUIImageFromURL (ImageUrl);
			SetThumbnail ();
		}

		private void SetThumbnail(){
			_thumbnail = Image.ImageScaledToFitSize(new CGSize (Widget.Autosize, Widget.Autosize));
		}

	
		public BoardEvent() {
			CreationDate = DateTime.Now;
		}

		public BoardEvent(string name, UIImage image, string imageUrl, DateTime startdate, DateTime enddate, CGAffineTransform transform, CGPoint center, string creatorid)
		{
			CreationDate = DateTime.Now;
			StartDate = startdate;
			EndDate = enddate;
			Name = name;
			Center = center;
			Transform = transform;

			SetImageFromUIImage (image);
			ImageUrl = imageUrl;
			CreatorId = creatorid;
		}

		public BoardEvent(FacebookEvent facebookEvent, CGPoint center, CGAffineTransform transform){
			try{
				StartDate = DateTime.Parse (facebookEvent.StartTime);
			}catch{
				StartDate = new DateTime();
			}
			try {
				EndDate = DateTime.Parse (facebookEvent.EndTime);
			}catch{
				EndDate = StartDate.AddHours(1);
			}
			Name = facebookEvent.Name;
			Description = facebookEvent.Description;
			Center = center;
			Transform = transform;
			FacebookId = facebookEvent.Id;

			var boardCoordinate = UIBoardInterface.board.GeolocatorObject.Coordinate;
			latitude = boardCoordinate.Latitude;
			longitude = boardCoordinate.Longitude;

			boardId = UIBoardInterface.board.Id;

			CoverLoaded = false;
		}

		public BoardEvent(FacebookEvent facebookEvent, CGPoint center, CGAffineTransform transform, Board board){
			try{
				StartDate = DateTime.Parse (facebookEvent.StartTime);
			}catch{
				StartDate = new DateTime();
			}
			try {
				EndDate = DateTime.Parse (facebookEvent.EndTime);
			}catch{
				EndDate = StartDate.AddHours(1);
			}
			Name = facebookEvent.Name;
			Description = facebookEvent.Description;
			Center = center;
			Transform = transform;
			FacebookId = facebookEvent.Id;

			var boardCoordinate = board.GeolocatorObject.Coordinate;
			latitude = boardCoordinate.Latitude;
			longitude = boardCoordinate.Longitude;

			boardId = board.Id;

			CoverLoaded = false;
		}
	}
}