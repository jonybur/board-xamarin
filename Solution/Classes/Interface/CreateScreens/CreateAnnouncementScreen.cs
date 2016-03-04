using CoreGraphics;
using UIKit;

using Board.Utilities;
using Board.Schema;

using System;
using Board.Interface.Buttons;

namespace Board.Interface.CreateScreens
{
	public class CreateAnnouncementScreen : CreateScreen
	{
		PlaceholderTextView textview;
		UIImage image;

		float positionY;

		UITapGestureRecognizer scrollViewTap;
		EventHandler nextButtonTap;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			content = new Announcement ();

			LoadContent ();

			string imagePath = "./screens/share/banner/" + AppDelegate.PhoneVersion + ".jpg";

			LoadBanner (imagePath);
			LoadNextButton ();
			LoadTextView ();

			positionY = (float)textview.Frame.Bottom + 50;

			LoadPostToButtons (positionY);

			CreateGestures ();
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			ScrollView.AddGestureRecognizer (scrollViewTap);
			NextButton.TouchUpInside += nextButtonTap;
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
			ScrollView.RemoveGestureRecognizer (scrollViewTap);
			NextButton.TouchUpInside -= nextButtonTap;
		}

		private void CreateGestures()
		{
			scrollViewTap = new UITapGestureRecognizer (obj => textview.ResignFirstResponder ());

			nextButtonTap += (sender, e) => {

				if (textview.IsPlaceHolder || textview.Text.Length == 0)
				{
					UIAlertController alert = UIAlertController.Create("Can't create announcement", "Please write a caption", UIAlertControllerStyle.Alert);

					alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));

					NavigationController.PresentViewController (alert, true, null);

					return;
				}

				Announcement ann = new Announcement();

				ann.SocialChannel = ShareButtons.GetActiveSocialChannels ();

				ann.Text = textview.AttributedText;

				Preview.Initialize(ann);

				// shows the image preview so that the user can position the image
				BoardInterface.scrollView.AddSubview(Preview.View);

				// switches to confbar
				ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.ConfirmationBar);

				NavigationController.PopViewController(false);
			};
		}


		private void LoadTextView()
		{
			var frame = new CGRect(10, Banner.Frame.Height, 
				AppDelegate.ScreenWidth - 50 - 23,
				140);

			textview = new PlaceholderTextView(frame, "Write a caption...");

			textview.KeyboardType = UIKeyboardType.Default;
			textview.ReturnKeyType = UIReturnKeyType.Default;
			textview.EnablesReturnKeyAutomatically = true;
			textview.AllowsEditingTextAttributes = true;
			textview.BackgroundColor = UIColor.White;
			textview.TextColor = AppDelegate.BoardBlue;
			textview.Font = UIFont.SystemFontOfSize (20);

			UIImageView colorWhite = new UIImageView(new CGRect (0, 0, AppDelegate.ScreenWidth, frame.Bottom));
			colorWhite.BackgroundColor = UIColor.White;

			ScrollView.AddSubviews (colorWhite, textview);
		}
	}
}