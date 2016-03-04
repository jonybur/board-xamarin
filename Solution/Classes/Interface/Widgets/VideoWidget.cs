using System;
using System.Threading;

using AVFoundation;
using Board.Schema;

using Board.Utilities;
using CoreGraphics;
using MediaPlayer;
using CoreMedia;
using Board.Interface.LookUp;
using Foundation;
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
			View.Layer.AddSublayer (videoLayer);

			/*
			UIImageView uiv = new UIImageView (pictureFrame);
			uiv.Image = vid.Thumbnail;
			View.AddSubview (uiv);

			// play button
			//UIImageView playButton = CreatePlayButton (pictureFrame);
			//View.AddSubview (playButton);
			*/

			View.Frame = new CGRect (vid.Frame.X, vid.Frame.Y, MountingView.Frame.Width, MountingView.Frame.Height);
			View.Transform = CGAffineTransform.MakeRotation(vid.Rotation);

			EyeOpen = false;

			CreateGestures ();
		}

		private void CreateGestures()
		{
			UITapGestureRecognizer doubleTap = CreateDoubleTapToLikeGesture ();

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				if (Preview.View != null) {
					return;
				}

				tg.NumberOfTapsRequired = 1; 

				if (LikeComponent.Frame.Left < tg.LocationInView(this.View).X &&
					LikeComponent.Frame.Top < tg.LocationInView(this.View).Y)
				{
					Like();
				}
				else{
					VideoLookUp lookUp = new VideoLookUp(video);
					AppDelegate.NavigationController.PresentViewController(lookUp, true, null);

					/*MPMoviePlayerController moviePlayer = new MPMoviePlayerController (video.Url);
					View.Superview.Superview.AddSubview (moviePlayer.View);
					moviePlayer.SetFullscreen (true, false);
					moviePlayer.Play ();*/
				}
			});

			tap.DelaysTouchesBegan = true;
			doubleTap.DelaysTouchesBegan = true;

			tap.RequireGestureRecognizerToFail (doubleTap);

			GestureRecognizers.Add (tap);
			GestureRecognizers.Add (doubleTap);
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
			float imgw, imgh;
			float autosize = AppDelegate.Autosize;

			float scale = (float)(vid.ThumbnailView.Frame.Width/vid.ThumbnailView.Frame.Height);

			if (scale >= 1) {
				imgw = autosize * scale;
				imgh = autosize;

				if (imgw > AppDelegate.ScreenWidth) {
					scale = (float)(vid.ThumbnailView.Frame.Height/vid.ThumbnailView.Frame.Width);
					imgw = AppDelegate.ScreenWidth;
					imgh = imgw * scale;
				}
			} else {
				scale = (float)(vid.ThumbnailView.Frame.Height / vid.ThumbnailView.Frame.Width);
				imgw = autosize;
				imgh = autosize * scale;
			}

			vid.ThumbnailView = new UIImageView(CommonUtils.ResizeImage (vid.ThumbnailView.Image, new CGSize (imgw, imgh)));

			CGRect frame = new CGRect (vid.Frame.X, vid.Frame.Y, imgw, imgh);

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
						View.InvokeOnMainThread (() => _player.Seek (new CMTime (0, 1000000000)));
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
			_player = null;
			loop = false;
			time = (int)videoDuration;
		}

	}
}

