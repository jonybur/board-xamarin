using Board.Interface.Buttons;

using Board.Schema;

using CoreGraphics;
using Facebook.CoreKit;
using UIKit;

namespace Board.Interface.LookUp
{
	public class EventLookUp : LookUp
	{
		public EventLookUp(BoardEvent boardEvent)
		{
			this.content = boardEvent;

			View.BackgroundColor = UIColor.FromRGB(250,250,250);

			ScrollView.UserInteractionEnabled = true;

			CreateButtons (BoardInterface.board.MainColor);

			View.AddSubviews (ScrollView, BackButton, LikeButton, FacebookButton, ShareButton);

			if (Profile.CurrentProfile.UserID == BoardInterface.board.CreatorId) {
				View.AddSubview (TrashButton);
			}
		}
	}
}

