using Board.Schema;
using CoreGraphics;
using UIKit;
using Foundation;
using System;
using System.Collections.Generic;

namespace Board.Interface.LookUp
{
	public class PollLookUp : UILookUp
	{
		private static List<AnswerButton> lstAnswers;

		public PollLookUp(Poll poll)
		{
			this.content = poll;

			UIColor backColor = UIColor.FromRGB(250,250,250);
			UIColor frontColor = AppDelegate.BoardBlack;

			View.BackgroundColor = backColor;

			CreateButtons (frontColor);

			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			ScrollView.UserInteractionEnabled = true;

			View.AddSubviews (ScrollView, BackButton, LikeButton, FacebookButton, ShareButton, TrashButton, EditButton);

			GenerateContent (poll);
		}

		private void GenerateContent(Poll poll)
		{
			UILabel insideText = CreateQuestion (poll);
			var lstAnswers = new List<AnswerButton> ();

			float height = (float)insideText.Frame.Height + 10;

			AnswerButton.Initialize ();

			View.AddSubview (insideText);

			foreach (string ans in poll.Answers) {
				AnswerButton button = new AnswerButton(ans, height, (float)insideText.Frame.Width);
				height += (float)button.Frame.Height;
				lstAnswers.Add (button);
				View.AddSubview (button);
			}

		}

		private UILabel CreateQuestion(Poll poll)
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

