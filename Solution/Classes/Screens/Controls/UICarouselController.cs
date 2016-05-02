using CoreGraphics;
using UIKit;
using MGImageUtilitiesBinding;

namespace Board.Screens.Controls
{
	public class UICarouselController : UIViewController
	{
		UILabel Title;
		UIScrollView ScrollView;

		public const int ScrollViewHeight = 100;
		public const int ItemSeparation = 30;

		public UICarouselController(){
			Title = new UILabel (new CGRect (ItemSeparation, 0, AppDelegate.ScreenWidth - ItemSeparation * 2, 20));
			Title.Font = AppDelegate.Narwhal18;
			Title.AdjustsFontSizeToFitWidth = true;
			Title.TextColor = AppDelegate.BoardOrange;
			Title.Text = "ACTIVE NOW";

			ScrollView = new UIScrollView (new CGRect (0, Title.Frame.Bottom + 15, AppDelegate.ScreenWidth, ScrollViewHeight));
			for (int i = 0; i < 3; i++) {
				var carouselLargeItem = new UICarouselLargeItem ();
				carouselLargeItem.Center = new CGPoint (ItemSeparation + carouselLargeItem.Frame.Width / 2 + (carouselLargeItem.Frame.Width + ItemSeparation) * i,
														carouselLargeItem.Frame.Height / 2);
				ScrollView.AddSubview (carouselLargeItem);
			}
			ScrollView.ContentSize = new CGSize (800, ScrollViewHeight);
			ScrollView.ShowsHorizontalScrollIndicator = false;
			ScrollView.UserInteractionEnabled = true;

			Add (Title);
			Add (ScrollView);

			View.Frame = new CGRect(0,0, AppDelegate.ScreenWidth, ScrollView.Frame.Bottom);
		}
	}

	public class UICarouselLargeItem : UIButton {
		public UICarouselLargeItem () {

			// 410 x 200

			Frame = new CGRect (0, 0, 205, UICarouselController.ScrollViewHeight);
			BackgroundColor = UIColor.Black;
			Layer.CornerRadius = 10;
		}
	}
}

