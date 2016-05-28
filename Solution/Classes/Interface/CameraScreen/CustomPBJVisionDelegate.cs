using AVFoundation;
using Foundation;
using PBJVisionBinding;
using UIKit;
using CoreGraphics;
using MGImageUtilitiesBinding;
using Board.Utilities;

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

			if (CameraController.Vision.FlashMode == PBJFlashMode.On) {
				CameraController.Vision.FlashMode = PBJFlashMode.Auto;
			}

			vision.StopPreview ();

			var cameraController = (CameraController)AppDelegate.NavigationController.TopViewController;

			// rotate
			image = CommonUtils.RotateImage(image, UIImageOrientation.Up);
			// scale
			image = image.ImageScaledToFitSize(new CGSize(AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			// compress
			var data = image.AsJPEG (0.5f);
			image = new UIImage (data);

			cameraController.ImportImage (image);
		}

		public override void VisionVideo (PBJVision vision, NSDictionary videoDict, NSError error)
		{
			if (CameraController.Vision.FlashMode == PBJFlashMode.On) {
				CameraController.Vision.FlashMode = PBJFlashMode.Auto;
			}

			vision.StopPreview ();

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
		
	}
}

