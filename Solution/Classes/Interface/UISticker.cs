using System;
using CoreGraphics;
using Board.Schema;
using UIKit;

namespace Board.Interface
{
	public static class UIPreviewSticker {
		public static UISticker PreviewSticker;
		public static Content GetContent(){
			return new Sticker ();
		}
	}

	public class UISticker : UITextView
	{
		UIGestureRecognizer panGestureRecognizer, rotationGestureRecognizer, pinchGestureRecognizer;
		CGAffineTransform ReferenceTransform;

		class CustomDelegate: UITextViewDelegate{
			public override bool ShouldChangeText (UITextView textView, Foundation.NSRange range, string text)
			{
				if (text == "\n"){
					textView.ResignFirstResponder ();
					return false;
				}
				return true;
			}

			CGPoint auxCenter;
			CGAffineTransform auxTransform;

			public override void EditingStarted (UITextView textView)
			{
				((UISticker)textView).UnsuscribeToGestures ();
				auxCenter = textView.Center;
				auxTransform = textView.Transform;
				textView.BackgroundColor = UIColor.FromRGBA(0, 0, 0, 100);
				textView.Transform = new CGAffineTransform (1, 0, 0, 1, 0, 0);
				var scrollView = AppDelegate.BoardInterface.BoardScroll.ScrollView;
				scrollView.ScrollEnabled = false;
				textView.Frame = new CGRect (scrollView.ContentOffset.X, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);
			}

			public override void EditingEnded (UITextView textView)
			{
				((UISticker)textView).SuscribeToGestures ();
				textView.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
				textView.SizeToFit ();
				textView.Center = auxCenter;
				textView.Transform = auxTransform;
				var scrollView = AppDelegate.BoardInterface.BoardScroll.ScrollView;
				scrollView.ScrollEnabled = true;
			}
		}

		public UISticker (Sticker content){
			// generates sticker from content
		}

		public UISticker ()
		{
			int fontsize = 50;
			Font = UIFont.SystemFontOfSize (fontsize);

			var size = new CGSize (AppDelegate.ScreenWidth, fontsize * 2);

			var boardScroll = AppDelegate.BoardInterface.BoardScroll;

			Frame = new CGRect (boardScroll.ScrollView.ContentOffset.X + AppDelegate.ScreenWidth / 2 - size.Width / 2,
				boardScroll.ScrollView.ContentOffset.Y + AppDelegate.ScreenHeight / 2 - size.Height / 2 - Board.Interface.Buttons.Button.ButtonSize / 2, size.Width, size.Height);
			
			TextAlignment = UITextAlignment.Center;

			ReturnKeyType = UIReturnKeyType.Done;

			AllowsEditingTextAttributes = true;

			EnablesReturnKeyAutomatically = true;

			Delegate = new CustomDelegate ();

			TextColor = UIColor.White;

			BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);

			AppDelegate.BoardInterface.BoardScroll.ScrollView.AddSubview (this);

			panGestureRecognizer = SetNewPanGestureRecognizer ();
			rotationGestureRecognizer = SetNewRotationGestureRecognizer (false);
			pinchGestureRecognizer = SetNewPinchGestureRecognizer ();

			UserInteractionEnabled = true;

			SuscribeToGestures ();
		}

		public void SuscribeToGestures(){
			AddGestureRecognizer (panGestureRecognizer);
			AddGestureRecognizer (rotationGestureRecognizer);
			AddGestureRecognizer (pinchGestureRecognizer);
		}

		public void UnsuscribeToGestures(){
			RemoveGestureRecognizer (panGestureRecognizer);
			RemoveGestureRecognizer (rotationGestureRecognizer);
			RemoveGestureRecognizer (pinchGestureRecognizer);
		}

		public UIPanGestureRecognizer SetNewPanGestureRecognizer()
		{
			float dx = 0;
			float dy = 0;

			var panGesture = new UIPanGestureRecognizer (pg => {
				if ((pg.State == UIGestureRecognizerState.Began || pg.State == UIGestureRecognizerState.Changed) && (pg.NumberOfTouches == 1)) {

					var p0 = pg.LocationInView(Superview);

					if (dx == 0)
						dx = (float)(p0.X - Center.X);

					if (dy == 0)
						dy = (float)(p0.Y - Center.Y);

					var p1 = new CGPoint (p0.X - dx, p0.Y - dy);

					Center = p1;

				} else if (pg.State == UIGestureRecognizerState.Ended) {
					dx = 0;
					dy = 0;
				}
			});
			return panGesture;
		}

		private UIRotationGestureRecognizer SetNewRotationGestureRecognizer(bool autoRotate)
		{
			var rotateGesture = new UIRotationGestureRecognizer (rg => {
				switch (rg.State) 
				{
					case UIGestureRecognizerState.Began:
						{
							ReferenceTransform = Transform;
							break;
						}
					case UIGestureRecognizerState.Changed:
						{
						float rotate = (float)rg.Rotation * 55 * ((float)Math.PI/180);
						Transform = CGAffineTransform.Rotate(ReferenceTransform, rotate);
							break;
						}

					default:
						break;
				}
			});
			return rotateGesture;
		}

		private UIPinchGestureRecognizer SetNewPinchGestureRecognizer(){
			float scale = 1;

			ReferenceTransform = Transform;

			var pinchGestureRecognizer = new UIPinchGestureRecognizer (pg => {

				switch(pg.State){
				case UIGestureRecognizerState.Began:
					{
						ReferenceTransform = Transform;
						break;
					}
				case UIGestureRecognizerState.Changed:
					{
						CGAffineTransform transform = CGAffineTransform.Scale(ReferenceTransform, pg.Scale, pg.Scale);
						Transform = transform;

						break;
					}

				}
			});

			return pinchGestureRecognizer;
		}

	}
}

