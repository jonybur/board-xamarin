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

			using (UIImage uiImage = UIImage.FromFile ("./boardinterface/strokebuttons/accept_3px.png")) {
				uiButton.SetImage (uiImage, UIControlState.Normal);
			}

			uiButton.Frame = new CGRect (0,0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint ((AppDelegate.ScreenWidth + ButtonSize) / 2 +
				(AppDelegate.ScreenWidth - ButtonSize) / 4, AppDelegate.ScreenHeight - ButtonSize / 2);
			
			eventHandlers.Add ((sender, e) => {
				
				// remove interaction capabilities from the preview
				Preview.RemoveUserInteraction ();

				// takes out the confirmation bar and resets navigation
				ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.NavigationBar);

				Content content = Preview.GetContent();

				string jsonString = JsonUtilty.GenerateUpdateJson(content);

				UIBoardInterface.DictionaryContent.Add (content.Id, content);

				// remove the preview imageview from the superview
				Preview.RemoveFromSuperview ();

				// adds widget to dictionary
				AppDelegate.BoardInterface.AddWidgetToDictionaryFromContent (content);

				// renders scrollview
				AppDelegate.BoardInterface.BoardScroll.SelectiveRendering();
			});

			uiButton.Alpha = 0f;
		}
	}
}

