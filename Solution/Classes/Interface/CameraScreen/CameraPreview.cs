using UIKit;
using PBJVisionBinding;
using CoreGraphics;
using AssetsLibrary;
using AVFoundation;

namespace Board.Interface.Camera
{
	public class CameraPreview : UIView
	{
		AVCaptureVideoPreviewLayer previewLayer;

		public CameraPreview()
		{	
			BackgroundColor = UIColor.Black;
			CGRect previewFrame = new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);
			Frame = previewFrame;

			previewLayer = PBJVision.SharedInstance.PreviewLayer;
			previewLayer.Frame = Bounds;
			previewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
			Layer.AddSublayer(previewLayer);

			CameraController.Vision = PBJVision.SharedInstance;
			CameraController.Vision.Delegate = new CustomPBJVisionDelegate ();
			CameraController.Vision.ThumbnailEnabled = true;
			CameraController.Vision.CameraOrientation = PBJCameraOrientation.Portrait;

			CameraController.Vision.OutputFormat = PBJOutputFormat.Preset;
			//CameraController.Vision.CustomVideoSize = new CGSize (previewFrame.Size.Width / 2 - 1, previewFrame.Size.Height / 2 - 1);
			CameraController.Vision.FocusMode = PBJFocusMode.ContinuousAutoFocus;

			CameraController.Vision.MaximumCaptureDuration = new CoreMedia.CMTime (20000000000, 1000000000);
			CameraController.Vision.CaptureSessionPreset = AVCaptureSession.PresetInputPriority;
			CameraController.Vision.CaptureDirectory = CameraController.CaptureDirectory;
			CameraController.Vision.StartPreview();
			//CameraController.Vision.CaptureSessionPreset = AVCaptureSession.PresetMedium;
			//CameraController.Vision.CaptureSessionPreset = AVCaptureSession.PresetInputPriority;

			UserInteractionEnabled = true;
		}
	}
}

