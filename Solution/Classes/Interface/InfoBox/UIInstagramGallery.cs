﻿using System.Collections.Generic;
using Clubby.Infrastructure;
using Clubby.Schema;
using CoreGraphics;
using Clubby.Interface.LookUp;
using Foundation;
using Haneke;
using UIKit;

namespace Clubby.Interface
{
	class UIInstagramGallery : UIScrollView {
		public static float ButtonSize;
		List<UIButton> InstagramPhotos;

		public UIInstagramGallery (float width, float yposition, List<Content> contents){
			ScrollEnabled = false;
			ButtonSize = width / 3 - 1;
			Frame = new CGRect (0, 0, width, ButtonSize * 2);

			InstagramPhotos = new List<UIButton> ();

			int imagesCount = (contents.Count > 11) ? 11 : contents.Count;

			if (imagesCount != 0) {
				foreach (Picture picture in contents) {
					SetImage (picture);
				}

				SetInstagramThumb ();
			}

			Fill ();

			Center = new CGPoint (Center.X, yposition + Frame.Height/2);
		}

		private void SetInstagramThumb(){
			var pictureButton = new UIButton(new CGRect (0, 0, ButtonSize, ButtonSize));
			var imageView = new UIImageView (pictureButton.Frame);
			imageView.SetImage ("./boardinterface/infobox/viewmore3.png");
			pictureButton.Alpha = .6f;
			pictureButton.AddSubview(imageView);
			pictureButton.ClipsToBounds = true;
			pictureButton.TouchUpInside += (sender, e) => {
				// opens instagram
				if (AppsController.CanOpenInstagram()){
					AppsController.OpenInstagram(UIVenueInterface.venue.InstagramId);
				}

				//location?id=LOCATION_ID
			};

			InstagramPhotos.Add(pictureButton);
		}

		private void SetImage (Picture picture){
			var pictureButton = new UIButton(new CGRect (0, 0, ButtonSize, ButtonSize));

			var imageView = new UIImageView (pictureButton.Frame);
			imageView.SetImage (new NSUrl(picture.ThumbnailImageUrl));
			imageView.ContentMode = UIViewContentMode.ScaleAspectFill;

			pictureButton.AddSubview(imageView);
			pictureButton.ClipsToBounds = true;
			pictureButton.TouchUpInside += (sender, e) => {
				var lookUp = new PictureLookUp(picture);
				AppDelegate.PushViewLikePresentView(lookUp);
			};

			InstagramPhotos.Add(pictureButton);
		}

		private void Fill (){
			int x = 1; float y = 1;
			float lastBottom = 0;
			foreach (var button in InstagramPhotos) {
				button.Center = new CGPoint ((Frame.Width / 6) * x, (ButtonSize + 1) * y - ButtonSize / 2);

				x += 2;

				if (x >= 6) {
					x = 1;
					y ++;
				}

				AddSubview (button);
				lastBottom = (float)button.Frame.Bottom;
			}

			ContentSize = new CGSize (Frame.Width, (float)lastBottom + 1);
			ContentOffset = new CGPoint (0, 0);
			Frame = new CGRect (0, 0, Frame.Width, lastBottom);
		}
	}
}