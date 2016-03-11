using Board.Schema;

using CoreGraphics;
using UIKit;

namespace Board.Interface.LookUp
{
	public class MapLookUp : LookUp
	{
		public MapLookUp(Map map)
		{
			this.content = map;

			UIColor backColor = UIColor.FromRGB(250,250,250);
			UIColor frontColor = AppDelegate.BoardBlack;

			View.BackgroundColor = backColor;

			CreateButtons (frontColor);

			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			ScrollView.UserInteractionEnabled = true;

			//UITextView textView = LoadTextView (announcement, frontColor);

			//ScrollView.AddSubview (textView);

			View.AddSubviews (ScrollView, BackButton, LikeButton, FacebookButton, ShareButton, TrashButton);
		}

		private UITextView LoadMap(Announcement announcement, UIColor color){

			UITextView textView = new UITextView(new CGRect (10,
				TrashButton.Frame.Bottom,
				AppDelegate.ScreenWidth - 20,
				AppDelegate.ScreenHeight - TrashButton.Frame.Bottom - LikeButton.Frame.Height));

			textView.AttributedText = announcement.Text;
			textView.Editable = false;
			textView.ScrollEnabled = true;
			textView.Selectable = true;
			textView.DataDetectorTypes = UIDataDetectorType.Link;
			textView.UserInteractionEnabled = true;
			textView.TextColor = color;
			textView.BackgroundColor = UIColor.FromRGBA (250, 250, 250, 0);

			return textView;

		}
	}
}

