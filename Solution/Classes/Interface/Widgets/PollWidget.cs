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

			UITextView insideText = CreateText ();
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

			View.Frame = new CGRect (poll.Frame.X, poll.Frame.Y, MountingView.Frame.Width, MountingView.Frame.Height);
			View.Transform = CGAffineTransform.MakeRotation(poll.Rotation);

			EyeOpen = false;

			CreateGestures ();
		}

		class AnswerButton : UIButton{

			UIImageView ImageView;
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
				ImageView = new UIImageView (EmptyRadio);
				ImageView.Frame = new CGRect (0, 0, 20, 20);
				ImageView.TintColor = BoardInterface.board.MainColor;
				ImageView.Center = new CGPoint (15, Frame.Height / 2);
			}

			private void SetFullImage()
			{
				ImageView.Image = FullRadio;
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

				AddSubviews (label, ImageView);

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
			textview.Font = AppDelegate.SystemFontOfSize18;
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

