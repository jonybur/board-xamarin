using System;
using AVFoundation;
using Board.Utilities;
using System.Threading;
using CoreGraphics;
using PBJVisionBinding;
using CoreMedia;
using UIKit;
using Foundation;

namespace Board.Infrastructure
{
	public class CameraController : UIViewController{
		
		PBJVision Vision;

		UIView PreviewView;

		UICameraButton CameraButton;
		UITapGestureRecognizer BackTap;
		UIImageView BackButton;

		public override void ViewDidLoad ()
		{
			PreviewView = CreatePreviewView ();
			CreateBackButton (UIColor.White);
			CameraButton = new UICameraButton (Vision);
			View.AddSubviews (PreviewView, BackButton, CameraButton);
		}

		public override void ViewDidAppear(bool animated)
		{
			BackButton.AddGestureRecognizer (BackTap);
			CameraButton.EnableGestures ();
		}

		public override void ViewDidDisappear(bool animated)
		{
			BackButton.RemoveGestureRecognizer (BackTap);
			CameraButton.DisableGestures ();
			MemoryUtility.ReleaseUIViewWithChildren (View);
		}

		public static string CaptureDirectory;

		private UIView CreatePreviewView()
		{	
			CaptureDirectory = (NSFileManager.DefaultManager.GetUrls (
				NSSearchPathDirectory.LibraryDirectory, 
				NSSearchPathDomain.User) [0]).Path;
			
			var PreviewView = new UIView();
			PreviewView.BackgroundColor = UIColor.Black;
			CGRect previewFrame = new CGRect(0,0,AppDelegate.ScreenWidth,AppDelegate.ScreenHeight);
			PreviewView.Frame = previewFrame;

			var previewLayer = PBJVision.SharedInstance.PreviewLayer;
			previewLayer.Frame = PreviewView.Bounds;
			previewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
			PreviewView.Layer.AddSublayer(previewLayer);

			Vision = PBJVision.SharedInstance;
			Vision.Delegate = new CustomPBJVisionDelegate ();
			Vision.ThumbnailEnabled = true;
			Vision.CameraOrientation = PBJCameraOrientation.Portrait;

			Vision.OutputFormat = PBJOutputFormat.Custom;
			Vision.CustomVideoSize = new CGSize (previewFrame.Size.Width - 1, previewFrame.Size.Height - 1);
			Vision.FocusMode = PBJFocusMode.ContinuousAutoFocus;
			Vision.MaximumCaptureDuration = new CoreMedia.CMTime (20000000000, 1000000000);

			Vision.CaptureDirectory = CaptureDirectory;
			Vision.StartPreview();

			return PreviewView;
		}
			
		class CustomPBJVisionDelegate : PBJVisionDelegate{
			public static NSUrl VideoPath;

			public override void Vision (PBJVision vision, NSDictionary photoDict, NSError error)
			{
				// KEYS:

				// PBJVisionPhotoJPEGKey
				// PBJVisionPhotoThumbnailKey
				// PBJVisionPhotoMetadataKey
				// PBJVisionPhotoImageKey
			}

			public override void VisionVideo (PBJVision vision, NSDictionary videoDict, NSError error)
			{
				NSString videoPath = videoDict.ObjectForKey (new NSString ("PBJVisionVideoPathKey")) as NSString;

				Compress (NSUrl.FromFilename(videoPath));

				/*

				encoder = SDAVAssetExportSession.ExportSessionWithAsset (asset) as SDAVAssetExportSession;

				var codecsettings = new AVVideoCodecSettings ();
				codecsettings.ProfileLevelH264 = AVVideoProfileLevelH264.HighAutoLevel;
				codecsettings.AverageBitRate = 2300000;
				//codecsettings.MaxKeyFrameInterval = 2;

				var compressed = new AVVideoSettingsCompressed ();
				compressed.Width = (int)AppDelegate.ScreenWidth;
				compressed.Height = (int)AppDelegate.ScreenHeight;
				compressed.Codec = AVVideoCodec.H264;
				compressed.CodecSettings = codecsettings;

				var audioSettings = new NSMutableDictionary();
				audioSettings.Add(AVAudioSettings.AVFormatIDKey, new NSString("kAudioFormatMPEG4AAC"));
				audioSettings.Add(AVAudioSettings.AVNumberOfChannelsKey, new NSNumber(2));
				audioSettings.Add(AVAudioSettings.AVSampleRateKey, new NSNumber(44100));
				audioSettings.Add(AVAudioSettings.AVEncoderBitRateKey, new NSNumber(128000));

				encoder.OutputURL = NSUrl.FromFilename (vision.CaptureDirectory + "/");//+CommonUtils.GenerateGuid()+".mov");
				encoder.OutputFileType = AVFileType.Mpeg4;
				encoder.VideoSettings = compressed.Dictionary;
				encoder.AudioSettings = audioSettings;
				encoder.ShouldOptimizeForNetworkUse = true;

				encoder.ExportAsynchronouslyWithCompletionHandler (Tasty);
				
				*/
			}

			private void Compress(NSUrl videoPath)
			{
				AVUrlAsset urlAsset = new AVUrlAsset (videoPath);
				AVAssetExportSession session = new AVAssetExportSession (urlAsset, AVAssetExportSession.PresetMediumQuality);
				string compressedGuid = CommonUtils.GenerateGuid ();
				VideoPath = NSUrl.FromFilename (CaptureDirectory + "/" + compressedGuid + ".mp4");
				var outputUrl = VideoPath;
				session.OutputUrl = outputUrl;
				session.OutputFileType = AVFileType.Mpeg4;

				var cameraController = (CameraController)AppDelegate.NavigationController.TopViewController;

				session.ExportAsynchronously (cameraController.ImportVideo);
			}
		}

		private void ImportVideo()
		{
			//ALAssetsLibrary lib = new ALAssetsLibrary ();
			//await lib.WriteVideoToSavedPhotosAlbumAsync (NSUrl.FromFilename(CaptureDirectory + "/" + compressedGuid + ".mp4"));
			//lib.Dispose ();

			// TODO: does something with the compressed video

			// TODO: deletes full res video

			//File.Delete (CaptureDirectory + "/" + compressedGuid + ".mp4");
			//compressedGuid = null;

			InvokeOnMainThread (() => LoadVideo (CustomPBJVisionDelegate.VideoPath));
		}

		AVPlayer _player;
		Thread looper;
		bool loop;
		int videoDuration;
		int time;

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

		private void LoadVideo(NSUrl videoPath)
		{
			PreviewView.RemoveFromSuperview ();

			AVPlayerItem _playerItem;
			using (AVAsset _asset = AVAsset.FromUrl (videoPath)) {
				_playerItem = new AVPlayerItem (_asset);
			}
			_player = new AVPlayer (_playerItem);
			AVPlayerLayer _playerLayer = AVPlayerLayer.FromPlayer (_player);
			_playerLayer.Frame = new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);
			_player.ActionAtItemEnd = AVPlayerActionAtItemEnd.Pause;
			_player.Play ();
			videoDuration = (int)Math.Floor (_player.CurrentItem.Asset.Duration.Seconds);

			looper = new Thread (new ThreadStart (LooperMethod));
			looper.Start ();
			loop = true;

			View.Layer.AddSublayer (_playerLayer);
		}

		private void CreateBackButton(UIColor buttonColor)
		{
			using (UIImage img = UIImage.FromFile ("./boardinterface/lookup/cancel.png")) {
				BackButton = new UIImageView(new CGRect(0,0,img.Size.Width * 2,img.Size.Height * 2));

				UIImageView subView = new UIImageView (new CGRect (0, 0, img.Size.Width / 2, img.Size.Height / 2));
				subView.Image = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				subView.Center = new CGPoint (BackButton.Frame.Width / 2, BackButton.Frame.Height / 2);
				subView.TintColor = buttonColor;

				BackButton.AddSubview (subView);
				BackButton.Center = new CGPoint (img.Size.Width / 2 + 10, 35);
			}

			BackButton.UserInteractionEnabled = true;

			BackTap = new UITapGestureRecognizer (tg => AppDelegate.PopViewLikeDismissView ());
		}

		sealed class UICameraButton : UIImageView{
			public UIImageView RingView;
			private UITapGestureRecognizer Tap;
			private UILongPressGestureRecognizer LongPress;

			public UICameraButton(PBJVision vision)
			{
				Frame = new CGRect(0, 0, 80, 80);

				using (UIImage img = UIImage.FromFile ("./camera/cameraback.png")) {
					Image = img;
					Center = new CGPoint (AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight - Frame.Height / 2 - 20);
				}

				using (UIImage img = UIImage.FromFile ("./camera/camerafront.png")) {
					RingView = new UIImageView(new CGRect(0, 0, Frame.Width, Frame.Height));
					UIImage ringImage = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
					RingView.Image = ringImage;
				}
				RingView.TintColor = UIColor.White;
				AddSubview(RingView);

				UserInteractionEnabled = true;

				Tap = new UITapGestureRecognizer (tg => {
					vision.CameraMode = PBJCameraMode.Photo;
					vision.CapturePhoto();

					Alpha = 0f;
				});

				LongPress = new UILongPressGestureRecognizer(tg => {
					vision.CameraMode = PBJCameraMode.Video;

					switch (tg.State)
					{
						case UIGestureRecognizerState.Began:
							if (!vision.Recording)
							{
								vision.StartVideoCapture();
							}else{
								vision.ResumeVideoCapture();
							}
						RingView.TintColor = UIColor.Red;
							break;
						case UIGestureRecognizerState.Ended:
						case UIGestureRecognizerState.Cancelled:
						case UIGestureRecognizerState.Failed:
								//vision.PauseVideoCapture();
							vision.EndVideoCapture();

							Alpha = 0f;
							break;
						default:
							break;
					}
				});

				LongPress.MinimumPressDuration = .3f;
			}

			public void EnableGestures()
			{
				AddGestureRecognizer (Tap);	
				AddGestureRecognizer (LongPress);
			}

			public void DisableGestures()
			{
				RemoveGestureRecognizer (Tap);
				RemoveGestureRecognizer (LongPress);
			}
		}

	}
}

