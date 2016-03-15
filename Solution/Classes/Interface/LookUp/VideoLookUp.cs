using Board.Schema;
using CoreGraphics;
using UIKit;
using AssetsLibrary;
using AVFoundation;
using AVKit;

namespace Board.Interface.LookUp
{
	public class VideoLookUp : LookUp
	{
		AVPlayerViewController playerView;
		AVPlayer player;
		UILongPressGestureRecognizer longpress;

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear (animated);

			player.Pause ();
			player.Dispose ();
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

			View.AddSubviews (ScrollView, BackButton, LikeButton, FacebookButton, TrashButton);
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear (animated);
			ScrollView.AddGestureRecognizer (longpress);
		}

		private async void SaveVideo(UIAlertAction action)
		{
			ALAssetsLibrary lib = new ALAssetsLibrary ();
			await lib.WriteVideoToSavedPhotosAlbumAsync(((Video)content).Url);
			lib.Dispose();
		}

		private AVPlayer LoadPlayer(Video video)
		{	
			AVPlayerItem playerItem;

			using (AVAsset _asset = AVAsset.FromUrl (video.Url)) {
				playerItem = new AVPlayerItem (_asset);
			}

			playerItem.AudioMix = new AVAudioMix ();
			player = new AVPlayer (playerItem);
			player.Seek (new CoreMedia.CMTime (0, 1000000000));

			player.Play ();

			return player;
		}
	}
}

