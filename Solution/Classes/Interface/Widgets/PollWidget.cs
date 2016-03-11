using System;
using Board.Schema;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;

namespace Board.Interface.Widgets
{
	public class PollWidget : Widget
	{
		// UIView contains ScrollView and BackButton
		// ScrollView contains LookUpImage
		private UITextView textview;

		public Poll poll
		{
			get { return (Poll)content; }
		}

		public PollWidget()
		{

		}

		public PollWidget(Poll poll)
		{
			content = poll;

			UITextView insideText = CreateText ();
			List<UIButton> lstAnswers = new List<UIButton> ();

			float height = (float)insideText.Frame.Height;

			foreach (string ans in poll.Answers) {
				UIButton button = CreateAnswer (ans, height, (float)insideText.Frame.Width);
				height += (float)button.Frame.Height;
				lstAnswers.Add (button);
			}

			// mounting
			CreateMounting (new CGRect(insideText.Frame.X, insideText.Frame.Y, insideText.Frame.Width, height));
			View = new UIView(MountingView.Frame);

			View.AddSubviews (MountingView, insideText);
			foreach (UIButton but in lstAnswers) {
				View.AddSubview (but);
			}

			View.Frame = new CGRect (poll.Frame.X, poll.Frame.Y, MountingView.Frame.Width, MountingView.Frame.Height);
			View.Transform = CGAffineTransform.MakeRotation(poll.Rotation);

			EyeOpen = false;

			CreateGestures ();
		}

		private UIButton CreateAnswer (string answer, float yposition, float width)
		{

			// 30 is the space for radio button, 40 is height of button
			UIButton button = new UIButton (new CGRect(0, yposition, width, 40));

			UILabel lblAnswer = new UILabel ();
			lblAnswer.Frame = new CGRect (0, 0, width - 60, 16);
			lblAnswer.Center = new CGPoint (width / 2 + 30 / 2, 40 / 2);

			lblAnswer.Font = UIFont.SystemFontOfSize (16);
			lblAnswer.AdjustsFontSizeToFitWidth = true;
			lblAnswer.Text = answer;
			lblAnswer.TextColor = BoardInterface.board.MainColor;

			button.AddSubview (lblAnswer);

			return button;
		}

		public void ScrollEnabled(bool value)
		{
			textview.ScrollEnabled = value;
		}

		private UITextView CreateText()
		{
			textview = new UITextView ();
			textview.Editable = false;
			textview.Selectable = false;
			textview.ScrollEnabled = true;
			textview.DataDetectorTypes = UIDataDetectorType.Link;
			textview.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
			textview.Text = poll.Question;
			textview.TextColor = BoardInterface.board.MainColor;
			textview.Font = UIFont.SystemFontOfSize (18);
			textview.SizeToFit ();

			if (textview.Frame.Width < 160) {
				textview.Frame = new CGRect (10, 10, 160, textview.Frame.Height + 24);
			} else if (textview.Frame.Width > 250) {
				textview.Frame = new CGRect (10, 10, 250, textview.Frame.Height + 24);
			}

			textview.ContentOffset = new CGPoint (0, 0);

			return textview;
		}

		public void SetFrame(CGRect frame)
		{
			View.Frame = frame;
		}

	}
}

