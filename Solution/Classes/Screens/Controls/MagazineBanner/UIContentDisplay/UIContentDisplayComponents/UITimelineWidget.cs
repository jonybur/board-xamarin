using Clubby.Infrastructure;
using Clubby.Schema;
using Clubby.Utilities;
using CoreAnimation;
using CoreGraphics;
using System.Text.RegularExpressions;
using Facebook.CoreKit;
using Foundation;
using Haneke;
using UIKit;

namespace Clubby.Screens.Controls
{
	public sealed class UITimelineWidget : UIView{
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

		UIImageView heartView, logoView;

		string venueLogoUrl;
		public UILabel LikeLabel;

		bool isLiked, hasBeenActivated;
		int likes;

		const string emptyHeartImageUrl = "./boardinterface/infobox/emptylike_white.png";
		const string fullHeartImageUrl = "./boardinterface/infobox/fulllike.png";
		const int XMargin = 10;

		public void MoveDown(){
			
			Center = new CGPoint (Center.X, Center.Y + 50);
		}

		public void ActivateImage(){
			if (hasBeenActivated) {
				return;
			}

			if (timelineWidget is UITimelinePicture) {
				var timelinePicture = ((UITimelinePicture)timelineWidget);
				timelinePicture.SetPictureImage ();
			}
			logoView.SetImage (new NSUrl(venueLogoUrl));

			hasBeenActivated = true;
		}

		// if null, no video
		public UITimelineVideo timelineVideo;

		public UITimelineWidget(Venue venue, Content _content){
			// makes the variable global
			content = _content;

			GenerateHeaderView(venue);

			float lastBottom = 0, widgetBottom = 0;

			if (content is Picture) {
				
				Picture picture = (Picture)content;

				timelineWidget = new UITimelinePicture (picture.ImageUrl);

				timelineWidget.Center = new CGPoint (AppDelegate.ScreenWidth / 2,
					headerView.Frame.Bottom + timelineWidget.Frame.Height / 2 + 10);

				widgetBottom = (float)timelineWidget.Frame.Bottom;

			} else if (content is Video) {

				Video video = (Video)content;

				timelineVideo = new UITimelineVideo (video.VideoUrl, video.ImageUrl, content.Id);

				timelineWidget = timelineVideo.View;

				timelineWidget.Center = new CGPoint (timelineWidget.Frame.Width / 2,
					headerView.Frame.Bottom + timelineWidget.Frame.Height / 2 + 10);

				timelineWidget.BackgroundColor = UIColor.Clear;

				widgetBottom = (float)timelineWidget.Frame.Y + AppDelegate.ScreenWidth;
			}

			GenerateLikeView ();
			likeButton.Center = new CGPoint (likeButton.Center.X, widgetBottom + likeButton.Frame.Height / 2);

			if (!string.IsNullOrEmpty (_content.Description)) {
				GenerateDescriptionView (venue.Name, _content.Description);
				AddSubview (descriptionView);
				lastBottom = (float)descriptionView.Frame.Bottom;
			} else {
				lastBottom = (float)likeButton.Frame.Bottom;
			}

			var doubleTapToLike = SetNewDoubleTapGestureRecognizer();
			timelineWidget.AddGestureRecognizer(doubleTapToLike);
			timelineWidget.UserInteractionEnabled = true;

			AddSubviews(headerView, timelineWidget, likeButton);

			Frame = new CGRect(0, 0, AppDelegate.ScreenWidth, lastBottom);
		}

		private void GenerateHeaderView(Venue venue){
			venueLogoUrl = venue.LogoUrl;
			int headerHeight = 40;

			headerView = new UIView ();
			headerView.Frame = new CGRect (XMargin, 0, AppDelegate.ScreenWidth - XMargin * 2, headerHeight);

			var boardButton = new UIButton ();

			boardButton.TouchUpInside += delegate {
			
				CATransaction.Begin ();

				boardButton.Alpha = 0.75f;

				CATransaction.Commit();

				CATransaction.CompletionBlock = delegate {
					AppDelegate.OpenBoard(venue);
					boardButton.Alpha = 1f;

				};
			};

			boardButton.Frame = new CGRect (0, 0, headerView.Frame.Width * .7f, headerHeight);

			logoView = new UIImageView ();
			logoView.Frame = new CGRect (0, 0, headerHeight, headerHeight);
			logoView.ContentMode = UIViewContentMode.ScaleAspectFit;
			logoView.Layer.CornerRadius = logoView.Frame.Width / 2;
			logoView.ClipsToBounds = true;

			var nameView = new UILabel ();
			nameView.Frame = new CGRect (logoView.Frame.Right + 10, 3, boardButton.Frame.Width, 20);
			nameView.Text =  CommonUtils.FirstLetterOfEveryWordToUpper (venue.Name);
			nameView.Font = UIFont.SystemFontOfSize (16, UIFontWeight.Medium);
			nameView.TextColor = UIColor.White;
			nameView.AdjustsFontSizeToFitWidth = true;

			var distanceView = new UILabel ();
			distanceView.Frame = new CGRect (nameView.Frame.X, nameView.Frame.Bottom, boardButton.Frame.Width, 20);
			var distance = CommonUtils.GetDistanceFromUserToBoard (venue);
			var formattedDistance = CommonUtils.GetFormattedDistance (distance);
			distanceView.Text = formattedDistance;
			distanceView.Font = UIFont.SystemFontOfSize (12, UIFontWeight.Light);
			distanceView.TextColor = UIColor.White;
			distanceView.AdjustsFontSizeToFitWidth = true;

			boardButton.AddSubviews (logoView, nameView, distanceView);

			var timeView = new UILabel();
			string timeAgo = CommonUtils.GetFormattedTimeDifference (content.CreationDate);
			timeView.Font = UIFont.SystemFontOfSize (14, UIFontWeight.Light);
			timeView.TextColor = UIColor.FromRGBA(255,255,255,200);
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

			isLiked = StorageController.GetLike (content.Id);
			var firstImage = isLiked ? fullHeartImageUrl : emptyHeartImageUrl;
			heartView.SetImage (firstImage);

			heartView.Center = new CGPoint (heartView.Frame.Width / 2, likeButton.Frame.Height / 2 - 5);

			LikeLabel = new UILabel();
			LikeLabel.Font = UIFont.SystemFontOfSize(18, UIFontWeight.Light);

			likes = content.Likes;
			LikeLabel.Text = likes.ToString();
			LikeLabel.TextColor = UIColor.White;

			var sizeLikeLabel = LikeLabel.Text.StringSize (LikeLabel.Font);
			LikeLabel.Frame = new CGRect(0, 0, sizeLikeLabel.Width * 2, sizeLikeLabel.Height);
			LikeLabel.Center = new CGPoint (heartView.Frame.Right + LikeLabel.Frame.Width / 2 + 10, heartView.Center.Y);
			likeButton.AddSubviews (heartView, LikeLabel);

			likeButton.TouchUpInside += (sender, e) => Like ();
		}

		public void AddLikeCount(int newLikes){
			likes += newLikes;
			LikeLabel.Text = likes.ToString ();
			var sizeLikeLabel = LikeLabel.Text.StringSize (LikeLabel.Font);
			LikeLabel.Frame = new CGRect(LikeLabel.Frame.X, LikeLabel.Frame.Y, sizeLikeLabel.Width * 2, sizeLikeLabel.Height);

		}

		private void Like(){
			if (!isLiked){
				
				likes++;
				heartView.SetImage(fullHeartImageUrl);

			} else {
				
				likes--;
				heartView.SetImage(emptyHeartImageUrl);

			}

			StorageController.ActionLike (content.Id);
			AppEvents.LogEvent ("likesTimelineWidget");

			LikeLabel.Text = likes.ToString();
			isLiked = !isLiked;
		}

		private void GenerateDescriptionView(string boardName, string description){
			var lineView = new UIImageView ();
			lineView.Frame = new CGRect (0, 0, AppDelegate.ScreenWidth - XMargin * 4, 1);
			lineView.BackgroundColor = UIColor.FromRGBA (255, 255, 255, 200);

			descriptionView = new UITextView ();
			descriptionView.Frame = new CGRect (XMargin * 2, likeButton.Frame.Bottom - 10, AppDelegate.ScreenWidth - XMargin * 4, 14);
			descriptionView.ScrollEnabled = false;
			descriptionView.Editable = false;
			//descriptionView.Selectable = false;
			descriptionView.DataDetectorTypes = UIDataDetectorType.Link;
			descriptionView.TintColor = AppDelegate.ClubbyBlue;

			boardName = boardName.ToUpper ();

			var descriptionNameAttributes = new UIStringAttributes {
				Font = UIFont.SystemFontOfSize (14, UIFontWeight.Regular)
			};

			var attributedString = new NSMutableAttributedString(description);
			attributedString.SetAttributes(descriptionNameAttributes, new NSRange(0, description.Length));
			
			SetRecognizers (ref attributedString, '@', "user");
			//SetRecognizers (ref attributedString, '#', "hashtag");

			descriptionView.AttributedText = attributedString;

			var size = descriptionView.SizeThatFits (descriptionView.Frame.Size);
			descriptionView.Frame = new CGRect (descriptionView.Frame.X, descriptionView.Frame.Y, descriptionView.Frame.Width, size.Height);
			descriptionView.TextColor = UIColor.White;
			descriptionView.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);

			descriptionView.AddSubview (lineView);
		}

		private void SetRecognizers(ref NSMutableAttributedString attributedString, char characterKey, string deepLinkKey){
			var linkAttributes = new UIStringAttributes {
				Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular)
			};

			// ATS
			var regex = new Regex(@"(?<="+characterKey+@")\w+");
			var matches = regex.Matches(attributedString.Value);
			foreach(Match m in matches) {
				var match = m.ToString ();
				var index = attributedString.Value.IndexOf (match, System.StringComparison.Ordinal) - 1;
				linkAttributes.Link = NSUrl.FromString ("clubby://"+ deepLinkKey +"?id=" + match);

				if (index - 1 > 0) {
					if (attributedString.Value [index - 1] == ' ') {
						attributedString.SetAttributes (linkAttributes, new NSRange (index, match.Length + 1));
					}
				} else {
					attributedString.SetAttributes (linkAttributes, new NSRange (index, match.Length + 1));
				}
			}
		}

		private UITapGestureRecognizer SetNewDoubleTapGestureRecognizer(){
			var doubletap = new UITapGestureRecognizer (tg => Like ());

			doubletap.NumberOfTapsRequired = 2;
			doubletap.DelaysTouchesBegan = true;

			return doubletap;
		}


		public sealed class UITimelineVideo : UIRepeatVideo {

			UIImageView topButton;
			public string Id;

			public UITimelineVideo(string _url, string imageUrl, string contentId) {
				Id = contentId;
				var frame = new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenWidth);
				this.Initialize(frame, new NSUrl(_url));

				topButton = new UIImageView(frame);

				var tap = new UITapGestureRecognizer (obj => {

					this.playerLayer.Player.Muted = !this.playerLayer.Player.Muted;

					if (this.playerLayer.Player.Muted){
						UITimelineContentDisplay.VideosToMute.Remove(Id);
					}else{
						UITimelineContentDisplay.VideosToMute.Add(Id);
					}

				});

				topButton.UserInteractionEnabled = true;
				topButton.AddGestureRecognizer(tap);

				this.Add(topButton);
			}

		}

		sealed class UITimelinePicture : UIImageView {

			string Url;

			public UITimelinePicture(string _url) {
				Url = _url;
				this.Frame = new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenWidth);
				this.ContentMode = UIViewContentMode.ScaleAspectFit;
			}

			public void SetPictureImage(){
				this.SetImage(new NSUrl(Url));
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

