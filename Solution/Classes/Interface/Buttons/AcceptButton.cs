using System;
using Board.Schema;
using Board.Utilities;
using Board.Infrastructure;
using CoreGraphics;
using UIKit;

namespace Board.Interface.Buttons
{
	public class AcceptButton : Button
	{
		public AcceptButton ()
		{
			uiButton = new UIButton (UIButtonType.Custom);

			using (UIImage uiImage = UIImage.FromFile ("./boardinterface/nubuttons/nuaccept.png")) {
				uiButton.SetImage (uiImage, UIControlState.Normal);
			}

			uiButton.Frame = new CGRect (0,0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint ((AppDelegate.ScreenWidth + ButtonSize) / 2 +
				(AppDelegate.ScreenWidth - ButtonSize) / 4, AppDelegate.ScreenHeight - ButtonSize / 2);
			
			eventHandlers.Add (async (sender, e) => {
				Content content;

				if (Preview.IsAlive){
					// remove interaction capabilities from the preview
					Preview.RemoveUserInteraction ();

					// takes out the confirmation bar and resets navigation
					ButtonInterface.SwitchButtonLayout (ButtonInterface.ButtonLayout.NavigationBar);

					content = await Preview.GetContent ();
				} else {
					UIPreviewSticker.PreviewSticker.UserInteractionEnabled = false;

					ButtonInterface.SwitchButtonLayout(ButtonInterface.ButtonLayout.NavigationBar);

					content = UIPreviewSticker.GetContent();
				}

				string jsonString = JsonUtilty.GenerateUpdateJson (content);

				bool wasUploaded = CloudController.UpdateBoard (UIBoardInterface.board.Id, jsonString);
				if (!wasUploaded){
					return;
				}

				UIBoardInterface.DictionaryContent.Add (content.Id, content);

				if (Preview.IsAlive){
					// remove the preview imageview from the superview
					Preview.RemoveFromSuperview ();

					// adds widget to dictionary
					AppDelegate.BoardInterface.AddWidgetToDictionaryFromContent (content);
				} else {
					// remove previewsticker
					UIPreviewSticker.PreviewSticker.RemoveFromSuperview();

					// add sticker to dictionary
					AppDelegate.BoardInterface.AddStickerToDictionaryFromContent (content as Sticker);
				}

				// renders scrollview
				AppDelegate.BoardInterface.BoardScroll.SelectiveRendering ();
			});

			uiButton.Alpha = 0f;
		}
	}
}

