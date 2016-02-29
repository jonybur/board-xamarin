﻿using System;
using System.Threading;

using AVFoundation;
using Board.Schema;

using Board.Utilities;
using CoreGraphics;
using MediaPlayer;
using CoreMedia;
using Foundation;
using UIKit;

namespace Board.Interface.Widgets
{
	public class VideoWidget : Widget
	{
		// UIView contains ScrollView and BackButton
		// ScrollView contains LookUpImage

		private Video video;

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
			View = new UIView(mounting.Frame);
			View.AddSubview (mounting);

			// picture
			CGRect pictureFrame = new CGRect (mounting.Frame.X + 10, 10, frame.Width, frame.Height);
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

			// like

			UIImageView like = CreateLike (mounting.Frame);
			View.AddSubview (like);

			// like label

			UILabel likeLabel = CreateLikeLabel (like.Frame);
			View.AddSubview (likeLabel);

			// eye

			eye = CreateEye (mounting.Frame);
			View.AddSubview (eye);

			View.Frame = new CGRect (vid.Frame.X, vid.Frame.Y, mounting.Frame.Width, mounting.Frame.Height);
			View.Transform = CGAffineTransform.MakeRotation(vid.Rotation);

			EyeOpen = false;

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				if (Preview.View != null) { return; }

				MPMoviePlayerController moviePlayer = new MPMoviePlayerController (NSUrl.FromFilename (video.Url));

				View.Superview.Superview.AddSubview(moviePlayer.View);

				moviePlayer.SetFullscreen (true, false);
				moviePlayer.Play ();

			});

			gestureRecognizers.Add (tap);
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
			Random rand = new Random ();
			string likeText = rand.Next(16, 98).ToString();
			CGSize likeLabelSize = likeText.StringSize (likeFont);
			UILabel likeLabel = new UILabel(new CGRect(frame.X - likeLabelSize.Width - 4, frame.Y + 4, likeLabelSize.Width, likeLabelSize.Height));
			likeLabel.TextColor = BoardInterface.board.MainColor;
			likeLabel.Font = likeFont;
			likeLabel.Text = likeText;
			likeLabel.TextAlignment = UITextAlignment.Right;

			return likeLabel;
		}

		private UIImageView CreateEye(CGRect frame)
		{
			CGSize iconSize = new CGSize (30, 30);

			UIImageView eyeView = new UIImageView(new CGRect (frame.X + 10, frame.Height - iconSize.Height - 5, iconSize.Width, iconSize.Height));
			eyeView.Image = Widget.ClosedEyeImage;
			eyeView.TintColor = UIColor.FromRGB(140,140,140);

			return eyeView;
		}

		private UIImageView CreateLike(CGRect frame)
		{
			CGSize iconSize = new CGSize (30, 30);

			UIImageView likeView = new UIImageView(new CGRect (frame.Width - iconSize.Width - 10,
				frame.Height - iconSize.Height - 5, iconSize.Width, iconSize.Height));
			likeView.Image = UIImage.FromFile ("./boardinterface/widget/like.png");
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
			View.Frame = frame;
		}

		private void LooperMethod()
		{
			while (loop) {

				int time = 0;

				while (time < videoDuration) {
					Thread.Sleep (1000);
					time++;
				}

				View.InvokeOnMainThread (() => _player.Seek (new CMTime (0, 1000000000)));
			}
		}

		bool loop;
		Thread looper;
		AVPlayer _player;
		double videoDuration;

		private AVPlayerLayer LoadVideoThumbnail(CGRect frame)
		{	
			AVAsset _asset;
			AVPlayerItem _playerItem;
			AVPlayerLayer _playerLayer;

			_asset = AVAsset.FromUrl (NSUrl.FromFilename (video.Url));
			_playerItem = new AVPlayerItem (_asset);
			_playerItem.AudioMix = new AVAudioMix ();
			_player = new AVPlayer (_playerItem);
			_playerLayer = AVPlayerLayer.FromPlayer (_player);
			_playerLayer.Frame = frame;
			_player.Seek (new CMTime (0, 1000000000));
			_player.Play ();
			_player.Muted = true;
			_player.Volume = 0;

			videoDuration = Math.Floor(_player.CurrentItem.Asset.Duration.Seconds);

			if (videoDuration > 5) {
				videoDuration = 5;
			}

			loop = true;
			looper = new Thread (new ThreadStart (LooperMethod));
			looper.Start ();

			return _playerLayer;
		}

		public void KillLooper()
		{
			loop = false;
		}

	}
}

