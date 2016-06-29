using System.Collections.Generic;
using CoreGraphics;
using UIKit;
using System;
using Board.Utilities;
using Board.Facebook;

namespace Board.Interface
{
	public sealed class UIInfoBox : UIScrollView
	{
		public const int XMargin = 10;

		UIMapContainer Container;
		UILabel NameLabel, CategoryLabel, InstagramLabel, OpenLabel;
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
			 
			Banner = new UITopBanner ((float)Frame.Width);
			NameLabel = new UITitleLabel (Banner.Bottom + 20, (float)Frame.Width,
				UIFont.SystemFontOfSize(20, UIFontWeight.Medium), CommonUtils.FirstLetterOfEveryWordToUpper(UIBoardInterface.board.Name));
			CategoryLabel = new UITitleLabel ((float)NameLabel.Frame.Bottom + 3, (float)Frame.Width,
				UIFont.SystemFontOfSize(14, UIFontWeight.Regular), board.Category.ToUpper());

			OpenLabel = new UITitleLabel ((float)CategoryLabel.Frame.Bottom, (float)Frame.Width, UIFont.SystemFontOfSize(14, UIFontWeight.Regular), string.Empty);
			OpenLabel.Text = "CHECKING...";

			FacebookUtils.MakeGraphRequest (board.FacebookId, "?fields=hours", CheckIfOpen);

			ActionButtons = new UIActionButtons (board, (float)OpenLabel.Frame.Bottom + 10, (float)Frame.Width);
			AboutBox = new UIAboutBox (board.About, (float)OpenLabel.Frame.Bottom + 75, (float)Frame.Width);

			float lastBottom = (float)AboutBox.Frame.Bottom;

			if (board.GeolocatorObject.Coordinate.Latitude != 0 && board.GeolocatorObject.Coordinate.Longitude != 0) {
				Container = new UIMapContainer (Frame, (float)AboutBox.Frame.Bottom + 30);
				lastBottom = (float)Container.Map.Frame.Bottom;
			}

			/*
			Line1 = new UIImageView (new CGRect (0, Container.Map.Frame.Bottom + 20, Frame.Width, 1));
			Line1.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 90);

			InstagramLabel = new UITitleLabel ((float)Line1.Frame.Bottom + 20, (float)Frame.Width,
												AppDelegate.Narwhal16, "LATEST CUSTOMER PHOTOS");
			
			var images = new List<UIImage> ();

			var testImage = UIImage.FromFile ("./demo/magazine/nantucket.png");
			for (int i = 0; i < 8; i++) {
				images.Add (testImage);
			}

			InstagramGallery = new UIInstagramGallery ((float)Frame.Width, (float)InstagramLabel.Frame.Bottom + 15, images);
			*/

			AddSubviews (Banner, CategoryLabel, NameLabel, NameLabel, AboutBox, OpenLabel);//, Line1, InstagramGallery, InstagramLabel);

			if (Container != null) {
				AddSubview(Container.Map);
			}

			foreach (var button in ActionButtons.ListActionButton) {
				AddSubview (button);
			}

			ContentSize = new CGSize (Frame.Width, lastBottom + Board.Interface.Buttons.ButtonInterface.ButtonBarHeight * 3);

			Scrolled += (sender, e) => {
				if (ContentOffset.Y < 0){
					Banner.Center = new CGPoint(Banner.Center.X, Banner.Frame.Height / 2 + ContentOffset.Y);
				}
			};
		}

		public void CheckIfOpen(List<FacebookElement> obj){
			if (obj == null) {
				OpenLabel.Text = "-";
				return;
			}

			if (obj.Count == 0) {
				OpenLabel.Text = "-";
				return;
			}

			if (obj.Count > 0) {

				var fbhour = (FacebookHours)obj[0];
				if (fbhour.Hours == null) {
					OpenLabel.Text = "-";
					return;
				}

				var dayOfWeek = DateTime.Today.DayOfWeek.ToString ().Substring (0, 3).ToLower ();

				var indexStart = fbhour.Hours.IndexOf (dayOfWeek + "_1_open", StringComparison.Ordinal);
				var indexEnd = fbhour.Hours.IndexOf (dayOfWeek + "_1_close", StringComparison.Ordinal);

				if (indexStart == -1 || indexEnd == -1) {
					OpenLabel.Text = "NOW CLOSED";
					OpenLabel.TextColor = UIColor.FromRGB (82, 6, 11);
					return;
				}
				indexStart += 15;
				indexEnd += 16;

				var startStringDate = fbhour.Hours.Substring (indexStart, 5);
				var endStringDate = fbhour.Hours.Substring (indexEnd, 5);

				var	startDate = DateTime.Parse (startStringDate);
				var	endDate = DateTime.Parse (endStringDate);

				var startTotalMinutes = startDate.TimeOfDay.TotalMinutes;
				var endTotalMinutes = endDate.TimeOfDay.TotalMinutes;
				var currentTotalMinutes = DateTime.Now.TimeOfDay.TotalMinutes;

				if (endTotalMinutes < startTotalMinutes) {
					endTotalMinutes += 1440;
				}

				if (startTotalMinutes <= currentTotalMinutes && currentTotalMinutes <= endTotalMinutes) {
					OpenLabel.Text = "NOW OPEN";
					OpenLabel.TextColor = UIColor.FromRGB (28, 57, 16);
				} else {
					OpenLabel.Text = "NOW CLOSED";
					OpenLabel.TextColor = UIColor.FromRGB (82, 6, 11);
				}
			}
		}
	}
}