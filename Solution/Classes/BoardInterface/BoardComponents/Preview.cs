
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
		public static TextBoxComponent textBoxComponent;
		public static PictureComponent pictureComponent;

		private static UIView uiView;

		public static bool IsPicturePreview;

		private static float Rotation = 0;

		public static UIView GetUIView()
		{
			return uiView;
		}

		public static void Initialize (UIImage image, CGPoint ContentOffset, UINavigationController navigationController)
		{
			Picture picture = new Picture ();
			picture.Image = image;

			IsPicturePreview = true;
		
			pictureComponent = new PictureComponent (picture);

			CGRect frame = pictureComponent.GetUIView ().Frame;

			uiView = new UIView (new CGRect(ContentOffset.X + AppDelegate.ScreenWidth / 2 - frame.Width / 2,
				ContentOffset.Y + AppDelegate.ScreenHeight / 2 - frame.Height / 2 - Button.ButtonSize / 2,frame.Width, frame.Height));

			//uiView.Alpha = .5f;
			uiView.AddGestureRecognizer (SetNewPanGestureRecognizer());
			uiView.AddGestureRecognizer (SetNewRotationGestureRecognizer(false));
			uiView.AddSubviews(pictureComponent.GetUIView());
		}

		public static async System.Threading.Tasks.Task Initialize (TextBox textBox, CGPoint ContentOffset, Action refreshContent, UINavigationController navigationController)
		{
			IsPicturePreview = false;
			// so now launch image preview to choose position in the board

			textBox.ImgX = (float)(ContentOffset.X + AppDelegate.ScreenWidth / 2 - textBox.ImgW / 2);
			textBox.ImgY = (float)(ContentOffset.Y + AppDelegate.ScreenHeight / 2 - textBox.ImgH / 2 - Button.ButtonSize / 2);

			CGRect frame = new CGRect (textBox.ImgX, textBox.ImgY, textBox.ImgW, textBox.ImgH);
			textBoxComponent = new TextBoxComponent (textBox);

			await textBoxComponent.Load (navigationController, refreshContent);

			// launches the textbox preview
			uiView = textBoxComponent.GetUIView ();
			//uiView.Alpha = .5f;
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
				r = (float)(rnd.NextDouble () / 3);
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
			Picture p1 = pictureComponent.GetPicture ();
			Picture p = new Picture (p1.Image, p1.Thumbnail, Rotation, rec, Profile.CurrentProfile.UserID);
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

