using System;
using Board.Interface;
using Board.Interface.Buttons;

using Board.Interface.Widgets;
using Board.Schema;

using CoreGraphics;
using Facebook.CoreKit;
using Board.Utilities;
using UIKit;

namespace Board.Interface
{
	public static class Preview
	{
		private static UIView view;
		public static UIView View{
			get { return view; }
		}
		private static float Rotation;

		public enum Type { Picture = 1, Video, Announcement, Event, Poll, Map };
		public static int TypeOfPreview;

		public static Widget widget;
		public static bool IsAlive;

		public static void Initialize(Content content)
		{
			content.CreationDate = DateTime.Now;

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

			var frame = widget.View.Frame;

			var boardScroll = AppDelegate.boardInterface.BoardScroll;

			view = new UIView (new CGRect (boardScroll.ScrollView.ContentOffset.X + AppDelegate.ScreenWidth / 2 - frame.Width / 2,
				boardScroll.ScrollView.ContentOffset.Y + AppDelegate.ScreenHeight / 2 - frame.Height / 2 - Board.Interface.Buttons.Button.ButtonSize / 2, frame.Width, frame.Height));
			widget.View.Frame = new CGRect(0, 0, view.Frame.Width, view.Frame.Height);

			view.Alpha = .5f;
			view.AddGestureRecognizer (SetNewPanGestureRecognizer());
			view.BackgroundColor = UIColor.Red;
			view.AddGestureRecognizer (SetNewRotationGestureRecognizer(false));
			view.AddSubviews(widget.View);

			IsAlive = true;

			// switches to confbar
			ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.ConfirmationBar);
		}

		public static UIPanGestureRecognizer SetNewPanGestureRecognizer()
		{
			float dx = 0;
			float dy = 0;

			UIPanGestureRecognizer panGesture = new UIPanGestureRecognizer (pg => {
				if ((pg.State == UIGestureRecognizerState.Began || pg.State == UIGestureRecognizerState.Changed) && (pg.NumberOfTouches == 1)) {

					var p0 = pg.LocationInView(view.Superview);

					if (dx == 0)
						dx = (float)(p0.X - view.Center.X);

					if (dy == 0)
						dy = (float)(p0.Y - view.Center.Y);

					var p1 = new CGPoint (p0.X - dx, p0.Y - dy);

					view.Center = p1;

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
				view.Transform = CGAffineTransform.MakeRotation (r);
			}

			UIRotationGestureRecognizer rotateGesture = new UIRotationGestureRecognizer (rg => {
				if ((rg.State == UIGestureRecognizerState.Began || rg.State == UIGestureRecognizerState.Changed) && (rg.NumberOfTouches == 2)) {
					view.Transform = CGAffineTransform.MakeRotation (rg.Rotation + r);
					Rotation = (float)(rg.Rotation + r);
				} else if (rg.State == UIGestureRecognizerState.Ended) {
					r += (float)rg.Rotation;
				}
			});
			return rotateGesture;
		}

		public static void RemoveUserInteraction()
		{
			view.UserInteractionEnabled = false;
		}

		public static void RemoveFromSuperview()
		{
			view.RemoveFromSuperview ();
			IsAlive = false;
			MemoryUtility.ReleaseUIViewWithChildren (view);
		}

		public static Content GetContent(){
			var boardScroll = AppDelegate.boardInterface.BoardScroll;

			view.Transform = CGAffineTransform.MakeRotation (0);
			view.Center = new CGPoint (view.Center.X - boardScroll.LastScreen * UIBoardScroll.ScrollViewWidthSize, view.Center.Y);

			Content content;

			if (widget is PictureWidget) {
				
				var pictureWidget = (PictureWidget)widget;
				content = new Picture (pictureWidget.picture.ImageView.Image, pictureWidget.picture.ThumbnailView.Image, Rotation, view.Center, Profile.CurrentProfile.UserID, DateTime.Now);

			} else if (widget is VideoWidget) {
				
				var videoWidget = (VideoWidget)widget;
				content = new Video (videoWidget.video.Url, videoWidget.video.ThumbnailView, Rotation, view.Center, Profile.CurrentProfile.UserID, DateTime.Now);

			} else if (widget is AnnouncementWidget) {
				
				var announcementWidget = (AnnouncementWidget)widget;
				content = new Announcement (announcementWidget.announcement.AttributedText, Rotation, view.Center, Profile.CurrentProfile.UserID, DateTime.Now);
				content.FacebookId = announcementWidget.announcement.FacebookId;
				content.SocialChannel = announcementWidget.announcement.SocialChannel;

			} else if (widget is EventWidget) {
				
				var eventWidget = (EventWidget)widget;
				content = new BoardEvent (eventWidget.boardEvent.Name, eventWidget.boardEvent.ImageView.Image, eventWidget.boardEvent.StartDate, eventWidget.boardEvent.EndDate, Rotation, view.Center, Profile.CurrentProfile.UserID, DateTime.Now);
				((BoardEvent)content).Description = eventWidget.boardEvent.Description;
				content.FacebookId = eventWidget.boardEvent.FacebookId;

			} else if (widget is PollWidget) {
				
				var pollWidget = (PollWidget)widget;
				content = new Poll (pollWidget.poll.Question, Rotation, view.Center, Profile.CurrentProfile.UserID, DateTime.Now, pollWidget.poll.Answers);
				
			} else if (widget is MapWidget) {
				
				content = new Map(Rotation, view.Center, Profile.CurrentProfile.UserID, DateTime.Now);

			} else {
				
				content = new Content ();

			}

			return content;
		}
	}
}

