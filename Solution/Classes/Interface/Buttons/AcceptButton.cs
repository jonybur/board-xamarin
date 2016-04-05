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

				Content content;

				switch (Preview.TypeOfPreview) {

					case (int)Preview.Type.Picture:
						content = Preview.GetPicture ();
						break;

					case (int)Preview.Type.Video:
						content = Preview.GetVideo ();
						break;

					case (int)Preview.Type.Announcement:
						content = Preview.GetAnnouncement ();
							
						if (AppDelegate.ServerActive && content.SocialChannel != null && content.SocialChannel.Count > 0) {
								if (content.SocialChannel.Contains (0)) {
								string json = "{ \"text\": \"" + ((Announcement)content).AttributedText + "\", " + "\"socialChannel\": \"" + "0" + "\" }";
									string result = CommonUtils.JsonPOSTRequest ("http://192.168.1.101:5000/api/publications?authToken=" + AppDelegate.EncodedBoardToken, json);
									Console.WriteLine (result);
								}
							}
						break;

					case (int)Preview.Type.Event:
						content = Preview.GetEvent();
						break;

					case (int)Preview.Type.Map:
						content = Preview.GetMap();
						break;

					case (int)Preview.Type.Poll:
						content = Preview.GetPoll();
						break;

					default:
						content = new Content();
						break;
				}

				string jsonString = JsonUtilty.GenerateUpdateJson(content);

				BoardInterface.DictionaryContent.Add (content.Id, content);

				// remove the preview imageview from the superview
				Preview.RemoveFromSuperview ();

				// adds widget to dictionary
				AppDelegate.boardInterface.AddWidgetToDictionaryFromContent (content);

				// renders scrollview
				AppDelegate.boardInterface.BoardScroll.SelectiveRendering();
			});

			uiButton.Alpha = 0f;
		}
	}
}

