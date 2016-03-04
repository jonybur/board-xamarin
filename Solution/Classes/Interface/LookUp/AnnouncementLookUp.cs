using Board.Interface.Buttons;

using Board.Schema;

using CoreGraphics;
using Facebook.CoreKit;
using UIKit;

namespace Board.Interface.LookUp
{
	public class AnnouncementLookUp : LookUp
	{
		public AnnouncementLookUp(Announcement announcement)
		{
			this.content = announcement;

			View.BackgroundColor = UIColor.FromRGB(250,250,250);

			ScrollView.UserInteractionEnabled = true;

			CreateButtons (BoardInterface.board.MainColor);

			UITextView textView = LoadTextView (announcement);

			ScrollView.AddSubview (textView);

			View.AddSubviews (ScrollView, BackButton, LikeButton, FacebookButton, ShareButton);

			if (Profile.CurrentProfile.UserID == BoardInterface.board.CreatorId) {
				View.AddSubview (TrashButton);
			}
		}

		private UITextView LoadTextView(Announcement announcement){

			UITextView textView = new UITextView(new CGRect (10,
				TrashButton.Frame.Bottom,
				AppDelegate.ScreenWidth - 20,
				AppDelegate.ScreenHeight - TrashButton.Frame.Bottom - LikeButton.Frame.Height));
			
			textView.AttributedText = announcement.Text;
			textView.Editable = false;
			textView.Selectable = true;
			textView.ScrollEnabled = true;
			textView.BackgroundColor = UIColor.FromRGB (250, 250, 250);

			return textView;

		}
	}
}

