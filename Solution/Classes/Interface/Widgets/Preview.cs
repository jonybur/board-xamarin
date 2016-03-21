using System;
using CoreGraphics;

using Foundation;
using UIKit;

using MediaPlayer;

using Board.Interface.Buttons;
using System.Collections.Generic;
using Board.Interface;

using Facebook.CoreKit;
using Board.Utilities;
using Board.Schema;
using Board.Interface.Widgets;

namespace Board.Interface
{
	public static class Preview
	{
		private static UIView uiView;
		public static UIView View{
			get { return uiView; }
		}
		private static float Rotation;

		public enum Type {Picture = 1, Video, Announcement, Event, Poll, Map};
		public static int TypeOfPreview;

		public static Widget widget;

		public static void Initialize(Content content)
		{
			if (content is Announcement) {

				widget = new AnnouncementWidget ((Announcement)content);
				TypeOfPreview = (int)Type.Announcement;
				((AnnouncementWidget)widget).ScrollEnabled (false);

			} else if (content is BoardEvent) {
				
				widget = new EventWidget ((BoardEvent)content);
				TypeOfPreview = (int)Type.Event;

			} else if (content is Poll) {
				
				widget = new PollWidget ((Poll)content);
				TypeOfPreview = (int)Type.Poll;

			} else if (content is Picture) {
				
				widget = new PictureWidget ((Picture)content);
				TypeOfPreview = (int)Type.Picture;

			} else if (content is Video) {
				
				widget = new VideoWidget ((Video)content);
				TypeOfPreview = (int)Type.Video;

			} else if (content is Map) {

				widget = new MapWidget ((Map)content);
				TypeOfPreview = (int)Type.Map;

			} else {
				
				widget = new Widget ();

			}

			CGRect frame = widget.View.Frame;

			uiView = new UIView (new CGRect(BoardInterface.scrollView.ContentOffset.X + AppDelegate.ScreenWidth / 2 - frame.Width / 2,
				BoardInterface.scrollView.ContentOffset.Y + AppDelegate.ScreenHeight / 2 - frame.Height / 2 - Board.Interface.Buttons.Button.ButtonSize / 2, frame.Width, frame.Height));

			uiView.Alpha = .5f;
			uiView.AddGestureRecognizer (SetNewPanGestureRecognizer());
			uiView.AddGestureRecognizer (SetNewRotationGestureRecognizer(false));
			uiView.AddSubviews(widget.View);

			// shows the image preview so that the user can position the image
			BoardInterface.scrollView.AddSubview(Preview.View);

			// switches to confbar
			ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.ConfirmationBar);
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

			UIRotationGestureRecognizer rotateGesture = new UIRotationGestureRecognizer (rg => {
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
			PictureWidget pictureWidget = (PictureWidget)widget;
			Picture p = new Picture (pictureWidget.picture.ImageView.Image, pictureWidget.picture.ThumbnailView.Image, Rotation, uiView.Frame.Location, Profile.CurrentProfile.UserID, DateTime.Now);
			return p;
		}

		public static Video GetVideo()
		{
			uiView.Transform = CGAffineTransform.MakeRotation (0);
			VideoWidget videoWidget = (VideoWidget)widget;
			Video v = new Video (videoWidget.video.Url, videoWidget.video.ThumbnailView, Rotation, uiView.Frame.Location, Profile.CurrentProfile.UserID, DateTime.Now);
			return v;
		}

		public static Announcement GetAnnouncement()
		{
			uiView.Transform = CGAffineTransform.MakeRotation (0);
			AnnouncementWidget announcementWidget = (AnnouncementWidget)widget;
			Announcement ann = new Announcement (announcementWidget.announcement.AttributedText, Rotation, uiView.Frame.Location, Profile.CurrentProfile.UserID, DateTime.Now);
			ann.FacebookId = announcementWidget.announcement.FacebookId;
			ann.SocialChannel = announcementWidget.announcement.SocialChannel;
			return ann;
		}

		public static BoardEvent GetEvent()
		{
			uiView.Transform = CGAffineTransform.MakeRotation (0);
			EventWidget eventWidget = (EventWidget)widget;
			BoardEvent bve = new BoardEvent (eventWidget.boardEvent.Name, eventWidget.boardEvent.ImageView.Image, eventWidget.boardEvent.StartDate, eventWidget.boardEvent.EndDate, Rotation, uiView.Frame.Location, Profile.CurrentProfile.UserID, DateTime.Now);
			bve.Description = eventWidget.boardEvent.Description;
			bve.FacebookId = eventWidget.boardEvent.FacebookId;
			return bve;
		}

		public static Poll GetPoll()
		{
			uiView.Transform = CGAffineTransform.MakeRotation (0);
			PollWidget pollWidget = (PollWidget)widget;
			Poll poll = new Poll (pollWidget.poll.Question, Rotation, uiView.Frame.Location, Profile.CurrentProfile.UserID, DateTime.Now, pollWidget.poll.Answers);
			return poll;
		}

		public static Map GetMap()
		{
			uiView.Transform = CGAffineTransform.MakeRotation (0);
			Map map = new Map(Rotation, uiView.Frame.Location, Profile.CurrentProfile.UserID, DateTime.Now);
			return map;
		}
	}
}

