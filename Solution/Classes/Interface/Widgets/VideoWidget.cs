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
			Frame = MountingView.Frame;
			AddSubview (MountingView);

			var repeaterVideo = new UIRepeatVideo (new CGRect (SideMargin, TopMargin, size.Width, size.Height), video.GetNSUrlForDisplay ());
			repeaterVideo.View.UserInteractionEnabled = false;
			repeaterVideo.VideoGravity = AVFoundation.AVLayerVideoGravity.ResizeAspectFill;

			AddSubview (repeaterVideo.View);

			Layer.AllowsEdgeAntialiasing = true;

			CreateGestures ();
		}

		public void SetFrame(CGRect frame)
		{
			Frame = frame;
		}
			
	}
}

