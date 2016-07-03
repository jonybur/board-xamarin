using System;
using System.Collections.Generic;
using Clubby.Infrastructure;
using Clubby.Schema;
using System.Linq;
using Clubby.Utilities;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using Haneke;
using UIKit;

namespace Clubby.Screens.Controls
{
	public class UICarouselContentDisplay : UIContentDisplay {

		const float SeparationBetweenCarousels = 30;

		public UICarouselContentDisplay(List<Venue> venueList){
			ListThumbs = new List<UIContentThumb> ();

			var magazineDictionary = new Dictionary<string, List<Venue>> ();

			var friendLikes = venueList.Select (x => x.FriendLikes).OrderByDescending(x=>x);
			var unique_likes = new HashSet<int> (friendLikes);

			foreach (var like in unique_likes) {
				if (like == 0) {
					continue;
				}
				magazineDictionary.Add ("Liked by " + like + " of your friends", venueList.FindAll (x => x.FriendLikes == like));
			}

			var testCarousels = new List<UICarouselController> ();
			int i = 0;
			foreach (var entry in magazineDictionary){
				var carousel = new UICarouselController (entry.Value, entry.Key);

				carousel.View.Center = new CGPoint (AppDelegate.ScreenWidth / 2,
					UIMenuBanner.Height + SeparationBetweenCarousels +
					carousel.View.Frame.Height / 2 + (carousel.View.Frame.Height + SeparationBetweenCarousels) * i);
				testCarousels.Add (carousel);

				ListViews.Add (carousel.View);

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

		public UICarouselController(List<Venue> boardList, string titleText){
			TitleLabel = new UILocationLabel (titleText, ItemSeparation);
			ListThumbs = new List<UIContentThumb> ();

			ScrollView = new UIScrollView (new CGRect (0, TitleLabel.Frame.Bottom + 15, AppDelegate.ScreenWidth, UICarouselLargeItem.Height + 45));

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
		public const int TextSpace = 60;

		public UICarouselLargeItem (Venue board) {
			Frame = new CGRect (0, 0, Width, Height);

			var backgroundImageView = new UIImageView ();
			backgroundImageView.Frame = Frame;
			backgroundImageView.BackgroundColor = UIColor.FromRGB(250,250,250);
			backgroundImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			backgroundImageView.SetImage (new NSUrl (board.CoverImageUrl));
			backgroundImageView.ClipsToBounds = true;
			backgroundImageView.Layer.CornerRadius = 10;

			var backgroundLogoImageView = new UIImageView ();
			backgroundLogoImageView.Frame = new CGRect (0, 0, Height / 2 + 6, Height / 2 + 6);
			backgroundLogoImageView.Center = new CGPoint (Frame.Width / 2, Frame.Height / 2);
			backgroundLogoImageView.BackgroundColor = UIColor.White;
			backgroundLogoImageView.Layer.CornerRadius = backgroundLogoImageView.Frame.Width / 2;
			backgroundLogoImageView.ClipsToBounds = true;

			var logoImageView = new UIImageView ();
			logoImageView.Frame = new CGRect (0, 0, Height / 2, Height / 2);
			logoImageView.Center = new CGPoint (Frame.Width / 2, Frame.Height / 2);
			logoImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			logoImageView.SetImage (new NSUrl (board.LogoUrl));
			logoImageView.ClipsToBounds = true;
			logoImageView.Layer.CornerRadius = logoImageView.Frame.Width / 2;

			var nameLabel = new UILabel ();
			if (board.GeolocatorObject.Coordinate.Latitude == 0 && board.GeolocatorObject.Coordinate.Longitude == 0) {
				nameLabel = CreateNameLabel (board.Name, Width);
			} else {
				nameLabel = CreateNameLabel (board.Name, CommonUtils.GetDistanceFromUserToBoard (board), Width);
			}
			nameLabel.Frame = new CGRect (5, Frame.Bottom + 10, nameLabel.Frame.Width - 10, nameLabel.Frame.Height);

			AddSubviews (backgroundImageView, backgroundLogoImageView, logoImageView, nameLabel);

			TouchEvent = (sender, e) => {
				
				CATransaction.Begin ();

				Alpha = 0.75f;

				CATransaction.Commit();

				CATransaction.CompletionBlock = delegate {
					AppDelegate.OpenBoard(board);
					Alpha = 1f;
				};
			};
		}
		private UILabel CreateNameLabel (string nameString, float width)
		{
			var label = new UILabel ();

			label.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);

			nameString = CommonUtils.FirstLetterOfEveryWordToUpper (nameString);
			nameString = CommonUtils.LimitStringToWidth (nameString, UIFont.SystemFontOfSize (14), width - 20);

			string compositeString = nameString;

			var nameAttributes = new UIStringAttributes {
				Font = UIFont.SystemFontOfSize (14),
				ForegroundColor = UIColor.White
			};

			var attributedString = new NSMutableAttributedString (compositeString);
			attributedString.SetAttributes (nameAttributes.Dictionary, new NSRange (0, nameString.Length));

			label.TextColor = AppDelegate.ClubbyBlack;
			label.Lines = 0;
			label.AttributedText = attributedString;
			label.AdjustsFontSizeToFitWidth = false;
			label.Font = UIFont.SystemFontOfSize (14);
			label.Frame = new CGRect (5, width, width - 10, TextSpace);

			var size = label.SizeThatFits (label.Frame.Size);
			label.Frame = new CGRect (label.Frame.X, label.Frame.Y, label.Frame.Width, size.Height);

			return label;
		}

		private UILabel CreateNameLabel (string nameString, double distance, float width)
		{
			var label = new UILabel ();

			label.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);

			var distanceTotalString = CommonUtils.GetFormattedDistance (distance);

			nameString = CommonUtils.FirstLetterOfEveryWordToUpper (nameString);
			nameString = CommonUtils.LimitStringToWidth (nameString, UIFont.SystemFontOfSize (14), width - 20);

			string compositeString = nameString + "\n" + distanceTotalString;

			var nameAttributes = new UIStringAttributes {
				Font = UIFont.SystemFontOfSize (14),
				ForegroundColor = UIColor.White
			};

			var distanceAttributes = new UIStringAttributes {
				Font = UIFont.SystemFontOfSize (14),
				ForegroundColor = UIColor.FromRGBA(255,255,255,200)
			};

			var attributedString = new NSMutableAttributedString (compositeString);
			attributedString.SetAttributes (nameAttributes.Dictionary, new NSRange (0, nameString.Length));
			attributedString.SetAttributes (distanceAttributes, new NSRange (nameString.Length, distanceTotalString.Length + 1));

			label.TextColor = AppDelegate.ClubbyBlack;
			label.Lines = 0;
			label.AttributedText = attributedString;
			label.AdjustsFontSizeToFitWidth = false;
			label.Font = UIFont.SystemFontOfSize (14);
			label.Frame = new CGRect (5, width, width - 10, TextSpace);

			var size = label.SizeThatFits (label.Frame.Size);
			label.Frame = new CGRect (label.Frame.X, label.Frame.Y, label.Frame.Width, size.Height);

			return label;
		}
	}
}

