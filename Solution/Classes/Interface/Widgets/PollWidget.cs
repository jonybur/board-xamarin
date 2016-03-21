using Board.Schema;
using UIKit;
using CoreGraphics;
using Foundation;
using System.Collections.Generic;

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

			UILabel insideText = CreateQuestion ();
			lstAnswers = new List<AnswerButton> ();

			float height = (float)insideText.Frame.Height + 10;

			AnswerButton.Initialize ();

			foreach (string ans in poll.Answers) {
				AnswerButton button = new AnswerButton(ans, height, (float)insideText.Frame.Width);
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

			View.Center = new CGPoint (poll.Position.X + View.Frame.Width / 2, poll.Position.Y + View.Frame.Height / 2);
			View.Transform = CGAffineTransform.MakeRotation(poll.Rotation);

			EyeOpen = false;

			CreateGestures ();
		}

		private UILabel CreateQuestion()
		{
			UILabel label = new UILabel ();
			label.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
			label.AttributedText = poll.Question;
			label.TextColor = BoardInterface.board.MainColor;
			label.Lines = 0;
			label.Font = AppDelegate.SystemFontOfSize18;

			NSString lorum = new NSString(poll.Question.Value);
			UIStringAttributes stringAttributes = new UIStringAttributes ();
			stringAttributes.Font = AppDelegate.SystemFontOfSize18;
			CGRect labrect = lorum.GetBoundingRect(new CGSize(250,450), NSStringDrawingOptions.UsesLineFragmentOrigin, stringAttributes, null);

			label.Frame = new CGRect (10, 10, 250, labrect.Size.Height);

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
			textview.AttributedText = poll.Question;
			textview.TextColor = BoardInterface.board.MainColor;
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

		public void SetFrame(CGRect frame)
		{
			View.Frame = frame;
		}

		class AnswerButton : UIButton{

			UIImageView AnswerImageView;
			UILabel label;

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
				AnswerImageView.TintColor = BoardInterface.board.MainColor;
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
				label.TextColor = BoardInterface.board.MainColor;

				SetEmptyImage();

				AddSubviews (label, AnswerImageView);

				TouchUpInside += (sender, e) => {
					SetFullImage();
					foreach (AnswerButton ansButton in lstAnswers)
					{
						ansButton.label.Text += " - 50%";
						ansButton.UserInteractionEnabled = false;
					}
				};

			}
		}

	}
}

