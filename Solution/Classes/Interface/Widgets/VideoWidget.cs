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

			var size = new CGSize (200, 200);

			// mounting

			CreateMounting (size);
			View = new UIView(MountingView.Frame);
			View.AddSubview (MountingView);

			// picture
			/*
			AVPlayerLayer videoLayer = LoadVideoThumbnail (new CGRect (MountingView.Frame.X + SideMargin, TopMargin, size.Width, size.Height));

			View.Layer.AddSublayer (videoLayer);

			videoLayer.AllowsEdgeAntialiasing = true;
			videoLayer.ModelLayer.AllowsEdgeAntialiasing = true;*/

			var playerView = new AVPlayerViewController ();
			playerView.ShowsPlaybackControls = true;
			playerView.View.Frame = new CGRect (0, 0, size.Width, size.Height);
			playerView.Player = LoadPlayer (video);

			View.AddSubview (playerView.View);
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

			var avasset = GetAVAssetFromRemoteUrl (video.Url);
			_playerItem = new AVPlayerItem (avasset);
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

			//loop = true;
			//looper = new Thread (new ThreadStart (LooperMethod));
			//looper.Start ();

			return _playerLayer;
		}

		private AVPlayer LoadPlayer(Video video)
		{	
			AVPlayerItem playerItem;

			using (var _asset = AVAsset.FromUrl (video.Url)) {
				playerItem = new AVPlayerItem (_asset);
			}

			playerItem.AudioMix = new AVAudioMix ();
			var player = new AVPlayer (playerItem);
			player.Seek (new CMTime (0, 1000000000));
			player.Play ();

			return player;
		}

		private AVAsset GetAVAssetFromRemoteUrl(NSUrl url){
			NSError err;
			NSData urlData = NSData.FromUrl (NSUrl.FromString("http://www.sample-videos.com/video/mp4/480/big_buck_bunny_480p_1mb.mp4"), NSDataReadingOptions.Coordinated, out err);

			NSUrl localPath = StorageController.StoreVideoInCache (urlData, content.Id);
			AVAsset asset = AVAsset.FromUrl (localPath);
			return asset;
		}

		/*
		+ (AVAsset*)getAVAssetFromRemoteUrl:(NSURL*)url 
		{   
		    if (!NSTemporaryDirectory())
		    {
		       // no tmp dir for the app (need to create one)
		    }

		    NSURL *tmpDirURL = [NSURL fileURLWithPath:NSTemporaryDirectory() isDirectory:YES];
		    NSURL *fileURL = [[tmpDirURL URLByAppendingPathComponent:@"temp"] URLByAppendingPathExtension:@"mp4"];
		    NSLog(@"fileURL: %@", [fileURL path]);

		    NSData *urlData = [NSData dataWithContentsOfURL:url];
		    [urlData writeToURL:fileURL options:NSAtomicWrite error:nil];

		    AVAsset *asset = [AVAsset assetWithURL:fileURL];
		    return asset;
		}
		+ (AVAsset*)getAVAssetFromLocalUrl:(NSURL*)url
		{
		    AVURLAsset *asset = [AVAsset assetWithURL:url];
		    return asset;
		}
		*/

		public void PauseVideo()
		{
			if (playing) {
				playing = false;
				_player.Pause ();
			}
		}

		public void KillVideo()
		{
			/*PauseVideo ();
			loop = false;
			time = (int)videoDuration;
			while (looper.IsAlive) {
			}
			_player = null;*/
		}

	}
}

