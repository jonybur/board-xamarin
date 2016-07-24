using AVFoundation;
using AVKit;
using Clubby.Utilities;
using CoreGraphics;
using CoreMedia;
using Foundation;

namespace Clubby.Screens.Controls
{
	public class UIRepeatVideo : AVPlayerViewController
	{
		public UIRepeatVideo () {
		}

		public UIRepeatVideo (CGRect frame, NSUrl url) {
			Initialize (frame, url);
		}

		public AVPlayerLayer playerLayer;
		public void Initialize(CGRect frame, NSUrl url){

			var playerAsset = AVAsset.FromUrl (url);
			var playerItem = new AVPlayerItem (playerAsset);

			var player = new AVPlayer (playerItem);
			player.ActionAtItemEnd = AVPlayerActionAtItemEnd.None;
			playerLayer = AVPlayerLayer.FromPlayer (player);
			playerLayer.Frame = frame;
			playerLayer.VideoGravity = AVLayerVideoGravity.ResizeAspect;
			playerLayer.Player.Play ();
			playerLayer.Player.Muted = true;

			this.View.Layer.AddSublayer (playerLayer);
			this.View.Frame = frame;

			SuscribeToObserver ();
			ShowsPlaybackControls = false;
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
			
			playerLayer.Player.Seek (new CMTime (0, 1000000000));
		}
	}
}