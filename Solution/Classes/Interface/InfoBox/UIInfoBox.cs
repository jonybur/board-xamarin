using System;
using System.Collections.Generic;
using Clubby.Facebook;
using Clubby.Schema;
using Clubby.Utilities;
using CoreGraphics;
using UIKit;

namespace Clubby.Interface
{
	public sealed class UIInfoBox : UIScrollView
	{
		public const int XMargin = 0;
		public const int XContentMargin = 10;

		UIMapContainer Container;
		UILabel NameLabel, AddressLabel, CategoryLabel, OpenLabel;//, InstagramLabel
		UITopBanner Banner;
		UITextView AboutBox;
		UIActionButtons ActionButtons;
		UIInstagramGallery InstagramGallery;
		UIImageView Line2;//, Line1
		List<UIView> ListSubviews;

		public UIInfoBox(Venue venue){
			Frame = new CGRect (0, 0, AppDelegate.ScreenWidth - XMargin * 2, AppDelegate.ScreenHeight);
			Center = new CGPoint (XMargin + Frame.Width / 2, AppDelegate.ScreenHeight / 2);
			BackgroundColor = AppDelegate.ClubbyBlack;
			ClipsToBounds = true;
			 
			Banner = new UITopBanner ((float)Frame.Width);
			NameLabel = new UITitleLabel (Banner.Bottom + 80, (float)Frame.Width,
				UIFont.SystemFontOfSize(26, UIFontWeight.Regular), CommonUtils.FirstLetterOfEveryWordToUpper(UIVenueInterface.venue.Name), AppDelegate.ClubbyYellow);
			
			CategoryLabel = new UITitleLabel ((float)NameLabel.Frame.Bottom + 6, (float)Frame.Width,
				UIFont.SystemFontOfSize(14, UIFontWeight.Regular), venue.GetAllCategories().ToUpper());

			OpenLabel = new UITitleLabel ((float)CategoryLabel.Frame.Bottom + 6, (float)Frame.Width, UIFont.SystemFontOfSize(14, UIFontWeight.Regular), string.Empty);
			OpenLabel.Text = "CHECKING...";

			FacebookUtils.MakeGraphRequest (venue.FacebookId, "?fields=hours", CheckIfOpen);

			ActionButtons = new UIActionButtons (venue, (float)OpenLabel.Frame.Bottom + 10, (float)Frame.Width);
			AboutBox = new UIAboutBox (venue.About, (float)OpenLabel.Frame.Bottom + 75, (float)Frame.Width);

			float lastBottom = (float)AboutBox.Frame.Bottom;

			if (venue.GeolocatorObject.Coordinate.Latitude != 0 && venue.GeolocatorObject.Coordinate.Longitude != 0) {
				
				AddressLabel = new UITitleLabel ((float)AboutBox.Frame.Bottom + 30, (float)Frame.Width,
					UIFont.SystemFontOfSize(14, UIFontWeight.Regular), venue.GeolocatorObject.AddressWithNeighborhood);
				AddressLabel.TextAlignment = UITextAlignment.Left;
				
				Container = new UIMapContainer (Frame, (float)AddressLabel.Frame.Bottom + 15);
				lastBottom = (float)Container.Map.Frame.Bottom;
			}

			//Line1 = new UIImageView (new CGRect (0, Container.Map.Frame.Bottom + 25, Frame.Width, 1));
			//Line1.BackgroundColor = UIColor.FromRGBA (255, 255, 255, 100);

			//InstagramLabel = new UITitleLabel ((float)Line1.Frame.Bottom + 20, (float)Frame.Width,
			//UIFont.SystemFontOfSize(14, UIFontWeight.Medium), "Latest Photos");

			var contentList = UIVenueInterface.venue.ContentList;
			int maxImages = contentList.Count < 11 ? contentList.Count : 11;

			var images = contentList.GetRange(0, maxImages);

			InstagramGallery = new UIInstagramGallery ((float)Frame.Width, (float)lastBottom /*+ 20*/, images);
			lastBottom = (float)InstagramGallery.Frame.Bottom;

			ListSubviews = new List<UIView> ();

			AddSubview(Banner);

			ListSubviews.Add (CategoryLabel);
			ListSubviews.Add (NameLabel);
			ListSubviews.Add (AboutBox);
			ListSubviews.Add (OpenLabel);
			ListSubviews.Add (InstagramGallery);
			ListSubviews.Add (AddressLabel);

			if (Container != null) {
				ListSubviews.Add (Container.Map);
			}

			foreach (var button in ActionButtons.ListActionButton) {
				ListSubviews.Add (button);
			}

			ContentSize = new CGSize (Frame.Width, lastBottom);

			SelectiveRendering (new CGPoint(0,0));

			Scrolled += (sender, e) => {
				if (ContentOffset.Y < 0){
					Banner.Center = new CGPoint(Banner.Center.X, Banner.Frame.Height / 2 + ContentOffset.Y);
				}

				SelectiveRendering(ContentOffset);
			};
		}

		public void SelectiveRendering(CGPoint contentOffset){

			foreach (var view in ListSubviews) {

				// if its on a screenheight * 2 range...
				if (view.Frame.Y > (contentOffset.Y - view.Frame.Height) &&
					view.Frame.Y < (contentOffset.Y + AppDelegate.ScreenHeight * 2)) {

					// if its on a screenheight range
					if (view.Frame.Y > (contentOffset.Y - view.Frame.Height) &&
						view.Frame.Y < (contentOffset.Y + AppDelegate.ScreenHeight)) {

						AddSubview (view);
					}

				} else if (view.Superview != null) {

					// if its not on a screenheight * 2 range and has been drawn, dissolve it
					view.RemoveFromSuperview ();

				}

			}
		}

		public void CheckIfOpen(List<FacebookElement> obj){
			if (obj == null) {
				OpenLabel.Text = string.Empty;
				return;
			}

			if (obj.Count == 0) {
				OpenLabel.Text = string.Empty;
				return;
			}

			if (obj.Count > 0) {

				var fbhour = (FacebookHours)obj[0];
				if (fbhour.Hours == null) {
					OpenLabel.Text = string.Empty;
					return;
				}

				var dayOfWeek = DateTime.Today.DayOfWeek.ToString ().Substring (0, 3).ToLower ();

				var indexStart = fbhour.Hours.IndexOf (dayOfWeek + "_1_open", StringComparison.Ordinal);
				var indexEnd = fbhour.Hours.IndexOf (dayOfWeek + "_1_close", StringComparison.Ordinal);

				if (indexStart == -1 || indexEnd == -1) {
					OpenLabel.Text = "NOW CLOSED";
					//OpenLabel.TextColor = UIColor.FromRGB (82, 6, 11);
					return;
				}
				indexStart += 15;
				indexEnd += 16;

				var startStringDate = fbhour.Hours.Substring (indexStart, 5);
				var endStringDate = fbhour.Hours.Substring (indexEnd, 5);

				var	startDate = DateTime.Parse (startStringDate);
				var	endDate = DateTime.Parse (endStringDate);

				var startTotalMinutes = startDate.TimeOfDay.TotalMinutes; 		 // 1380
				var endTotalMinutes = endDate.TimeOfDay.TotalMinutes;     		 // 300 + 1440 = 1740
				var currentTotalMinutes = DateTime.Now.TimeOfDay.TotalMinutes;   // 122.68

				if (endTotalMinutes < startTotalMinutes) {
					endTotalMinutes += 1440;
				}

				if (startTotalMinutes <= currentTotalMinutes && currentTotalMinutes <= endTotalMinutes) {
					OpenLabel.Text = "NOW OPEN";
					//OpenLabel.TextColor = UIColor.FromRGB (28, 57, 16);
				} else {
					OpenLabel.Text = "NOW CLOSED";
					//OpenLabel.TextColor = UIColor.FromRGB (82, 6, 11);
				}
			}
		}
	}
}