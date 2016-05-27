using Board.Schema;
using CoreGraphics;
using UIKit;
using System.Threading.Tasks;
using Foundation;
using Haneke;

namespace Board.Interface.Widgets
{
	public class PictureWidget : Widget
	{
		// UIView contains ScrollView and BackButton
		// ScrollView contains LookUpImage

		UIImageView PictureImageView;


		public Picture picture
		{
			get { return (Picture)content; }
		}

		public PictureWidget()
		{
		}

		public PictureWidget(Picture pic)
		{
			content = pic;

			PictureImageView = new UIImageView ();
			PictureImageView.Frame = new CGRect (0, 0, Widget.Autosize, Widget.Autosize);
			PictureImageView.LayoutIfNeeded ();

			CreateMounting (PictureImageView.Frame.Size);
			AddSubviews (MountingView, PictureImageView);

			PictureImageView.ContentMode = UIViewContentMode.ScaleAspectFill;

			PictureImageView.SetImage (new NSUrl (picture.ImageUrl), UIImage.FromFile ("./demo/magazine/westpalmbeach.png"), ImageFromHaneke, ErrorFromHaneke);

			PictureImageView.Layer.AllowsEdgeAntialiasing = true;

			EyeOpen = false;

			CreateGestures ();
		}

		public void ImageFromHaneke(UIImage obj){

			PictureImageView.RemoveFromSuperview ();
			MountingView.RemoveFromSuperview ();

			picture.SetImageFromUIImage (obj);

			PictureImageView.Frame = new CGRect (SideMargin, TopMargin, picture.Thumbnail.Size.Width, picture.Thumbnail.Size.Height);
			PictureImageView.Image = picture.Thumbnail;
			CreateMounting (PictureImageView.Frame.Size);

			Frame = MountingView.Frame;

			AddSubviews (MountingView, PictureImageView);
		}

		public void ErrorFromHaneke(NSError obj){
			System.Console.WriteLine ("stop");
		}

		public void SetFrame(CGRect frame)
		{
			Frame = frame;
		}

	}
}

