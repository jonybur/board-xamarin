using AVFoundation;
using AVKit;
using Board.Utilities;
using CoreGraphics;
using CoreMedia;
using Foundation;

namespace Board.Screens.Controls
{
	public class UIRepeatVideo : AVPlayerViewController
	{
		public UIRepeatVideo () {
		}

		public UIRepeatVideo (CGRect frame, NSUrl url) {
			View.Frame = frame;

			var playerAsset = AVAsset.FromUrl (url);
			var playerItem = new AVPlayerItem (playerAsset);
			Player = new AVPlayer (playerItem);
			Player.ActionAtItemEnd = AVPlayerActionAtItemEnd.None;

			SuscribeToObserver ();
			ShowsPlaybackControls = false;

			Player.Play ();
			Player.Muted = true;
		}

		public override void ViewDidUnload () {
			UnsuscribeToObserver ();
			Player.Pause ();
			Player.ReplaceCurrentItemWithPlayerItem (null);
			MemoryUtility.ReleaseUIViewWithChildren (View);
		}

		public void SuscribeToObserver(){
			NSNotificationCenter.DefaultCenter.AddObserver (AVPlayerItem.DidPlayToEndTimeNotification, SeekToBeginning);
		}

		public void UnsuscribeToObserver(){
			NSNotificationCenter.DefaultCenter.RemoveObserver (AVPlayerItem.DidPlayToEndTimeNotification);
		}

		private void SeekToBeginning(NSNotification obj){
			Player.Seek (new CMTime (0, 1000000000));
		}
	}
}