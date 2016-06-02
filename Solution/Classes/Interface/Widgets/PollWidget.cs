using Board.Schema;
using UIKit;
using CoreGraphics;
using Foundation;
using System.Collections.Generic;
using System;

namespace Board.Interface.Widgets
{
	public class PollWidget : Widget
	{
		// UIView contains ScrollView and BackButton
		// ScrollView contains LookUpImage
		private static List<AnswerButton> lstAnswers;

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

			var insideText = CreateQuestion ();
			lstAnswers = new List<AnswerButton> ();

			float yposition = (float)insideText.Frame.Bottom + 5;

			AnswerButton.Initialize ();

			foreach (string ans in poll.Answers) {
				var button = new AnswerButton(ans, yposition, (float)insideText.Frame.Width);
				yposition += (float)button.Frame.Height;
				lstAnswers.Add (button);
			}

			// mounting
			CreateMounting (new CGSize(insideText.Frame.Width, yposition));
			Frame = MountingView.Frame;
			AddSubviews (MountingView, insideText);

			foreach (var but in lstAnswers) {
				AddSubview (but); 
			}

			
			CreateGestures ();
		}

		private UILabel CreateQuestion()
		{
			var label = new UILabel ();
			label.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
			label.Text = poll.Question;
			label.TextColor = Widget.HighlightColor;
			label.Lines = 0;
			label.Font = AppDelegate.SystemFontOfSize18;

			UIStringAttributes stringAttributes = new UIStringAttributes ();
			stringAttributes.Font = AppDelegate.SystemFontOfSize18;
			var stringForBoundingRect = new NSString(poll.Question);
			var labrect = stringForBoundingRect.GetBoundingRect(new CGSize(250,450), NSStringDrawingOptions.UsesLineFragmentOrigin, stringAttributes, null);

			label.Frame = new CGRect (SideMargin + 5, TopMargin + 5, 250, labrect.Height);

			return label;
		}

		private UITextView CreateText()
		{
			UITextView textview;
			textview = new UITextView ();
			textview.Editable = false;
			textview.Selectable = false;
			textview.ScrollEnabled = false;
			textview.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
			textview.DataDetectorTypes = UIDataDetectorType.Link;
			textview.Text = poll.Question;
			textview.TextColor = Widget.HighlightColor;
			textview.Font = AppDelegate.SystemFontOfSize18;
			textview.SizeToFit ();
			textview.LayoutIfNeeded ();
			int numLines = (int)(textview.Frame.Height / 18);

			if (textview.Frame.Width < 160) {
				textview.Frame = new CGRect (10, 10, 160, numLines * 30);
			} else if (textview.Frame.Width > 250) {
				textview.Frame = new CGRect (10, 10, 250, numLines * 30);
			}

			textview.ContentOffset = new CGPoint (0, 0);

			return textview;
		}

		public class AnswerButton : UIButton{

			UIImageView AnswerImageView;
			UILabel label;

			EventHandler touchUpInside;

			static UIImage EmptyRadio;
			static UIImage FullRadio;

			public static void Initialize()
			{
				using (UIImage img = UIImage.FromFile ("./boardinterface/widget/radiobut2.png")){
					EmptyRadio = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				}

				using (UIImage img = UIImage.FromFile ("./boardinterface/widget/radiobutfull.png")){
					FullRadio = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				}
			}

			private void SetEmptyImage()
			{
				AnswerImageView = new UIImageView (EmptyRadio);
				AnswerImageView.Frame = new CGRect (0, 0, 20, 20);
				AnswerImageView.TintColor = Widget.HighlightColor;
				AnswerImageView.Center = new CGPoint (15, Frame.Height / 2);
			}

			private void SetFullImage()
			{
				AnswerImageView.Image = FullRadio;
			}

			public AnswerButton(string answer, float yposition, float width)
			{
				Frame = new CGRect(10, yposition, width, 50);

				label = new UILabel ();
				label.Frame = new CGRect (0, 0, width - 60, 16);
				label.Center = new CGPoint (width / 2 + 10, Frame.Height / 2);

				label.Font = AppDelegate.SystemFontOfSize16;
				label.AdjustsFontSizeToFitWidth = true;
				label.Text = answer;
				label.TextColor = Widget.HighlightColor;

				SetEmptyImage();

				AddSubviews (label, AnswerImageView);

				touchUpInside = (sender, e) => {
					SetFullImage();
					foreach (AnswerButton ansButton in lstAnswers)
					{
						ansButton.label.Text += " - 50%";
						ansButton.UserInteractionEnabled = false;
						ansButton.TouchUpInside -= ansButton.touchUpInside;
					}
				};

				TouchUpInside += touchUpInside;
			}
		}
	}


}

