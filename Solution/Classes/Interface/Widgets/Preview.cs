using System;
using Board.Interface;
using Board.Infrastructure;
using Board.Interface.Buttons;
using Board.Interface.Widgets;
using Board.Schema;
using Board.Utilities;
using CoreGraphics;
using Facebook.CoreKit;
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
				((PictureWidget)widget).Initialize ();
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

			var boardScroll = AppDelegate.BoardInterface.BoardScroll;

			view = new UIView (new CGRect (boardScroll.ScrollView.ContentOffset.X + AppDelegate.ScreenWidth / 2 - frame.Width / 2,
				boardScroll.ScrollView.ContentOffset.Y + AppDelegate.ScreenHeight / 2 - frame.Height / 2 - Board.Interface.Buttons.Button.ButtonSize / 2, frame.Width, frame.Height));
			widget.View.Frame = new CGRect(0, 0, view.Frame.Width, view.Frame.Height);

			view.Alpha = .5f;
			view.AddGestureRecognizer (SetNewRotationGestureRecognizer(false));
			view.AddGestureRecognizer (SetNewPinchGestureRecognizer());
			view.AddGestureRecognizer (SetNewPanGestureRecognizer());
			view.AddSubviews(widget.View);

			IsAlive = true;

			// switches to confbar
			ButtonInterface.SwitchButtonLayout (ButtonInterface.ButtonLayout.ConfirmationBar);
		}

		public static UIPanGestureRecognizer SetNewPanGestureRecognizer()
		{
			float dx = 0;
			float dy = 0;

			var panGesture = new UIPanGestureRecognizer (pg => {
				if ((pg.State == UIGestureRecognizerState.Began || pg.State == UIGestureRecognizerState.Changed)) {
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

			panGesture.Delegate = new CustomDelegate ();

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

			var rotateGesture = new UIRotationGestureRecognizer (rg => {
				if ((rg.State == UIGestureRecognizerState.Began || rg.State == UIGestureRecognizerState.Changed) && (rg.NumberOfTouches == 2)) {

					view.Transform = CGAffineTransform.Rotate(view.Transform, rg.Rotation);

					Rotation = (float)(rg.Rotation + r);

					rg.Rotation = 0;

				} else if (rg.State == UIGestureRecognizerState.Ended) {
					r += (float)rg.Rotation;
				}

			});

			rotateGesture.Delegate = new CustomDelegate ();

			return rotateGesture;
		}

		private static UIPinchGestureRecognizer SetNewPinchGestureRecognizer(){
			
			var panGesture = new UIPinchGestureRecognizer (pinch => {
				if ((pinch.State == UIGestureRecognizerState.Began || pinch.State == UIGestureRecognizerState.Changed) && (pinch.NumberOfTouches == 2)) {
					var pinchView = pinch.View;
					var bounds = pinchView.Bounds;
					var pinchCenter = pinch.LocationInView(pinchView);
					pinchCenter.X -= bounds.GetMidX();
					pinchCenter.Y -= bounds.GetMidY();
					var transform = pinchView.Transform;
					transform = CGAffineTransform.Translate(transform, pinchCenter.X, pinchCenter.Y);
					var scale = pinch.Scale;
					transform = CGAffineTransform.Scale(transform, scale, scale);
					transform = CGAffineTransform.Translate(transform, -pinchCenter.X, -pinchCenter.Y);

					if (transform.xx < 1.5f && transform.xx > .75f && transform.yx < 1.5f && transform.yy > .75f) {
						pinchView.Transform = transform;
					}

					pinch.Scale = 1f;
				} 

			});

			panGesture.Delegate = new CustomDelegate ();

			return panGesture;
		}

		class CustomDelegate : UIGestureRecognizerDelegate{
			public override bool ShouldRecognizeSimultaneously (UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
			{
				if (gestureRecognizer.View != otherGestureRecognizer.View) {
					return false;
				}
				return true;
			}
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

		public static async System.Threading.Tasks.Task<Content> GetContent(){
			var boardScroll = AppDelegate.BoardInterface.BoardScroll;

			view.Center = new CGPoint (view.Center.X - boardScroll.LastScreen * UIBoardScroll.ScrollViewWidthSize, view.Center.Y);

			Content content;

			if (widget is PictureWidget) {
				
				var pictureWidget = (PictureWidget)widget;

				var imageURL = CloudController.UploadToAmazon (pictureWidget.picture.Image);
				
				content = new Picture (pictureWidget.picture.Image, imageURL, view.Center, Profile.CurrentProfile.UserID, DateTime.Now, view.Transform);

			} else if (widget is VideoWidget) {
				
				var videoWidget = (VideoWidget)widget;

				// if the video is from facebook, set local url, upload to amazon
				string amazonUrl;
				if (videoWidget.video.AmazonUrl != null) {
					var byteArray = await CommonUtils.DownloadByteArrayFromURL (videoWidget.video.AmazonUrl);
					amazonUrl = CloudController.UploadToAmazon (byteArray);
				} else {
					amazonUrl = CloudController.UploadToAmazon (videoWidget.video.LocalNSUrl);
				}

				content = new Video (amazonUrl, videoWidget.video.Thumbnail, view.Center, Profile.CurrentProfile.UserID, DateTime.Now, view.Transform);

			} else if (widget is AnnouncementWidget) {
				
				var announcementWidget = (AnnouncementWidget)widget;
				content = new Announcement (announcementWidget.announcement.AttributedText, view.Center, Profile.CurrentProfile.UserID, DateTime.Now, view.Transform);
				content.FacebookId = announcementWidget.announcement.FacebookId;
				content.SocialChannel = announcementWidget.announcement.SocialChannel;

			} else if (widget is EventWidget) {
				
				var eventWidget = (EventWidget)widget;
				var imageURL = CloudController.UploadToAmazon (eventWidget.boardEvent.Image);
				content = new BoardEvent (eventWidget.boardEvent.Name, eventWidget.boardEvent.Image, imageURL, eventWidget.boardEvent.StartDate, eventWidget.boardEvent.EndDate, view.Transform, view.Center, Profile.CurrentProfile.UserID, DateTime.Now);
				((BoardEvent)content).Description = eventWidget.boardEvent.Description;
				content.FacebookId = eventWidget.boardEvent.FacebookId;

			} else if (widget is PollWidget) {
				
				var pollWidget = (PollWidget)widget;
				content = new Poll (pollWidget.poll.Question, view.Transform, view.Center, Profile.CurrentProfile.UserID, DateTime.Now, pollWidget.poll.Answers);
				
			} else if (widget is MapWidget) {
				
				content = new Map(view.Transform, view.Center, Profile.CurrentProfile.UserID, DateTime.Now);

			} else {
				
				content = new Content ();

			}

			return content;
		}
	}
}

