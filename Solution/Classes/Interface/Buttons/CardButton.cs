using CoreGraphics;
using CoreAnimation;
using Board.Interface.CreateScreens;
using UIKit;

namespace Board.Interface.Buttons
{
	public class CardButton : Button
	{
		public CardButton ()
		{
			uiButton = new UIButton (UIButtonType.Custom);

			using (UIImage uiImage = UIImage.FromFile ("./boardinterface/strokebuttons/card_3px.png")) {
				uiButton.SetImage (uiImage, UIControlState.Normal);
			}

			uiButton.Frame = new CGRect (0,0, ButtonSize, ButtonSize);

			uiButton.Center = new CGPoint ((AppDelegate.ScreenWidth + ButtonSize) / 2 +
				(AppDelegate.ScreenWidth - ButtonSize) / 8 + 10, AppDelegate.ScreenHeight - ButtonSize / 2);
			
			eventHandlers.Add ((sender, e) => {
				UIAlertController alert = UIAlertController.Create(null, "Select the type of widget", UIAlertControllerStyle.ActionSheet);

				alert.AddAction (UIAlertAction.Create ("Announcement", UIAlertActionStyle.Default, CreateAnnouncement));
				alert.AddAction (UIAlertAction.Create ("Event", UIAlertActionStyle.Default, CreateEvent));
				alert.AddAction (UIAlertAction.Create ("Poll (coming soon)", UIAlertActionStyle.Default, null));
				alert.AddAction (UIAlertAction.Create ("Map (coming soon)", UIAlertActionStyle.Default, null));
				alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));

				AppDelegate.boardInterface.NavigationController.PresentViewController (alert, true, null);
			});
		}

		private static void CreateAnnouncement(UIAlertAction act)
		{
			CreateAnnouncementScreen announcementScreen = new CreateAnnouncementScreen ();
			AppDelegate.PushViewLikePresentView (announcementScreen);
		}

		private static void CreateEvent(UIAlertAction act)
		{
			CreateEventScreen eventScreen = new CreateEventScreen ();
			AppDelegate.PushViewLikePresentView (eventScreen);
		}

		private static void CreatePoll(UIAlertAction act)
		{
			CreatePollScreen pollScreen = new CreatePollScreen ();
			AppDelegate.PushViewLikePresentView (pollScreen);
		}
	}
}

