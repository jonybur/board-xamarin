using AVFoundation;
using CoreGraphics;
using Foundation;
using Board.Screens.Controls;

namespace Board.Interface
{
	public class VideoPreview : UIRepeatVideo
	{
		public VideoPreview(CGRect frame) {
			View.Frame = frame;
		}

		public void LoadVideo(NSUrl url) {

			var playerAsset = AVAsset.FromUrl (url);
			var playerItem = new AVPlayerItem (playerAsset);

			if (Player == null) {
				Player = new AVPlayer (playerItem);
				Player.ActionAtItemEnd = AVPlayerActionAtItemEnd.None;

				SuscribeToObserver ();
				ShowsPlaybackControls = false;

				Player.Play ();
				Player.Muted = true;
			} else {
				Player.ReplaceCurrentItemWithPlayerItem (playerItem);
			}
		}
	}
}