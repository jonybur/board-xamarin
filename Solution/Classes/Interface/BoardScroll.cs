using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Board.Interface;
using Board.Interface.Buttons;
using Board.Interface.Widgets;
using Foundation;
using CoreGraphics;
using CoreAnimation;
using MGImageUtilitiesBinding;
using UIKit;

namespace Board.Interface
{
	public class UIBoardScroll : UIScrollView
	{
		public UIScrollView ScrollView;
		private UIImageView TopBanner;

		public static int BannerHeight = 66;
		public static int ButtonBarHeight = 45;
		private float TempContentOffset; 
		bool isDragging;

		public enum Tags : byte { Background = 1, Content };

		public static int ScrollViewWidthSize = 2600;
		readonly List<Widget> DrawnWidgets;
		EventHandler ScrolledEvent, DragStartsEvent;
		EventHandler<DraggingEventArgs> DragEndsEvent;
		Thread scrolls;

		public UIBoardScroll ()
		{
			DrawnWidgets = new List<Widget> ();
			GenerateScrollViews ();

			LoadBackground ();

			var infoBox = new InfoBox (BoardInterface.board);
			ScrollView.AddSubview (infoBox);

			SuscribeToEvents ();
		}

		private void GenerateScrollViews()
		{
			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));

			ScrollView.Bounces = false;

			DragStartsEvent = (delegate {
				isDragging = true;
			});

			DragEndsEvent = (delegate { 
				isDragging = false;
			});

			ScrolledEvent = (sender, e) => {

				// call from here "open eye" function
				if (!isanimating){ GetSpeed(); }
			
				InfiniteScroll();

				SelectiveRendering();
			};

			Frame = new CGRect (0, 0, ScrollViewWidthSize, AppDelegate.ScreenHeight);

			LoadMainLogo ();

			AddSubview (ScrollView); 
		}

		DateTime lastOffsetCapture;
		CGPoint lastOffset;
		bool isScrollingFast;
		float scrollSpeed;

		private void GetSpeed(){
			if (!ScrollView.Dragging) {
				return;
			}
			var currentOffset = ScrollView.ContentOffset;

			TimeSpan timeDiff = DateTime.Now.Subtract (lastOffsetCapture);
			if (timeDiff.TotalMilliseconds > 1) {
				float distance = (float)(currentOffset.X - lastOffset.X);

				distance = Math.Abs (distance);
				scrollSpeed = ((float)distance * 15);

				if (scrollSpeed <= 5) {
					scrollSpeed = 0f;
				}
				lastOffset = currentOffset;
				lastOffsetCapture = DateTime.Now;
			}
		}

		private void LoadBackground()
		{	
			ScrollView.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);

			// this limits the size of the scrollview
			ScrollView.ContentSize = new CGSize(ScrollViewWidthSize, AppDelegate.ScreenHeight);
			// sets the scrollview on the middle of the view
			ScrollView.SetContentOffset (new CGPoint(ScrollViewWidthSize/2 - AppDelegate.ScreenWidth/2, 0), false);

			MaximumZoomScale = 1f;
			MinimumZoomScale = .15f;

			RemoveGestureRecognizer (PinchGestureRecognizer);
		}

		private void LoadMainLogo()
		{
			TopBanner = new UIImageView (new CGRect(0, 0, AppDelegate.ScreenWidth, BannerHeight));
			TopBanner.BackgroundColor = UIColor.White;

			float autosize = (float)TopBanner.Frame.Height - 25;

			UIImage logo = BoardInterface.board.ImageView.Image;
			UIImage logoImage = logo.ImageScaledToFitSize  (new CGSize (autosize, autosize));
			UIImageView mainLogo = new UIImageView(logoImage);
			mainLogo.Center = new CGPoint (TopBanner.Frame.Width / 2, TopBanner.Frame.Height / 2 + 10);

			TopBanner.Tag = (int)Tags.Background;

			TopBanner.AddSubview (mainLogo);

			AddSubview (TopBanner);
		}

		public void SelectiveRendering(){
			if (!(BoardInterface.DictionaryWidgets == null || BoardInterface.DictionaryWidgets.Count == 0))
			{
				List<Widget> WidgetsToDraw = BoardInterface.DictionaryWidgets.Values.ToList().FindAll(item => ((item.View.Frame.X) > (ScrollView.ContentOffset.X - AppDelegate.ScreenWidth)) &&
					((item.View.Frame.X + item.View.Frame.Width) < (ScrollView.ContentOffset.X + AppDelegate.ScreenWidth + AppDelegate.ScreenWidth)));

				// takes wids that are close
				DrawWidgets(WidgetsToDraw);

				if (!ScrollView.Dragging) {

					// the ones at the left ;; the ones at the right ;; if it doesnt have an open eye
					// checks only on the wids that are drawn
					List<Widget> WidgetsToOpenEyes = WidgetsToDraw.FindAll (item => ((item.View.Frame.X) > ScrollView.ContentOffset.X) &&
						((item.View.Frame.X + item.View.Frame.Width) < (ScrollView.ContentOffset.X + AppDelegate.ScreenWidth)) &&
						!item.EyeOpen);

					if (WidgetsToOpenEyes != null && WidgetsToOpenEyes.Count > 0) {
						foreach (var wid in WidgetsToOpenEyes) {
							OpenEye (wid);
						}
					}

				}
			}

			float midScroll = ScrollViewWidthSize / 2;
			float midScreen = (float)ScrollView.ContentOffset.X + AppDelegate.ScreenWidth / 2;
			float absoluteDifference = Math.Abs (midScreen - midScroll);

			if (absoluteDifference > AppDelegate.ScreenWidth / 2) {
				float distance = Math.Abs (absoluteDifference - AppDelegate.ScreenWidth / 2);
				float alpha = (100 - distance) / 100;

				TopBanner.Alpha = alpha;
			} else {
				TopBanner.Alpha = 1f;
			}
		}

		bool isanimating = false;
		private void InfiniteScroll(){
			float rightBound = (float)(ScrollView.ContentOffset.X + AppDelegate.ScreenWidth);

			if (isanimating) {
				return;	
			}

			if (Frame.X < 0) {
				Frame = new CGRect (frameXAfterAnimation + (ScrollViewWidthSize - rightBound), Frame.Y, Frame.Width, Frame.Height);
				if (0 < Frame.X) {
					Frame = new CGRect (0, Frame.Y, Frame.Width, Frame.Height);
				}
			} else if (rightBound >= ScrollViewWidthSize){// || (Frame.X <0 && isDragging)){
				// should allow moving frame with deaceleration value
				AnimateMovement();
				// ScrollView.Frame = new CGRect(ScrollView.Frame.X - scrollSpeed, ScrollView.Frame.Y, ScrollView.Frame.Width, ScrollView.Frame.Height);

			}/* else if (ScrollView.ContentOffset.X <= 0) {
				// should allow moving frame with deaceleration value
			}*/
		}

		float frameXAfterAnimation;


		/*
transformAnimation.toValue=[NSValue valueWithCATransform3D:CATransform3DMakeScale(scaleFactorX, scaleFactorY, scaleFactorZ)];
transformAnimation.removedOnCompletion = FALSE;

[layer addAnimation:transformAnimation forKey:@"transform"];
			*/



		private void AnimateMovement()
		{
			ScrollView.UserInteractionEnabled = false;
			isanimating = true;
			BackgroundColor = UIColor.Red;

			CATransaction.Begin();

			CATransaction.CompletionBlock = delegate {
				ScrollView.UserInteractionEnabled = true;
				//isanimating = false;
				frameXAfterAnimation = (float)Frame.X;
			};

			//float exRight = (float)Frame.Right;
			CABasicAnimation resizeAnimation =  new CABasicAnimation();
			resizeAnimation.KeyPath = "bounds";
			resizeAnimation.From = NSNumber.FromCGRect(new CGRect(0, 0, Bounds.Width, Bounds.Height));
			resizeAnimation.To = NSNumber.FromCGRect(new CGRect(0, 0, 200, Bounds.Height));

			CABasicAnimation repositionAnimation =  new CABasicAnimation();
			repositionAnimation.KeyPath = "position.x";
			repositionAnimation.To = NSNumber.FromFloat(100);

			CAAnimationGroup animationGroup = new CAAnimationGroup ();
			animationGroup.Duration = ScrollView.DecelerationRate;
			animationGroup.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut);
			animationGroup.RemovedOnCompletion = true;
			animationGroup.Animations = new CAAnimation[]{ resizeAnimation, repositionAnimation };

			Bounds = new CGRect(0, 0, 200, Bounds.Height);
			Frame = new CGRect (0, 0, Bounds.Width, Bounds.Height);

			//ScrollView.SetContentOffset(new CGPoint(ScrollViewWidthSize - Frame.Width, 0), true);

			// TODO: estoy trabajando acá... probar hacer un uiview animate para el width del scrollview también

			UIView.Animate (ScrollView.DecelerationRate, SetContentOffsetWithCustomDuration, () => ScrollView.Frame = Frame);
			Layer.AddAnimation(animationGroup, "allanimations");

			CATransaction.Commit();
		}

		private void SetContentOffsetWithCustomDuration(){
			ScrollView.SetContentOffset (new CGPoint (ScrollView.ContentOffset.X + 300, 0), false);
		}

		/*

		private void AnimateMovement()
		{
			CATransaction.Begin();

			CATransaction.CompletionBlock = delegate {
				isanimating = false;
				frameXAfterAnimation = (float)Frame.X;
			};

			float exRight = (float)Frame.Right;
			Frame = new CGRect(Frame.X - scrollSpeed, Frame.Y, Frame.Width, Frame.Height);
			CABasicAnimation animation =  new CABasicAnimation();
			isanimating = true;
			animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut);
			animation.KeyPath = "position.x";
			animation.From = new NSNumber(exRight - Frame.Width / 2);
			animation.To = new NSNumber(exRight - Frame.Width / 2 - scrollSpeed);
			animation.Duration = ScrollView.DecelerationRate;
			Layer.AddAnimation(animation, "basic");

			CATransaction.Commit();
		}

		*/

		public void DrawWidgets(List<Widget> widgetsToDraw) {
			List<Widget> WidgetsToRemove = DrawnWidgets.FindAll (item => !widgetsToDraw.Contains (item));

			foreach (var wid in WidgetsToRemove) {
				wid.View.RemoveFromSuperview ();
				DrawnWidgets.Remove (wid);
			}

			foreach (var wid in widgetsToDraw) {
				// if the widget hasnt been drawn
				if (wid.View.Superview != ScrollView) {
					// draw it!
					ScrollView.AddSubview (wid.View);
					DrawnWidgets.Add (wid);
				}
			}
		}

		private void OpenEye(Widget widget) {
			widget.OpenEye();
			ButtonInterface.navigationButton.SubtractNavigationButtonText();
		}

		public void ZoomScrollview()
		{
			AddSubview (TopBanner);
			SetZoomScale(1f, true);
			ScrollView.Frame = new CGRect(0, 0, AppDelegate.ScreenWidth, ScrollView.Frame.Height);
			ScrollView.SetContentOffset (new CGPoint (TempContentOffset, 0), false);
			SendSubviewToBack (TopBanner);
		}

		public void UnzoomScrollview()
		{
			TopBanner.RemoveFromSuperview ();

			// TODO: remove hardcode and programatically derive the correct zooming value (.15f is current)
			TempContentOffset = (float)ScrollView.ContentOffset.X;
			ScrollView.Frame = new CGRect(0, AppDelegate.ScreenHeight/2 - 70, ScrollViewWidthSize, ScrollView.Frame.Height);
			SetZoomScale(.15f, true);
		}

		private void SuscribeToEvents()
		{
			ScrollView.Scrolled += ScrolledEvent;
			ScrollView.DraggingStarted += DragStartsEvent;
			ScrollView.DraggingEnded += DragEndsEvent;
			ViewForZoomingInScrollView += sv => Subviews [0];
		}

		private void UnsuscribeToEvents()
		{
			ScrollView.Scrolled -= ScrolledEvent;
			ScrollView.DraggingStarted -= DragStartsEvent;
			ScrollView.DraggingEnded -= DragEndsEvent;
			ViewForZoomingInScrollView -= sv => Subviews [0];
		}

	}
}

