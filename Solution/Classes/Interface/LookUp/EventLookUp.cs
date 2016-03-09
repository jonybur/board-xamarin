using Board.Schema;
using CoreGraphics;
using System.Globalization;
using Facebook.CoreKit;
using UIKit;
using MonoTouch.Dialog;
using System;
using Foundation;
using EventKit;
using EventKitUI;

namespace Board.Interface.LookUp
{
	public class EventLookUp : LookUp
	{
		UIImageView PictureBox;
		UIImageView CalendarBox;
		UITextView DescriptionBox;

		public EventLookUp(BoardEvent boardEvent)
		{
			content = boardEvent;

			View.BackgroundColor = UIColor.FromRGB(250,250,250);

			CreateButtons (UIColor.Black);

			ScrollView = new UIScrollView (new CGRect (10, TrashButton.Frame.Bottom,
				AppDelegate.ScreenWidth - 20,
				AppDelegate.ScreenHeight - TrashButton.Frame.Bottom - LikeButton.Frame.Height));
			ScrollView.UserInteractionEnabled = true;

			View.AddSubviews (ScrollView, BackButton, LikeButton, FacebookButton, ShareButton, TrashButton);

			PictureBox = CreatePictureBox ();
			CalendarBox = CreateCalendarBox();
			DescriptionBox = CreateTextView (boardEvent.Description);

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => CreateEvent (boardEvent));
			CalendarBox.AddGestureRecognizer (tap);
			CalendarBox.UserInteractionEnabled = true;

			ScrollView.ScrollEnabled = true;
			ScrollView.ContentSize = new CGSize (ScrollView.Frame.Width, DescriptionBox.Frame.Bottom + 10);

			ScrollView.AddSubviews (PictureBox, CalendarBox, DescriptionBox);
		}

		private UITextView CreateTextView(string text){

			UITextView textView = new UITextView(new CGRect (0,
				CalendarBox.Frame.Bottom + 10,
				0,
				0));

			textView.AttributedText = new NSAttributedString(text);
			textView.Font = UIFont.SystemFontOfSize (16);
			textView.Editable = false;
			textView.ScrollEnabled = false;
			textView.Selectable = true;
			textView.DataDetectorTypes = UIDataDetectorType.Link;
			textView.UserInteractionEnabled = true;
			textView.TextColor = UIColor.Black;
			textView.BackgroundColor = UIColor.FromRGBA (250, 250, 250, 0);
			textView.SizeToFit ();
			textView.Frame = new CGRect (textView.Frame.X, textView.Frame.Y, PictureBox.Frame.Width, textView.Frame.Height);

			return textView;

		}

		private UIImageView CreatePictureBox()
		{
			CGSize imageSize = ((BoardEvent)content).ImageView.Frame.Size;

			CGSize posterSize = new CGSize (AppDelegate.ScreenWidth - 20, 500);

			float imgw, imgh;

			float scale = (float)(imageSize.Width/imageSize.Height);

			if (scale < 1) {
				scale = (float)(imageSize.Height/imageSize.Width);
				imgw = (float)posterSize.Width;
				imgh = imgw * scale;
			} else {
				imgh = (float)posterSize.Height;
				imgw = imgh * scale;
			}

			UIImageView box = new UIImageView (new CGRect (0, 0,
				imgw, imgh));
			box.Image = ((BoardEvent)content).ImageView.Image;

			return box;
		}

		private UILabel CreateCalendarLabel(string dateString, CGRect topFrame)
		{
			UIFont font = UIFont.BoldSystemFontOfSize (18);
			//string dateString = boardEvent.Date.ToString ("f");
			UILabel dateLabel = new UILabel (new CGRect (topFrame.X, topFrame.Bottom + 20, topFrame.Width, dateString.StringSize (font).Height));
			dateLabel.Text = dateString;
			dateLabel.TextColor = BoardInterface.board.MainColor;
			dateLabel.Font = font;
			dateLabel.AdjustsFontSizeToFitWidth = true;

			return dateLabel;
		}

		private UIImageView CreateCalendarBox()
		{
			UIImageView box = new UIImageView (new CGRect(10, PictureBox.Frame.Bottom + 20, PictureBox.Frame.Width, 90));

			// empieza en 55 termina en 110
			UILabel dayNumber = new UILabel (new CGRect (0, 0, 90, 74));
			dayNumber.Font = UIFont.SystemFontOfSize (74);
			dayNumber.Text = ((BoardEvent)content).StartDate.Day.ToString();
			dayNumber.AdjustsFontSizeToFitWidth = true;
			dayNumber.TextAlignment = UITextAlignment.Center;
			dayNumber.TextColor = BoardInterface.board.MainColor;
			dayNumber.TextAlignment = UITextAlignment.Center;

			// empieza en 0 termina en 24
			UILabel dayName = new UILabel (new CGRect (dayNumber.Frame.Right + 10, 0, 200, 30));
			dayName.Font = UIFont.SystemFontOfSize (16);
			dayName.Text = ((BoardEvent)content).StartDate.DayOfWeek.ToString();
			dayName.TextAlignment = UITextAlignment.Left;
			dayName.TextColor = BoardInterface.board.MainColor;
			dayName.AdjustsFontSizeToFitWidth = true;
			dayName.SizeToFit ();

			// empieza en 105 termina en 135
			UILabel monthName = new UILabel (new CGRect (dayName.Frame.Left, dayName.Frame.Bottom + 5, 100, 30));
			monthName.Font = UIFont.SystemFontOfSize (16);
			int monthNumber = ((BoardEvent)content).StartDate.Month;
			monthName.Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthNumber).ToUpper();
			monthName.TextAlignment = UITextAlignment.Left;
			monthName.TextColor = BoardInterface.board.MainColor;
			monthName.AdjustsFontSizeToFitWidth = true;
			monthName.SizeToFit ();


			// empieza en 105 termina en 135
			UILabel addToCalendar = new UILabel (new CGRect (dayName.Frame.Left, monthName.Frame.Bottom + 5, 100, 30));
			addToCalendar.Font = UIFont.BoldSystemFontOfSize (16);
			addToCalendar.Text = "ADD TO CALENDAR >";
			addToCalendar.TextAlignment = UITextAlignment.Left;
			addToCalendar.TextColor = BoardInterface.board.MainColor;
			addToCalendar.AdjustsFontSizeToFitWidth = true;
			addToCalendar.SizeToFit ();

			// empieza en 26 termina en 56
			UILabel timeLabel = new UILabel (new CGRect(dayName.Frame.Right + 20, 0, 200, 30));
			timeLabel.Font = UIFont.SystemFontOfSize (16);
			timeLabel.Text = ((BoardEvent)content).StartDate.ToString("t");
			timeLabel.TextAlignment = UITextAlignment.Left;
			timeLabel.TextColor = BoardInterface.board.MainColor;
			timeLabel.AdjustsFontSizeToFitWidth = true;
			timeLabel.SizeToFit ();

			UIImageView timeView = new UIImageView (new CGRect (0, 0, 10, 10));
			using (UIImage img = UIImage.FromFile ("./boardinterface/widget/time.png")) {
				timeView.Image = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				timeView.TintColor = BoardInterface.board.MainColor;
				timeView.Center = new CGPoint (timeLabel.Frame.Left - 10, timeLabel.Center.Y);
			}


			box.AddSubviews (dayName, timeView, timeLabel, dayNumber, monthName, addToCalendar);

			return box;
		}

		private void CreateEvent(BoardEvent boardEvent)
		{
			App.Current.EventStore.RequestAccess (EKEntityType.Event, (granted, e) => {});

			if (EKEventStore.GetAuthorizationStatus (EKEntityType.Event) == EKAuthorizationStatus.Authorized) {
				Console.WriteLine (EKEventStore.GetAuthorizationStatus (EKEntityType.Event) == EKAuthorizationStatus.Authorized);

				EKEventEditViewController eventController = new EKEventEditViewController ();
				 
				// set the controller's event store - it needs to know where/how to save the event
				eventController.EventStore = App.Current.EventStore;

				CreateEventEditViewDelegate eventControllerDelegate = new CreateEventEditViewDelegate (eventController);
				eventController.EditViewDelegate = eventControllerDelegate;

				EKEvent newEvent = EKEvent.FromStore (App.Current.EventStore);

				NSDate startNSDate = (NSDate)DateTime.SpecifyKind (boardEvent.StartDate, DateTimeKind.Local);
				NSDate endNSDate = (NSDate)DateTime.SpecifyKind (boardEvent.EndDate, DateTimeKind.Local);

				// set the alarm for 10 minutes from now
				newEvent.AddAlarm (EKAlarm.FromDate (startNSDate));

				// make the event start 20 minutes from now and last 30 minutes
				newEvent.StartDate = startNSDate;
				newEvent.EndDate = endNSDate;
				newEvent.Title = boardEvent.Name + " @ " + BoardInterface.board.Name;

				string signature = "- Created with Board";

				if (boardEvent.Description != string.Empty) {
					signature = "\n\n" + signature;
				}
					
				newEvent.Notes = boardEvent.Description + signature;
				newEvent.Location = BoardInterface.board.Location;

				eventController.Event = newEvent;

				// show the event controller
				PresentViewController (eventController, true, null);
			} else if (EKEventStore.GetAuthorizationStatus(EKEntityType.Event) == EKAuthorizationStatus.Denied) {
				UIAlertController alert = UIAlertController.Create("Please authorize Board to use Calendar", "The option is on Settings -> Privacy -> Calendar", UIAlertControllerStyle.Alert);

				alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));

				PresentViewController (alert, true, null);
			}
		}
	}

	public class CreateEventEditViewDelegate : EKEventEditViewDelegate
	{
		// we need to keep a reference to the controller so we can dismiss it
		protected EKEventEditViewController eventController;

		public CreateEventEditViewDelegate (EKEventEditViewController eventController)
		{
			// save our controller reference
			this.eventController = eventController;
		}

		// completed is called when a user eith
		public override void Completed (EKEventEditViewController controller, EKEventEditViewAction action)
		{
			eventController.DismissViewController (true, null);
		}
	}

	public class App
	{
		public static App Current {
			get { return current; }
		}
		private static App current;

		public EKEventStore EventStore {
			get { return eventStore; }
		}
		protected EKEventStore eventStore;

		static App ()
		{
			current = new App();
		}
		protected App () 
		{
			eventStore = new EKEventStore ( );
		}
	}
}

