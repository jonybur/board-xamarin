using Board.Schema;
using CoreGraphics;
using System.Globalization;
using UIKit;
using System;
using Foundation;
using EventKit;
using EventKitUI;

namespace Board.Interface.LookUp
{
	public class EventLookUp : LookUp
	{
		UIImageView PictureBox;
		UILabel NameLabel;
		UIImageView CalendarBox;
		UITextView DescriptionBox;

		public EventLookUp(BoardEvent boardEvent)
		{
			content = boardEvent;

			UIColor backColor = UIColor.FromRGB(250,250,250);
			UIColor frontColor = AppDelegate.BoardBlack;

			//View.BackgroundColor = UIColor.FromRGB(250,250,250);
			View.BackgroundColor = backColor;

			CreateButtons (frontColor);

			ScrollView = new UIScrollView (new CGRect (10, TrashButton.Frame.Bottom,
				AppDelegate.ScreenWidth - 20,
				AppDelegate.ScreenHeight - TrashButton.Frame.Bottom - LikeButton.Frame.Height));
			ScrollView.UserInteractionEnabled = true;

			View.AddSubviews (ScrollView, BackButton, LikeButton, FacebookButton, ShareButton, TrashButton);

			PictureBox = CreatePictureBox ();
			NameLabel = CreateNameLabel (boardEvent.Name, backColor);
			CalendarBox = CreateCalendarBox(backColor);
			DescriptionBox = CreateTextView (boardEvent.Description, frontColor);

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => CreateEvent (boardEvent));
			CalendarBox.AddGestureRecognizer (tap);
			CalendarBox.UserInteractionEnabled = true;

			ScrollView.ScrollEnabled = true;
			ScrollView.ContentSize = new CGSize (ScrollView.Frame.Width, DescriptionBox.Frame.Bottom + 30);

			UIImageView calendarBoxBackground = new UIImageView(new CGRect(0, PictureBox.Frame.Bottom, AppDelegate.ScreenWidth, (CalendarBox.Frame.Bottom + 30) - PictureBox.Frame.Bottom));
			calendarBoxBackground.BackgroundColor = AppDelegate.BoardBlack;

			ScrollView.AddSubviews (PictureBox, calendarBoxBackground, NameLabel, CalendarBox, DescriptionBox);
		}

		private UILabel CreateNameLabel(string name, UIColor color)
		{
			UILabel label = new UILabel (new CGRect(0, PictureBox.Frame.Bottom + 20, PictureBox.Frame.Width, 25));
			label.Text = name;
			label.Font = UIFont.BoldSystemFontOfSize (20);
			label.TextColor = color;
			label.TextAlignment = UITextAlignment.Center;
			label.AdjustsFontSizeToFitWidth = true;
			return label;
		}

		private UITextView CreateTextView(string text, UIColor color){

			UITextView textView = new UITextView(new CGRect (0,
				CalendarBox.Frame.Bottom + 45,
				0,
				0));

			textView.AttributedText = new NSAttributedString(text);
			textView.Font = AppDelegate.SystemFontOfSize16;
			textView.Editable = false;
			textView.ScrollEnabled = false;
			textView.Selectable = true;
			textView.DataDetectorTypes = UIDataDetectorType.Link;
			textView.UserInteractionEnabled = true;
			textView.TextColor = color;
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

			float scale = (float)(imageSize.Height/imageSize.Width);
			imgw = (float)posterSize.Width;
			imgh = imgw * scale;

			UIImageView box = new UIImageView (new CGRect (0, 0,
				imgw, imgh));
			box.Image = ((BoardEvent)content).ImageView.Image;

			return box;
		}

		private UIImageView CreateCalendarBox(UIColor color)
		{
			UIImageView box = new UIImageView (new CGRect(20, NameLabel.Frame.Bottom + 30, PictureBox.Frame.Width - 40, 90));

			UILabel monthName = new UILabel (new CGRect (0, 0, 90, 16));
			monthName.Font = AppDelegate.SystemFontOfSize16;
			int monthNumber = ((BoardEvent)content).StartDate.Month;
			monthName.Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthNumber).ToUpper();
			monthName.TextAlignment = UITextAlignment.Center;
			monthName.TextColor = color;
			monthName.AdjustsFontSizeToFitWidth = true;

			// empieza en 55 termina en 110
			UILabel dayNumber = new UILabel (new CGRect (0, monthName.Frame.Bottom + 5, 90, 74));
			dayNumber.Font = UIFont.SystemFontOfSize (74);
			dayNumber.Text = ((BoardEvent)content).StartDate.Day.ToString();
			dayNumber.AdjustsFontSizeToFitWidth = true;
			dayNumber.TextAlignment = UITextAlignment.Center;
			dayNumber.TextColor = color;
			dayNumber.TextAlignment = UITextAlignment.Center;

			// empieza en 0 termina en 24
			UILabel dayName = new UILabel (new CGRect (dayNumber.Frame.Right + 15, dayNumber.Frame.Top, 200, 30));
			dayName.Font = AppDelegate.SystemFontOfSize16;
			dayName.Text = "Starts " + ((BoardEvent)content).StartDate.DayOfWeek.ToString();
			dayName.TextAlignment = UITextAlignment.Left;
			dayName.TextColor = color;
			dayName.AdjustsFontSizeToFitWidth = true;
			dayName.SizeToFit ();

			UILabel endName = new UILabel (new CGRect (dayName.Frame.Left, dayName.Frame.Bottom + 5, 100, 30));
			endName.Font = AppDelegate.SystemFontOfSize16;
			string endString = "Ends";
			if (((BoardEvent)content).EndDate.DayOfWeek.ToString() != ((BoardEvent)content).StartDate.DayOfWeek.ToString())
			{
				endString += " " + ((BoardEvent)content).EndDate.DayOfWeek.ToString();
			}
			endName.Text = endString;
			endName.TextAlignment = UITextAlignment.Left;
			endName.TextColor = color;
			endName.AdjustsFontSizeToFitWidth = true;
			endName.SizeToFit ();

			// empieza en 105 termina en 135
			/**/

			// empieza en 105 termina en 135
			UILabel addToCalendar = new UILabel (new CGRect (dayName.Frame.Left, endName.Frame.Bottom + 5, 100, 30));
			addToCalendar.Font = UIFont.BoldSystemFontOfSize (16);
			addToCalendar.Text = "ADD TO CALENDAR >";
			addToCalendar.TextAlignment = UITextAlignment.Left;
			addToCalendar.TextColor = color;
			addToCalendar.AdjustsFontSizeToFitWidth = true;
			addToCalendar.SizeToFit ();

			// empieza en 26 termina en 56
			UILabel timeLabel = new UILabel (new CGRect(dayName.Frame.Right + 20, dayName.Frame.Top, 200, 30));
			timeLabel.Font = AppDelegate.SystemFontOfSize16;
			timeLabel.Text = ((BoardEvent)content).StartDate.ToString("t");
			timeLabel.TextAlignment = UITextAlignment.Left;
			timeLabel.TextColor = color;
			timeLabel.AdjustsFontSizeToFitWidth = true;
			timeLabel.SizeToFit ();

			UIImageView timeView = new UIImageView (new CGRect (0, 0, 10, 10));
			using (UIImage img = UIImage.FromFile ("./boardinterface/widget/time.png")) {
				timeView.Image = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				timeView.TintColor = color;
				timeView.Center = new CGPoint (timeLabel.Frame.Left - 10, timeLabel.Center.Y);
			}

			// empieza en 26 termina en 56
			UILabel timeEndLabel = new UILabel (new CGRect(endName.Frame.Right + 20, endName.Frame.Top, 200, 30));
			timeEndLabel.Font = AppDelegate.SystemFontOfSize16;
			timeEndLabel.Text = ((BoardEvent)content).EndDate.ToString("t");
			timeEndLabel.TextAlignment = UITextAlignment.Left;
			timeEndLabel.TextColor = color;
			timeEndLabel.AdjustsFontSizeToFitWidth = true;
			timeEndLabel.SizeToFit ();

			UIImageView timeEndView = new UIImageView (new CGRect (0, 0, 10, 10));
			using (UIImage img = UIImage.FromFile ("./boardinterface/widget/time.png")) {
				timeEndView.Image = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				timeEndView.TintColor = color;
				timeEndView.Center = new CGPoint (timeEndLabel.Frame.Left - 10, timeEndLabel.Center.Y);
			}


			box.AddSubviews (monthName, dayName, timeView, timeLabel, dayNumber, endName, timeEndLabel, timeEndView, addToCalendar);

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

