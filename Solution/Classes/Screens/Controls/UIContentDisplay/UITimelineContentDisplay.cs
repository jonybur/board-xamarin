using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using Board.Schema;
using Board.Utilities;
using Foundation;
using Haneke;

namespace Board.Screens.Controls
{	
	public class UITimelineContentDisplay : UIContentDisplay {

		const float SeparationBetweenObjects = 30;

		public UITimelineContentDisplay(List<Board.Schema.Board> listboards) {
			//var ListImageViews = new List<UIImageView> ();

			float yposition = UIMagazineBannerPage.Height + UIMenuBanner.Height + 30;

			var picture = new Picture ();
			picture.ImageUrl = "https://board-alpha-media.s3.amazonaws.com/716598f1-f0f7-4ac0-b33f-3c2871c4c935/7317f06a-407a-4cdd-b1ac-8eef53624437.jpg";

			for (int i = 0; i < 5; i++) {
				var timelineWidget = new UITimelineWidget (listboards [0], picture);
				timelineWidget.Center = new CGPoint (AppDelegate.ScreenWidth / 2, yposition + timelineWidget.Frame.Height / 2);
				
				AddSubview (timelineWidget);

				yposition += (float)timelineWidget.Frame.Height + SeparationBetweenObjects;
			}


			var size = new CGSize (AppDelegate.ScreenWidth, yposition + UIActionButton.Height);
			Frame = new CGRect (0, 0, size.Width, size.Height);
			UserInteractionEnabled = true;
		}

		class UITimelineWidget : UIView{
			// includes logo, name, distance and time
			UIView headerView;

			// includes heart, number
			UIButton likeButton;

			// includes line, name of board & description
			UITextView descriptionView;

			// can be a picture, a video, an announcement, etc.
			UIView timelineContent;

			const int XMargin = 10;

			public UITimelineWidget(Board.Schema.Board board, Content content){
				GenerateHeaderView(board);

				if (content is Picture){
					
					var pictureContent = (Picture)content;

					timelineContent = new UITimelinePicture(pictureContent);
					timelineContent.Center = new CGPoint(AppDelegate.ScreenWidth / 2,
															headerView.Frame.Bottom + timelineContent.Frame.Height / 2 + 10);

					if (!string.IsNullOrEmpty(pictureContent.Description)) {
						GenerateDescriptionView(pictureContent.Description);
					}
				}

				GenerateLikeView(content.Id);

				AddSubviews(headerView, timelineContent);

				Frame = new CGRect(0, 0, AppDelegate.ScreenWidth, timelineContent.Frame.Bottom);
			}

			private void GenerateHeaderView(Board.Schema.Board board){

				int headerHeight = 40;

				headerView = new UIView ();
				headerView.Frame = new CGRect (XMargin, 0, AppDelegate.ScreenWidth - XMargin * 2, headerHeight);

				var boardButton = new UIButton ();
				boardButton.Frame = new CGRect (0, 0, headerView.Frame.Width / 2, headerHeight);

				var logoView = new UIImageView ();
				logoView.Frame = new CGRect (0, 0, headerHeight, headerHeight);
				logoView.ContentMode = UIViewContentMode.ScaleAspectFit;
				logoView.SetImage (new NSUrl(board.LogoUrl));

				var nameView = new UILabel ();
				nameView.Frame = new CGRect (logoView.Frame.Right + 10, 3, boardButton.Frame.Width, 20);
				nameView.Text = board.Name;
				nameView.Font = AppDelegate.Narwhal14;
				nameView.TextColor = UIColor.Black;
				nameView.AdjustsFontSizeToFitWidth = true;

				var distanceView = new UILabel ();
				distanceView.Frame = new CGRect (nameView.Frame.X, nameView.Frame.Bottom, boardButton.Frame.Width, 20);
				var distance = CommonUtils.GetDistanceFromUserToBoard (board);
				var formattedDistance = CommonUtils.GetFormattedDistance (distance);
				distanceView.Text = formattedDistance;
				distanceView.Font = AppDelegate.Narwhal12;
				distanceView.TextColor = UIColor.Black;
				distanceView.AdjustsFontSizeToFitWidth = true;

				boardButton.AddSubviews (logoView, nameView, distanceView);

				var timeView = new UILabel();
				string timeAgo = "5m";
				timeView.Font = UIFont.SystemFontOfSize (14);
				timeView.TextColor = UIColor.FromRGBA(0,0,0,220);
				timeView.Text = timeAgo;
				var timeViewSize = timeView.Text.StringSize (timeView.Font);
				timeView.Frame = new CGRect(0, 0, timeViewSize.Width, timeViewSize.Height);
				timeView.Center = new CGPoint (headerView.Frame.Width - timeViewSize.Width / 2 - XMargin, headerView.Frame.Height / 2);

				headerView.AddSubviews (boardButton, timeView);
			}

			// contentId to fetch likes
			private void GenerateLikeView(string contentId){
				likeButton = new UIButton ();
				likeButton.Frame = new CGRect (XMargin, 0, AppDelegate.ScreenWidth / 4, 30);

				var heartView = new UIImageView ();
				var likeLabel = new UILabel ();

				likeButton.AddSubviews (heartView, likeLabel);
			}

			private void GenerateDescriptionView(string description){
				descriptionView = new UITextView ();
				descriptionView.Frame = new CGRect (XMargin, likeButton.Frame.Bottom, AppDelegate.ScreenWidth - XMargin * 2, 50);

			}

			sealed class UITimelinePicture : UIImageView{
				// has picture

				public UITimelinePicture(Picture picture) {
					this.Frame = new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenWidth);
					this.ContentMode = UIViewContentMode.ScaleAspectFit;
					this.SetImage(new NSUrl(picture.ImageUrl));

					// doubletap to like
				}
			}

		}
	}
}

