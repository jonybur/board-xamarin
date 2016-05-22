using System.Globalization;
using Board.Schema;
using CoreGraphics;
using UIKit;
using System.Threading.Tasks;
using MGImageUtilitiesBinding;

namespace Board.Interface.Widgets
{
	public class EventWidget : Widget
	{
		// UIView contains ScrollView and BackButton
		// ScrollView contains LookUpImage

		UIImageView PictureBox, CalendarBox;

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

			CalendarBox = CreateCalendarBox();
			//PictureBox = CreatePictureBox (calendarBox.Frame);

			var totalRect = new CGRect (CalendarBox.Frame.X, CalendarBox.Frame.Y, CalendarBox.Frame.Width + 100 + 5, CalendarBox.Frame.Height);

			// mounting
			CreateMounting (totalRect.Size);

			//PictureBox.Center = new CGPoint (MountingView.Frame.Width - PictureBox.Frame.Width / 2 - SideMargin * 2, PictureBox.Center.Y);

			Frame = MountingView.Frame;

			MountingView.AddSubview (CalendarBox);

			AddSubview (MountingView);

			EyeOpen = false;

			CreateGestures ();
		}

		public async Task Initialize(){
			if (boardEvent.Image == null || boardEvent.Thumbnail == null) {
				await boardEvent.GetImageFromUrl (boardEvent.ImageUrl);
			}

			UnsuscribeToEvents ();

			var size = boardEvent.Thumbnail.Size;

			// picture

			PictureBox = CreatePictureBox ();/*new UIImageView ();
			PictureBox.Frame = new CGRect (CalendarBox.Frame.Right + 10, CalendarBox.Frame.Top, 100, CalendarBox.Frame.Height);
			PictureBox.Image = boardEvent.Thumbnail;*/
			PictureBox.Layer.AllowsEdgeAntialiasing = true;
			MountingView.AddSubview (PictureBox);

			EyeOpen = false;

			AppDelegate.BoardInterface.BoardScroll.SelectiveRendering ();

			SuscribeToEvents ();
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

		private UIImageView CreatePictureBox()
		{
			var box = new UIImageView (new CGRect (100 + SideMargin, TopMargin, 100, 140));
			box.BackgroundColor = UIColor.White;

			float imgw, imgh;
			const float autosize = 100;

			float scale = (float)(boardEvent.Image.Size.Width/boardEvent.Image.Size.Height);

			if (scale >= 1) {
				scale = (float)(boardEvent.Image.Size.Height/boardEvent.Image.Size.Width);
				imgw = (float)box.Frame.Width;
				imgh = imgw * scale;

			} else {
				imgh = (float)box.Frame.Height;
				imgw = imgh * scale;
			}

			var eventPoster = new UIImageView (new CGRect(0, 0, imgw, imgh));
			eventPoster.Image = boardEvent.Thumbnail.ImageScaledToFitSize(eventPoster.Frame.Size);
			eventPoster.Center = new CGPoint (box.Frame.Width / 2, box.Frame.Height / 2);
			eventPoster.Layer.AllowsEdgeAntialiasing = true;

			box.AddSubview (eventPoster);

			return box;
		}

		public void SetFrame(CGRect frame)
		{
			Frame = frame;
		}

	}
}

