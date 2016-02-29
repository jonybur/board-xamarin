using Board.Schema;
using System.Globalization;
using CoreGraphics;
using System;
using UIKit;

namespace Board.Interface.Widgets
{
	public class EventWidget : Widget
	{
		// UIView contains ScrollView and BackButton
		// ScrollView contains LookUpImage
		private BoardEvent boardEvent;

		public BoardEvent BoardEvent
		{
			get { return boardEvent; }
		}

		public EventWidget()
		{

		}

		public EventWidget(BoardEvent ev)
		{
			boardEvent = ev;

			UIImageView calendarBox = CreateCalendarBox();
			UIImageView pictureBox = CreatePictureBox (calendarBox.Frame);

			CGRect totalRect = new CGRect (calendarBox.Frame.X, calendarBox.Frame.Y, calendarBox.Frame.Width + pictureBox.Frame.Width + 10, calendarBox.Frame.Height);

			// mounting
			UIImageView mounting = CreateMounting (totalRect);
			View = new UIView(mounting.Frame);
			View.AddSubviews (mounting, calendarBox, pictureBox);

			// like
			UIImageView like = CreateLike (mounting.Frame);
			View.AddSubview (like);

			// like label

			UILabel likeLabel = CreateLikeLabel (like.Frame);
			View.AddSubview (likeLabel);

			// eye
			eye = CreateEye (mounting.Frame);

			View.AddSubview (eye);

			View.Frame = new CGRect (boardEvent.Frame.X, boardEvent.Frame.Y, mounting.Frame.Width, mounting.Frame.Height);
			View.Transform = CGAffineTransform.MakeRotation(boardEvent.Rotation);

			EyeOpen = false;
		}

		private UIImageView CreateCalendarBox()
		{
			UIImageView box = new UIImageView (new CGRect(10, 10, 100, 140));

			// empieza en 0 termina en 24
			UILabel dayName = new UILabel (new CGRect (10, 0, 80, 30));
			dayName.Font = UIFont.SystemFontOfSize (24);
			dayName.Text = boardEvent.Date.DayOfWeek.ToString();
			dayName.TextAlignment = UITextAlignment.Center;
			dayName.TextColor = BoardInterface.board.MainColor;
			dayName.AdjustsFontSizeToFitWidth = true;

			// empieza en 40 termina en 100 y
			UILabel dayNumber = new UILabel (new CGRect (0, 40, 100, 60));
			dayNumber.Font = UIFont.SystemFontOfSize (60);
			dayNumber.Text = boardEvent.Date.Day.ToString();
			dayNumber.AdjustsFontSizeToFitWidth = true;
			dayNumber.TextColor = BoardInterface.board.MainColor;
			dayNumber.TextAlignment = UITextAlignment.Center;

			// empieza en 105 termina en 135
			UILabel monthName = new UILabel (new CGRect (10, 105, 80, 30));
			monthName.Font = UIFont.SystemFontOfSize (24);
			int monthNumber = boardEvent.Date.Month;
			monthName.Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthNumber).ToUpper();
			monthName.TextAlignment = UITextAlignment.Center;
			monthName.TextColor = BoardInterface.board.MainColor;
			monthName.AdjustsFontSizeToFitWidth = true;

			box.AddSubviews (dayName, dayNumber, monthName);

			return box;
		}

		private UIImageView CreatePictureBox(CGRect calendarBoxFrame)
		{
			UIImageView box = new UIImageView (new CGRect (calendarBoxFrame.Right + 10, calendarBoxFrame.Top, 100, calendarBoxFrame.Height));

			float imgw, imgh;
			float autosize = (float)calendarBoxFrame.Width;

			float scale = (float)(boardEvent.ImageView.Frame.Width/boardEvent.ImageView.Frame.Height);

			if (scale >= 1) {
				imgw = autosize * scale;
				imgh = autosize;

				if (imgw > AppDelegate.ScreenWidth) {
					scale = (float)(boardEvent.ImageView.Frame.Height/boardEvent.ImageView.Frame.Width);
					imgw = AppDelegate.ScreenWidth;
					imgh = imgw * scale;
				}
			} else {
				scale = (float)(boardEvent.ImageView.Frame.Height/boardEvent.ImageView.Frame.Width);
				imgw = autosize;
				imgh = autosize * scale;
			}

			UIImageView eventPoster = new UIImageView (new CGRect(0, 0, imgw, imgh));
			eventPoster.Image = boardEvent.ImageView.Image;
			eventPoster.Center = new CGPoint (calendarBoxFrame.Width / 2, calendarBoxFrame.Height / 2);

			box.AddSubview (eventPoster);

			return box;
		}

		private UIImageView CreateMounting(CGRect frame)
		{
			CGRect mountingFrame = new CGRect (0, 0, frame.Width + 20, frame.Height + 50);

			UIImageView mountingView = CreateColorView (mountingFrame, UIColor.FromRGB(250,250,250).CGColor);

			return mountingView;
		}

		private UILabel CreateLikeLabel(CGRect frame)
		{
			UIFont likeFont = UIFont.SystemFontOfSize (20);
			Random rand = new Random ();
			string likeText = rand.Next(16, 98).ToString();
			CGSize likeLabelSize = likeText.StringSize (likeFont);
			UILabel likeLabel = new UILabel(new CGRect(frame.X - likeLabelSize.Width - 4, frame.Y + 4, likeLabelSize.Width, likeLabelSize.Height));
			likeLabel.TextColor = BoardInterface.board.MainColor;
			likeLabel.Font = likeFont;
			likeLabel.Text = likeText;
			likeLabel.TextAlignment = UITextAlignment.Right;

			return likeLabel;
		}

		private UIImageView CreateEye(CGRect frame)
		{
			CGSize iconSize = new CGSize (30, 30);

			UIImageView eyeView = new UIImageView(new CGRect (frame.X + 10, frame.Height - iconSize.Height - 5, iconSize.Width, iconSize.Height));
			eyeView.Image = Widget.ClosedEyeImage;
			eyeView.TintColor = UIColor.FromRGB(140,140,140);

			return eyeView;
		}

		private UIImageView CreateLike(CGRect frame)
		{
			CGSize iconSize = new CGSize (30, 30);

			UIImageView likeView = new UIImageView(new CGRect (frame.Width - iconSize.Width - 10,
				frame.Height - iconSize.Height - 5, iconSize.Width, iconSize.Height));
			likeView.Image = UIImage.FromFile ("./boardinterface/widget/like.png");
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

		public void SetFrame(CGRect frame)
		{
			View.Frame = frame;
		}

	}
}

