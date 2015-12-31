using System;
using Newtonsoft.Json;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;

using System.Threading.Tasks;
using System.Threading;

using System.Collections.Generic;

namespace Solution
{
	public class TextBoxComponent
	{
		// UIView contains ScrollView and BackButton
		// ScrollView contains LookUpImage
		private const int sizePicture = 60;
		private const string fontName = "Roboto-Regular";

		private UIView uiView;
		private TextBox textbox;

		public TextBox GetTextBox()
		{
			textbox.SetPosition (new CGPoint(uiView.Frame.X, uiView.Frame.Y));
			return textbox;
		}

		public UIView GetUIView()
		{
			return uiView;
		}

		public TextBoxComponent()
		{

		}

		public TextBoxComponent(TextBox tb)
		{
			textbox = tb;

			CGRect frame = new CGRect (tb.ImgX, tb.ImgY, tb.ImgW, tb.ImgH);

			// TODO: programatically calculate proper size of textbox in base of text length
			uiView = new UIView(frame);
		}

		public void SetFrame(CGRect frame)
		{
			uiView.Frame = frame;
		}

		/*public TextBoxComponent(TextBox tb, RectangleF frame)
		{
			textbox = tb;

			// TODO: programatically calculate proper size of textbox in base of text length
			uiView = new UIView(frame);

			uiView.Transform = CGAffineTransform.MakeRotation (tb.Rotation);
		}*/

		public async Task LoadTextBoxComponent(UINavigationController navigationController, Action refreshContent)
		{
			UITapGestureRecognizer tap = new UITapGestureRecognizer  ((tg) => {
				// TODO: goes to chat

				UIAlertController alert = UIAlertController.Create("Are you sure you want to remove this announcement from the Board?", "It will be permanently deleted", UIAlertControllerStyle.ActionSheet);

				alert.AddAction (UIAlertAction.Create ("Accept", UIAlertActionStyle.Default, action => ArchiveTextBox (refreshContent)));
				alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));

				navigationController.PresentViewController (alert, true, null);
			});

			uiView.AddGestureRecognizer (tap);
			uiView.UserInteractionEnabled = true;
			
			User user = await AppDelegate.CloudController.LookupUser (textbox.UserId);
			uiView.BackgroundColor = BoardInterface.InterfaceColor;

			List<UILabel> labels = CreateLabels (user, uiView.Frame);

			foreach (UILabel label in labels) {
				uiView.AddSubview (label);	
			}

			uiView.Transform = CGAffineTransform.MakeRotation (textbox.Rotation);
		}


		private async void ArchiveTextBox(Action refreshContent)
		{
			// sets the OnGallery status on true for the picture
			StorageController.SendTextBoxToGallery (textbox.Id);

			// then removes the picture from the cloud storage
			await AppDelegate.CloudController.RemoveTextBoxAsync (textbox);

			// refreshes the main view
			refreshContent ();
		}

		private List<UILabel> CreateLabels(User user, CGRect bounds)
		{
			List <UILabel> lstLabels = new List<UILabel> ();
			float nameLabelHeight = 18;
			float contentLabelHeight = 18;

			// // // // NAME // // // //

			UILabel nameLabel = new UILabel (new CGRect (0, sizePicture + 20, bounds.Width, nameLabelHeight + 5)) {
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.Clear,
				TextColor = UIColor.White,
				Font = UIFont.FromName(fontName, nameLabelHeight),
				Text = user.Name,
				AdjustsFontSizeToFitWidth = true
			};

			lstLabels.Add (nameLabel);

			// // // // CONTENT // // // //

			contentLabelHeight = 16;

			string text = textbox.Text;

			UIFont contentFont = UIFont.FromName (fontName, contentLabelHeight);
			/*if (text.StringSize (contentFont).Width > bounds.Width) {
				text = text.Insert (text.Length / 2, "\t");
			}*/

			UILabel contentLabel = new UILabel (new CGRect (0, nameLabel.Frame.Y + nameLabelHeight + 10, 
																bounds.Width, contentLabelHeight + 5)) {
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.Clear,
				TextColor = UIColor.White,
				Font = contentFont,
				Text = textbox.Text
			};

			lstLabels.Add (contentLabel);

			return lstLabels;
		}
	}
}

