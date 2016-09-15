using System;
using System.Collections.Generic;
using Board.Facebook;
using Board.Schema;
using Board.JsonResponses;
using Board.Screens.Controls;
using Board.Interface.VenueInterface;
using Board.Utilities;
using System.Linq;
using DACircularProgress;
using CoreGraphics;
using UIKit;

namespace Board.Interface
{
	public sealed class UIInfoBox : UIScrollView
	{
		public const int XMargin = 0;
		public const int XContentMargin = 10;

		UIMapContainer Container;
		UILabel NameLabel, AddressLabel, CategoryLabel, OpenLabel;
		UITopBanner Banner;
		UITextView AboutBox;
		UIActionButtons ActionButtons;
		UIInstagramGallery InstagramGallery;
		List<UIView> ListSubviews;

		CircularProgressView progressView;

		public UIInfoBox(string instagramId){
			Frame = new CGRect (0, 0, AppDelegate.ScreenWidth - XMargin * 2, AppDelegate.ScreenHeight);
			Center = new CGPoint (XMargin + Frame.Width / 2, AppDelegate.ScreenHeight / 2);
			BackgroundColor = UIColor.White;
			ClipsToBounds = true;

			LoadCircularProgress ();

			GetInstagramPage (instagramId);
		}

		private void LoadCircularProgress(){
			progressView = new CircularProgressView ();
			progressView.Progress = 0.35f;
			progressView.IndeterminateDuration = 1.0f;
			progressView.Indeterminate = true;
			progressView.Frame = new CGRect (0, 0, 60, 60);
			progressView.Center = new CGPoint (AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight / 2);
			AddSubview (progressView);
		}

		public void StopCircularProgress(){
			progressView.RemoveFromSuperview();
		}

		private async void GetInstagramPage(string instagramId){
			ListSubviews = new List<UIView> ();

			var page = await Infrastructure.CloudController.GetInstagramPage (instagramId);

			var contentList = InstagramPageResponse.GenerateContentList (page.items);

			UILogoImage logo;
			float yposition = 80;

			if (page.items.Count > 0) {
				logo = new UILogoImage (page.items [0].user.profile_picture);
				ListSubviews.Add (logo);
				yposition = (float)logo.Frame.Bottom + 30;
			}

			NameLabel = new UITitleLabel (yposition, (float)Frame.Width,
				UIFont.SystemFontOfSize (26, UIFontWeight.Regular), instagramId, AppDelegate.BoardBlack);

			var images = contentList.GetRange (0, contentList.Count);

			InstagramGallery = new UIInstagramGallery ((float)Frame.Width, (float)NameLabel.Frame.Bottom + 30, images, instagramId);
			float lastBottom = (float)InstagramGallery.Frame.Bottom;

			ListSubviews.Add (NameLabel);
			ListSubviews.Add (InstagramGallery);

			if (Container != null) {
				ListSubviews.Add (Container.Map);
			}

			SetDefaults (lastBottom);

			StopCircularProgress ();
		}

		public UIInfoBox(Board.Schema.Board venue){
			Frame = new CGRect (0, 0, AppDelegate.ScreenWidth - XMargin * 2, AppDelegate.ScreenHeight);
			Center = new CGPoint (XMargin + Frame.Width / 2, AppDelegate.ScreenHeight / 2);
			BackgroundColor = UIColor.White;
			ClipsToBounds = true;

			Banner = new UITopBanner ((float)Frame.Width);
			NameLabel = new UITitleLabel (Banner.Bottom + 80, (float)Frame.Width,
				UIFont.SystemFontOfSize (26, UIFontWeight.Regular), CommonUtils.FirstLetterOfEveryWordToUpper (UIVenueInterface.board.Name), AppDelegate.BoardBlack);

			CategoryLabel = new UITitleLabel ((float)NameLabel.Frame.Bottom + 6, (float)Frame.Width,
				UIFont.SystemFontOfSize (14, UIFontWeight.Regular), venue.Category.ToUpper ());

			OpenLabel = new UITitleLabel ((float)CategoryLabel.Frame.Bottom + 6, (float)Frame.Width, UIFont.SystemFontOfSize (14, UIFontWeight.Regular), string.Empty);
			OpenLabel.Text = "CHECKING...";

			FacebookUtils.MakeGraphRequest (venue.FacebookId, "?fields=hours", CheckIfOpen);

			ActionButtons = new UIActionButtons (venue, (float)OpenLabel.Frame.Bottom + 10, (float)Frame.Width);
			AboutBox = new UIAboutBox (venue.About, (float)OpenLabel.Frame.Bottom + 75, (float)Frame.Width);

			float lastBottom = (float)AboutBox.Frame.Bottom;

			if (venue.GeolocatorObject.Coordinate.Latitude != 0 && venue.GeolocatorObject.Coordinate.Longitude != 0) {

				AddressLabel = new UITitleLabel ((float)AboutBox.Frame.Bottom + 30, (float)Frame.Width,
					UIFont.SystemFontOfSize (14, UIFontWeight.Regular), venue.GeolocatorObject.AddressWithNeighborhood);
				AddressLabel.TextAlignment = UITextAlignment.Left;

				Container = new UIMapContainer (Frame, (float)AddressLabel.Frame.Bottom + 15);
				lastBottom = (float)Container.Map.Frame.Bottom;
			}

			var contentList = UIMagazine.TimelineContent.ContentList.Where (x => x.InstagramId == UIVenueInterface.board.InstagramId).ToList ();
			int maxImages = contentList.Count < 11 ? contentList.Count : 11;

			var images = contentList.GetRange (0, maxImages);

			InstagramGallery = new UIInstagramGallery ((float)Frame.Width, (float)lastBottom, images, UIVenueInterface.board.InstagramId);
			lastBottom = (float)InstagramGallery.Frame.Bottom;

			ListSubviews = new List<UIView> ();

			AddSubview (Banner);

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

			SetDefaults (lastBottom);
		}


		private void SetDefaults(float lastBottom){

			ContentSize = new CGSize (Frame.Width, lastBottom);

			SelectiveRendering (new CGPoint(0,0));

			Scrolled += (sender, e) => {
				if (ContentOffset.Y < 0){
					if (Banner != null){
						Banner.Center = new CGPoint(Banner.Center.X, Banner.Frame.Height / 2 + ContentOffset.Y);
					}
				}

				SelectiveRendering(ContentOffset);
			};
		}

		public void SelectiveRendering(CGPoint contentOffset){

			foreach (var view in ListSubviews) {

				if (view == null) {
					continue;
				}

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
					OpenLabel.Text = string.Empty;
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

					if (currentTotalMinutes - startTotalMinutes < 0) {
						currentTotalMinutes += 1440;
					}
				}

				if (startTotalMinutes <= currentTotalMinutes && currentTotalMinutes <= endTotalMinutes) {
					OpenLabel.Text = "NOW OPEN";
				} else {
					OpenLabel.Text = "OPENS AT " + startDate.ToString ("h:mm tt");
				}
			}
		}
	}
}