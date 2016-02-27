using System;
using Board.Schema;
using Board.Utilities;
using CoreGraphics;
using UIKit;

namespace Board.Interface.Buttons
{
	public class AcceptButton : Button
	{
		public AcceptButton (Action refreshPictures)
		{
			uiButton = new UIButton (UIButtonType.Custom);

			UIImage uiImage = UIImage.FromFile ("./boardinterface/strokebuttons/accept_3px.png");
			uiButton.SetImage (uiImage, UIControlState.Normal);

			uiButton.Frame = new CGRect (0,0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint ((AppDelegate.ScreenWidth + ButtonSize) / 2 +
				(AppDelegate.ScreenWidth - ButtonSize) / 4, AppDelegate.ScreenHeight - ButtonSize / 2);

			EventHandler touchUpInside = null;

			touchUpInside = delegate(object sender, EventArgs e) {

				// remove interaction capabilities from the preview
				Preview.RemoveUserInteraction ();

				// takes out the confirmation bar and resets navigation
				ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.NavigationBar);

				switch (Preview.TypeOfPreview) {
				case (int)Preview.Type.Picture:
					// create the picture from the preview
					Picture p = Preview.GetPicture ();

					// if the picture is not null...
					if (p != null) {
						// uploads

						BoardInterface.ListPictures.Add (p);
					}
					break;

				case (int)Preview.Type.Video:
					Video v = Preview.GetVideo ();

					if (v != null) {
						BoardInterface.ListVideos.Add (v);
					}
					break;

				case (int)Preview.Type.Announcement:
					Announcement ann = Preview.GetAnnouncement ();

					if (ann != null) {

						if (ann.SocialChannel != null && ann.SocialChannel.Count > 0) {
							if (ann.SocialChannel.Contains (0)) {
								string json = "{ \"text\": \"" + ann.Text + "\", " +
								              "\"socialChannel\": \"" + "0" + "\" }";

								string result = CommonUtils.JsonPOSTRequest ("http://192.168.1.101:5000/api/publications?authToken=" + AppDelegate.EncodedBoardToken, json);

								Console.WriteLine (result);
							}
						}

						BoardInterface.ListAnnouncements.Add (ann);
					}
					break;
				}

				// remove the preview imageview from the superview
				Preview.RemoveFromSuperview ();

				// refreshes the scrollview
				refreshPictures ();
			};

			uiButton.TouchUpInside += touchUpInside;

			uiButton.Alpha = 0f;
		}

	}
}

