using Board.Schema;
using CoreGraphics;
using UIKit;
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

			if (pic.Image == null) {
				PictureImageView.Frame = new CGRect (SideMargin, TopMargin, Widget.Autosize, Widget.Autosize);
				PictureImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
				PictureImageView.SetImage (new NSUrl (picture.ImageUrl),
					UIImage.FromFile ("./demo/magazine/nantucket.png"),
					ImageFromHaneke, ErrorFromHaneke);
			} else {
				SetWidget ();
			}

			PictureImageView.Layer.AllowsEdgeAntialiasing = true;
			
			CreateGestures ();
		}

		public void ImageFromHaneke(UIImage obj){
			Center = new CGPoint ();
			Transform = CGAffineTransform.MakeIdentity ();

			picture.SetImageFromUIImage (obj);

			SetWidget ();
		}

		private void SetWidget(){
			PictureImageView.Image = picture.Thumbnail;
			PictureImageView.ClipsToBounds = true;
			CreateMounting (PictureImageView.Frame.Size);

			Frame = MountingView.Frame;

			AddSubviews (MountingView, PictureImageView);

			var descriptionView = CreateDescriptionView (picture.Description, PictureImageView.Frame.Size);
			AddSubview (descriptionView);

			AppDelegate.BoardInterface.BoardScroll.SelectiveRendering ();
		}

		public void ErrorFromHaneke(NSError obj){
			System.Console.WriteLine ("ERROR: " + obj.LocalizedDescription);
		}

	}
}

