using System;
using System.Collections.Generic;
using BigTed;
using Board.Facebook;
using Board.Picker;
using Board.Schema;
using Board.Utilities;
using CoreGraphics;
using Foundation;
using UIKit;

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

		UIImageView whiteBack;

		public CreateEventScreen()
		{}

		public CreateEventScreen(BoardEvent boardEvent)
		{
			content = boardEvent;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			bool isEditing = true;

			if (content == null) {
				content = new BoardEvent ();
				isEditing = false;
			}

			LoadContent ();

			string imagePath = "./boardinterface/screens/event/banner/" + AppDelegate.PhoneVersion + ".jpg";

			whiteBack = new UIImageView ();
			ScrollView.AddSubview (whiteBack);

			LoadBanner (imagePath, "events", LoadFromFacebookEvent);
			LoadPictureBox ();
			LoadNameLabel ((float) PictureBox.Frame.Bottom + 50);
			LoadTextView ((float) NameLabel.Frame.Bottom + 10);
			LoadStartDateView ((float) DescriptionView.Frame.Bottom + 30, isEditing);
			LoadEndDateView ((float) StartDateView.Frame.Bottom + 20, isEditing);
			LoadPostToButtons ((float) EndDateView.Frame.Bottom + 60);
			LoadNextButton (isEditing);

			if (((BoardEvent)content).ImageView != null && ((BoardEvent)content).ImageView.Image != null) {
				SetImage (((BoardEvent)content).ImageView.Image);
			} else {
				using (UIImage img = UIImage.FromFile ("./boardinterface/screens/event/placeholder.png")) {
					SetImage (img);
				}
			}

			CreateGestures (isEditing);

			View.BackgroundColor = UIColor.White;
		}

		private void LoadEndDateView(float yPosition, bool isEditing)
		{
			UIDatePicker DatePicker = new UIDatePicker (new CGRect (0, AppDelegate.ScreenHeight - 250, AppDelegate.ScreenWidth, 250));
			DatePicker.TimeZone = NSTimeZone.SystemTimeZone;
			DatePicker.Date = (NSDate)DateTime.Now;
			DatePicker.BackgroundColor = UIColor.White;
			DatePicker.Mode = UIDatePickerMode.DateAndTime;
			DatePicker.MinimumDate = (NSDate)DateTime.Now;
			DatePicker.ValueChanged += (sender, e) => {
				DateTime selectedDate = ((DateTime)DatePicker.Date).ToLocalTime();
				EndDateView.Text = "Ends: " + selectedDate.ToString("g");
				((BoardEvent)content).EndDate = selectedDate;
			};

			UIToolbar toolbar = new UIToolbar (new CGRect(0, DatePicker.Frame.Top- 40, AppDelegate.ScreenWidth, 40));
			UIBarButtonItem btnLeft = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, null);
			UIBarButtonItem btnCenter = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, null);
			UIBarButtonItem btnRight = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate(object sender, EventArgs e) {
				EndDateView.ResignFirstResponder();
				StartDateView.ResignFirstResponder();
				toolbar.Alpha = 0f;
			});
			toolbar.SetItems (new []{btnLeft, btnCenter, btnRight}, true);
			toolbar.UserInteractionEnabled = true;
			toolbar.Alpha = 0f;
			View.AddSubview (toolbar);

			EndDateView = new UITextField(new CGRect (15, yPosition, AppDelegate.ScreenWidth - 30, 24));
			EndDateView.InputView = DatePicker;
			EndDateView.BackgroundColor = UIColor.White;
			EndDateView.Font = AppDelegate.SystemFontOfSize18;

			var placeholderAttribute = new UIStringAttributes {
				Font = AppDelegate.SystemFontOfSize18,
				ForegroundColor = UIColor.FromRGB(101, 149, 183)
			};
			var prettyString = new NSMutableAttributedString ("End Date");
			prettyString.SetAttributes (placeholderAttribute, new NSRange (0, prettyString.Length));
			EndDateView.AttributedPlaceholder = prettyString;

			EndDateView.TextColor = AppDelegate.BoardBlue;
			EndDateView.Started += (sender, e) => {
				toolbar.Alpha = 1f;
				ScrollView.SetContentOffset(new CGPoint(0, EndDateView.Frame.Y - 200), true);
			};

			ScrollView.AddSubview (EndDateView);

			if (isEditing) {
				EndDateView.Text = "Ends: " + ((BoardEvent)content).EndDate.ToString ("g");
			}

		}

		private void LoadStartDateView(float yPosition, bool isEditing)
		{
			UIDatePicker DatePicker = new UIDatePicker (new CGRect (0, AppDelegate.ScreenHeight - 250, AppDelegate.ScreenWidth, 250));
			DatePicker.TimeZone = NSTimeZone.LocalTimeZone;
			DatePicker.Date = (NSDate)DateTime.Now.ToLocalTime();
			DatePicker.BackgroundColor = UIColor.White;
			DatePicker.Mode = UIDatePickerMode.DateAndTime;
			DatePicker.MinimumDate = (NSDate)DateTime.Now.ToLocalTime();
			DatePicker.ValueChanged += (sender, e) => {
				DateTime selectedDate = ((DateTime)DatePicker.Date).ToLocalTime();
				StartDateView.Text = "Starts: " + selectedDate.ToString("g");
				((BoardEvent)content).StartDate = selectedDate;
			};

			UIToolbar toolbar = new UIToolbar (new CGRect(0, DatePicker.Frame.Top- 40, AppDelegate.ScreenWidth, 40));
			UIBarButtonItem btnLeft = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, null);
			UIBarButtonItem btnCenter = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, null);
			UIBarButtonItem btnRight = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate(object sender, EventArgs e) {
				StartDateView.ResignFirstResponder();
				EndDateView.ResignFirstResponder();
				toolbar.Alpha = 0f;
			});
			toolbar.SetItems (new []{btnLeft, btnCenter, btnRight}, true);
			toolbar.UserInteractionEnabled = true;
			toolbar.Alpha = 0f;
			View.AddSubview (toolbar);
		
			StartDateView = new UITextField(new CGRect (15, yPosition, AppDelegate.ScreenWidth - 30, 24));
			StartDateView.InputView = DatePicker;
			StartDateView.BackgroundColor = UIColor.White;
			StartDateView.Font = AppDelegate.SystemFontOfSize18;

			var placeholderAttribute = new UIStringAttributes {
				Font = AppDelegate.SystemFontOfSize18,
				ForegroundColor = UIColor.FromRGB(101, 149, 183)
			};
			var prettyString = new NSMutableAttributedString ("Start Date");
			prettyString.SetAttributes (placeholderAttribute, new NSRange (0, prettyString.Length));
			StartDateView.AttributedPlaceholder = prettyString;

			StartDateView.TextColor = AppDelegate.BoardBlue;
			StartDateView.Started += (sender, e) => {
				toolbar.Alpha = 1f;
				ScrollView.SetContentOffset(new CGPoint(0, StartDateView.Frame.Y - 200), true);
			};

			ScrollView.AddSubview (StartDateView);

			if (isEditing) {
				StartDateView.Text = "Starts: " + ((BoardEvent)content).StartDate.ToString ("g");
			}

		}

		private void LoadNameLabel(float yPosition)
		{
			NameLabel = new UITextField (new CGRect(15, yPosition, AppDelegate.ScreenWidth - 30, 20));
			NameLabel.Font = UIFont.BoldSystemFontOfSize (18);

			var placeholderAttribute = new UIStringAttributes {
				Font = AppDelegate.SystemFontOfSize18,
				ForegroundColor = UIColor.FromRGB(101, 149, 183)
			};
			var prettyString = new NSMutableAttributedString ("Event Name");
			prettyString.SetAttributes (placeholderAttribute, new NSRange (0, prettyString.Length));
			NameLabel.AttributedPlaceholder = prettyString;

			NameLabel.TextColor = AppDelegate.BoardBlue;
			NameLabel.KeyboardType = UIKeyboardType.Default;
			NameLabel.ReturnKeyType = UIReturnKeyType.Done;
			NameLabel.BackgroundColor = UIColor.White;

			if (((BoardEvent)content).Name != null) {
				NameLabel.Text = ((BoardEvent)content).Name;
			}

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
			DescriptionView.Font = AppDelegate.SystemFontOfSize18;

			if (((BoardEvent)content).Description != null) {
				DescriptionView.SetText (((BoardEvent)content).Description);
			}

			ScrollView.AddSubviews (DescriptionView);
		}
			
		private void LoadPictureBox()
		{
			PictureBox = new UIImageView ();

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

			float scale = (float)(imageSize.Height/imageSize.Width);
			imgw = (float)posterSize.Width;
			imgh = imgw * scale;

			PictureBox.Frame = new CGRect (10, Banner.Frame.Bottom, imgw, imgh);
			PictureBox.Image = img;

			AdjustContentSize ();
		}

		private void AdjustContentSize()
		{
			float yPosition = (float)PictureBox.Frame.Bottom + 50;

			NameLabel.Frame = new CGRect (NameLabel.Frame.X, yPosition, NameLabel.Frame.Width, NameLabel.Frame.Height);

			yPosition = (float)NameLabel.Frame.Bottom + 20;

			DescriptionView.Frame = new CGRect (DescriptionView.Frame.X, yPosition, DescriptionView.Frame.Width, DescriptionView.Frame.Height);

			yPosition = (float)DescriptionView.Frame.Bottom + 40;

			StartDateView.Frame = new CGRect (StartDateView.Frame.X, yPosition,
				StartDateView.Frame.Width, StartDateView.Frame.Height);

			yPosition = (float)StartDateView.Frame.Bottom + 20;

			EndDateView.Frame = new CGRect (EndDateView.Frame.X, yPosition,
				EndDateView.Frame.Width, EndDateView.Frame.Height);

			yPosition = (float)EndDateView.Frame.Bottom + 50;

			ShareButtons.View.Frame = new CGRect (ShareButtons.View.Frame.X, yPosition,
				ShareButtons.View.Frame.Width, ShareButtons.View.Frame.Height);

			whiteBack.Frame = new CGRect (0, NameLabel.Frame.Top - 15, AppDelegate.ScreenWidth, EndDateView.Frame.Bottom - NameLabel.Frame.Top + 30);
			whiteBack.BackgroundColor = UIColor.White;

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

		private async void LoadCover(List<FacebookElement> elementList)
		{
			if (elementList.Count > 0) {
				FacebookCover cover = elementList [0] as FacebookCover;
				if (cover != null) {
					try{
						UIImage facebookImage = await CommonUtils.DownloadUIImageFromURL (cover.Source);
						SetImage (facebookImage);
					} catch (Exception ex) {
						Console.WriteLine ("ERROR: " + ex.Message);
					}
					BTProgressHUD.Dismiss ();

				}
			}
		}

		private void CreateGestures(bool isEditing)
		{
			scrollViewTap = new UITapGestureRecognizer (obj => {
				DescriptionView.ResignFirstResponder ();
				StartDateView.ResignFirstResponder();
				NameLabel.ResignFirstResponder();
			});

			nextButtonTap += (sender, e) => {
				content.SocialChannel = ShareButtons.GetActiveSocialChannels ();

				if (!isEditing)
				{
					BoardEvent boardEvent = (BoardEvent)content;

					boardEvent.Description = DescriptionView.Text;
					boardEvent.ImageView = new UIImageView(PictureBox.Image);
					boardEvent.CreationDate = DateTime.Now;

					Preview.Initialize(boardEvent);
				
				} else {
					

				}

				NextButton.TouchUpInside -= nextButtonTap;
				ScrollView.RemoveGestureRecognizer (scrollViewTap);
				Banner.UnsuscribeToEvents ();

				MemoryUtility.ReleaseUIViewWithChildren (View, true);

				if (!isEditing) {
					NavigationController.PopViewController(false);
				} else {
					AppDelegate.PopViewLikeDismissView();
				}
			};

			NextButton.TouchUpInside += nextButtonTap;
			ScrollView.AddGestureRecognizer (scrollViewTap);


		}
	}
}

