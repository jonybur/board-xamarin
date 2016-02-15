using System;
using Newtonsoft.Json;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;
using System.Threading.Tasks;

using MediaPlayer;

using Board.Interface;

using System.Collections.Generic;

using Facebook.CoreKit;
using Board.Schema;
using Board.Interface.Widgets;

namespace Board.Interface
{
	public static class Preview
	{
		public static AnnouncementWidget announcementWidget;
		public static PictureWidget pictureWidget;
		public static VideoWidget videoWidget;

		private static UIView uiView;
		public static UIView View{
			get { return uiView; }
		}
		private static float Rotation = 0;

		public enum Type {Picture = 1, Video, Announcement};
		public static int TypeOfPreview;

		public static void Initialize (Announcement ann, UINavigationController navigationController)
		{
			TypeOfPreview = (int)Type.Announcement;

			announcementWidget = new AnnouncementWidget (ann);

			CGRect frame = announcementWidget.View.Frame;

			uiView = new UIView (new CGRect(BoardInterface.scrollView.ContentOffset.X + AppDelegate.ScreenWidth / 2 - frame.Width / 2,
				BoardInterface.scrollView.ContentOffset.Y + AppDelegate.ScreenHeight / 2 - frame.Height / 2 - Board.Interface.Buttons.Button.ButtonSize / 2, frame.Width, frame.Height));

			uiView.Alpha = .5f;
			uiView.AddGestureRecognizer (SetNewPanGestureRecognizer());
			uiView.AddGestureRecognizer (SetNewRotationGestureRecognizer(false));
			uiView.AddSubviews(announcementWidget.View);
		}

		public static Picture Initialize (UIImage image, UINavigationController navigationController)
		{
			TypeOfPreview = (int)Type.Picture;

			Picture picture = new Picture ();
			picture.Image = image;

			pictureWidget = new PictureWidget (picture);

			CGRect frame = pictureWidget.View.Frame;

			uiView = new UIView (new CGRect(BoardInterface.scrollView.ContentOffset.X + AppDelegate.ScreenWidth / 2 - frame.Width / 2,
				BoardInterface.scrollView.ContentOffset.Y + AppDelegate.ScreenHeight / 2 - frame.Height / 2 - Board.Interface.Buttons.Button.ButtonSize / 2, frame.Width, frame.Height));

			uiView.Alpha = .5f;
			uiView.AddGestureRecognizer (SetNewPanGestureRecognizer());
			uiView.AddGestureRecognizer (SetNewRotationGestureRecognizer(false));
			uiView.AddSubviews(pictureWidget.View);

			return picture;
		}

		public static Video Initialize (string Url, UINavigationController navigationController)
		{
			TypeOfPreview = (int)Type.Video;

			Video video = new Video ();

			MPMoviePlayerController moviePlayer = new MPMoviePlayerController (new NSUrl(Url));
			video.Thumbnail = moviePlayer.ThumbnailImageAt (0, MPMovieTimeOption.Exact);
			moviePlayer.Pause ();
			moviePlayer.Dispose ();

			video.Url = Url;

			videoWidget = new VideoWidget (video);

			CGRect frame = videoWidget.View.Frame;

			uiView = new UIView (new CGRect(BoardInterface.scrollView.ContentOffset.X + AppDelegate.ScreenWidth / 2 - frame.Width / 2,
				BoardInterface.scrollView.ContentOffset.Y + AppDelegate.ScreenHeight / 2 - frame.Height / 2 - Board.Interface.Buttons.Button.ButtonSize / 2, frame.Width, frame.Height));

			uiView.Alpha = .5f;
			uiView.AddGestureRecognizer (SetNewPanGestureRecognizer());
			uiView.AddGestureRecognizer (SetNewRotationGestureRecognizer(false));
			uiView.AddSubviews(videoWidget.View);

			return video;
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
			Picture p = new Picture (pictureWidget.Picture.Image, pictureWidget.Picture.Thumbnail, Rotation, uiView.Frame, Profile.CurrentProfile.UserID);
			return p;
		}

		public static Video GetVideo()
		{
			uiView.Transform = CGAffineTransform.MakeRotation (0);
			Video v = new Video (videoWidget.Video.Url, videoWidget.Video.Thumbnail, Rotation, uiView.Frame, Profile.CurrentProfile.UserID);
			return v;
		}

		public static Announcement GetAnnouncement()
		{
			uiView.Transform = CGAffineTransform.MakeRotation (0);
			Announcement ann = new Announcement (announcementWidget.Announcement.Text, Rotation, uiView.Frame, Profile.CurrentProfile.UserID);
			ann.SocialChannel = announcementWidget.Announcement.SocialChannel;
			return ann;
		}

	}
}

