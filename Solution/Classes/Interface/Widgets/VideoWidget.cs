using System;
using System.Threading;
using AVFoundation;
using Board.Schema;
using MediaPlayer;
using MGImageUtilitiesBinding;
using CoreGraphics;
using CoreMedia;
using UIKit;

namespace Board.Interface.Widgets
{
	public class VideoWidget : Widget
	{
		// UIView contains ScrollView and BackButton
		// ScrollView contains LookUpImage

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

			CGRect frame = GetFrame (vid);

			// mounting

			CreateMounting (frame);
			View = new UIView(MountingView.Frame);
			View.AddSubview (MountingView);

			// picture
			CGRect pictureFrame = new CGRect (MountingView.Frame.X + 10, 10, frame.Width, frame.Height);
			AVPlayerLayer videoLayer = LoadVideoThumbnail (pictureFrame);
			videoLayer.AllowsEdgeAntialiasing = true;
			View.Layer.AddSublayer (videoLayer);

			/*
			UIImageView uiv = new UIImageView (pictureFrame);
			uiv.Image = vid.Thumbnail;
			View.AddSubview (uiv);

			// play button
			//UIImageView playButton = CreatePlayButton (pictureFrame);
			//View.AddSubview (playButton);
			*/

			SetTransforms ();

			EyeOpen = false;

			CreateGestures ();
		}

		private UIImageView CreatePlayButton(CGRect frame)
		{
			UIImageView playButton;

			using (UIImage playButtonImage = UIImage.FromFile ("./boardinterface/playbutton.png")) {
				CGSize imageSize = new CGSize (playButtonImage.Size.Width / 2, playButtonImage.Size.Height / 2);
			
				playButton = new UIImageView (new CGRect (frame.Width / 2 - imageSize.Width / 4, frame.Height / 2 - imageSize.Height / 4, imageSize.Width, imageSize.Height));

				playButton.Image = playButtonImage;
			}
			playButton.Alpha = .95f;

			return playButton;
		}

		private CGRect GetFrame(Video vid)
		{
			float autosize = Widget.Autosize;

			using (MPMoviePlayerController moviePlayer = new MPMoviePlayerController (vid.Url)) {
				vid.ThumbnailView = new UIImageView(moviePlayer.ThumbnailImageAt (0, MPMovieTimeOption.Exact));
				moviePlayer.Pause ();
				moviePlayer.Dispose ();	
			}

			vid.ThumbnailView = new UIImageView(vid.ThumbnailView.Image.ImageScaledToFitSize(new CGSize (autosize, autosize)));

			CGRect frame = new CGRect (0, 0, vid.ThumbnailView.Frame.Width, vid.ThumbnailView.Frame.Height);

			return frame;
		}

		public void SetFrame(CGRect frame)
		{
			View.Frame = frame;
		}

		private void LooperMethod()
		{
			while (loop) {

				time = 0;

				if (playing) {
					while (time < videoDuration) {
						Thread.Sleep (1000);
						time++;
					}

					if (_player != null) {
						try{
							// TODO: fix bug that makes _player null
							View.InvokeOnMainThread (() => _player.Seek (new CMTime (0, 1000000000)));
						} catch (Exception ex) {
							Console.WriteLine (ex.Message);
						}
					}
				}
			}
		}

		Thread looper;
		AVPlayer _player;

		double videoDuration;
		bool loop;
		bool playing;

		int time;

		private AVPlayerLayer LoadVideoThumbnail(CGRect frame)
		{	
			AVPlayerItem _playerItem;
			AVPlayerLayer _playerLayer;

			using (AVAsset _asset = AVAsset.FromUrl (video.Url)) {
				_playerItem = new AVPlayerItem (_asset);
			}
			_playerItem.AudioMix = new AVAudioMix ();
			_player = new AVPlayer (_playerItem);
			_playerLayer = AVPlayerLayer.FromPlayer (_player);
			_playerLayer.Frame = frame;
			_player.Seek (new CMTime (0, 1000000000));
			_player.Muted = true;
			_player.Volume = 0;
			_player.Play ();
			playing = true;

			videoDuration = Math.Floor(_player.CurrentItem.Asset.Duration.Seconds);

			/*if (videoDuration > 5) {
				videoDuration = 5;
			}*/

			loop = true;
			looper = new Thread (new ThreadStart (LooperMethod));
			looper.Start ();

			return _playerLayer;
		}

		public void PauseVideo()
		{
			if (playing) {
				playing = false;
				_player.Pause ();
			}
		}

		public void KillVideo()
		{
			PauseVideo ();
			loop = false;
			time = (int)videoDuration;
			while (looper.IsAlive) {
			}
			_player = null;
		}

	}
}

