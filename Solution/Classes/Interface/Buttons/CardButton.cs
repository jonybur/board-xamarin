using System;
using CoreGraphics;
using UIKit;

namespace Board.Interface.Buttons
{
	public class CardButton : Button
	{
		public CardButton ()
		{
			uiButton = new UIButton (UIButtonType.Custom);

			UIImage uiImage = UIImage.FromFile ("./boardinterface/strokebuttons/card_3px.png");

			uiButton.SetImage (uiImage, UIControlState.Normal);
			uiButton.Frame = new CGRect (0,0, ButtonSize, ButtonSize);

			uiButton.Center = new CGPoint ((AppDelegate.ScreenWidth + ButtonSize) / 2 +
				(AppDelegate.ScreenWidth - ButtonSize) / 8 + 10, AppDelegate.ScreenHeight - ButtonSize / 2);
			
			uiButton.TouchUpInside += (object sender, EventArgs e) => {
				UIAlertController alert = UIAlertController.Create(null, "Select the type of component", UIAlertControllerStyle.ActionSheet);

				alert.AddAction (UIAlertAction.Create ("Announcement", UIAlertActionStyle.Default, CreateAnnouncement));
				alert.AddAction (UIAlertAction.Create ("Event", UIAlertActionStyle.Default, null));
				alert.AddAction (UIAlertAction.Create ("Poll", UIAlertActionStyle.Default, null));
				alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));

				AppDelegate.NavigationController.PresentViewController (alert, true, null);
			};
		}

		private void CreateAnnouncement(UIAlertAction act)
		{
			AnnouncementScreen announcementScreen = new AnnouncementScreen ();
			AppDelegate.NavigationController.PushViewController (announcementScreen, true);
		}
	}
}

