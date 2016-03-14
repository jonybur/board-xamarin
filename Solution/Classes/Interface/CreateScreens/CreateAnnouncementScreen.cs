using CoreGraphics;
using UIKit;

using Board.Utilities;
using Board.Schema;
using Board.Facebook;
using Foundation;

using System;
using Board.Interface.Buttons;

namespace Board.Interface.CreateScreens
{
	public class CreateAnnouncementScreen : CreateScreen
	{
		PlaceholderTextView textview;
		UITapGestureRecognizer scrollViewTap;
		EventHandler nextButtonTap;

		float positionY;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			content = new Announcement ();

			LoadContent ();

			string imagePath = "./boardinterface/screens/announcement/banner/" + AppDelegate.PhoneVersion + ".jpg";

			LoadBanner (imagePath, "posts", LoadFromFacebookEvent);
			LoadNextButton ();
			LoadTextView ();

			positionY = (float)textview.Frame.Bottom + 50;

			LoadPostToButtons (positionY);

			CreateGestures ();
		}

		private void LoadFromFacebookEvent(FacebookElement FBElement)
		{
			ShareButtons.ActivateFacebook ();

			FacebookPost FBPost = (FacebookPost)FBElement;

			content.FacebookId = FBPost.Id;
			textview.SetText (FBPost.Message);
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

				Announcement ann = (Announcement)content;

				ann.Text = textview.AttributedText;
				ann.SocialChannel = ShareButtons.GetActiveSocialChannels ();
				ann.CreationDate = DateTime.Now;

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
			var frame = new CGRect(10, Banner.Frame.Bottom, 
				AppDelegate.ScreenWidth - 20,
				140);

			textview = new PlaceholderTextView(frame, "Write a caption...");

			textview.KeyboardType = UIKeyboardType.Default;
			textview.TextColor = AppDelegate.BoardBlue;
			textview.ReturnKeyType = UIReturnKeyType.Default;
			textview.EnablesReturnKeyAutomatically = true;
			textview.AllowsEditingTextAttributes = true;
			textview.BackgroundColor = UIColor.White;
			textview.Font = AppDelegate.SystemFontOfSize18;

			UIImageView colorWhite = new UIImageView(new CGRect (0, 0, AppDelegate.ScreenWidth, frame.Bottom));
			colorWhite.BackgroundColor = UIColor.White;

			ScrollView.AddSubviews (colorWhite, textview);
		}
	}
}