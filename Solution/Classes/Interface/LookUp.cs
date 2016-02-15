using Board.Interface.Buttons;

using Board.Schema;

using CoreGraphics;
using UIKit;

namespace Board.Interface
{
	public class LookUp : UIViewController
	{
		// UIView contains ScrollView and BackButton
		// ScrollView contains LookUpImage
		private UIView uiView;

		public UIView GetLookUpUIView()
		{
			return uiView;
		}

		public LookUp()
		{
		}

		public LookUp(Picture picture)
		{
			AutomaticallyAdjustsScrollViewInsets = false;
			uiView = new UIView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			UIScrollView scrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));

			uiView.BackgroundColor = UIColor.Black;

			UIImageView lookUpImage = CreateImageFrame (picture.Image);
			scrollView.AddSubview (lookUpImage);
			scrollView.MaximumZoomScale = 4f;
			scrollView.MinimumZoomScale = 1f;
			scrollView.ViewForZoomingInScrollView += (UIScrollView sv) => {
				return lookUpImage;
			};

			UITapGestureRecognizer doubletap = new UITapGestureRecognizer  ((tg) => {
				
				if (scrollView.ZoomScale > 1)
					scrollView.SetZoomScale(1f, true);
				else
					scrollView.SetZoomScale(3f, true);

				tg.NumberOfTapsRequired = 2;

				/* CODE TO MANUALLY ZOOM TO A SPECIFIC POINT ON THE IMAGE
				 
					CGPoint anchor = tg.LocationInView(lookUpImage);
					anchor = new CGPoint(anchor.X - lookUpImage.Bounds.Size.Width / 2,
						anchor.Y - lookUpImage.Bounds.Size.Height / 2);

					CGAffineTransform affineMatrix = lookUpImage.Transform;
					affineMatrix = CGAffineTransform.Translate(affineMatrix, anchor.X, anchor.Y);
					affineMatrix = CGAffineTransform.Scale(affineMatrix, tg.Scale, tg.Scale);
					affineMatrix = CGAffineTransform.Translate(affineMatrix, -anchor.X, -anchor.Y);
					lookUpImage.Transform = affineMatrix;

					tg.Scale = 1;
				*/

			});

			scrollView.AddGestureRecognizer (doubletap);
			scrollView.UserInteractionEnabled = true;

			UIImageView backButton = CreateBackButton ();
			uiView.AddSubviews (scrollView, backButton);

			View.Add (uiView);
		}

		private UIImageView CreateImageFrame(UIImage image)
		{
			float imgw, imgh;
			float scale = (float)(image.Size.Height/image.Size.Width);

			imgw = AppDelegate.ScreenWidth;
			imgh = AppDelegate.ScreenWidth * scale;

			UIImageView imageView = new UIImageView (new CGRect (0, AppDelegate.ScreenHeight/2 - imgh / 2, imgw, imgh));
			imageView.Layer.AnchorPoint = new CGPoint(.5f, .5f);
			imageView.Image = image;
			imageView.UserInteractionEnabled = true;

			UIImageView blackTop = CreateColorView(new CGRect(0,0,AppDelegate.ScreenWidth, imageView.Frame.Top), UIColor.Black.CGColor);

			UIImageView composite = new UIImageView(new CGRect(0,0,AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));

			composite.AddSubviews (blackTop, imageView);


			return composite;
		}


		private UIImageView CreateColorView(CGRect frame, CGColor color)
		{
			UIGraphics.BeginImageContext (new CGSize(frame.Size.Width, frame.Size.Height));
			CGContext context = UIGraphics.GetCurrentContext ();

			context.SetFillColor(color);
			context.FillRect(frame);

			UIImage orange = UIGraphics.GetImageFromCurrentImageContext ();
			UIImageView uiv = new UIImageView (orange);
			uiv.Frame = frame;

			return uiv;
		}


		private UIImageView CreateBackButton()
		{
			UIImage doneBut = UIImage.FromFile ("./boardscreen/lookup/done.png");
			UIImageView uiv = new UIImageView(new CGRect(0,0,doneBut.Size.Width/2,doneBut.Size.Height/2));
			uiv.Image = doneBut;
			uiv.UserInteractionEnabled = true;
			// hardcoded to be set in correct location
			uiv.Center = new CGPoint (AppDelegate.ScreenWidth - doneBut.Size.Width / 2 + 15, 45);

			UITapGestureRecognizer tapGesture= new UITapGestureRecognizer  ((tg) => {
				// user tapped on "Done" button
				NavigationController.PopViewController(true);
				ButtonInterface.SwitchButtonLayout((int)ButtonInterface.ButtonLayout.NavigationBar);
				View.Dispose();
			});

			uiv.AddGestureRecognizer (tapGesture);

			return uiv;
		}
	}
}

