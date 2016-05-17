using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Board.Interface.Widgets;
using Board.Utilities;
using CoreGraphics;
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

		public string Description;

		public Picture() {
		}

		public Picture(UIImage image, string imageUrl, float rotation, CGPoint center, string creatorid, DateTime creationdate)
		{
			ImageUrl = imageUrl;
			SetImageFromUIImage (image);
			Rotation = rotation;
			Center = center;
			CreatorId = creatorid;
			CreationDate = creationdate;
		}
	}
}

