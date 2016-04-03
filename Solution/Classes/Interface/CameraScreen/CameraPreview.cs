using UIKit;
using PBJVisionBinding;
using CoreGraphics;
using AVFoundation;

namespace Board.Interface.Camera
{
	public class CameraPreview : UIView
	{
		public CameraPreview()
		{	
			BackgroundColor = UIColor.Black;
			CGRect previewFrame = new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);
			Frame = previewFrame;

			var previewLayer = PBJVision.SharedInstance.PreviewLayer;
			previewLayer.Frame = Bounds;
			previewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
			Layer.AddSublayer(previewLayer);

			CameraController.Vision = PBJVision.SharedInstance;
			CameraController.Vision.Delegate = new CustomPBJVisionDelegate ();
			CameraController.Vision.ThumbnailEnabled = true;
			CameraController.Vision.CameraOrientation = PBJCameraOrientation.Portrait;
			CameraController.Vision.OutputFormat = PBJOutputFormat.Preset;
			CameraController.Vision.MaximumCaptureDuration = new CoreMedia.CMTime (20000000000, 1000000000);
			CameraController.Vision.CaptureSessionPreset = AVCaptureSession.PresetInputPriority;
			CameraController.Vision.CaptureDirectory = CameraController.CaptureDirectory;

			CameraController.Vision.FocusMode = PBJFocusMode.ContinuousAutoFocus;
			CameraController.Vision.ExposureMode = PBJExposureMode.ContinuousAutoExposure;

			CameraController.Vision.StartPreview();

			UserInteractionEnabled = true;
		}

		public static CGPoint ConvertToPointOfInterestFromViewCoordinates(CGPoint viewCoordinates, CGRect frame){
			var previewLayer = PBJVision.SharedInstance.PreviewLayer;

			CGPoint pointOfInterest = new CGPoint(.5f, .5f);
			CGSize frameSize = frame.Size;

			if (previewLayer.VideoGravity == AVLayerVideoGravity.Resize) {
				pointOfInterest = new CGPoint (viewCoordinates.Y / frameSize.Height, 1f - (viewCoordinates.X / frameSize.Width));
			} else {
				CGSize apertureSize = new CGSize (frame.Height, frame.Width);
				if (apertureSize != new CGSize (0, 0)) {
					CGPoint point = viewCoordinates;
					float apertureRatio = (float)(apertureSize.Height / apertureSize.Width);
					float viewRatio = (float)(frameSize.Width / frameSize.Height);
					float xc = .5f;
					float yc = .5f;

					if (previewLayer.VideoGravity == AVLayerVideoGravity.ResizeAspect) {
						if (viewRatio > apertureRatio) {
							float y2 = (float)frameSize.Height;
							float x2 = (float)frameSize.Height * apertureRatio;
							float x1 = (float)frameSize.Width;
							float blackBar = (x1 - x2) / 2;
							if (point.X >= blackBar && point.X <= blackBar + x2) {
								xc = (float)point.Y / y2;
								yc = 1f - (((float)point.X - blackBar) / x2);
							}
						} else {
							float y2 = (float)frameSize.Width / apertureRatio;
							float y1 = (float)frameSize.Height;
							float x2 = (float)frameSize.Width;
							float blackBar = (y1 - y2) / 2;
							if (point.Y >= blackBar && point.Y <= blackBar + y2) {
								xc = (((float)point.Y - blackBar) / y2);
								yc = 1f - ((float)point.X / x2);
							}
						}
					} else if (previewLayer.VideoGravity == AVLayerVideoGravity.ResizeAspectFill) {
						if (viewRatio > apertureRatio) {
							float y2 = (float)(apertureSize.Width * (frameSize.Width / apertureSize.Height));
							xc = (float)((point.Y + ((y2 - frameSize.Height) / 2f)) / y2);
							yc = (float)((frameSize.Width - point.X) / frameSize.Width);
						} else {
							float x2 = (float)(apertureSize.Height * (frameSize.Height / apertureSize.Width));
							xc = (float)(1f - ((point.X + ((x2 - frameSize.Width) / 2)) / x2));
							yc = (float)(point.Y / frameSize.Height);
						}
					}

					pointOfInterest = new CGPoint (xc, yc);
				}
			}

			return pointOfInterest;
		}
	}
}

