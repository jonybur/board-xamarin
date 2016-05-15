using System;
using System.Threading;
using AVFoundation;
using Board.Schema;
using MediaPlayer;
using Foundation;
using AVKit;
using MGImageUtilitiesBinding;
using Board.Infrastructure;
using CoreGraphics;
using CoreMedia;
using UIKit;

namespace Board.Interface.Widgets
{
	public class VideoWidget : Widget
	{
		public Video video
		{
			get { return (Video)content; }
		}

		public VideoWidget()
		{
			
		}

		public VideoWidget(Video vid)
		{
			content = vid;

			var size = new CGSize (200, 200);

			// mounting

			CreateMounting (size);
			View = new UIView(MountingView.Frame);
			View.AddSubview (MountingView);

			// picture

			var playerView = new AVPlayerViewController ();
			playerView.View.Frame = new CGRect (SideMargin, TopMargin, size.Width, size.Height);

			var playerAsset = AVAsset.FromUrl (vid.Url);
			var playerItem = new AVPlayerItem (playerAsset);
			var player = new AVPlayer (playerItem);
			player.ActionAtItemEnd = AVPlayerActionAtItemEnd.None;

			NSNotificationCenter.DefaultCenter.AddObserver (AVPlayerItem.DidPlayToEndTimeNotification, delegate(NSNotification obj) {
				player.Seek (new CMTime (0, 1000000000));
			});

			player.Play ();
			player.Muted = true;
			playerView.Player = player;

			View.AddSubview (playerView.View);

			View.Layer.AllowsEdgeAntialiasing = true;

			EyeOpen = false;

			CreateGestures ();
		}

		public void SetFrame(CGRect frame)
		{
			View.Frame = frame;
		}
			
	}
}

