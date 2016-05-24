using System;
using UIKit;
using CoreGraphics;
using System.Threading.Tasks;
using Board.Utilities;
using System.Collections.Generic;
using BigTed;
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

		public string Name;
		public string Description;
		public DateTime StartDate;
		public DateTime EndDate;
		public string ImageUrl;
		public const string Type = "events";

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
		}

		public BoardEvent(string name, UIImage image, string imageUrl, DateTime startdate, DateTime enddate, CGAffineTransform transform, CGPoint center, string creatorid)
		{
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

			CoverLoaded = false;
		}
	}
}