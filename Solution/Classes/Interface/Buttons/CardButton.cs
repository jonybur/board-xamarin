using CoreGraphics;
using CoreAnimation;
using Board.Interface.CreateScreens;
using UIKit;
using Board.Schema;

namespace Board.Interface.Buttons
{
	public class CardButton : BIButton
	{
		public CardButton ()
		{
			using (var img = UIImage.FromFile ("./boardinterface/nubuttons/nucard.png")) {
				SetImage (img, UIControlState.Normal);
			}

			Frame = new CGRect (0,0, ButtonSize, ButtonSize);

			Center = new CGPoint ((AppDelegate.ScreenWidth + ButtonSize) / 2 +
				(AppDelegate.ScreenWidth - ButtonSize) / 8 + 10, AppDelegate.ScreenHeight - ButtonSize / 2);
			
			eventHandlers.Add ((sender, e) => {
				var alert = UIAlertController.Create(null, "Select the type of widget", UIAlertControllerStyle.ActionSheet);

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
			var announcementScreen = new CreateAnnouncementScreen ();
			AppDelegate.PushViewLikePresentView (announcementScreen);
		}

		private static void CreateEvent(UIAlertAction act)
		{
			var eventScreen = new CreateEventScreen ();
			AppDelegate.PushViewLikePresentView (eventScreen);
		}

		private static void CreatePoll(UIAlertAction act)
		{
			var pollScreen = new CreatePollScreen ();
			AppDelegate.PushViewLikePresentView (pollScreen);
		}

		private static void CreateSticker(UIAlertAction act){
			UIPreviewSticker.PreviewSticker = new UISticker ();
			UIPreviewSticker.PreviewSticker.BecomeFirstResponder ();

			// switches to confbar
			ButtonInterface.SwitchButtonLayout (ButtonInterface.ButtonLayout.ConfirmationBar);
		}

		private static void CreateMap(UIAlertAction act)
		{
			Preview.Initialize (new Map ());
		}
	}
}

