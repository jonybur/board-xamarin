using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using Board.Schema;
using Board.Utilities;
using Foundation;
using Haneke;
using System.Linq;

namespace Board.Screens.Controls
{	
	public class UITimelineContentDisplay : UIContentDisplay {

		const float SeparationBetweenObjects = 30;

		public UITimelineContentDisplay(List<Board.Schema.Board> boardList, List<Content> timelineContent) {
			
			float yposition = UIMagazineBannerPage.Height + UIMenuBanner.Height + 30;

			foreach (var content in timelineContent){
				var board = boardList.FirstOrDefault (x => x.Id == content.boardId);

				if (board == null) {
					continue;
				}

				var timelineWidget = new UITimelineWidget (board, content);
				timelineWidget.Center = new CGPoint (AppDelegate.ScreenWidth / 2, yposition + timelineWidget.Frame.Height / 2);
				
				AddSubview (timelineWidget);

				yposition += (float)timelineWidget.Frame.Height + SeparationBetweenObjects;
			}

			var size = new CGSize (AppDelegate.ScreenWidth, yposition + UIActionButton.Height * 2);
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
				GenerateHeaderView(board, content);

				if (content is Picture){
					
					var pictureContent = (Picture)content;

					timelineContent = new UITimelinePicture(pictureContent);
					timelineContent.Center = new CGPoint(AppDelegate.ScreenWidth / 2,
															headerView.Frame.Bottom + timelineContent.Frame.Height / 2 + 10);
				}

				GenerateLikeView(content.Id);

				likeButton.Center = new CGPoint(likeButton.Center.X, timelineContent.Frame.Bottom + likeButton.Frame.Height / 2);

				GenerateDescriptionView(board.Name, "Launching our delicious tapas locas!");

				AddSubviews(headerView, timelineContent, likeButton, descriptionView);

				Frame = new CGRect(0, 0, AppDelegate.ScreenWidth, descriptionView.Frame.Bottom);
			}

			private void GenerateHeaderView(Board.Schema.Board board, Content content){

				int headerHeight = 40;

				headerView = new UIView ();
				headerView.Frame = new CGRect (XMargin, 0, AppDelegate.ScreenWidth - XMargin * 2, headerHeight);

				var boardButton = new UIButton ();

				boardButton.TouchUpInside += delegate {
					AppDelegate.OpenBoard (board);
				};

				boardButton.Frame = new CGRect (0, 0, headerView.Frame.Width * .7f, headerHeight);

				var logoView = new UIImageView ();
				logoView.Frame = new CGRect (0, 0, headerHeight, headerHeight);
				logoView.ContentMode = UIViewContentMode.ScaleAspectFit;
				logoView.SetImage (new NSUrl(board.LogoUrl));

				var nameView = new UILabel ();
				nameView.Frame = new CGRect (logoView.Frame.Right + 10, 3, boardButton.Frame.Width, 20);
				nameView.Text = board.Name;
				nameView.Font = AppDelegate.Narwhal16;
				nameView.TextColor = AppDelegate.BoardOrange;
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
				string timeAgo = CommonUtils.GetFormattedTimeDifference (content.CreationDate);
				timeView.Font = UIFont.SystemFontOfSize (14, UIFontWeight.Light);
				timeView.TextColor = UIColor.FromRGBA(0,0,0,200);
				timeView.Text = timeAgo;
				var timeViewSize = timeView.Text.StringSize (timeView.Font);
				timeView.Frame = new CGRect(0, 0, timeViewSize.Width, timeViewSize.Height);
				timeView.Center = new CGPoint (headerView.Frame.Width - timeViewSize.Width / 2 - XMargin, headerView.Frame.Height / 2);

				headerView.AddSubviews (boardButton, timeView);
			}

			const string emptyHeartImageUrl = "./boardinterface/infobox/emptylike.png";
			const string fullHeartImageUrl = "./boardinterface/infobox/fulllike.png";

			// contentId to fetch likes
			private void GenerateLikeView(string contentId){
				int heartSize = 20;

				likeButton = new UIButton ();
				likeButton.Frame = new CGRect (XMargin * 2, 0, AppDelegate.ScreenWidth / 4, 50);

				var heartView = new UIImageView ();
				heartView.Frame = new CGRect (0, 0, heartSize, heartSize);
				heartView.SetImage (emptyHeartImageUrl);
				heartView.Center = new CGPoint (heartView.Frame.Width / 2, likeButton.Frame.Height / 2 - 5);

				var likeLabel = new UILabel ();
				likeLabel = new UILabel();
				likeLabel.Font = UIFont.SystemFontOfSize(18, UIFontWeight.Light);
				likeLabel.Text = "0";
				var sizeLikeLabel = likeLabel.Text.StringSize (likeLabel.Font);
				likeLabel.Frame = new CGRect(0, 0, sizeLikeLabel.Width, sizeLikeLabel.Height);
				likeLabel.Center = new CGPoint (heartView.Frame.Right + likeLabel.Frame.Width / 2 + 10, heartView.Center.Y);

				likeButton.AddSubviews (heartView, likeLabel);

				bool isLiked = false;

				likeButton.TouchUpInside += (sender, e) => {
					if (!isLiked){
						heartView.SetImage(fullHeartImageUrl);
					} else {
						heartView.SetImage(emptyHeartImageUrl);
					}
					isLiked = !isLiked;
				};
			}

			private void GenerateDescriptionView(string boardName, string description){
				var lineView = new UIImageView ();
				lineView.Frame = new CGRect (0, 0, AppDelegate.ScreenWidth - XMargin * 4, 1);
				lineView.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 40);

				descriptionView = new UITextView ();
				descriptionView.Frame = new CGRect (XMargin * 2, likeButton.Frame.Bottom - 10, AppDelegate.ScreenWidth - XMargin * 4, 14);
				descriptionView.ScrollEnabled = false;
				descriptionView.Editable = false;

				boardName = boardName.ToUpper ();

				var boardNameAttributes = new UIStringAttributes {
					Font = UIFont.SystemFontOfSize(14, UIFontWeight.Bold)
				};

				var descriptionNameAttributes = new UIStringAttributes {
					Font = UIFont.SystemFontOfSize (14, UIFontWeight.Regular)
				};

				var attributedString = new NSMutableAttributedString(boardName + " " + description);
				attributedString.SetAttributes(boardNameAttributes, new NSRange(0, boardName.Length));
				attributedString.SetAttributes(descriptionNameAttributes, new NSRange(boardName.Length, description.Length));

				descriptionView.AttributedText = attributedString;

				var size = descriptionView.SizeThatFits (descriptionView.Frame.Size);
				descriptionView.Frame = new CGRect (descriptionView.Frame.X, descriptionView.Frame.Y, descriptionView.Frame.Width, size.Height);

				descriptionView.AddSubview (lineView);
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

