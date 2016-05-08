using System;
using System.Collections.Generic;
using Board.Schema;
using Board.Utilities;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Board.Interface.CreateScreens
{
	public class CreatePollScreen : CreateScreen
	{
		PlaceholderTextView textview;
		UITapGestureRecognizer scrollViewTap;
		EventHandler nextButtonTap;
		UITextField answerField1;
		UITextField answerField2;

		float positionY;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			content = new Poll ();

			LoadContent ();

			LoadBanner (null, null, "POLL", "cross_left");
			LoadNextButton (false);
			LoadTextView ();
			answerField1 = LoadAnswerField ((float)textview.Frame.Bottom + 20, 1);
			answerField2 = LoadAnswerField ((float)answerField1.Frame.Bottom + 15, 2);

			positionY = (float)answerField2.Frame.Bottom + 70;

			UIImageView colorWhite = new UIImageView(new CGRect (0, 0, AppDelegate.ScreenWidth, answerField2.Frame.Bottom + 10));
			colorWhite.BackgroundColor = UIColor.White;

			ScrollView.AddSubviews (colorWhite, textview, answerField1, answerField2);

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
			MemoryUtility.ReleaseUIViewWithChildren (View, true);
		}

		private void CreateGestures()
		{
			scrollViewTap = new UITapGestureRecognizer (obj => {
				textview.ResignFirstResponder ();
				answerField1.ResignFirstResponder();
				answerField2.ResignFirstResponder();
				ScrollView.SetContentOffset(new CGPoint(0, -15), true);
			});

			nextButtonTap += (sender, e) => {

				if (textview.IsPlaceHolder || textview.Text.Length == 0 || answerField1.Text.Length == 0 || answerField2.Text.Length == 0) {
					UIAlertController alert = UIAlertController.Create ("Can't create poll", "Please complete all fields", UIAlertControllerStyle.Alert);
					alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));
					NavigationController.PresentViewController (alert, true, null);
					return;
				}

				Poll poll = (Poll)content;

				poll.Answers = new List<string>();
				poll.Answers.Add(answerField1.Text);
				poll.Answers.Add(answerField2.Text);
				poll.Question = textview.AttributedText;
				poll.SocialChannel = ShareButtons.GetActiveSocialChannels ();
				poll.CreationDate = DateTime.Now;

				Preview.Initialize (poll);

				NavigationController.PopViewController (false);
			};
		}

		private UITextField LoadAnswerField(float yPosition, int answerNumber)
		{
			UITextField answerField = new UITextField(new CGRect (15, yPosition, AppDelegate.ScreenWidth - 30, 24));
			answerField.BackgroundColor = UIColor.White;
			answerField.Font = AppDelegate.SystemFontOfSize18;

			var placeholderAttribute = new UIStringAttributes {
				Font = AppDelegate.SystemFontOfSize18,
				ForegroundColor = UIColor.FromRGB(101, 149, 183)
			};
			var prettyString = new NSMutableAttributedString ("Answer " + answerNumber);
			prettyString.SetAttributes (placeholderAttribute, new NSRange (0, prettyString.Length));
			answerField.AttributedPlaceholder = prettyString;

			answerField.TextColor = AppDelegate.BoardBlue;

			answerField.Started += (sender, e) => ScrollView.SetContentOffset (new CGPoint (0, answerField.Frame.Y - 200), true);

			answerField.ReturnKeyType = UIReturnKeyType.Done;
			answerField.EnablesReturnKeyAutomatically = true;

			answerField.ShouldReturn += textField => {
				answerField.ResignFirstResponder();
				ScrollView.SetContentOffset(new CGPoint(0, -15), true);
				return true;
			};

			return answerField;
		}


		private void LoadTextView()
		{
			var frame = new CGRect(10, Banner.Frame.Bottom, 
				AppDelegate.ScreenWidth - 20,
				140);

			textview = new PlaceholderTextView(frame, "Write a question...");

			textview.KeyboardType = UIKeyboardType.Default;
			textview.TextColor = AppDelegate.BoardBlue;
			textview.ReturnKeyType = UIReturnKeyType.Default;
			textview.EnablesReturnKeyAutomatically = true;
			textview.AllowsEditingTextAttributes = true;
			textview.BackgroundColor = UIColor.White;
			textview.Font = AppDelegate.SystemFontOfSize18;
			textview.EnablesReturnKeyAutomatically = true;

			textview.Started += (sender, e) => ScrollView.SetContentOffset (new CGPoint (0, -15), true);

			textview.ShouldChangeText += ((textView, range, text) => {
				var newLength = textview.Text.Length + text.Length - range.Length;
				return newLength <= 140;
			});
		}
	}
}

