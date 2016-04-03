using System;
using AVFoundation;
using UIKit;
using CoreMedia;
using System.Threading;
using CoreGraphics;
using Foundation;

namespace Board.Interface
{
	public class VideoPreview : UIView
	{
		AVPlayer _player;
		Thread looper;
		bool loop;
		int videoDuration;
		int time;

		public VideoPreview()
		{
			Frame = new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);
		}

		private void LooperMethod()
		{
			while (loop) {

				time = 0;

				while (time < videoDuration) {
					Thread.Sleep (1000);
					time++;
				}

				if (_player != null) {
					try{
						InvokeOnMainThread (() => _player.Seek (new CMTime (0, 1000000000)));
					} catch (Exception ex) {
						Console.WriteLine (ex.Message);
					}
				}

			}
		}

		public void LoadVideo(NSUrl videoPath)
		{
			AVPlayerItem _playerItem;
			using (AVAsset _asset = AVAsset.FromUrl (videoPath)) {
				_playerItem = new AVPlayerItem (_asset);
			}
			_player = new AVPlayer (_playerItem);
			var _playerLayer = AVPlayerLayer.FromPlayer (_player);
			_playerLayer.Frame = new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);
			_player.ActionAtItemEnd = AVPlayerActionAtItemEnd.Pause;
			_player.Play ();
			videoDuration = (int)Math.Floor (_player.CurrentItem.Asset.Duration.Seconds);

			looper = new Thread (new ThreadStart (LooperMethod));
			looper.Start ();
			loop = true;

			Alpha = 1f;

			Layer.AddSublayer (_playerLayer);
		}

		public void KillVideo()
		{
			loop = false;
			time = videoDuration;

			if (Layer.Sublayers != null) {
				foreach (var layer in Layer.Sublayers) {
					layer.RemoveFromSuperLayer ();
				}
			}

			if (_player != null) {
				_player.Pause ();
				_player.Dispose ();
			}
		}
	}
}

