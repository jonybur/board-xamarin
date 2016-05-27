using AVFoundation;
using AVKit;
using Board.Interface.CreateScreens;
using Board.Interface.LookUp;
using Board.Schema;
using Board.Utilities;
using CoreGraphics;
using UIKit;

namespace Board.Interface.FacebookImport
{
	public class VideoImportLookUp : UILookUp
	{
		AVPlayerViewController playerView;
		UIImageView NextButton;
		UITapGestureRecognizer NextTap;

		public VideoImportLookUp(Video video)
		{
			content = video;

			View.BackgroundColor = UIColor.Black;

			CreateButtons (UIColor.White);

			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			ScrollView.UserInteractionEnabled = true;

			playerView = new AVPlayerViewController ();
			playerView.ShowsPlaybackControls = true;
			playerView.View.Frame = new CGRect (0, TrashButton.Frame.Bottom, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight - TrashButton.Frame.Bottom - LikeButton.Frame.Height);
			playerView.Player = LoadPlayer (video);

			ScrollView.AddSubview (playerView.View);

			CreateNextButton (UIColor.White);

			var descriptionBox = CreateDescriptionBox (video.Description);
			descriptionBox.Center = new CGPoint (AppDelegate.ScreenWidth / 2, LikeButton.Frame.Top - descriptionBox.Frame.Height / 2 - 5);

			View.AddSubviews (ScrollView, descriptionBox, BackButton, NextButton);
		}

		private UITextView CreateDescriptionBox(string description){
			var textview = new UITextView ();

			textview.Editable = false;
			textview.Selectable = false;
			textview.ScrollEnabled = true;
			textview.DataDetectorTypes = UIDataDetectorType.Link;
			textview.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
			textview.Text = description;
			textview.Font = UIFont.SystemFontOfSize (14);
			textview.TextColor = UIColor.White;
			var size = textview.SizeThatFits (new CGSize (AppDelegate.ScreenWidth - 10, 60));
			textview.Frame = new CGRect (5, 0, size.Width, size.Height);

			textview.ContentOffset = new CGPoint (0, 0);

			return textview;
		}

		private void CreateNextButton(UIColor buttonColor)
		{
			using (UIImage img = UIImage.FromFile ("./camera/nextbutton.png")) {
				NextButton = new UIImageView(new CGRect(0,0,img.Size.Width * 2,img.Size.Height * 2));

				UIImageView subView = new UIImageView (new CGRect (0, 0, img.Size.Width / 2, img.Size.Height / 2));
				subView.Image = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				subView.Center = new CGPoint (NextButton.Frame.Width / 2, NextButton.Frame.Height / 2);
				subView.TintColor = buttonColor;

				NextButton.AddSubview (subView);
				NextButton.Center = new CGPoint (AppDelegate.ScreenWidth - img.Size.Width / 2 - 5, AppDelegate.ScreenHeight - img.Size.Height / 2 - 5);
			}

			NextButton.UserInteractionEnabled = true;

			NextTap = new UITapGestureRecognizer (tg => {
				var video = content as Video;
				var createScreen = new CreateMediaScreen(video);
				AppDelegate.NavigationController.PushViewController(createScreen, true);
			});
		}

		private AVPlayer LoadPlayer(Video video)
		{	
			AVPlayerItem playerItem;

			using (AVAsset _asset = AVAsset.FromUrl (video.AmazonNSUrl)) {
				playerItem = new AVPlayerItem (_asset);
			}

			playerItem.AudioMix = new AVAudioMix ();
			var player = new AVPlayer (playerItem);
			player.Seek (new CoreMedia.CMTime (0, 1000000000));
			player.Play ();

			return player;
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear (animated);
			NextButton.AddGestureRecognizer (NextTap);
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear (animated);
			NextButton.RemoveGestureRecognizer (NextTap);

			playerView.Player.Pause ();
			playerView.Player.ReplaceCurrentItemWithPlayerItem (null);

			ScrollView = null;

			MemoryUtility.ReleaseUIViewWithChildren (View);
		}
	}
}