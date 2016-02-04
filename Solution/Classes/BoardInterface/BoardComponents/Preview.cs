
using System;
using Newtonsoft.Json;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;
using System.Threading.Tasks;

using System.Collections.Generic;

using Facebook.CoreKit;

namespace Solution
{
	public static class Preview
	{
		// TODO: preview should handle picture previews with a PictureComponent
		public static TextBoxComponent textBoxComponent;

		private static UIView uiView;
		private static UIImage compressedImage;

		public static bool IsPicturePreview;

		private static float Rotation = 0;

		public static UIView GetUIView()
		{
			return uiView;
		}

		public static void Initialize (UIImage image, CGPoint ContentOffset)
		{
			IsPicturePreview = true;
			compressedImage = image;

			// the image is uploadable
			// so now launch image preview to choose position in the board
			float imgx, imgy, imgw, imgh;
			float autosize = 150;

			float scale = (float)(image.Size.Width/image.Size.Height);

			if (scale >= 1) {
				imgw = autosize * scale;
				imgh = autosize;

				if (imgw > AppDelegate.ScreenWidth) {
					scale = (float)(image.Size.Height/image.Size.Width);
					imgw = AppDelegate.ScreenWidth;
					imgh = imgw * scale;
				}
			} else {
				scale = (float)(image.Size.Height / image.Size.Width);
				imgw = autosize;
				imgh = autosize * scale;
			}


			imgx = (float)(ContentOffset.X + AppDelegate.ScreenWidth / 2 - imgw / 2);
			imgy = (float)(ContentOffset.Y + AppDelegate.ScreenHeight / 2 - imgh / 2 - Button.ButtonSize / 2);

			// launches the image preview

			CGRect frame = new CGRect (imgx, imgy, imgw, imgh);
			uiView = new UIView (frame);

			UIImageView uiImageView =  new UIImageView (new CGRect(0,0,frame.Width, frame.Height));
		
			UIImage thumbImg = image.Scale (new CGSize (imgw, imgh));
			uiImageView.Image = thumbImg;
			uiImageView.Alpha = .5f;

			uiImageView.UserInteractionEnabled = true;
			uiImageView.AddGestureRecognizer (SetNewRotationGestureRecognizer(true));
			uiImageView.AddGestureRecognizer (SetNewPanGestureRecognizer());

			uiView.AddSubviews(uiImageView);
		}

		public static async System.Threading.Tasks.Task Initialize (TextBox textBox, CGPoint ContentOffset, Action refreshContent, UINavigationController navigationController)
		{
			TextBoxComponent textBoxComponent;

			IsPicturePreview = false;
			// so now launch image preview to choose position in the board

			textBox.ImgX = (float)(ContentOffset.X + AppDelegate.ScreenWidth / 2 - textBox.ImgW / 2);
			textBox.ImgY = (float)(ContentOffset.Y + AppDelegate.ScreenHeight / 2 - textBox.ImgH / 2 - Button.ButtonSize / 2);

			CGRect frame = new CGRect (textBox.ImgX, textBox.ImgY, textBox.ImgW, textBox.ImgH);
			textBoxComponent = new TextBoxComponent (textBox);

			await textBoxComponent.Load (navigationController, refreshContent);

			// launches the textbox preview
			uiView = textBoxComponent.GetUIView ();
			uiView.Alpha = .5f;
			uiView.UserInteractionEnabled = true;
			uiView.AddGestureRecognizer (SetNewPanGestureRecognizer());
			uiView.AddGestureRecognizer (SetNewRotationGestureRecognizer(false));
		}

		public static UIPanGestureRecognizer SetNewPanGestureRecognizer()
		{
			float dx = 0;
			float dy = 0;

			UIPanGestureRecognizer panGesture = new UIPanGestureRecognizer ((pg) => {
				if ((pg.State == UIGestureRecognizerState.Began || pg.State == UIGestureRecognizerState.Changed) && (pg.NumberOfTouches == 1)) {

					var p0 = pg.LocationInView(uiView.Superview);

					if (dx == 0)
						dx = (float)(p0.X - uiView.Center.X);

					if (dy == 0)
						dy = (float)(p0.Y - uiView.Center.Y);

					var p1 = new CGPoint (p0.X - dx, p0.Y - dy);

					uiView.Center = p1;

				} else if (pg.State == UIGestureRecognizerState.Ended) {
					dx = 0;
					dy = 0;
				}
			});
			return panGesture;
		}

		private static UIRotationGestureRecognizer SetNewRotationGestureRecognizer(bool autoRotate)
		{
			float r = 0;
			if (autoRotate) {
				Random rnd = new Random ();
				r = (float)(rnd.NextDouble () / 2) - .25f;
				Rotation = r;
				uiView.Transform = CGAffineTransform.MakeRotation (r);
			}

			UIRotationGestureRecognizer rotateGesture = new UIRotationGestureRecognizer ((rg) => {
				if ((rg.State == UIGestureRecognizerState.Began || rg.State == UIGestureRecognizerState.Changed) && (rg.NumberOfTouches == 2)) {
					uiView.Transform = CGAffineTransform.MakeRotation (rg.Rotation + r);
					Rotation = (float)(rg.Rotation + r);
				} else if (rg.State == UIGestureRecognizerState.Ended) {
					r += (float)rg.Rotation;
				}
			});
			return rotateGesture;
		}

		public static void RemoveUserInteraction()
		{
			uiView.UserInteractionEnabled = false;
		}

		public static void RemoveFromSuperview()
		{
			uiView.RemoveFromSuperview ();
			uiView.Dispose ();
		}

		public static Picture GetPicture()
		{
			CGRect rec = new CGRect (uiView.Center, uiView.Bounds.Size);
			Picture p = new Picture (	string.Empty, string.Empty, Rotation, rec, Profile.CurrentProfile.UserID);
			return p;
		}

		public static TextBox GetTextBox()
		{
			TextBox aux = textBoxComponent.GetTextBox ();
			aux.SetRotation (Rotation);
			return aux;
		}


	}
}

