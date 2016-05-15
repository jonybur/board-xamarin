using System;
using UIKit;
using CoreGraphics;
using System.Threading.Tasks;
using Board.Utilities;
using MGImageUtilitiesBinding;
using Board.Interface.Widgets;
using System.Runtime.Serialization;

namespace Board.Schema
{
	// The Picture class as it is uploaded on Azure
	// Board's way of storing its UIImageViews on the DB
	public class BoardEvent : Content
	{
		public string Name;
	
		public string Description;

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

		public DateTime StartDate;

		public DateTime EndDate;

		public string ImageUrl;

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
			Type = "events";
		}

		public BoardEvent(string name, UIImage image, string imageUrl, DateTime startdate, DateTime enddate, float rotation, CGPoint center, string creatorid, DateTime creationdate)
		{
			Type = "events";
			SetImageFromUIImage (image);
			Name = name;
			ImageUrl = imageUrl;
			StartDate = startdate;
			EndDate = enddate;
			Rotation = rotation;
			Center = center;
			CreatorId = creatorid;
			CreationDate = creationdate;
		}
	}
}