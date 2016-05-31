using System.Collections.Generic;
using CoreGraphics;
using UIKit;

namespace Board.Interface
{
	public sealed class UIInfoBox : UIScrollView
	{
		public const int XMargin = 10;

		UIMapContainer Container;
		UILabel NameLabel, CategoryLabel, InstagramLabel;
		UITopBanner Banner;
		UITextView AboutBox;
		UIActionButtons ActionButtons;
		UIInstagramGallery InstagramGallery;
		UIImageView Line1, Line2;

		public UIInfoBox(Board.Schema.Board board){
			Frame = new CGRect (0, 0, AppDelegate.ScreenWidth - XMargin * 2, AppDelegate.ScreenHeight);
			Center = new CGPoint (XMargin + Frame.Width / 2, AppDelegate.ScreenHeight / 2);
			BackgroundColor = UIColor.White;
			ClipsToBounds = true;
			 
			Banner = new UITopBanner (board.Logo, (float)Frame.Width);
			NameLabel = new UITitleLabel (Banner.Bottom + 20, (float)Frame.Width,
											AppDelegate.Narwhal20, UIBoardInterface.board.Name);
			CategoryLabel = new UITitleLabel ((float)NameLabel.Frame.Bottom + 5, (float)Frame.Width,
				AppDelegate.Narwhal14, "NIGHT CLUB · COFFEE SHOP | OPEN NOW");

			Line2 = new UIImageView (new CGRect (0, CategoryLabel.Frame.Bottom + 5, Frame.Width, 1));
			Line2.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 90);
			
			AboutBox = new UIAboutBox (board.About, (float)CategoryLabel.Frame.Bottom + 10, (float)Frame.Width);
			Container = new UIMapContainer (Frame);
			ActionButtons = new UIActionButtons (board, (float)Container.Map.Frame.Top - 60, (float)Frame.Width);
			Line1 = new UIImageView (new CGRect (0, Container.Map.Frame.Bottom + 20, Frame.Width, 1));
			Line1.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 90);

			InstagramLabel = new UITitleLabel ((float)Line1.Frame.Bottom + 20, (float)Frame.Width,
												AppDelegate.Narwhal16, "LATEST CUSTOMER PHOTOS");

			var images = new List<UIImage> ();
			var testImage = UIImage.FromFile ("./demo/magazine/westpalmbeach.png");
			for (int i = 0; i < 12; i++) {
				images.Add (testImage);
			}

			InstagramGallery = new UIInstagramGallery ((float)Frame.Width, (float)InstagramLabel.Frame.Bottom + 20, images);
			AddSubviews (Banner, CategoryLabel, Container.Map, InstagramLabel, Line1, NameLabel, AboutBox, InstagramGallery);

			foreach (var button in ActionButtons.ListActionButton) {
				AddSubview (button);
			}

			ContentSize = new CGSize (Frame.Width, InstagramGallery.Frame.Bottom + Board.Interface.Buttons.ButtonInterface.ButtonBarHeight * 3);

			Scrolled += (sender, e) => {
				if (ContentOffset.Y < 0){
					Banner.Center = new CGPoint(Banner.Center.X, Banner.Frame.Height / 2 + ContentOffset.Y);
				}
			};
		}
	}
}