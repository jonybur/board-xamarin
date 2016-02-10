using System;
using Newtonsoft.Json;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;
using System.Threading.Tasks;

using MediaPlayer;

using System.Collections.Generic;

using Facebook.CoreKit;

namespace Solution
{
	public static class Preview
	{
		public static TextBoxComponent textBoxComponent;
		public static PictureComponent pictureComponent;
		public static VideoComponent videoComponent;

		private static UIView uiView;
		public static UIView View{
			get { return uiView; }
		}
		private static float Rotation = 0;

		public enum Type {Picture = 1, Video, TextBox};
		public static int TypeOfPreview;

		public static void Initialize (UIImage image, CGPoint ContentOffset, UINavigationController navigationController)
		{
			TypeOfPreview = (int)Type.Picture;

			Picture picture = new Picture ();
			picture.Image = image;

			pictureComponent = new PictureComponent (picture);

			CGRect frame = pictureComponent.View.Frame;

			uiView = new UIView (new CGRect(ContentOffset.X + AppDelegate.ScreenWidth / 2 - frame.Width / 2,
				ContentOffset.Y + AppDelegate.ScreenHeight / 2 - frame.Height / 2 - Button.ButtonSize / 2, frame.Width, frame.Height));

			uiView.Alpha = .5f;
			uiView.AddGestureRecognizer (SetNewPanGestureRecognizer());
			uiView.AddGestureRecognizer (SetNewRotationGestureRecognizer(false));
			uiView.AddSubviews(pictureComponent.View);
		}

		public static void Initialize (string Url, CGPoint ContentOffset, UINavigationController navigationController)
		{
			TypeOfPreview = (int)Type.Video;

			Video video = new Video ();

			MPMoviePlayerController moviePlayer = new MPMoviePlayerController (new NSUrl(Url));
			video.Thumbnail = moviePlayer.ThumbnailImageAt (0, MPMovieTimeOption.Exact);

			video.Url = Url;

			videoComponent = new VideoComponent (video);

			CGRect frame = videoComponent.View.Frame;

			uiView = new UIView (new CGRect(ContentOffset.X + AppDelegate.ScreenWidth / 2 - frame.Width / 2,
				ContentOffset.Y + AppDelegate.ScreenHeight / 2 - frame.Height / 2 - Button.ButtonSize / 2, frame.Width, frame.Height));

			uiView.Alpha = .5f;
			uiView.AddGestureRecognizer (SetNewPanGestureRecognizer());
			uiView.AddGestureRecognizer (SetNewRotationGestureRecognizer(false));
			uiView.AddSubviews(videoComponent.View);
		}

		public static async System.Threading.Tasks.Task Initialize (TextBox textBox, CGPoint ContentOffset, Action refreshContent, UINavigationController navigationController)
		{
			TypeOfPreview = (int)Type.TextBox;
			// so now launch image preview to choose position in the board

			textBox.ImgX = (float)(ContentOffset.X + AppDelegate.ScreenWidth / 2 - textBox.ImgW / 2);
			textBox.ImgY = (float)(ContentOffset.Y + AppDelegate.ScreenHeight / 2 - textBox.ImgH / 2 - Button.ButtonSize / 2);

			textBoxComponent = new TextBoxComponent (textBox);

			await textBoxComponent.Load (navigationController, refreshContent);

			// launches the textbox preview
			uiView = textBoxComponent.GetUIView ();
			//uiView.Alpha = .5f;
			uiView.UserInteractionEnabled = true;
			uiView.AddGestureRecognizer (SetNewPanGestureRecognizer());
			uiView.AddGestureRecognizer (SetNewRotationGestureRecognizer(false));
		}

		public static UIPanGestureRecognizer SetNewPanGestureRecognizer()
		{
			float dx = 0;
			float dy = 0;

			UIPanGestureRecognizer panGesture = new UIPanGestureRecognizer ((pg) => {
				if ((pg.State == UIGestureRecognizerState.Began || pg.State == UIGestureRecognizerState.Changed) && (pg.NumberOfTouches == 1)) {

					var p0 = pg.LocationInView(uiView.Superview);

					if (dx == 0)
						dx = (float)(p0.X - uiView.Center.X);

					if (dy == 0)
						dy = (float)(p0.Y - uiView.Center.Y);

					var p1 = new CGPoint (p0.X - dx, p0.Y - dy);

					uiView.Center = p1;

				} else if (pg.State == UIGestureRecognizerState.Ended) {
					dx = 0;
					dy = 0;
				}
			});
			return panGesture;
		}

		private static UIRotationGestureRecognizer SetNewRotationGestureRecognizer(bool autoRotate)
		{
			float r = 0;
			Rotation = 0;

			if (autoRotate) {
				Random rnd = new Random ();
				r = (float)(rnd.NextDouble () / 3);
				Rotation = r;
				uiView.Transform = CGAffineTransform.MakeRotation (r);
			}

			UIRotationGestureRecognizer rotateGesture = new UIRotationGestureRecognizer ((rg) => {
				if ((rg.State == UIGestureRecognizerState.Began || rg.State == UIGestureRecognizerState.Changed) && (rg.NumberOfTouches == 2)) {
					uiView.Transform = CGAffineTransform.MakeRotation (rg.Rotation + r);
					Rotation = (float)(rg.Rotation + r);
				} else if (rg.State == UIGestureRecognizerState.Ended) {
					r += (float)rg.Rotation;
				}
			});
			return rotateGesture;
		}

		public static void RemoveUserInteraction()
		{
			uiView.UserInteractionEnabled = false;
		}

		public static void RemoveFromSuperview()
		{
			uiView.RemoveFromSuperview ();
			uiView.Dispose ();
			uiView = null;
		}

		public static Picture GetPicture()
		{
			uiView.Transform = CGAffineTransform.MakeRotation (0);
			Picture p = new Picture (pictureComponent.Picture.Image, pictureComponent.Picture.Thumbnail, Rotation, uiView.Frame, Profile.CurrentProfile.UserID);
			return p;
		}

		public static Video GetVideo()
		{
			uiView.Transform = CGAffineTransform.MakeRotation (0);
			Video v = new Video (videoComponent.Video.Url, videoComponent.Video.Thumbnail, Rotation, uiView.Frame, Profile.CurrentProfile.UserID);
			return v;
		}

		public static TextBox GetTextBox()
		{
			TextBox aux = textBoxComponent.GetTextBox ();
			aux.SetRotation (Rotation);
			return aux;
		}


	}
}

