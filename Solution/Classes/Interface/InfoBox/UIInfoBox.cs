using System;
using System.Collections.Generic;
using Clubby.Facebook;
using Clubby.Schema;
using Clubby.Screens;
using Clubby.Utilities;
using CoreGraphics;
using UIKit;

namespace Clubby.Interface
{
	public sealed class UIInfoBox : UIScrollView
	{
		public const int XMargin = 0;

		UIMapContainer Container;
		UILabel NameLabel, CategoryLabel, InstagramLabel, OpenLabel;
		UITopBanner Banner;
		UITextView AboutBox;
		UIActionButtons ActionButtons;
		UIInstagramGallery InstagramGallery;
		UIImageView Line1, Line2;
		List<UIView> ListSubviews;

		public UIInfoBox(Venue venue){
			Frame = new CGRect (0, 0, AppDelegate.ScreenWidth - XMargin * 2, AppDelegate.ScreenHeight);
			Center = new CGPoint (XMargin + Frame.Width / 2, AppDelegate.ScreenHeight / 2);
			BackgroundColor = UIColor.White;
			ClipsToBounds = true;
			 
			Banner = new UITopBanner ((float)Frame.Width);
			NameLabel = new UITitleLabel (Banner.Bottom + 20, (float)Frame.Width,
				UIFont.SystemFontOfSize(20, UIFontWeight.Medium), CommonUtils.FirstLetterOfEveryWordToUpper(UIVenueInterface.venue.Name));
			
			CategoryLabel = new UITitleLabel ((float)NameLabel.Frame.Bottom + 3, (float)Frame.Width,
				UIFont.SystemFontOfSize(14, UIFontWeight.Regular), venue.GetAllCategories().ToUpper());

			OpenLabel = new UITitleLabel ((float)CategoryLabel.Frame.Bottom, (float)Frame.Width, UIFont.SystemFontOfSize(14, UIFontWeight.Regular), string.Empty);
			OpenLabel.Text = "CHECKING...";

			FacebookUtils.MakeGraphRequest (venue.FacebookId, "?fields=hours", CheckIfOpen);

			ActionButtons = new UIActionButtons (venue, (float)OpenLabel.Frame.Bottom + 10, (float)Frame.Width);
			AboutBox = new UIAboutBox (venue.About, (float)OpenLabel.Frame.Bottom + 75, (float)Frame.Width);

			float lastBottom = (float)AboutBox.Frame.Bottom;

			if (venue.GeolocatorObject.Coordinate.Latitude != 0 && venue.GeolocatorObject.Coordinate.Longitude != 0) {
				Container = new UIMapContainer (Frame, (float)AboutBox.Frame.Bottom + 30);
				lastBottom = (float)Container.Map.Frame.Bottom;
			}

			Line1 = new UIImageView (new CGRect (0, Container.Map.Frame.Bottom + 20, Frame.Width, 1));
			Line1.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 90);

			InstagramLabel = new UITitleLabel ((float)Line1.Frame.Bottom + 20, (float)Frame.Width,
				UIFont.SystemFontOfSize(16, UIFontWeight.Medium), "Latest Photos");

			var images = UIVenueInterface.venue.ContentList.GetRange(0, 11);

			InstagramGallery = new UIInstagramGallery ((float)Frame.Width, (float)InstagramLabel.Frame.Bottom + 15, images);
			lastBottom = (float)InstagramGallery.Frame.Bottom;

			ListSubviews = new List<UIView> ();

			AddSubview(Banner);

			ListSubviews.Add (CategoryLabel);
			ListSubviews.Add (NameLabel);
			ListSubviews.Add (AboutBox);
			ListSubviews.Add (OpenLabel);
			ListSubviews.Add (Line1);
			ListSubviews.Add (InstagramGallery);
			ListSubviews.Add (InstagramLabel);

			if (Container != null) {
				ListSubviews.Add (Container.Map);
			}

			foreach (var button in ActionButtons.ListActionButton) {
				ListSubviews.Add (button);
			}

			ContentSize = new CGSize (Frame.Width, lastBottom + 20 * 3);

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