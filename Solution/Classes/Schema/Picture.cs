using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Board.Interface.Widgets;
using Board.Utilities;
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

		public const string Type = "pictures";

		public void SetImageFromUIImage(UIImage image){
			_image = image;
			SetThumbnailFromUIImage (image);
		}

		public void SetThumbnailFromUIImage(UIImage image){
			_thumbnail = image.ImageScaledToFitSize(new CGSize (Widget.Autosize, Widget.Autosize));
		}

		public string Description;

		public Picture() {
		}

		public Picture(UIImage image, string imageUrl, CGPoint center, string creatorid, CGAffineTransform transform)
		{
			ImageUrl = imageUrl;
			SetImageFromUIImage (image);
			Transform = transform;
			Center = center;
			CreatorId = creatorid;
		}

		public Picture(FacebookPhoto facebookPhoto, CGPoint center, CGAffineTransform transform){
			CreationDate = DateTime.Parse(facebookPhoto.CreatedTime);
			FacebookId = facebookPhoto.Id;
			Center = center;
			Transform = transform;
		}
	}
}

