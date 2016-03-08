using Board.Interface.Buttons;

using Board.Schema;

using CoreGraphics;
using Facebook.CoreKit;
using UIKit;
using AVFoundation;
using AVKit;

namespace Board.Interface.LookUp
{
	public class VideoLookUp : LookUp
	{
		public VideoLookUp(Video video)
		{
			content = video;

			View.BackgroundColor = UIColor.Black;

			CreateButtons (UIColor.White);

			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			ScrollView.UserInteractionEnabled = true;

			AVPlayerViewController playerView = new AVPlayerViewController ();
			playerView.ShowsPlaybackControls = true;
			playerView.View.Frame = new CGRect (0, TrashButton.Frame.Bottom, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight - TrashButton.Frame.Bottom - LikeButton.Frame.Height);
			playerView.Player = LoadPlayer (video);

			ScrollView.AddSubview (playerView.View);

			View.AddSubviews (ScrollView, BackButton, LikeButton, FacebookButton, ShareButton, TrashButton);
		}

		private AVPlayer LoadPlayer(Video video)
		{	
			AVPlayerItem playerItem;
			AVPlayer player;

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

