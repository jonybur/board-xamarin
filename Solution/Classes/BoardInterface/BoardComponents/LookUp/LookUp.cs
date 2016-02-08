using System;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;
using CoreGraphics;

using System.Threading.Tasks;

namespace Solution
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

			/*
			UIPinchGestureRecognizer pinch = new UIPinchGestureRecognizer ((tg) => {	
				CGPoint anchor = tg.LocationInView(lookUpImage);
				anchor = new CGPoint(anchor.X - lookUpImage.Bounds.Size.Width / 2,
					anchor.Y - lookUpImage.Bounds.Size.Height / 2);

				CGAffineTransform affineMatrix = lookUpImage.Transform;
				affineMatrix = CGAffineTransform.Translate(affineMatrix, anchor.X, anchor.Y);
				affineMatrix = CGAffineTransform.Scale(affineMatrix, tg.Scale, tg.Scale);
				affineMatrix = CGAffineTransform.Translate(affineMatrix, -anchor.X, -anchor.Y);
				lookUpImage.Transform = affineMatrix;

				tg.Scale = 1;

			});
			scrollView.AddGestureRecognizer (pinch);
			*/

			UITapGestureRecognizer doubletap = new UITapGestureRecognizer  ((tg) => {

				// TODO: zoom at a certain point in the image 
				if (scrollView.ZoomScale > 1)
					scrollView.SetZoomScale(1f, true);
				else
					scrollView.SetZoomScale(3f, true);

				tg.NumberOfTapsRequired = 2;

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

			UIImageView imageView = new UIImageView (new CGRect (0, 0, imgw, imgh));
			imageView.Layer.AnchorPoint = new CGPoint(.5f, .5f);
			imageView.Center = new CGPoint(AppDelegate.ScreenWidth/2, AppDelegate.ScreenHeight/2);
			imageView.Image = image;
			imageView.UserInteractionEnabled = true;

			return imageView;
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

