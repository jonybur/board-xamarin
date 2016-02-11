using System;
using System.Drawing;
using System.Linq;
using Newtonsoft.Json;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;

using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Net.Http;

namespace Solution
{
	public class PictureComponent
	{
		// UIView contains ScrollView and BackButton
		// ScrollView contains LookUpImage
		private UIView uiView;
		public UIView View
		{
			get { return uiView; }
		}

		private Picture picture;

		UIImageView eye;
		UIImage closedEyeImage;
		UIImage openEyeImage;

		private bool eyeOpen;
		public bool EyeOpen{
			get { return eyeOpen; }
		}

		public Picture Picture
		{
			get { return picture; }
		}

		public PictureComponent()
		{

		}

		public PictureComponent(Picture pic)
		{
			picture = pic;

			CGRect frame = GetFrame (pic);

			// mounting

			UIImageView mounting = CreateMounting (frame);
			uiView = new UIView(mounting.Frame);
			uiView.AddSubview (mounting);

			// picture

			CGRect pictureFrame = new CGRect (mounting.Frame.X + 10, 10, frame.Width, frame.Height);
			UIImageView uiv = new UIImageView (pictureFrame);
			uiv.Image = picture.Thumbnail;
			uiView.AddSubview (uiv);

			// like

			UIImageView like = CreateLike (mounting.Frame);
			uiView.AddSubview (like);

			// like label

			UILabel likeLabel = CreateLikeLabel (like.Frame);
			uiView.AddSubview (likeLabel);

			// eye

			eye = CreateEye (mounting.Frame);
			uiView.AddSubview (eye);

			uiView.Frame = new CGRect (pic.ImgX, pic.ImgY, mounting.Frame.Width, mounting.Frame.Height);
			uiView.Transform = CGAffineTransform.MakeRotation(pic.Rotation);

			eyeOpen = false;
		}

		public void OpenEye()
		{
			eye.Image = openEyeImage;
			eye.TintColor = AppDelegate.BoardOrange;
			eyeOpen = true;
		}

		private UIImageView CreateMounting(CGRect frame)
		{
			CGRect mountingFrame = new CGRect (0, 0, frame.Width + 20, frame.Height + 50);

			UIImageView mountingView = CreateColorView (mountingFrame, UIColor.White.CGColor);

			return mountingView;
		}

		private UILabel CreateLikeLabel(CGRect frame)
		{
			UIFont likeFont = UIFont.SystemFontOfSize (20);
			string likeText = "0";
			CGSize likeLabelSize = likeText.StringSize (likeFont);
			UILabel likeLabel = new UILabel(new CGRect(frame.X - likeLabelSize.Width - 4, frame.Y + 4, likeLabelSize.Width, likeLabelSize.Height));
			likeLabel.TextColor = AppDelegate.BoardOrange;
			likeLabel.Font = likeFont;
			likeLabel.Text = likeText;
			likeLabel.TextAlignment = UITextAlignment.Right;

			return likeLabel;
		}

		private UIImageView CreateEye(CGRect frame)
		{
			CGSize iconSize = new CGSize (30, 30);

			UIImageView eyeView = new UIImageView(new CGRect (frame.X + 10, frame.Height - iconSize.Height - 5, iconSize.Width, iconSize.Height));
			closedEyeImage = UIImage.FromFile ("./boardscreen/closedeye.png");
			openEyeImage = UIImage.FromFile ("./boardscreen/openeye3.png");
			closedEyeImage = closedEyeImage.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
			openEyeImage = openEyeImage.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
			eyeView.Image = closedEyeImage;
			eyeView.TintColor = UIColor.FromRGB(140,140,140);

			return eyeView;
		}

		private UIImageView CreateLike(CGRect frame)
		{
			CGSize iconSize = new CGSize (30, 30);

			UIImageView likeView = new UIImageView(new CGRect (frame.Width - iconSize.Width - 10,
				frame.Height - iconSize.Height - 5, iconSize.Width, iconSize.Height));
			likeView.Image = UIImage.FromFile ("./boardscreen/like.png");
			likeView.Image = likeView.Image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
			likeView.TintColor = UIColor.FromRGB(140,140,140);

			return likeView;
		}


		private UIImageView CreateColorView(CGRect frame, CGColor color)
		{
			UIGraphics.BeginImageContext (new CGSize(frame.Size.Width, frame.Size.Height));
			CGContext context = UIGraphics.GetCurrentContext ();

			context.SetFillColor(color);
			context.FillRect(frame);

			UIImage orange = UIGraphics.GetImageFromCurrentImageContext ();
			UIImageView uiv = new UIImageView (orange);
			uiv.Frame = frame;

			return uiv;
		}

		private CGRect GetFrame(Picture picture)
		{
			float imgw, imgh;
			float autosize = 150;

			float scale = (float)(picture.Image.Size.Width/picture.Image.Size.Height);

			if (scale >= 1) {
				imgw = autosize * scale;
				imgh = autosize;

				if (imgw > AppDelegate.ScreenWidth) {
					scale = (float)(picture.Image.Size.Height/picture.Image.Size.Width);
					imgw = AppDelegate.ScreenWidth;
					imgh = imgw * scale;
				}
			} else {
				scale = (float)(picture.Image.Size.Height / picture.Image.Size.Width);
				imgw = autosize;
				imgh = autosize * scale;
			}

			picture.Thumbnail = CommonUtils.ResizeImage (picture.Image, new CGSize (imgw, imgh));

			CGRect frame = new CGRect (picture.ImgX, picture.ImgY, imgw, imgh);

			return frame;
		}

		public void SetFrame(CGRect frame)
		{
			uiView.Frame = frame;
		}

	}
}

