using System.Collections.Generic;
using Board.Infrastructure;
using Board.Interface;
using Board.JsonResponses;
using CoreGraphics;
using Foundation;
using Haneke;
using UIKit;

namespace Board.Screens.Controls
{
	public class UICarouselContentDisplay : UIContentDisplay {

		const float SeparationBetweenCarousels = 30;

		public UICarouselContentDisplay(MagazineResponse magazine){
			ListThumbs = new List<UIContentThumb> ();

			var magazineDictionary = new Dictionary<string, List<Board.Schema.Board>> ();
			var magazineList = new List<Board.Schema.Board> ();
			string section = string.Empty;

			foreach (var entries in magazine.data.entries) {
				var board = CloudController.GenerateBoardFromBoardResponse (entries.board);

				if (section == string.Empty) {
					section = entries.section;
					magazineList.Add (board);
					continue;
				}

				if (section != entries.section) {
					magazineDictionary.Add (section, magazineList);

					section = entries.section;
					magazineList = new List<Board.Schema.Board> ();
				}
				magazineList.Add (board);
			}
			magazineDictionary.Add (section, magazineList);

			var testCarousels = new List<UICarouselController> ();

			int i = 0;
			foreach (var entry in magazineDictionary){
				var carousel = new UICarouselController (entry.Value, entry.Key);

				carousel.View.Center = new CGPoint (AppDelegate.ScreenWidth / 2,
					UIMagazineBannerPage.Height + UIMenuBanner.Height + SeparationBetweenCarousels +
					carousel.View.Frame.Height / 2 + (carousel.View.Frame.Height + SeparationBetweenCarousels) * i);
				testCarousels.Add (carousel);

				AddSubview (carousel.View);

				ListThumbs.AddRange (carousel.ListThumbs);
				i++;
			}
			var size = new CGSize (AppDelegate.ScreenWidth, (float)testCarousels[testCarousels.Count - 1].View.Frame.Bottom + UIActionButton.Height * 2 + SeparationBetweenCarousels);
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

		public UICarouselController(List<Board.Schema.Board> boardList, string titleText){
			TitleLabel = new UILocationLabel (titleText, ItemSeparation);
			ListThumbs = new List<UIContentThumb> ();

			ScrollView = new UIScrollView (new CGRect (0, TitleLabel.Frame.Bottom + 15, AppDelegate.ScreenWidth, UICarouselLargeItem.Height));

			ScrollView.ScrollsToTop = false;

			for (int i = 0; i < boardList.Count; i++) {
				var carouselLargeItem = new UICarouselLargeItem (boardList[i]);
				carouselLargeItem.Center = new CGPoint (ItemSeparation + carouselLargeItem.Frame.Width / 2 + (carouselLargeItem.Frame.Width + ItemSeparation) * i,
														carouselLargeItem.Frame.Height / 2);
				ListThumbs.Add (carouselLargeItem);
				ScrollView.AddSubview (carouselLargeItem);
			}
			ScrollView.ContentSize = new CGSize (ItemSeparation + boardList.Count * (UICarouselLargeItem.Width + ItemSeparation),
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

			var backgroundImageView = new UIImageView ();
			backgroundImageView.Frame = Frame;
			backgroundImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			backgroundImageView.SetImage (new NSUrl (board.CoverImageUrl));

			var logoImageView = new UIImageView ();
			logoImageView.Frame = new CGRect (0, 0, Height / 2, Height / 2);
			logoImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			logoImageView.SetImage (new NSUrl (board.LogoUrl));

			var backgroundLogoImageView = new UIImageView ();
			backgroundLogoImageView.Frame = new CGRect (0, 0, Height / 2 + 6, Height / 2 + 6);
			backgroundLogoImageView.BackgroundColor = UIColor.White;

			logoImageView.Center = new CGPoint (Frame.Width / 2, Frame.Height / 2);
			backgroundLogoImageView.Center = logoImageView.Center;

			AddSubviews (backgroundImageView, backgroundLogoImageView, logoImageView);

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

