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

			var size = new CGSize (300, 100);//GetFrame (vid);

			// mounting

			CreateMounting (size);
			View = new UIView(MountingView.Frame);
			View.AddSubview (MountingView);

			// picture
			CGRect pictureFrame = new CGRect (MountingView.Frame.X + SideMargin, TopMargin, size.Width, size.Height);
			AVPlayerLayer videoLayer = LoadVideoThumbnail (pictureFrame);
			View.Layer.AddSublayer (videoLayer);
			videoLayer.AllowsEdgeAntialiasing = true;
			videoLayer.ModelLayer.AllowsEdgeAntialiasing = true;
			View.Layer.AllowsEdgeAntialiasing = true;

			/*
			UIImageView uiv = new UIImageView (pictureFrame);
			uiv.Image = vid.Thumbnail;
			View.AddSubview (uiv);

			// play button
			//UIImageView playButton = CreatePlayButton (pictureFrame);
			//View.AddSubview (playButton);
			*/

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

		private CGSize GetFrame(Video vid)
		{
			float autosize = Widget.Autosize;

			using (MPMoviePlayerViewController moviePlayer = new MPMoviePlayerViewController (vid.Url)) {
				moviePlayer.MoviePlayer.Play ();

				vid.Thumbnail = moviePlayer.MoviePlayer.ThumbnailImageAt (0, MPMovieTimeOption.Exact);

				moviePlayer.MoviePlayer.Pause ();
				moviePlayer.MoviePlayer.Dispose ();	
			}

			vid.Thumbnail = vid.Thumbnail.ImageScaledToFitSize(new CGSize (autosize, autosize));

			var size = new CGSize (vid.Thumbnail.Size.Width, vid.Thumbnail.Size.Height);

			return size;
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
							// TODO: wait until loopermethod is dead before disposing videowidget
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

