using Board.Interface.Buttons;
using Board.Utilities;
using CoreGraphics;
using Foundation;
using UIKit;
using Board.Interface.Widgets;
using Facebook.CoreKit;
using System;
using Board.Schema;
using Plugin.Share;

namespace Board.Interface.LookUp
{
	public class LookUp : UIViewController
	{
		public Content content;

		protected UIScrollView ScrollView;

		UITapGestureRecognizer backTap;
		UITapGestureRecognizer likeTap;
		UITapGestureRecognizer facebookTap;
		UITapGestureRecognizer wazeTap;
		UITapGestureRecognizer uberTap;
		UITapGestureRecognizer shareTap;
		UITapGestureRecognizer trashTap;

		protected UIImageView BackButton;
		protected UIImageView LikeButton;
		protected UIImageView FacebookButton;
		protected UIImageView UberButton;
		protected UIImageView WazeButton;
		protected UIImageView ShareButton;
		protected UIImageView TrashButton;

		UIWindow window;

		public LookUp()
		{
			AutomaticallyAdjustsScrollViewInsets = false;
			View = new UIView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
		}

		public void CreateButtons(UIColor buttonColor)
		{
			CreateBackButton (buttonColor);
			CreateLikeButton (buttonColor);
			CreateFacebookButton (buttonColor);
			CreateUberButton (buttonColor);
			CreateShareButton (buttonColor);
			CreateTrashButton (buttonColor);
		
			if (string.IsNullOrEmpty(content.FacebookId)) {
				FacebookButton.Alpha = 0f;
			}

			if (Profile.CurrentProfile.UserID != BoardInterface.board.CreatorId) {
				TrashButton.Alpha = 0f;
			}
		}

		public override void ViewDidAppear(bool animated)
		{
			BackButton.AddGestureRecognizer (backTap);
			LikeButton.AddGestureRecognizer (likeTap);
			FacebookButton.AddGestureRecognizer (facebookTap);
			ShareButton.AddGestureRecognizer (shareTap);
			UberButton.AddGestureRecognizer (uberTap);
			TrashButton.AddGestureRecognizer (trashTap);
		}

		public override void ViewDidDisappear(bool animated)
		{
			BackButton.RemoveGestureRecognizer (backTap);
			LikeButton.RemoveGestureRecognizer (likeTap);
			LikeButton.RemoveGestureRecognizer (facebookTap);
			ShareButton.RemoveGestureRecognizer (shareTap);
			UberButton.RemoveGestureRecognizer (uberTap);
			TrashButton.RemoveGestureRecognizer (trashTap);
		}

		protected void CreateBackButton(UIColor buttonColor)
		{
			using (UIImage img = UIImage.FromFile ("./boardinterface/lookup/cancel.png")) {
				BackButton = new UIImageView(new CGRect(0,0,img.Size.Width * 2,img.Size.Height * 2));

				UIImageView subView = new UIImageView (new CGRect (0, 0, img.Size.Width / 2, img.Size.Height / 2));
				subView.Image = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				subView.Center = new CGPoint (BackButton.Frame.Width / 2, BackButton.Frame.Height / 2);
				subView.TintColor = buttonColor;

				BackButton.AddSubview (subView);
				BackButton.Center = new CGPoint (img.Size.Width / 2 + 10, 35);
			}
			 
			BackButton.UserInteractionEnabled = true;

			backTap = new UITapGestureRecognizer (tg => {
				// user tapped on "Done" button
				AppDelegate.PopViewLikeDismissView();
				ButtonInterface.SwitchButtonLayout((int)ButtonInterface.ButtonLayout.NavigationBar);
				MemoryUtility.ReleaseUIViewWithChildren (View);
			});
		}

		protected void CreateTrashButton(UIColor buttonColor)
		{
			using (UIImage img = UIImage.FromFile ("./boardinterface/lookup/trash.png")) {
				TrashButton = new UIImageView(new CGRect(0,0,img.Size.Width * 2,img.Size.Height * 2));

				UIImageView subView = new UIImageView (new CGRect (0, 0, img.Size.Width / 2, img.Size.Height / 2));
				subView.Image = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				subView.Center = new CGPoint (TrashButton.Frame.Width / 2, TrashButton.Frame.Height / 2);
				subView.TintColor = buttonColor;

				TrashButton.AddSubview (subView);
				TrashButton.Center = new CGPoint (AppDelegate.ScreenWidth - img.Size.Width / 2 - 10, 35);
			}

			TrashButton.UserInteractionEnabled = true;

			trashTap = new UITapGestureRecognizer (tg => {

				UIAlertController alert = UIAlertController.Create("Remove this widget from the Board?", "You can't undo this action", UIAlertControllerStyle.ActionSheet);

				alert.AddAction (UIAlertAction.Create ("Accept", UIAlertActionStyle.Default, RemoveWidget));
				alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, HideWindow));

				window = new UIWindow(new CGRect(0,0,AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
				window.RootViewController = new UIViewController();
				window.WindowLevel = UIWindowLevel.Alert + 1;

				window.MakeKeyAndVisible();
				window.RootViewController.PresentViewController(alert, true, null);
			});
		}

		private void RemoveWidget(UIAlertAction action)
		{
			BoardInterface.DictionaryContent.Remove (content.Id);

			Widget widget;

			BoardInterface.DictionaryWidgets.TryGetValue (content.Id, out widget);

			if (widget != null) {
				widget.UnsuscribeToEvents ();
				widget.View.RemoveFromSuperview ();
				BoardInterface.DictionaryWidgets.Remove (content.Id);
			}
				
			AppDelegate.PopViewLikeDismissView ();
			window.Hidden = true;
		}

		private void HideWindow(UIAlertAction action)
		{
			window.Hidden = true;
		}

		protected void CreateLikeButton(UIColor buttonColor)
		{
			UIImageView subView;
			UILabel lblLikes;
			int randomLike;

			using (UIImage img = UIImage.FromFile ("./boardinterface/lookup/like.png")) {
				Random rand = new Random ();
				randomLike = rand.Next (16, 98);
				string text = randomLike.ToString();
				UIFont font = UIFont.SystemFontOfSize (14);

				LikeButton = new UIImageView(new CGRect(0, 0, img.Size.Width + text.StringSize(font).Width + 5, img.Size.Height * 2));

				subView = new UIImageView (new CGRect (0, 0, img.Size.Width / 2, img.Size.Height / 2));
				subView.Image = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				subView.TintColor = buttonColor;
				subView.Center = new CGPoint (img.Size.Width / 2, BackButton.Frame.Height / 2);

				lblLikes = new UILabel (new CGRect (0, 0, text.StringSize(font).Width + 10, 14));
				lblLikes.Font = font;
				lblLikes.Text = text;
				lblLikes.TextColor = buttonColor;
				lblLikes.Center = new CGPoint (subView.Center.X + lblLikes.Frame.Width / 2 + 15, subView.Center.Y);

				LikeButton.AddSubviews(subView, lblLikes);
				LikeButton.Center = new CGPoint (LikeButton.Frame.Width / 2 + 10, AppDelegate.ScreenHeight - 25);
			}

			LikeButton.UserInteractionEnabled = true;

			bool liked = false;

			likeTap = new UITapGestureRecognizer (tg => {
				if (!liked)
				{
					randomLike ++;
					lblLikes.Text = randomLike.ToString();
					subView.TintColor = AppDelegate.BoardOrange;
					lblLikes.TextColor = AppDelegate.BoardOrange;
					liked = true;
				}
				else {
					randomLike --;
					lblLikes.Text = randomLike.ToString();
					subView.TintColor = buttonColor;
					lblLikes.TextColor = buttonColor;
					liked = false;
				}
			});
		}


		protected void CreateUberButton(UIColor buttonColor)
		{
			using (UIImage img = UIImage.FromFile ("./boardinterface/lookup/uber.png")) {
				const string text = "Open in Uber";
				UIFont font = UIFont.SystemFontOfSize (14);

				UberButton = new UIImageView(new CGRect(0, 0, img.Size.Width + text.StringSize(font).Width + 5, img.Size.Height * 2));

				UIImageView subView = new UIImageView (new CGRect (0, 0, img.Size.Width / 2, img.Size.Height / 2));
				subView.Image = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				subView.TintColor = buttonColor;
				subView.Center = new CGPoint (img.Size.Width / 2, BackButton.Frame.Height / 2);

				UILabel lblLikes = new UILabel (new CGRect (0, 0, text.StringSize(font).Width, 14));
				lblLikes.Font = font;
				lblLikes.Text = text;
				lblLikes.TextColor = buttonColor;
				lblLikes.Center = new CGPoint (subView.Center.X + lblLikes.Frame.Width / 2 + 15, subView.Center.Y);

				UberButton.AddSubviews(subView, lblLikes);
				UberButton.Center = new CGPoint (AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight - 25);
			}

			UberButton.UserInteractionEnabled = true;

			uberTap = new UITapGestureRecognizer (tg => {
				NSUrl url = new NSUrl("uber://");

				if (UIApplication.SharedApplication.CanOpenUrl(url)) {
					double latitude = 25.792826, longitude = -80.129943;

					// TODO: fix this
					NSUrl uberRequest = new NSUrl("uber://?action=setPickup&pickup=my_location&dropoff[latitude]="+ latitude +"&dropoff[longitude]="+longitude+ "&product_id=7-UVBjdHfUrKKeZU9nDlP_HktFs3iWVT");//+"&dropoff[nickname]=" + BoardInterface.board.Name);

					UIApplication.SharedApplication.OpenUrl(uberRequest);
				}
				else {
					// No Uber app! Open mobile website.
				}
			});
		}


		protected void CreateFacebookButton(UIColor buttonColor)
		{
			using (UIImage img = UIImage.FromFile ("./boardinterface/lookup/facebook.png")) {
				const string text = "Open in Facebook";
				UIFont font = UIFont.SystemFontOfSize (14);

				FacebookButton = new UIImageView(new CGRect(0, 0, img.Size.Width + text.StringSize(font).Width + 5, img.Size.Height * 2));

				UIImageView subView = new UIImageView (new CGRect (0, 0, img.Size.Width / 2, img.Size.Height / 2));
				subView.Image = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				subView.TintColor = buttonColor;
				subView.Center = new CGPoint (img.Size.Width / 2, BackButton.Frame.Height / 2);

				UILabel lblLikes = new UILabel (new CGRect (0, 0, text.StringSize(font).Width, 14));
				lblLikes.Font = font;
				lblLikes.Text = text;
				lblLikes.TextColor = buttonColor;
				lblLikes.Center = new CGPoint (subView.Center.X + lblLikes.Frame.Width / 2 + 15, subView.Center.Y);

				FacebookButton.AddSubviews(subView, lblLikes);
				FacebookButton.Center = new CGPoint (AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight - 25);
			}

			FacebookButton.UserInteractionEnabled = true;

			facebookTap = new UITapGestureRecognizer (tg => {
				NSUrl url;

				if(content is BoardEvent)
				{
					// opens in app
					url = new NSUrl("https://www.facebook.com/" + content.FacebookId);
				}else {

					// opens in safari
					url = new NSUrl("https://facebook.com/" + content.FacebookId);
				}

				UIApplication.SharedApplication.OpenUrl(url);


			});
		}

		protected void CreateShareButton(UIColor buttonColor)
		{
			using (UIImage img = UIImage.FromFile ("./boardinterface/lookup/share.png")) {
				const string text = "Share";
				UIFont font = UIFont.SystemFontOfSize (14);

				ShareButton = new UIImageView(new CGRect(0, 0, img.Size.Width + text.StringSize(font).Width + 5, img.Size.Height * 2));

				UIImageView subView = new UIImageView (new CGRect (0, 0, img.Size.Width / 2, img.Size.Height / 2));
				subView.Image = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				subView.TintColor = buttonColor;
				subView.Center = new CGPoint (img.Size.Width / 2, BackButton.Frame.Height / 2);

				UILabel lblLikes = new UILabel (new CGRect (0, 0, text.StringSize(font).Width, 14));
				lblLikes.Font = font;
				lblLikes.Text = text;
				lblLikes.TextColor = buttonColor;
				lblLikes.Center = new CGPoint (subView.Center.X + lblLikes.Frame.Width / 2 + 15, subView.Center.Y);

				ShareButton.AddSubviews(subView, lblLikes);
				ShareButton.Center = new CGPoint (AppDelegate.ScreenWidth - ShareButton.Frame.Width / 2 - 10, AppDelegate.ScreenHeight - 25);
			}

			ShareButton.UserInteractionEnabled = true;

			shareTap = new UITapGestureRecognizer (async tg => {
				if (tg == null)
					throw new ArgumentNullException ("tg");

				await ShareImplementation.Init ();
				ShareImplementation a = new ShareImplementation ();
		
				string metadata = "["+BoardInterface.board.Name + " via Board, " + content.CreationDate.ToString("M/d h:m tt") + "]: \n";

				if (content is Announcement)
				{
					await a.Share (metadata + ((Announcement)content).Text.Value, null);
				}

				else if (content is BoardEvent)
				{
					await a.Share (metadata + ((BoardEvent)content).Name + " \nStarts: " + ((BoardEvent)content).StartDate.ToString("g")
						+ " \nEnds: " + ((BoardEvent)content).EndDate.ToString("g"), null);
				}
			});
		}
	}
}

