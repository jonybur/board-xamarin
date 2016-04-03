using Foundation;
using PBJVisionBinding;
using AVFoundation;
using CoreGraphics;
using System;
using UIKit;

namespace Board.Interface.Camera
{
	class CustomPBJVisionDelegate : PBJVisionDelegate{
		public static NSUrl VideoPath;

		public override void Vision (PBJVision vision, UIImage image)
		{
			// gets preview picture (picture from image buffer)
			if (image == null) {
				CameraController.Vision.CapturePreviewPhoto();
				return;
			}

			var cameraController = (CameraController)AppDelegate.NavigationController.TopViewController;

			cameraController.ImportImage (image);
		}

		public override void VisionVideo (PBJVision vision, NSDictionary videoDict, NSError error)
		{
			//vision.StopPreview ();

			NSString videoPath = videoDict.ObjectForKey (new NSString ("PBJVisionVideoPathKey")) as NSString;
			NSUrl videoURL = NSUrl.FromFilename(videoPath);

			// treat as video
			var urlAsset = new AVUrlAsset (videoURL);
			AVAssetExportSession session = new AVAssetExportSession (urlAsset, AVAssetExportSession.PresetMediumQuality);
			//VideoPath = NSUrl.FromFilename (CameraController.CaptureDirectory + "/" + compressedGuid + ".mp4");
			VideoPath = videoURL;

			var outputUrl = VideoPath;
			session.OutputUrl = outputUrl;
			session.OutputFileType = AVFileType.Mpeg4;

			var cameraController = (CameraController)AppDelegate.NavigationController.TopViewController;
			session.ExportAsynchronously (cameraController.ImportVideo);
		}
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
}

