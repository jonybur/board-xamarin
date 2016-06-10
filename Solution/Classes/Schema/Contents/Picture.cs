using System;
using System.Runtime.Serialization;
using Board.Interface.Widgets;
using Board.Interface;
using CoreGraphics;
using Board.Facebook;
using MGImageUtilitiesBinding;
using UIKit;

namespace Board.Schema
{
	public class Picture : Content
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

		public string ImageUrl;

		public string Type {
			get { return "pictures"; }
		}

		public void SetImageFromUIImage(UIImage image){
			_image = image;
			SetThumbnailFromUIImage (image);
		}

		public void SetThumbnailFromUIImage(UIImage image){
			_thumbnail = image.ImageScaledToFitSize(new CGSize (Widget.Autosize, Widget.Autosize));
		}

		public string Description;

		public Picture() {
			CreationDate = DateTime.Now;
		}

		public Picture(UIImage image, string imageUrl, CGPoint center, string creatorid, CGAffineTransform transform)
		{
			CreationDate = DateTime.Now;
			ImageUrl = imageUrl;
			SetImageFromUIImage (image);
			Transform = transform;
			Center = center;
			CreatorId = creatorid;
		}

		public Picture(FacebookPost facebookPost, string imageUrl, CGPoint center, CGAffineTransform transform){
			CreationDate = DateTime.Parse(facebookPost.CreatedTime);
			FacebookId = facebookPost.Id;
			Center = center;
			Transform = transform;
			ImageUrl = imageUrl;
			Description = facebookPost.Message;

			var boardCoordinate = UIBoardInterface.board.GeolocatorObject.Coordinate;
			latitude = boardCoordinate.Latitude;
			longitude = boardCoordinate.Longitude;

			boardId = UIBoardInterface.board.Id;
		}

		public Picture(FacebookPhoto facebookPhoto, CGPoint center, CGAffineTransform transform){
			CreationDate = DateTime.Parse(facebookPhoto.CreatedTime);
			FacebookId = facebookPhoto.Id;
			Center = center;
			Transform = transform;
			Description = facebookPhoto.Name;

			var boardCoordinate = UIBoardInterface.board.GeolocatorObject.Coordinate;
			latitude = boardCoordinate.Latitude;
			longitude = boardCoordinate.Longitude;

			boardId = UIBoardInterface.board.Id;
		}
	}
}

