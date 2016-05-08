using CoreGraphics;
using UIKit;
using MGImageUtilitiesBinding;
using System.Collections.Generic;
using Board.Interface;

namespace Board.Screens.Controls
{
	public class UICarouselContentDisplay : UIContentDisplay {

		const float SeparationBetweenCarousels = 20;

		// TODO: recieve 'editors magazine'
		public UICarouselContentDisplay(){
			ListThumbs = new List<UIContentThumb> ();
			List<UICarouselController> testCarousels = new List<UICarouselController> ();

			string[] carouselNames = new string[]{"BEST BARMANS", "CHILL MUSIC", "GOOD VIBES", "BEST BARMANS", "CHILL MUSIC" };

			for (int i = 0; i < 5; i++) {
				var carousel = new UICarouselController (carouselNames[i]);
				carousel.View.Center = new CGPoint (AppDelegate.ScreenWidth / 2,
					UIMagazineBannerPage.Height + UIMenuBanner.Height + SeparationBetweenCarousels + carousel.View.Frame.Height / 2 + (carousel.View.Frame.Height + SeparationBetweenCarousels) * i);
				testCarousels.Add (carousel);
				AddSubview (carousel.View);

				ListThumbs.AddRange (carousel.ListThumbs);
			}
			var size = new CGSize (AppDelegate.ScreenWidth, (float)testCarousels[testCarousels.Count - 1].View.Frame.Bottom + UIActionButton.Height + SeparationBetweenCarousels);
			Frame = new CGRect (0, 0, size.Width, size.Height);
			UserInteractionEnabled = true;
		}
	}

	public class UICarouselController : UIViewController
	{
		UILocationLabel TitleLabel;
		UIScrollView ScrollView;
		public List<UIContentThumb> ListThumbs;

		public const int ItemSeparation = 20;

		// TODO: recieve List<Board> boardList
		public UICarouselController(string titleText){
			TitleLabel = new UILocationLabel (titleText, ItemSeparation);
			ListThumbs = new List<UIContentThumb> ();

			ScrollView = new UIScrollView (new CGRect (0, TitleLabel.Frame.Bottom + 15, AppDelegate.ScreenWidth, UICarouselLargeItem.Height));
			for (int i = 0; i < 3; i++) {
				var carouselLargeItem = new UICarouselLargeItem (new Board.Schema.Board());
				carouselLargeItem.Center = new CGPoint (ItemSeparation + carouselLargeItem.Frame.Width / 2 + (carouselLargeItem.Frame.Width + ItemSeparation) * i,
														carouselLargeItem.Frame.Height / 2);
				ListThumbs.Add (carouselLargeItem);
				ScrollView.AddSubview (carouselLargeItem);
			}
			ScrollView.ContentSize = new CGSize (ItemSeparation + 3 * (UICarouselLargeItem.Width + ItemSeparation),
				UICarouselLargeItem.Height);
			ScrollView.ShowsHorizontalScrollIndicator = false;
			ScrollView.UserInteractionEnabled = true;

			Add (TitleLabel);
			Add (ScrollView);

			View.Frame = new CGRect(0,0, AppDelegate.ScreenWidth, ScrollView.Frame.Bottom);
		}
	}

	public class UICarouselLargeItem : UIContentThumb {

		public const int Width = 200;
		public const int Height = 100;

		public UICarouselLargeItem (Board.Schema.Board board) {
			Frame = new CGRect (0, 0, Width, Height);
			BackgroundColor = UIColor.Black;
			Layer.CornerRadius = 10;
			using (var img = UIImage.FromFile ("./screens/main/magazine/carouselitem/1.png")) {
				var scaledImg = img.ImageScaledToFitSize (Frame.Size);
				SetImage (scaledImg, UIControlState.Normal);
			}
			ClipsToBounds = true;

			TouchEvent = (sender, e) => {
				if (AppDelegate.BoardInterface == null)
				{
					AppDelegate.BoardInterface = new UIBoardInterface (board);
					AppDelegate.NavigationController.PushViewController (AppDelegate.BoardInterface, true);
				}
			};
		}
	}
}

