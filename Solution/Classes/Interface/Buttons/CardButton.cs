using CoreGraphics;
using CoreAnimation;
using Board.Interface.CreateScreens;
using UIKit;
using Board.Schema;

namespace Board.Interface.Buttons
{
	public class CardButton : Button
	{
		public CardButton ()
		{
			uiButton = new UIButton (UIButtonType.Custom);

			using (UIImage uiImage = UIImage.FromFile ("./boardinterface/nubuttons/nucard.png")) {
				uiButton.SetImage (uiImage, UIControlState.Normal);
			}

			uiButton.Frame = new CGRect (0,0, ButtonSize, ButtonSize);

			uiButton.Center = new CGPoint ((AppDelegate.ScreenWidth + ButtonSize) / 2 +
				(AppDelegate.ScreenWidth - ButtonSize) / 8 + 10, AppDelegate.ScreenHeight - ButtonSize / 2);
			
			eventHandlers.Add ((sender, e) => {
				UIAlertController alert = UIAlertController.Create(null, "Select the type of widget", UIAlertControllerStyle.ActionSheet);

				alert.AddAction (UIAlertAction.Create ("Announcement", UIAlertActionStyle.Default, CreateAnnouncement));
				alert.AddAction (UIAlertAction.Create ("Event", UIAlertActionStyle.Default, CreateEvent));
				alert.AddAction (UIAlertAction.Create ("Poll", UIAlertActionStyle.Default, CreatePoll));
				alert.AddAction (UIAlertAction.Create ("Promotion (coming soon)", UIAlertActionStyle.Default, null));
				alert.AddAction (UIAlertAction.Create ("Spotify® Playlist (coming soon)", UIAlertActionStyle.Default, null));
				alert.AddAction (UIAlertAction.Create ("Sticker (coming soon)", UIAlertActionStyle.Default, CreateSticker));
				alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));

				AppDelegate.BoardInterface.NavigationController.PresentViewController (alert, true, null);
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

		private static void CreateSticker(UIAlertAction act){
			UISticker sticker = new UISticker ();
			System.Console.WriteLine (sticker.CanBecomeFirstResponder);
			sticker.BecomeFirstResponder ();
		}

		private static void CreateMap(UIAlertAction act)
		{
			Preview.Initialize (new Map ());
		}
	}
}

