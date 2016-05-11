using System;
using System.Collections.Generic;
using System.Linq;
using Board.Interface;
using Board.Interface.Widgets;
using CoreGraphics;
using MGImageUtilitiesBinding;
using UIKit;

namespace Board.Interface
{
	public class UIBoardScroll : UIScrollView
	{
		public static int BannerHeight = 66, ButtonBarHeight = 45;
		public static int ScrollViewTotalWidthSize = 2600 * 53;
		public static int ScrollViewWidthSize = 2600;

		float virtualLeftBound;
		int leftScreenNumber = 0, rightScreenNumber = 0;

		public UIScrollView ScrollView;
		private UIImageView TopBanner;
		readonly List<Widget> DrawnWidgets;
		EventHandler DragStartsEvent, ScrolledEvent, DecelerationStartsEvent, DecelerationEndsEvent;
		EventHandler<WillEndDraggingEventArgs> WillEndDragEvent;
		EventHandler<DraggingEventArgs> DragEndsEvent;
		UIInfoBox infoBox;
		private float TempContentOffset;
		public int LastScreen;
		public bool IsHighlighting;
		private bool IsDragging, ShouldOpenEyes;

		public float VirtualLeftBound{
			get { 
				return virtualLeftBound;
			}	
		}

		public int LeftScreenNumber{
			get{ 
				int leftscreen = leftScreenNumber;
				if (leftScreenNumber == rightScreenNumber) {
					leftscreen--;
				}
				return leftscreen;
			}
		}

		public int CurrentLeftScreenNumber{
			get {
				return leftScreenNumber;
			}
		}

		public int CurrentRightScreenNumber{
			get {
				return rightScreenNumber;
			}
		}

		public int RightScreenNumber{
			get{ 
				int rightscreen = rightScreenNumber;
				if (rightScreenNumber == leftScreenNumber) {
					rightscreen++;
				}
				return rightscreen;
			}
		}

		public enum Tags : byte { 
			Background = 1, Content
		};

		public UIBoardScroll ()
		{
			DrawnWidgets = new List<Widget> ();
			GenerateScrollViews ();

			LoadBackground ();

			infoBox = new UIInfoBox (UIBoardInterface.board);
			ScrollView.AddSubview (infoBox);

			SuscribeToEvents ();
		}

		private void GenerateScrollViews()
		{
			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));

			ScrollView.Bounces = false;

			bool isSnapping = false;
			CGPoint target = new CGPoint();

			ScrolledEvent = (sender, e) => {
				
				if (isSnapping){
					
					if (ScrollView.ContentOffset == target || IsDragging){
						isSnapping = false;
					}

				} else if (!IsHighlighting) {
					
					if (Math.Abs ((ScrollView.ContentOffset.X + Frame.Width / 2) - infoBox.Center.X) < 100)
					{
						target = new CGPoint(infoBox.Center.X - infoBox.Frame.Width / 2 - UIInfoBox.XMargin, 0);
						ScrollView.SetContentOffset (target, true);
						isSnapping = true;
					}

				}

				SelectiveRendering();
			};

			DragStartsEvent = (delegate {
				IsDragging = true;
				IsHighlighting = true;
				ShouldOpenEyes = true;
			});

			DragEndsEvent = (delegate { 
				IsDragging = false;
				IsHighlighting = false;
				ShouldOpenEyes = false;
			});

			DecelerationStartsEvent = (delegate {
				ShouldOpenEyes = true;
			});

			DecelerationEndsEvent = (delegate {
				ShouldOpenEyes = false;	
			});

			WillEndDragEvent = (delegate {
				ShouldOpenEyes = true;	
			});

			Frame = new CGRect (0, 0, ScrollViewTotalWidthSize, AppDelegate.ScreenHeight);

			LoadMainLogo ();

			AddSubview (ScrollView); 

			ScrollView.SetNeedsDisplay ();
		}

		private void LoadBackground()
		{	
			ScrollView.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);

			// this limits the size of the scrollview
			ScrollView.ContentSize = new CGSize(ScrollViewTotalWidthSize, AppDelegate.ScreenHeight);
			// sets the scrollview on the middle of the view
			ScrollView.SetContentOffset (new CGPoint(ScrollViewTotalWidthSize / 2 - AppDelegate.ScreenWidth/2, 0), false);

			MaximumZoomScale = 1f;
			MinimumZoomScale = .15f;

			RemoveGestureRecognizer (PinchGestureRecognizer);
		}

		private void LoadMainLogo()
		{
			TopBanner = new UIImageView (new CGRect(0, 0, AppDelegate.ScreenWidth, BannerHeight));
			TopBanner.BackgroundColor = UIColor.White;

			float autosize = (float)TopBanner.Frame.Height - 25;

			UIImage logo = UIBoardInterface.board.Image;
			UIImage logoImage = logo.ImageScaledToFitSize  (new CGSize (autosize, autosize));
			UIImageView mainLogo = new UIImageView(logoImage);
			mainLogo.Center = new CGPoint (TopBanner.Frame.Width / 2, TopBanner.Frame.Height / 2 + 10);

			TopBanner.Tag = (int)Tags.Background;

			TopBanner.AddSubview (mainLogo);

			//AddSubview (TopBanner);
		}

		public void SelectiveRendering(){
			
			// clusterfuck
			float physicalRightBound = (float)(ScrollView.ContentOffset.X + AppDelegate.ScreenWidth);
			float physicalLeftBound = (float)(ScrollView.ContentOffset.X);

			rightScreenNumber = (int)Math.Floor((physicalRightBound) / ScrollViewWidthSize);
			float virtualRightBound = physicalRightBound - ScrollViewWidthSize * rightScreenNumber;
			leftScreenNumber = (int)Math.Floor ((physicalLeftBound) / ScrollViewWidthSize);
			virtualLeftBound = physicalLeftBound - ScrollViewWidthSize * leftScreenNumber;

			virtualRightBound = (virtualRightBound > 0) ? virtualRightBound : 0;
			virtualLeftBound = (virtualLeftBound > 0) ? virtualLeftBound : 0;

			if (!(UIBoardInterface.DictionaryWidgets == null || UIBoardInterface.DictionaryWidgets.Count == 0))
			{
				List<Widget> WidgetsToDraw = UIBoardInterface.DictionaryWidgets.Values.ToList ().FindAll (item =>
					((item.content.Center.X - item.View.Frame.Width / 2) > (virtualLeftBound - AppDelegate.ScreenWidth) &&
                     (item.content.Center.X - item.View.Frame.Width / 2 < (virtualLeftBound + AppDelegate.ScreenWidth))) ||
					((item.content.Center.X + item.View.Frame.Width / 2) < (virtualRightBound + AppDelegate.ScreenWidth) &&
					 (item.content.Center.X + item.View.Frame.Width / 2 > (virtualRightBound - AppDelegate.ScreenWidth))));

				rightScreenNumber = (int)Math.Floor((physicalRightBound + AppDelegate.ScreenWidth) / ScrollViewWidthSize);
				virtualRightBound = physicalRightBound + AppDelegate.ScreenWidth - ScrollViewWidthSize * rightScreenNumber;
				leftScreenNumber = (int)Math.Floor ((physicalLeftBound - AppDelegate.ScreenWidth) / ScrollViewWidthSize);
				virtualLeftBound =  physicalLeftBound - AppDelegate.ScreenWidth - ScrollViewWidthSize * leftScreenNumber;

				// takes wids that are close
				DrawWidgets(WidgetsToDraw, virtualLeftBound, virtualRightBound, leftScreenNumber, rightScreenNumber);

				// the ones at the left ;; the ones at the right ;; if it doesnt have an open eye
				// checks only on the wids that are drawn

				rightScreenNumber = (int)Math.Floor((physicalRightBound) / ScrollViewWidthSize);
				leftScreenNumber = (int)Math.Floor ((physicalLeftBound) / ScrollViewWidthSize);
				virtualRightBound = physicalRightBound - ScrollViewWidthSize * rightScreenNumber;
				virtualLeftBound = physicalLeftBound - ScrollViewWidthSize * leftScreenNumber;

				if (ShouldOpenEyes) {
					List<Widget> WidgetsToOpenEyes = WidgetsToDraw.FindAll (item => !item.EyeOpen &&
					                                ((item.content.Center.X - item.View.Frame.Width / 2 > virtualLeftBound &&
					                                item.content.Center.X - item.View.Frame.Width / 2 < virtualLeftBound + AppDelegate.ScreenWidth &&
					                                item.content.Center.X + item.View.Frame.Width / 2 < virtualRightBound &&
					                                item.content.Center.X + item.View.Frame.Width / 2 > virtualRightBound - AppDelegate.ScreenWidth &&
					                                leftScreenNumber == rightScreenNumber) || (leftScreenNumber != rightScreenNumber &&
					                                (item.content.Center.X + item.View.Frame.Width / 2 < virtualRightBound ||
					                                item.content.Center.X - item.View.Frame.Width / 2 > virtualLeftBound))));
			
			
					if (WidgetsToOpenEyes != null && WidgetsToOpenEyes.Count > 0) {
						foreach (var wid in WidgetsToOpenEyes) {
							wid.OpenEye ();
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

		public void ZoomScrollview() {
			AddSubview (TopBanner);
			SetZoomScale(1f, true);
			ScrollView.Frame = new CGRect(0, 0, AppDelegate.ScreenWidth, ScrollView.Frame.Height);
			ScrollView.SetContentOffset (new CGPoint (TempContentOffset, 0), false);
			SendSubviewToBack (TopBanner);
		}

		// TODO: change this to work with transform, probably even kill ZoomingScrollView (!)
		public void UnzoomScrollview()
		{
			TopBanner.RemoveFromSuperview ();
			TempContentOffset = (float)ScrollView.ContentOffset.X;
			ScrollView.Frame = new CGRect(0, AppDelegate.ScreenHeight/2 - 70, ScrollViewWidthSize, ScrollView.Frame.Height);
			SetZoomScale(.15f, true);
		}

		private void SuscribeToEvents()
		{
			ScrollView.Scrolled += ScrolledEvent;
			ScrollView.DraggingStarted += DragStartsEvent;
			ScrollView.DraggingEnded += DragEndsEvent;
			ScrollView.DecelerationStarted += DecelerationStartsEvent;
			ScrollView.DecelerationEnded += DecelerationEndsEvent;
			ScrollView.WillEndDragging += WillEndDragEvent;

			ViewForZoomingInScrollView += sv => Subviews [0];
		}

		private void UnsuscribeToEvents()
		{
			ScrollView.Scrolled -= ScrolledEvent;

			ScrollView.DraggingStarted -= DragStartsEvent;
			ScrollView.DraggingEnded -= DragEndsEvent;
			ViewForZoomingInScrollView -= sv => Subviews [0];

			ScrollView.DecelerationStarted -= DecelerationStartsEvent;
			ScrollView.DecelerationEnded -= DecelerationEndsEvent;
			ScrollView.WillEndDragging -= WillEndDragEvent;
		}

	}
}

