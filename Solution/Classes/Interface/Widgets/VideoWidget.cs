using Board.Schema;
using Board.Screens.Controls;
using CoreGraphics;
using UIKit;

namespace Board.Interface.Widgets
{
	public class VideoWidget : Widget
	{
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

			var repeaterVideo = new UIRepeatVideo (new CGRect (SideMargin, TopMargin, size.Width, size.Height), video.GetNSUrlForDisplay ());
			repeaterVideo.View.UserInteractionEnabled = false;
			repeaterVideo.VideoGravity = AVFoundation.AVLayerVideoGravity.ResizeAspectFill;

			View.AddSubview (repeaterVideo.View);

			View.Layer.AllowsEdgeAntialiasing = true;

			EyeOpen = false;

			CreateGestures ();
		}

		public void SetFrame(CGRect frame)
		{
			View.Frame = frame;
		}
			
	}
}

