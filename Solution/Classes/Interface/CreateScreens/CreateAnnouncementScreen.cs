using CoreGraphics;
using UIKit;
using Board.Utilities;
using Board.Schema;
using Board.Facebook;
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

		public CreateAnnouncementScreen()
		{
		}

		public CreateAnnouncementScreen(Announcement announcement)
		{
			content = announcement;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			bool isEditing = true;

			if (content == null) {
				content = new Announcement ();
				isEditing = false;
			}

			LoadContent ();

			LoadBanner ("posts", LoadFromFacebookEvent, "ANNOUNCEMENT", "cross_left", "import_right");
			LoadNextButton (isEditing);
			LoadTextView ();

			positionY = (float)textview.Frame.Bottom + 60;

			LoadPostToButtons (positionY);

			CreateGestures (isEditing);
		}

		private void LoadFromFacebookEvent(FacebookElement FBElement)
		{
			ShareButtons.ActivateFacebook ();
			FacebookPost FBPost = (FacebookPost)FBElement;
			content.FacebookId = FBPost.Id;
			textview.SetText (FBPost.Message);
		}
			
		private void CreateGestures(bool isEditing)
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

				if (!isEditing)
				{
					Announcement ann = (Announcement)content;

					ann.AttributedText = textview.AttributedText;
					ann.SocialChannel = ShareButtons.GetActiveSocialChannels ();
					ann.CreationDate = DateTime.Now;

					// TODO: only if this is a new announcement, else update the announcement

					Preview.Initialize(ann);
				} else {



				}

				ScrollView.RemoveGestureRecognizer (scrollViewTap);
				NextButton.TouchUpInside -= nextButtonTap;
				Banner.UnsuscribeToEvents();

				MemoryUtility.ReleaseUIViewWithChildren (View, true);

				if (!isEditing) {
					NavigationController.PopViewController(false);
				} else {
					AppDelegate.PopViewLikeDismissView();
				}
				
			};


			ScrollView.AddGestureRecognizer (scrollViewTap);

			NextButton.TouchUpInside += nextButtonTap;
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

			if (((Announcement)content).AttributedText != null) {
				textview.AttributedText = (((Announcement)content).AttributedText);
				textview.IsPlaceHolder = false;
			}

			UIImageView colorWhite = new UIImageView(new CGRect (0, 0, AppDelegate.ScreenWidth, frame.Bottom));
			colorWhite.BackgroundColor = UIColor.White;

			ScrollView.AddSubviews (colorWhite, textview);
		}
	}
}