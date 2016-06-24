using UIKit;
using CoreGraphics;
using Board.Schema;
using Board.Utilities;
using Board.Infrastructure;
using CoreAnimation;
using Foundation;
using Haneke;

namespace Board.Screens.Controls
{
	class UITimelineWidget : UIView{
		// includes logo, name, distance and time
		UIView headerView;

		// includes heart, number
		UIButton likeButton;

		// includes line, name of board & description
		UITextView descriptionView;

		// can be a picture, a video, an announcement, etc.
		UIView timelineWidget;

		// content being displayed in widget
		Content content;

		bool isLiked;
		int likes;
		UIImageView heartView;
		UILabel likeLabel;

		const string emptyHeartImageUrl = "./boardinterface/infobox/emptylike.png";
		const string fullHeartImageUrl = "./boardinterface/infobox/fulllike.png";

		const int XMargin = 10;

		public UITimelineWidget(Board.Schema.Board board, Content _content){
			// makes the variable global
			content = _content;

			GenerateHeaderView(board);

			float lastBottom = 0;

			if (content is Picture){
				Picture picture = (Picture)content;

				timelineWidget = new UITimelinePicture(picture.ImageUrl);
				timelineWidget.Center = new CGPoint(AppDelegate.ScreenWidth / 2,
					headerView.Frame.Bottom + timelineWidget.Frame.Height / 2 + 10);

				GenerateLikeView();
				likeButton.Center = new CGPoint(likeButton.Center.X, timelineWidget.Frame.Bottom + likeButton.Frame.Height / 2);

				if (!string.IsNullOrEmpty(picture.Description)){
					GenerateDescriptionView(board.Name, picture.Description);
					AddSubview(descriptionView);
				}

				lastBottom = (float)descriptionView.Frame.Bottom;

			} else if (content is Announcement) {
				Announcement announcement = (Announcement)content;

				timelineWidget = new UITimelineAnnouncement(announcement.Text);
				timelineWidget.Center = new CGPoint(AppDelegate.ScreenWidth / 2,
					headerView.Frame.Bottom + timelineWidget.Frame.Height / 2 + 20);

				GenerateLikeView();
				likeButton.Center = new CGPoint(likeButton.Center.X, timelineWidget.Frame.Bottom + 10 + likeButton.Frame.Height / 2);

				lastBottom = (float)likeButton.Frame.Bottom;
			}

			var doubleTapToLike = SetNewDoubleTapGestureRecognizer();
			timelineWidget.AddGestureRecognizer(doubleTapToLike);
			timelineWidget.UserInteractionEnabled = true;

			AddSubviews(headerView, timelineWidget, likeButton);

			Frame = new CGRect(0, 0, AppDelegate.ScreenWidth, lastBottom);
		}

		private void GenerateHeaderView(Board.Schema.Board board){

			int headerHeight = 40;

			headerView = new UIView ();
			headerView.Frame = new CGRect (XMargin, 0, AppDelegate.ScreenWidth - XMargin * 2, headerHeight);

			var boardButton = new UIButton ();

			boardButton.TouchUpInside += delegate {
			
				CATransaction.Begin ();

				BigTed.BTProgressHUD.Show();
				boardButton.Alpha = 0.75f;

				CATransaction.Commit();

				CATransaction.CompletionBlock = delegate {
					AppDelegate.OpenBoard(board);
				};
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

		// contentId to fetch likes
		private void GenerateLikeView(){
			int heartSize = 20;

			likeButton = new UIButton ();
			likeButton.Frame = new CGRect (XMargin * 2, 0, AppDelegate.ScreenWidth / 4, 50);

			heartView = new UIImageView ();
			heartView.Frame = new CGRect (0, 0, heartSize, heartSize);

			isLiked = UIMagazine.UserLikes[content.Id];
			var firstImage = isLiked ? fullHeartImageUrl : emptyHeartImageUrl;
			heartView.SetImage (firstImage);

			heartView.Center = new CGPoint (heartView.Frame.Width / 2, likeButton.Frame.Height / 2 - 5);

			likeLabel = new UILabel ();
			likeLabel = new UILabel();
			likeLabel.Font = UIFont.SystemFontOfSize(18, UIFontWeight.Light);

			likes = UIMagazine.ContentLikes[content.Id];
			likeLabel.Text = likes.ToString();

			var sizeLikeLabel = likeLabel.Text.StringSize (likeLabel.Font);
			likeLabel.Frame = new CGRect(0, 0, sizeLikeLabel.Width * 2, sizeLikeLabel.Height);
			likeLabel.Center = new CGPoint (heartView.Frame.Right + likeLabel.Frame.Width / 2 + 10, heartView.Center.Y);
			likeButton.AddSubviews (heartView, likeLabel);

			likeButton.TouchUpInside += (sender, e) => {
				Like();
			};
		}

		private void Like(){
			if (!isLiked){
				CloudController.SendLike(content.Id);
				likes++;

				UIMagazine.ContentLikes [content.Id]++;
				UIMagazine.UserLikes [content.Id] = true;

				heartView.SetImage(fullHeartImageUrl);
			} else {
				CloudController.SendDislike(content.Id);
				likes--;

				UIMagazine.ContentLikes [content.Id]--;
				UIMagazine.UserLikes [content.Id] = false;

				heartView.SetImage(emptyHeartImageUrl);
			}
			likeLabel.Text = likes.ToString();
			isLiked = !isLiked;
		}

		private void GenerateDescriptionView(string boardName, string description){
			var lineView = new UIImageView ();
			lineView.Frame = new CGRect (0, 0, AppDelegate.ScreenWidth - XMargin * 4, 1);
			lineView.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 40);

			descriptionView = new UITextView ();
			descriptionView.Frame = new CGRect (XMargin * 2, likeButton.Frame.Bottom - 10, AppDelegate.ScreenWidth - XMargin * 4, 14);
			descriptionView.ScrollEnabled = false;
			descriptionView.Editable = false;
			descriptionView.DataDetectorTypes = UIDataDetectorType.Link;

			boardName = boardName.ToUpper ();

			var boardNameAttributes = new UIStringAttributes {
				Font = UIFont.SystemFontOfSize(14, UIFontWeight.Bold)
			};

			var descriptionNameAttributes = new UIStringAttributes {
				Font = UIFont.SystemFontOfSize (14, UIFontWeight.Regular)
			};

			var attributedString = new NSMutableAttributedString(boardName + " " + description);
			attributedString.SetAttributes(boardNameAttributes, new NSRange(0, boardName.Length));
			attributedString.SetAttributes(descriptionNameAttributes, new NSRange(boardName.Length, description.Length+1));

			descriptionView.AttributedText = attributedString;

			var size = descriptionView.SizeThatFits (descriptionView.Frame.Size);
			descriptionView.Frame = new CGRect (descriptionView.Frame.X, descriptionView.Frame.Y, descriptionView.Frame.Width, size.Height);

			descriptionView.AddSubview (lineView);
		}

		private UITapGestureRecognizer SetNewDoubleTapGestureRecognizer(){
			var doubletap = new UITapGestureRecognizer (tg => {
				Like();
			});

			doubletap.NumberOfTapsRequired = 2;
			doubletap.DelaysTouchesBegan = true;

			return doubletap;
		}

		sealed class UITimelinePicture : UIImageView {

			public UITimelinePicture(string url) {
				this.Frame = new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenWidth);
				this.ContentMode = UIViewContentMode.ScaleAspectFit;
				this.SetImage(new NSUrl(url));
			}
		}

		sealed class UITimelineAnnouncement : UITextView {

			public UITimelineAnnouncement(string text) {
				this.Frame = new CGRect(30, 0, AppDelegate.ScreenWidth - 70, 10);
				this.ContentMode = UIViewContentMode.ScaleAspectFit;
				this.Text = text;
				this.Font = UIFont.SystemFontOfSize(16, UIFontWeight.Light);
				this.ScrollEnabled = false;
				this.Editable = false;
				this.DataDetectorTypes = UIDataDetectorType.Link;
				var size = this.SizeThatFits(this.Frame.Size);
				this.Frame = new CGRect(this.Frame.X, this.Frame.Y, this.Frame.Width, size.Height);
			}
		}

	}
}

