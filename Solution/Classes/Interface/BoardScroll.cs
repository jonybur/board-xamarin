using System;
using System.Collections.Generic;
using System.Linq;
using Board.Interface;
using Board.Interface.Buttons;
using Board.Interface.Widgets;
using CoreGraphics;
using MGImageUtilitiesBinding;
using UIKit;

namespace Board.Interface
{
	public class UIBoardScroll : UIScrollView
	{
		public UIScrollView ScrollView;
		private UIImageView TopBanner;

		public float LastScreen;
		public static int BannerHeight = 66;
		public static int ButtonBarHeight = 45;
		private float TempContentOffset;

		public enum Tags : byte { Background = 1, Content };

		public static int ScrollViewTotalWidthSize = 2600 * 102;
		public static int ScrollViewWidthSize = 2600;
		readonly List<Widget> DrawnWidgets;
		EventHandler DragStartsEvent, ScrolledEvent;
		EventHandler<DraggingEventArgs> DragEndsEvent;
		InfoBox infoBox;
		bool isDragging;

		//GetSpeedAttributes
		DateTime lastOffsetCapture;
		CGPoint lastOffset;
		bool isScrollingFast;
		float scrollSpeed;

		//AnimationAttributes
		bool isAnimating;
		float frameWAfterAnimation;


		public UIBoardScroll ()
		{
			DrawnWidgets = new List<Widget> ();
			GenerateScrollViews ();

			LoadBackground ();

			infoBox = new InfoBox (BoardInterface.board);
			ScrollView.AddSubview (infoBox);

			SuscribeToEvents ();
		}

		private void GenerateScrollViews()
		{
			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			//ScrollView.DecelerationRate = .4f;

			ScrollView.Bounces = false;

			bool isSnapping = false;
			CGPoint target = new CGPoint();

			ScrolledEvent = (sender, e) => {
				
				if (isSnapping){
					if (ScrollView.ContentOffset == target || isDragging){
						isSnapping = false;
					}

				} else if (!isDragging){
					Console.WriteLine("SEP:" + Math.Abs ((ScrollView.ContentOffset.X + Frame.Width / 2) - infoBox.Center.X));
					if (Math.Abs ((ScrollView.ContentOffset.X + Frame.Width / 2) - infoBox.Center.X) < 100)
					{
						target = new CGPoint(infoBox.Center.X - infoBox.Frame.Width / 2 - 10, 0);
						ScrollView.SetContentOffset (target, true);
						isSnapping = true;
					}
				}

				// call from here "open eye" function
				//if (!isAnimating){ GetSpeed(); }
			
				//InfiniteScroll();

				SelectiveRendering();
			};

			DragStartsEvent = (delegate {
				isDragging = true;
			});

			DragEndsEvent = (delegate { 
				isDragging = false;
			});


			/*
			ScrollView.WillEndDragging += (sender, e) => {
				Console.WriteLine(e.TargetContentOffset.X - LastScreen * ScrollViewWidthSize);
				if (Math.Abs ((e.TargetContentOffset.X + Frame.Width / 2) - infoBox.Center.X) > 200)
					{ return; }
					
				var target = new CGPoint(infoBox.Center.X - infoBox.Frame.Width / 2 - 10, 0);
				ScrollView.SetContentOffset (target, true);
			};
			*/

			Frame = new CGRect (0, 0, ScrollViewTotalWidthSize, AppDelegate.ScreenHeight);

			LoadMainLogo ();

			AddSubview (ScrollView); 

			ScrollView.SetNeedsDisplay ();
		}

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
			ScrollView.ContentSize = new CGSize(ScrollViewTotalWidthSize, AppDelegate.ScreenHeight);
			// sets the scrollview on the middle of the view
			ScrollView.SetContentOffset (new CGPoint(ScrollViewTotalWidthSize/2 + ScrollViewWidthSize / 2- AppDelegate.ScreenWidth/2, 0), false);

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
			float leftScreenNumber = 0;

			if (!(BoardInterface.DictionaryWidgets == null || BoardInterface.DictionaryWidgets.Count == 0))
			{
				// clusterfuck
				float physicalRightBound = (float)(ScrollView.ContentOffset.X + AppDelegate.ScreenWidth);
				float physicalLeftBound = (float)(ScrollView.ContentOffset.X);

				float rightScreenNumber = (float)Math.Floor((physicalRightBound) / ScrollViewWidthSize);
				float virtualRightBound = physicalRightBound - ScrollViewWidthSize * rightScreenNumber;
				leftScreenNumber = (float)Math.Floor ((physicalLeftBound) / ScrollViewWidthSize);
				float virtualLeftBound = physicalLeftBound - ScrollViewWidthSize * leftScreenNumber;

				virtualRightBound = (virtualRightBound > 0) ? virtualRightBound : 0;
				virtualLeftBound = (virtualLeftBound > 0) ? virtualLeftBound : 0;

				List<Widget> WidgetsToDraw = BoardInterface.DictionaryWidgets.Values.ToList ().FindAll (item =>
					((item.content.Center.X - item.View.Frame.Width / 2) > (virtualLeftBound - AppDelegate.ScreenWidth) &&
                     (item.content.Center.X - item.View.Frame.Width / 2 < (virtualLeftBound + AppDelegate.ScreenWidth))) ||
					((item.content.Center.X + item.View.Frame.Width / 2) < (virtualRightBound + AppDelegate.ScreenWidth) &&
					 (item.content.Center.X + item.View.Frame.Width / 2 > (virtualRightBound - AppDelegate.ScreenWidth))));

				rightScreenNumber = (float)Math.Floor((physicalRightBound + AppDelegate.ScreenWidth) / ScrollViewWidthSize);
				virtualRightBound = physicalRightBound + AppDelegate.ScreenWidth - ScrollViewWidthSize * rightScreenNumber;
				leftScreenNumber = (float)Math.Floor ((physicalLeftBound - AppDelegate.ScreenWidth) / ScrollViewWidthSize);
				virtualLeftBound =  physicalLeftBound - AppDelegate.ScreenWidth - ScrollViewWidthSize * leftScreenNumber;

				// takes wids that are close
				DrawWidgets(WidgetsToDraw, virtualLeftBound, virtualRightBound, leftScreenNumber, rightScreenNumber);

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

			// manages topbanner opacity
			float midScroll = (ScrollViewWidthSize * leftScreenNumber) + ScrollViewWidthSize / 2;
			float midScreen = (float)ScrollView.ContentOffset.X + AppDelegate.ScreenWidth / 2;
			float absoluteDifference = Math.Abs (midScreen - midScroll);

			if (absoluteDifference > AppDelegate.ScreenWidth / 2) {
				float distance = Math.Abs (absoluteDifference - AppDelegate.ScreenWidth / 2);
				float alpha = (100 - distance) / 100;

				TopBanner.Alpha = alpha;
			} else {
				TopBanner.Alpha = 1f;
			}

			if (leftScreenNumber != LastScreen) {
				LastScreen = leftScreenNumber;
				infoBox.Center = new CGPoint (midScroll, infoBox.Center.Y);
			}
		}

		public void DrawWidgets(List<Widget> widgetsToDraw, float virtualLeftBound, float virtualRightBound, float leftScreenNumber, float rightScreenNumber) {
			
			List<Widget> WidgetsToRemove = DrawnWidgets.FindAll (item => !widgetsToDraw.Contains (item));

			foreach (var wid in WidgetsToRemove) {
				wid.View.RemoveFromSuperview ();
				DrawnWidgets.Remove (wid);
			}

			foreach (var wid in widgetsToDraw) {
				// if the widget hasnt been drawn
				if (wid.View.Superview != ScrollView) {

					if (rightScreenNumber != leftScreenNumber) {

						if (virtualLeftBound < wid.content.Center.X + wid.View.Frame.Width / 2) {
							wid.SetTransforms (ScrollViewWidthSize * leftScreenNumber);
						} else if (wid.content.Center.X - wid.View.Frame.Width / 2 < virtualRightBound) {
							wid.SetTransforms (ScrollViewWidthSize * rightScreenNumber);
						}
							
					} else {
						// da igual, imprimo en la pantalla
						wid.SetTransforms (ScrollViewWidthSize * rightScreenNumber);
					}

					//wid.SetTransforms (2600);
					ScrollView.AddSubview (wid.View);
					DrawnWidgets.Add (wid);
				}
			}
		}

		private void AnimateMovement()
		{
			ScrollView.UserInteractionEnabled = false;
			isAnimating = true;

			UIView.Animate (ScrollView.DecelerationRate, SetContentOffsetWithCustomDuration, SetFinalAnimationValues);
		}

		private void SetContentOffsetWithCustomDuration(){
			Frame = new CGRect (-scrollSpeed, 0, Frame.Width, Frame.Height);
		}

		private void SetFinalAnimationValues(){
			Frame = new CGRect (0, 0, Frame.Width - scrollSpeed, Frame.Height);
			ScrollView.Frame = Frame;
			ScrollView.ContentOffset = new CGPoint (ScrollView.ContentSize.Width - Frame.Width, 0);

			frameWAfterAnimation = (float)Frame.Width;
			ScrollView.UserInteractionEnabled = true;
			isAnimating = false;
		}

		private void OpenEye(Widget widget) {
			widget.OpenEye();
			ButtonInterface.navigationButton.SubtractNavigationButtonText();
		}

		public void ZoomScrollview() {
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

