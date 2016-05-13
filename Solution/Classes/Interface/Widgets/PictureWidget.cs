using Board.Schema;
using CoreGraphics;
using UIKit;
using System.Threading.Tasks;

namespace Board.Interface.Widgets
{
	public class PictureWidget : Widget
	{
		// UIView contains ScrollView and BackButton
		// ScrollView contains LookUpImage

		UIImageView PictureFrame;


		public Picture picture
		{
			get { return (Picture)content; }
		}

		public PictureWidget()
		{

		}

		public async Task Initialize(){
			if (picture.Image == null || picture.Thumbnail == null) {
				await picture.GetImageFromUrl (picture.ImageUrl);
			}

			UnsuscribeToEvents ();

			var size = picture.Thumbnail.Size;

			MountingView.RemoveFromSuperview ();
			PictureFrame.RemoveFromSuperview ();

			// mounting

			CreateMounting (size);
			View = new UIView(MountingView.Frame);
			View.AddSubview (MountingView);

			// picture

			PictureFrame = new UIImageView ();
			PictureFrame.Frame = new CGRect (MountingView.Frame.X + SideMargin, TopMargin, size.Width, size.Height);
			PictureFrame.Image = picture.Thumbnail;
			PictureFrame.Layer.AllowsEdgeAntialiasing = true;
			View.AddSubview (PictureFrame);

			EyeOpen = false;

			AppDelegate.BoardInterface.BoardScroll.SelectiveRendering ();

			SuscribeToEvents ();
		}

		public PictureWidget(Picture pic)
		{
			content = pic;

			var size = new CGSize(200,200);

			// mounting

			CreateMounting (size);
			View = new UIView(MountingView.Frame);
			View.AddSubview (MountingView);

			// picture

			PictureFrame = new UIImageView ();
			PictureFrame.Frame = new CGRect (MountingView.Frame.X + SideMargin, TopMargin, size.Width, size.Height);
			PictureFrame.Layer.AllowsEdgeAntialiasing = true;
			View.AddSubview (PictureFrame);

			EyeOpen = false;

			CreateGestures ();
		}

		public void SetFrame(CGRect frame)
		{
			View.Frame = frame;
		}

	}
}

