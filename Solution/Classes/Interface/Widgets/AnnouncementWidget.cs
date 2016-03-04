using Board.Schema;
using System;
using CoreGraphics;
using UIKit;
using Board.Interface.LookUp;

namespace Board.Interface.Widgets
{
	public class AnnouncementWidget : Widget
	{
		// UIView contains ScrollView and BackButton
		// ScrollView contains LookUpImage
		private UITextView textview;

		public Announcement announcement
		{
			get { return (Announcement)content; }
		}

		public AnnouncementWidget()
		{

		}

		public AnnouncementWidget(Announcement ann)
		{
			content = ann;

			UITextView insideText = CreateText ();

			// mounting
			CreateMounting (insideText.Frame);
			View = new UIView(MountingView.Frame);

			View.AddSubviews (MountingView, insideText);

			View.Frame = new CGRect (ann.Frame.X, ann.Frame.Y, MountingView.Frame.Width, MountingView.Frame.Height);
			View.Transform = CGAffineTransform.MakeRotation(ann.Rotation);

			EyeOpen = false;

			CreateGestures ();
		}

		private void CreateGestures()
		{
			UITapGestureRecognizer doubleTap = CreateDoubleTapToLikeGesture ();

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				if (Preview.View != null) { return; }

				tg.NumberOfTapsRequired = 1; 

				if (LikeComponent.Frame.Left < tg.LocationInView(this.View).X &&
					LikeComponent.Frame.Top < tg.LocationInView(this.View).Y)
				{
					Like();
				}
				else{
					AnnouncementLookUp lookUp = new AnnouncementLookUp(announcement);
					AppDelegate.NavigationController.PresentViewController(lookUp, true, null);
				}
			});

			tap.DelaysTouchesBegan = true;
			doubleTap.DelaysTouchesBegan = true;

			tap.RequireGestureRecognizerToFail (doubleTap);

			GestureRecognizers.Add (tap);
			GestureRecognizers.Add (doubleTap);
		}

		public void ScrollEnabled(bool value)
		{
			textview.ScrollEnabled = value;
		}

		private UITextView CreateText()
		{
			textview = new UITextView ();
			textview.Editable = false;
			textview.Selectable = true;
			textview.ScrollEnabled = true;
			textview.BackgroundColor = UIColor.FromRGBA (250, 250, 250, 0);
			textview.AttributedText = announcement.Text;
			textview.SizeToFit ();
			textview.TextColor = BoardInterface.board.MainColor;

			if (textview.Frame.Width < 160) {
				textview.Frame = new CGRect (10, 10, 160, textview.Frame.Height);
			} else if (textview.Frame.Width > 250) {
				textview.Frame = new CGRect (10, 10, 250, textview.Frame.Height);
			}

			if (textview.Frame.Height < 80) {
				textview.Frame = new CGRect (10, 10, textview.Frame.Width, 80);
			} else if (textview.Frame.Height > 180) {
				textview.Frame = new CGRect (10, 10, textview.Frame.Width, 180);
			}

			return textview;
		}

		public void SetFrame(CGRect frame)
		{
			View.Frame = frame;
		}

	}
}

