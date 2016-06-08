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

			var size = new CGSize (Widget.Autosize, Widget.Autosize);

			// mounting

			CreateMounting (size);
			Frame = MountingView.Frame;
			AddSubview (MountingView);

			var repeaterVideo = new UIRepeatVideo (new CGRect (SideMargin, TopMargin, size.Width, size.Height), video.GetNSUrlForDisplay ());
			repeaterVideo.View.UserInteractionEnabled = false;
			repeaterVideo.VideoGravity = AVFoundation.AVLayerVideoGravity.ResizeAspectFill;

			AddSubview (repeaterVideo.View);

			var descriptionView = CreateDescriptionView (video.Description, repeaterVideo.View.Frame.Size);
			AddSubview (descriptionView);

			Layer.AllowsEdgeAntialiasing = true;

			CreateGestures ();
		}

		public void SetFrame(CGRect frame)
		{
			Frame = frame;
		}
			
	}
}

