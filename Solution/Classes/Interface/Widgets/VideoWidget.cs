using System;
using System.Threading;

using AVFoundation;
using Board.Schema;

using Board.Utilities;
using CoreGraphics;
using CoreMedia;
using Foundation;
using UIKit;

namespace Board.Interface.Widgets
{
	public class VideoWidget
	{
		// UIView contains ScrollView and BackButton
		// ScrollView contains LookUpImage
		private UIView uiView;
		public UIView View
		{
			get { return uiView; }
		}

		private Video video;

		UIImageView eye;
		UIImage closedEyeImage;
		UIImage openEyeImage;

		private bool eyeOpen;
		public bool EyeOpen{
			get { return eyeOpen; }
		}

		public Video Video
		{
			get { return video; }
		}

		public VideoWidget()
		{

		}

		public VideoWidget(Video vid)
		{
			video = vid;

			CGRect frame = GetFrame (vid);

			// mounting

			UIImageView mounting = CreateMounting (frame);
			uiView = new UIView(mounting.Frame);
			uiView.AddSubview (mounting);

			// picture
			CGRect pictureFrame = new CGRect (mounting.Frame.X + 10, 10, frame.Width, frame.Height);
			AVPlayerLayer videoLayer = LoadVideoThumbnail (pictureFrame);
			uiView.Layer.AddSublayer (videoLayer);

			/*
			UIImageView uiv = new UIImageView (pictureFrame);
			uiv.Image = vid.Thumbnail;
			uiView.AddSubview (uiv);

			// play button
			//UIImageView playButton = CreatePlayButton (pictureFrame);
			//uiView.AddSubview (playButton);
			*/

			// like

			UIImageView like = CreateLike (mounting.Frame);
			uiView.AddSubview (like);

			// like label

			UILabel likeLabel = CreateLikeLabel (like.Frame);
			uiView.AddSubview (likeLabel);

			// eye

			eye = CreateEye (mounting.Frame);
			uiView.AddSubview (eye);

			uiView.Frame = new CGRect (vid.Frame.X, vid.Frame.Y, mounting.Frame.Width, mounting.Frame.Height);
			uiView.Transform = CGAffineTransform.MakeRotation(vid.Rotation);

			eyeOpen = false;
		}

		public void OpenEye()
		{
			eye.Image = openEyeImage;
			eye.TintColor = AppDelegate.BoardOrange;
			eyeOpen = true;
		}

		private UIImageView CreateMounting(CGRect frame)
		{
			CGRect mountingFrame = new CGRect (0, 0, frame.Width + 20, frame.Height + 50);

			UIImageView mountingView = CreateColorView (mountingFrame, UIColor.FromRGB(250,250,250).CGColor);

			return mountingView;
		}

		private UIImageView CreatePlayButton(CGRect frame)
		{
			UIImage playButtonImage = UIImage.FromFile ("./boardinterface/playbutton.png");
			CGSize imageSize = new CGSize (playButtonImage.Size.Width / 2, playButtonImage.Size.Height / 2);


			UIImageView playButton = new UIImageView(new CGRect (frame.Width / 2 - imageSize.Width / 4, frame.Height / 2 - imageSize.Height / 4, imageSize.Width, imageSize.Height));

			playButton.Image = playButtonImage;
			playButton.Alpha = .95f;

			return playButton;
		}

		private UILabel CreateLikeLabel(CGRect frame)
		{
			UIFont likeFont = UIFont.SystemFontOfSize (20);
			string likeText = "0";
			CGSize likeLabelSize = likeText.StringSize (likeFont);
			UILabel likeLabel = new UILabel(new CGRect(frame.X - likeLabelSize.Width - 4, frame.Y + 4, likeLabelSize.Width, likeLabelSize.Height));
			likeLabel.TextColor = AppDelegate.BoardOrange;
			likeLabel.Font = likeFont;
			likeLabel.Text = likeText;
			likeLabel.TextAlignment = UITextAlignment.Right;

			return likeLabel;
		}

		private UIImageView CreateEye(CGRect frame)
		{
			CGSize iconSize = new CGSize (30, 30);

			UIImageView eyeView = new UIImageView(new CGRect (frame.X + 10, frame.Height - iconSize.Height - 5, iconSize.Width, iconSize.Height));
			closedEyeImage = UIImage.FromFile ("./boardinterface/closedeye.png");
			openEyeImage = UIImage.FromFile ("./boardinterface/openeye3.png");
			closedEyeImage = closedEyeImage.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
			openEyeImage = openEyeImage.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
			eyeView.Image = closedEyeImage;
			eyeView.TintColor = UIColor.FromRGB(140,140,140);

			return eyeView;
		}

		private UIImageView CreateLike(CGRect frame)
		{
			CGSize iconSize = new CGSize (30, 30);

			UIImageView likeView = new UIImageView(new CGRect (frame.Width - iconSize.Width - 10,
				frame.Height - iconSize.Height - 5, iconSize.Width, iconSize.Height));
			likeView.Image = UIImage.FromFile ("./boardinterface/like.png");
			likeView.Image = likeView.Image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
			likeView.TintColor = UIColor.FromRGB(140,140,140);

			return likeView;
		}


		private UIImageView CreateColorView(CGRect frame, CGColor color)
		{
			UIGraphics.BeginImageContext (new CGSize(frame.Size.Width, frame.Size.Height));
			CGContext context = UIGraphics.GetCurrentContext ();

			context.SetFillColor(color);
			context.FillRect(frame);

			UIImage orange = UIGraphics.GetImageFromCurrentImageContext ();
			UIImageView uiv = new UIImageView (orange);
			uiv.Frame = frame;

			return uiv;
		}

		private CGRect GetFrame(Video vid)
		{
			float imgw, imgh;
			float autosize = 150;

			float scale = (float)(vid.Thumbnail.Size.Width/vid.Thumbnail.Size.Height);

			if (scale >= 1) {
				imgw = autosize * scale;
				imgh = autosize;

				if (imgw > AppDelegate.ScreenWidth) {
					scale = (float)(vid.Thumbnail.Size.Height/vid.Thumbnail.Size.Width);
					imgw = AppDelegate.ScreenWidth;
					imgh = imgw * scale;
				}
			} else {
				scale = (float)(vid.Thumbnail.Size.Height / vid.Thumbnail.Size.Width);
				imgw = autosize;
				imgh = autosize * scale;
			}

			vid.Thumbnail = CommonUtils.ResizeImage (vid.Thumbnail, new CGSize (imgw, imgh));

			CGRect frame = new CGRect (vid.Frame.X, vid.Frame.Y, imgw, imgh);

			return frame;
		}

		public void SetFrame(CGRect frame)
		{
			uiView.Frame = frame;
		}

		const int NSEC_PER_SEC = 1000000000;

		private void LooperMethod()
		{
			while (keepLooping) {

				int time = 0;

				while (time < videoDuration) {
					System.Threading.Thread.Sleep (1000);
					time++;
				}

				uiView.InvokeOnMainThread (() => {
					_player.Seek (new CMTime (0, NSEC_PER_SEC));
				});
			}
		}

		bool keepLooping = true;
		Thread looper;
		AVPlayer _player;
		double videoDuration;

		private AVPlayerLayer LoadVideoThumbnail(CGRect frame)
		{	
			AVAsset _asset;
			AVPlayerItem _playerItem;
			AVPlayerLayer _playerLayer;

			_asset = AVAsset.FromUrl (NSUrl.FromString (video.Url));
			_playerItem = new AVPlayerItem (_asset);
			_playerItem.AudioMix = new AVAudioMix ();
			_player = new AVPlayer (_playerItem);
			_playerLayer = AVPlayerLayer.FromPlayer (_player);
			_playerLayer.Frame = frame;
			_player.Seek (new CMTime (0, NSEC_PER_SEC));
			_player.Play ();
			_player.Muted = true;
			_player.Volume = 0;

			videoDuration = Math.Floor(_player.CurrentItem.Asset.Duration.Seconds);

			looper = new Thread (new ThreadStart (LooperMethod));
			looper.Start ();

			return _playerLayer;
		}

	}
}

