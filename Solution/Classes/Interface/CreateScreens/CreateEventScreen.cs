using Board.Schema;
using Board.Facebook;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using System;
using Board.Utilities;
using Board.Interface.Buttons;
using Foundation;
using Board.Picker;
using BigTed;

namespace Board.Interface.CreateScreens
{
	public class CreateEventScreen : CreateScreen
	{
		EventHandler nextButtonTap;
		UIImageView PictureBox;
		UITextField NameLabel;
		PlaceholderTextView DescriptionView;
		UITapGestureRecognizer scrollViewTap;
		UIWindow window;

		UITextField StartDateView;
		UITextField EndDateView;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			content = new BoardEvent ();

			LoadContent ();

			string imagePath = "./boardinterface/screens/event/banner/" + AppDelegate.PhoneVersion + ".jpg";

			LoadBanner (imagePath, "events", LoadFromFacebookEvent);
			LoadPictureBox ();
			LoadNameLabel ((float) PictureBox.Frame.Bottom + 50);
			LoadTextView ((float) NameLabel.Frame.Bottom + 10);
			LoadStartDateView ((float) DescriptionView.Frame.Bottom + 30);
			LoadEndDateView ((float) StartDateView.Frame.Bottom + 20);
			LoadPostToButtons ((float) EndDateView.Frame.Bottom + 50);
			LoadNextButton ();

			AdjustContentSize ();

			CreateGestures ();
		}

		public override void ViewDidAppear(bool animated){
			base.ViewDidAppear (animated);
			NextButton.TouchUpInside += nextButtonTap;
			ScrollView.AddGestureRecognizer (scrollViewTap);
		}

		public override void ViewDidDisappear(bool animated){
			base.ViewDidDisappear (animated);
			NextButton.TouchUpInside -= nextButtonTap;
			ScrollView.RemoveGestureRecognizer (scrollViewTap);
		}

		private void LoadEndDateView(float yPosition)
		{
			UIDatePicker DatePicker = new UIDatePicker (new CGRect (0, AppDelegate.ScreenHeight - 250, AppDelegate.ScreenWidth, 250));
			DatePicker.TimeZone = NSTimeZone.LocalTimeZone;
			DatePicker.Date = (NSDate)DateTime.Now;
			DatePicker.BackgroundColor = UIColor.White;
			DatePicker.Mode = UIDatePickerMode.DateAndTime;
			DatePicker.MinimumDate = (NSDate)DateTime.Now;
			DatePicker.ValueChanged += (object sender, EventArgs e) => {
				EndDateView.Text = "Ends: " + ((DateTime)DatePicker.Date).ToString("g");
				((BoardEvent)content).EndDate = (DateTime)DatePicker.Date;
			};

			UIToolbar toolbar = new UIToolbar (new CGRect(0, DatePicker.Frame.Top- 40, AppDelegate.ScreenWidth, 40));
			UIBarButtonItem btnLeft = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, null);
			UIBarButtonItem btnCenter = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, null);
			UIBarButtonItem btnRight = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate(object sender, EventArgs e) {
				EndDateView.ResignFirstResponder();
				toolbar.Alpha = 0f;
			});
			toolbar.SetItems (new []{btnLeft, btnCenter, btnRight}, true);
			toolbar.UserInteractionEnabled = true;
			toolbar.Alpha = 0f;
			View.AddSubview (toolbar);

			EndDateView = new UITextField(new CGRect (15, yPosition, AppDelegate.ScreenWidth - 30, 24));
			EndDateView.InputView = DatePicker;
			EndDateView.BackgroundColor = UIColor.White;
			EndDateView.Font = UIFont.SystemFontOfSize (18);

			var placeholderAttribute = new UIStringAttributes {
				Font = UIFont.SystemFontOfSize (18),
				ForegroundColor = UIColor.FromRGB(101, 149, 183)
			};
			var prettyString = new NSMutableAttributedString ("End Date");
			prettyString.SetAttributes (placeholderAttribute, new NSRange (0, prettyString.Length));
			EndDateView.AttributedPlaceholder = prettyString;

			EndDateView.TextColor = AppDelegate.BoardBlue;
			EndDateView.Started += (object sender, EventArgs e) => {
				toolbar.Alpha = 1f;
				ScrollView.SetContentOffset(new CGPoint(0, EndDateView.Frame.Y - 200), true);
			};

			ScrollView.AddSubview (EndDateView);
		}

		private void LoadStartDateView(float yPosition)
		{
			UIDatePicker DatePicker = new UIDatePicker (new CGRect (0, AppDelegate.ScreenHeight - 250, AppDelegate.ScreenWidth, 250));
			//DatePicker.TimeZone = NSTimeZone.LocalTimeZone;
			DatePicker.Date = (NSDate)DateTime.Now;
			DatePicker.BackgroundColor = UIColor.White;
			DatePicker.Mode = UIDatePickerMode.DateAndTime;
			DatePicker.MinimumDate = (NSDate)DateTime.Now;
			DatePicker.ValueChanged += (object sender, EventArgs e) => {
				StartDateView.Text = "Starts: " + DatePicker.Date.ToString();
				((BoardEvent)content).StartDate = (DateTime)DatePicker.Date;
			};

			UIToolbar toolbar = new UIToolbar (new CGRect(0, DatePicker.Frame.Top- 40, AppDelegate.ScreenWidth, 40));
			UIBarButtonItem btnLeft = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, null);
			UIBarButtonItem btnCenter = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, null);
			UIBarButtonItem btnRight = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate(object sender, EventArgs e) {
				StartDateView.ResignFirstResponder();
				toolbar.Alpha = 0f;
			});
			toolbar.SetItems (new []{btnLeft, btnCenter, btnRight}, true);
			toolbar.UserInteractionEnabled = true;
			toolbar.Alpha = 0f;
			View.AddSubview (toolbar);
		
			StartDateView = new UITextField(new CGRect (15, yPosition, AppDelegate.ScreenWidth - 30, 24));
			StartDateView.InputView = DatePicker;
			StartDateView.BackgroundColor = UIColor.White;
			StartDateView.Font = UIFont.SystemFontOfSize (18);

			var placeholderAttribute = new UIStringAttributes {
				Font = UIFont.SystemFontOfSize (18),
				ForegroundColor = UIColor.FromRGB(101, 149, 183)
			};
			var prettyString = new NSMutableAttributedString ("Start Date");
			prettyString.SetAttributes (placeholderAttribute, new NSRange (0, prettyString.Length));
			StartDateView.AttributedPlaceholder = prettyString;

			StartDateView.TextColor = AppDelegate.BoardBlue;
			StartDateView.Started += (object sender, EventArgs e) => {
				toolbar.Alpha = 1f;
				ScrollView.SetContentOffset(new CGPoint(0, StartDateView.Frame.Y - 200), true);
			};


			ScrollView.AddSubview (StartDateView);
		}

		private void LoadNameLabel(float yPosition)
		{
			NameLabel = new UITextField (new CGRect(15, yPosition, AppDelegate.ScreenWidth - 30, 20));
			NameLabel.Font = UIFont.SystemFontOfSize (18);

			var placeholderAttribute = new UIStringAttributes {
				Font = UIFont.SystemFontOfSize (18),
				ForegroundColor = UIColor.FromRGB(101, 149, 183)
			};
			var prettyString = new NSMutableAttributedString ("Event Name");
			prettyString.SetAttributes (placeholderAttribute, new NSRange (0, prettyString.Length));
			NameLabel.AttributedPlaceholder = prettyString;

			NameLabel.TextColor = AppDelegate.BoardBlue;
			NameLabel.KeyboardType = UIKeyboardType.Default;
			NameLabel.ReturnKeyType = UIReturnKeyType.Done;
			NameLabel.BackgroundColor = UIColor.White;

			NameLabel.ShouldReturn += (textField) => {
				NameLabel.ResignFirstResponder();
				return true;
			};

			ScrollView.AddSubview (NameLabel);
		}

		private void LoadTextView(float yPosition)
		{
			var frame = new CGRect(10, yPosition, 
				AppDelegate.ScreenWidth - 20,
				140);

			DescriptionView = new PlaceholderTextView(frame, "Write a description...");

			DescriptionView.TextColor = AppDelegate.BoardBlue;
			DescriptionView.KeyboardType = UIKeyboardType.Default;
			DescriptionView.ReturnKeyType = UIReturnKeyType.Default;
			DescriptionView.EnablesReturnKeyAutomatically = true;
			DescriptionView.AllowsEditingTextAttributes = true;
			DescriptionView.BackgroundColor = UIColor.White;
			DescriptionView.Font = UIFont.SystemFontOfSize (18);

			ScrollView.AddSubviews (DescriptionView);
		}
			
		private void LoadPictureBox()
		{
			PictureBox = new UIImageView (new CGRect (10, Banner.Frame.Bottom,
				AppDelegate.ScreenWidth - 20, 200));
			PictureBox.BackgroundColor = UIColor.FromRGB (100, 100, 100);

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				ImagePicker ip = new ImagePicker(SetImage, HideWindow);

				window = new UIWindow(new CGRect(0,0,AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
				window.RootViewController = new UIViewController();
				window.MakeKeyAndVisible();
				
				window.RootViewController.PresentViewController(ip.UIImagePicker, true, null);
			});

			PictureBox.UserInteractionEnabled = true;
			PictureBox.AddGestureRecognizer (tap);

			ScrollView.AddSubview (PictureBox);
		}

		private void SetImage(UIImage img)
		{
			CGSize imageSize = img.Size;

			CGSize posterSize = new CGSize (AppDelegate.ScreenWidth - 20, 500);

			float imgw, imgh;

			float scale = (float)(imageSize.Width/imageSize.Height);

			if (scale <= 1) {
				scale = (float)(imageSize.Height/imageSize.Width);
				imgw = (float)posterSize.Width;
				imgh = imgw * scale;
			} else {
				imgh = (float)posterSize.Height;
				imgw = imgh * scale;
			}

			PictureBox.Frame = new CGRect (10, Banner.Frame.Bottom, imgw, imgh);
			PictureBox.Image = img;

			AdjustContentSize ();
		}

		private void AdjustContentSize()
		{
			float yPosition = (float)PictureBox.Frame.Bottom + 50;

			NameLabel.Frame = new CGRect (NameLabel.Frame.X, yPosition, NameLabel.Frame.Width, NameLabel.Frame.Height);

			yPosition = (float)NameLabel.Frame.Bottom + 10;

			DescriptionView.Frame = new CGRect (DescriptionView.Frame.X, yPosition, DescriptionView.Frame.Width, DescriptionView.Frame.Height);

			yPosition = (float)DescriptionView.Frame.Bottom + 30;

			StartDateView.Frame = new CGRect (StartDateView.Frame.X, yPosition,
				StartDateView.Frame.Width, StartDateView.Frame.Height);

			yPosition = (float)StartDateView.Frame.Bottom + 20;

			EndDateView.Frame = new CGRect (EndDateView.Frame.X, yPosition,
				EndDateView.Frame.Width, EndDateView.Frame.Height);

			yPosition = (float)EndDateView.Frame.Bottom + 50;

			ShareButtons.View.Frame = new CGRect (ShareButtons.View.Frame.X, yPosition,
				ShareButtons.View.Frame.Width, ShareButtons.View.Frame.Height);

			ScrollView.ContentSize = new CGSize(AppDelegate.ScreenWidth, yPosition + ShareButtons.View.Frame.Height + NextButton.Frame.Height + 50);
		}
			
		private void HideWindow()
		{
			window.Hidden = true;
		}

		private void LoadFromFacebookEvent(FacebookElement FBElement)
		{
			ShareButtons.ActivateFacebook ();

			FacebookEvent FBEvent = (FacebookEvent)FBElement;
			content.FacebookId = FBEvent.Id;

			((BoardEvent)content).Name = FBEvent.Name;
			NameLabel.Text = FBEvent.Name;

			((BoardEvent)content).Description = FBEvent.Description;
			DescriptionView.SetText (FBEvent.Description);

			((BoardEvent)content).StartDate = Convert.ToDateTime (FBEvent.StartTime);
			StartDateView.Text = "Starts: " + ((BoardEvent)content).StartDate.ToString("g");

			((BoardEvent)content).EndDate = Convert.ToDateTime (FBEvent.EndTime);
			EndDateView.Text = "Ends: " +((BoardEvent)content).EndDate.ToString("g");

			BTProgressHUD.Show ();
			FacebookUtils.MakeGraphRequest (FBEvent.Id, "?fields=cover", LoadCover);
		}

		private async void LoadCover(List<FacebookElement> ElementList)
		{
			if (ElementList.Count > 0) {
				FacebookCover cover = ElementList [0] as FacebookCover;
				if (cover != null) {
					UIImage facebookImage = await CommonUtils.DownloadUIImageFromURL (cover.Source);
					BTProgressHUD.Dismiss ();

					SetImage (facebookImage);
				}
			}
		}

		private void CreateGestures()
		{
			scrollViewTap = new UITapGestureRecognizer (obj => {
				DescriptionView.ResignFirstResponder ();
				StartDateView.ResignFirstResponder();
				NameLabel.ResignFirstResponder();
			});

			nextButtonTap += (sender, e) => {
				
				content.SocialChannel = ShareButtons.GetActiveSocialChannels ();

				BoardEvent boardEvent = (BoardEvent)content;

				boardEvent.Description = DescriptionView.Text;
				boardEvent.ImageView = new UIImageView(PictureBox.Image);
				boardEvent.CreationDate = DateTime.Now;

				Preview.Initialize(boardEvent);

				// shows the image preview so that the user can position the image
				BoardInterface.scrollView.AddSubview (Preview.View);

				// switches to confbar
				ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.ConfirmationBar);

				AppDelegate.NavigationController.PopViewController(false);
			};


		}
	}
}

