using Board.Schema;
using CoreGraphics;
using UIKit;
using AssetsLibrary;
using AVFoundation;
using AVKit;

namespace Board.Interface.LookUp
{
	public class VideoLookUp : UILookUp
	{
		AVPlayerViewController playerView;
		UILongPressGestureRecognizer longpress;

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear (animated);

			playerView.Player.Pause ();
			playerView.Player.Dispose ();
			playerView.Dispose ();
			ScrollView.RemoveGestureRecognizer (longpress);

			ScrollView = null;
		}

		public VideoLookUp(Video video)
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

			longpress = new UILongPressGestureRecognizer (tg => {
				UIAlertController alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);

				alert.AddAction (UIAlertAction.Create ("Save Video", UIAlertActionStyle.Default, SaveVideo));
				alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));	

				AppDelegate.NavigationController.PresentViewController(alert, true, null);	
			});

			var descriptionBox = CreateDescriptionBox (video.Description);
			descriptionBox.Center = new CGPoint (AppDelegate.ScreenWidth / 2, LikeButton.Frame.Top - descriptionBox.Frame.Height / 2 - 5);

			View.AddSubviews (ScrollView, descriptionBox, BackButton, LikeButton, FacebookButton, TrashButton);
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear (animated);
			ScrollView.AddGestureRecognizer (longpress);
		}

		private async void SaveVideo(UIAlertAction action)
		{
			ALAssetsLibrary lib = new ALAssetsLibrary ();
			await lib.WriteVideoToSavedPhotosAlbumAsync(((Video)content).LocalNSUrl);
			lib.Dispose();
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

	}
}

