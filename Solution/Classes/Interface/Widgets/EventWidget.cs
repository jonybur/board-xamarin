using System.Globalization;
using Board.Schema;
using CoreGraphics;
using UIKit;
using MGImageUtilitiesBinding;

namespace Board.Interface.Widgets
{
	public class EventWidget : Widget
	{
		// UIView contains ScrollView and BackButton
		// ScrollView contains LookUpImage

		public BoardEvent boardEvent
		{
			get { return (BoardEvent)content; }
		}

		public EventWidget()
		{

		}

		public EventWidget(BoardEvent ev)
		{
			content = ev;

			UIImageView calendarBox = CreateCalendarBox();
			UIImageView pictureBox = CreatePictureBox (calendarBox.Frame);

			var totalRect = new CGRect (calendarBox.Frame.X, calendarBox.Frame.Y, calendarBox.Frame.Width + pictureBox.Frame.Width + 5, calendarBox.Frame.Height);

			// mounting
			CreateMounting (totalRect);

			pictureBox.Center = new CGPoint (MountingView.Frame.Width - pictureBox.Frame.Width / 2 - SideMargin * 2, pictureBox.Center.Y);

			View = new UIView(MountingView.Frame);

			MountingView.AddSubviews (calendarBox, pictureBox);

			View.AddSubviews (MountingView);

			EyeOpen = false;

			CreateGestures ();
		}

		private UIImageView CreateCalendarBox()
		{
			var box = new UIImageView (new CGRect(SideMargin, TopMargin, 100, 140));

			// empieza en 0 termina en 24
			UILabel dayName = new UILabel (new CGRect (10, 0, 80, 30));
			dayName.Font = AppDelegate.SystemFontOfSize18;
			dayName.Text = boardEvent.StartDate.DayOfWeek.ToString();
			dayName.TextAlignment = UITextAlignment.Center;
			dayName.TextColor = Widget.HighlightColor;
			dayName.AdjustsFontSizeToFitWidth = true;

			// empieza en 26 termina en 56
			UILabel timeLabel = new UILabel (new CGRect (25, 32, 80, 30));
			timeLabel.Font = UIFont.SystemFontOfSize (14);
			timeLabel.Text = boardEvent.StartDate.ToString("t");
			timeLabel.TextAlignment = UITextAlignment.Center;
			timeLabel.SizeToFit ();
			timeLabel.TextColor = Widget.HighlightColor;
			timeLabel.AdjustsFontSizeToFitWidth = true;

			UIImageView timeView = new UIImageView (new CGRect (0, 0, 10, 10));
			using (UIImage img = UIImage.FromFile ("./boardinterface/widget/time.png")) {
				timeView.Image = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				timeView.TintColor = Widget.HighlightColor;
				timeView.Center = new CGPoint (timeLabel.Frame.Left - 10, timeLabel.Center.Y);
			}

			// empieza en 55 termina en 110
			UILabel dayNumber = new UILabel (new CGRect (0, 55, 100, 55));
			dayNumber.Font = UIFont.SystemFontOfSize (60);
			dayNumber.Text = boardEvent.StartDate.Day.ToString();
			dayNumber.AdjustsFontSizeToFitWidth = true;
			dayNumber.TextColor = Widget.HighlightColor;
			dayNumber.TextAlignment = UITextAlignment.Center;

			// empieza en 105 termina en 135
			UILabel monthName = new UILabel (new CGRect (10, 110, 80, 30));
			monthName.Font = AppDelegate.SystemFontOfSize16;
			int monthNumber = boardEvent.StartDate.Month;
			monthName.Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthNumber).ToUpper();
			monthName.TextAlignment = UITextAlignment.Center;
			monthName.TextColor = Widget.HighlightColor;
			monthName.AdjustsFontSizeToFitWidth = true;

			box.AddSubviews (dayName, timeView, timeLabel, dayNumber, monthName);

			return box;
		}

		private UIImageView CreatePictureBox(CGRect calendarBoxFrame)
		{
			var box = new UIImageView (new CGRect (calendarBoxFrame.Right + 10, calendarBoxFrame.Top, 100, calendarBoxFrame.Height));
			box.BackgroundColor = UIColor.White;

			float imgw, imgh;
			const float autosize = 100;

			float scale = (float)(boardEvent.ImageView.Frame.Width/boardEvent.ImageView.Frame.Height);

			if (scale >= 1) {
				scale = (float)(boardEvent.ImageView.Frame.Height/boardEvent.ImageView.Frame.Width);
				imgw = (float)box.Frame.Width;
				imgh = imgw * scale;

			} else {
				imgh = (float)box.Frame.Height;
				imgw = imgh * scale;
			}

			var eventPoster = new UIImageView (new CGRect(0, 0, imgw, imgh));
			eventPoster.Image = boardEvent.ImageView.Image.ImageScaledToFitSize(eventPoster.Frame.Size);
			eventPoster.Center = new CGPoint (box.Frame.Width / 2, box.Frame.Height / 2);
			eventPoster.Layer.AllowsEdgeAntialiasing = true;

			box.AddSubview (eventPoster);

			return box;
		}

		public void SetFrame(CGRect frame)
		{
			View.Frame = frame;
		}

	}
}

