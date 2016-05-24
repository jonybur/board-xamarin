using System;
using System.Collections.Generic;
using System.Linq;
using Board.Interface;
using Board.Interface.Widgets;
using Board.Screens;
using CoreGraphics;
using UIKit;
using Board.Schema;

namespace Board.Interface
{
	public class UIBoardScroll : UIScrollView
	{
		public static int BannerHeight = 66, ButtonBarHeight = 45;
		public static float ScrollViewTotalWidthSize = 2600 * 54;
		public static float ScrollViewWidthSize = 2600;

		float virtualLeftBound;
		int leftScreenNumber, rightScreenNumber;

		sealed class UIBackButton : UIButton {
			public UIBackButton(){
				Frame = new CGRect(0, 0, 70, 60);

				using (var image = UIImage.FromFile ("./boardinterface/back_button3.png")) {
					var imageView = new UIImageView ();
					imageView.Frame = new CGRect (0, 0, image.Size.Width / 2, image.Size.Height / 2);
					imageView.Image = image;
					imageView.Center = new CGPoint (Frame.Width / 2 + 5, Frame.Height / 2 + 10);

					AddSubview (imageView);
				}

				bool blockButton = false;
				TouchUpInside += (sender, e) => {
					if (!blockButton){
						var containerScreen = AppDelegate.NavigationController.ViewControllers[AppDelegate.NavigationController.ViewControllers.Length - 2] as ContainerScreen;
						if (containerScreen!= null) {
							containerScreen.LoadMainMenu();
						}
						AppDelegate.PopViewControllerWithCallback(AppDelegate.ExitBoardInterface);
						blockButton = true;
					}
				};
			}
		}

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

		private void CalculateBoardSize(){
			ScrollViewWidthSize = 2600;

			var centerOfScreen = (ScrollViewWidthSize + AppDelegate.ScreenWidth) / 2;
			var orderedContent = UIBoardInterface.DictionaryContent.Values.ToList ().OrderByDescending (x => x.Center.X).ToList();

			if (orderedContent.Count == 0) {
				ScrollViewWidthSize = AppDelegate.ScreenWidth * 2;
				ScrollViewTotalWidthSize = ScrollViewWidthSize * 100;
				return;
			}

			if (orderedContent.Count == 1) {
				var contentX = orderedContent [0].CenterX;
				if (contentX < AppDelegate.ScreenWidth * 1.5f) {
					ScrollViewWidthSize = AppDelegate.ScreenWidth * 2;
					ScrollViewTotalWidthSize = ScrollViewWidthSize * 100;
					return;
				}

				orderedContent [0].Center = new CGPoint(AppDelegate.ScreenWidth, orderedContent [0].Center.Y);
				ScrollViewWidthSize = AppDelegate.ScreenWidth * 2;
				ScrollViewTotalWidthSize = ScrollViewWidthSize * 100;
				return;
			}

			float lastRightContentX = 0, lastLeftContentX = 0;
			foreach (var content in orderedContent) {
				if (content.Center.X > centerOfScreen) {
					lastLeftContentX = content.CenterX;
				} else {
					lastRightContentX = content.CenterX;
					break;
				}
			}

			var awidth = (lastLeftContentX - lastRightContentX);

			// x2 - x1 = a

			foreach (var content in orderedContent) {
				if (content.Center.X >= lastLeftContentX) {
					content.Center = new CGPoint (content.Center.X - awidth + AppDelegate.ScreenWidth, content.Center.Y);
				}
			}

			ScrollViewWidthSize -= (float)awidth;
			ScrollViewWidthSize += AppDelegate.ScreenWidth;
			ScrollViewTotalWidthSize = ScrollViewWidthSize * 54;

			// x obj a la izq - a = nueva pos de obj a la izq
			// ancho del board = 2600 - a 
		}

		public CGPoint ConvertPointToBoardScrollPoint(CGPoint center){
			return new CGPoint (center.X - LastScreen * UIBoardScroll.ScrollViewWidthSize, center.Y);
		}

		private void GenerateScrollViews()
		{
			if (!UIBoardInterface.UserCanEditBoard) {
				CalculateBoardSize ();
			}

			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));

			ScrollView.ShowsHorizontalScrollIndicator = false;
			ScrollView.ShowsVerticalScrollIndicator = false;
			ShowsHorizontalScrollIndicator = false;
			ShowsVerticalScrollIndicator = false;

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

			AddSubview (ScrollView);

			var backButton = new UIBackButton ();
			AddSubview (backButton);

			ScrollView.SetNeedsDisplay ();
		}

		private void LoadBackground()
		{	
			ScrollView.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);

			// this limits the size of the scrollview
			ScrollView.ContentSize = new CGSize(ScrollViewTotalWidthSize, AppDelegate.ScreenHeight);
			// sets the scrollview on the middle of the view
			ScrollView.SetContentOffset (new CGPoint(ScrollViewTotalWidthSize / 2 + AppDelegate.ScreenWidth / 2 - UIInfoBox.XMargin, 0), false);

			MaximumZoomScale = 1f;
			MinimumZoomScale = .15f;

			RemoveGestureRecognizer (PinchGestureRecognizer);
		}

		// TODO: always render widgets that are being edited.
		// TODO: fix stutter on scroll limit
		public void SelectiveRendering(){
			
			// clusterfuck
			float physicalRightBound = (float)(ScrollView.ContentOffset.X + AppDelegate.ScreenWidth / 2);
			float physicalLeftBound = (float)(ScrollView.ContentOffset.X - AppDelegate.ScreenWidth / 2);

			rightScreenNumber = (int)Math.Floor((physicalRightBound) / ScrollViewWidthSize);
			float virtualRightBound = physicalRightBound - ScrollViewWidthSize * rightScreenNumber;
			leftScreenNumber = (int)Math.Floor ((physicalLeftBound) / ScrollViewWidthSize);
			virtualLeftBound = physicalLeftBound - ScrollViewWidthSize * leftScreenNumber;

			virtualRightBound = (virtualRightBound > 0) ? virtualRightBound : 0;
			virtualLeftBound = (virtualLeftBound > 0) ? virtualLeftBound : 0;

			if (!(UIBoardInterface.DictionaryWidgets == null || UIBoardInterface.DictionaryWidgets.Count == 0))
			{
				List<Widget> WidgetsToDraw = UIBoardInterface.DictionaryWidgets.Values.ToList ().FindAll (item =>
					((item.content.Center.X - item.Frame.Width / 2) > (virtualLeftBound - AppDelegate.ScreenWidth * 1.5f) &&
                     (item.content.Center.X - item.Frame.Width / 2 < (virtualLeftBound + AppDelegate.ScreenWidth * 1.5f))) ||
					((item.content.Center.X + item.Frame.Width / 2) < (virtualRightBound + AppDelegate.ScreenWidth * 1.5f) &&
					 (item.content.Center.X + item.Frame.Width / 2 > (virtualRightBound - AppDelegate.ScreenWidth * 1.5f))));

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
					                                ((item.content.Center.X - item.Frame.Width / 2 > virtualLeftBound &&
					                                item.content.Center.X - item.Frame.Width / 2 < virtualLeftBound + AppDelegate.ScreenWidth &&
					                                item.content.Center.X + item.Frame.Width / 2 < virtualRightBound &&
					                                item.content.Center.X + item.Frame.Width / 2 > virtualRightBound - AppDelegate.ScreenWidth &&
					                                leftScreenNumber == rightScreenNumber) || (leftScreenNumber != rightScreenNumber &&
					                                (item.content.Center.X + item.Frame.Width / 2 < virtualRightBound ||
					                                item.content.Center.X - item.Frame.Width / 2 > virtualLeftBound))));
			
			
					if (WidgetsToOpenEyes != null && WidgetsToOpenEyes.Count > 0) {
						foreach (var wid in WidgetsToOpenEyes) {
							wid.OpenEye ();
						}
					}
				}
			}

			// manages topbanner opacity
			//float leftScroll = (ScrollViewWidthSize * leftScreenNumber);
			//float midScroll = leftScroll + ScrollViewWidthSize / 2;
			//float midScreen = (float)ScrollView.ContentOffset.X + AppDelegate.ScreenWidth / 2;
			//float absoluteDifference = Math.Abs (midScreen - midScroll);

			/*if (absoluteDifference > AppDelegate.ScreenWidth / 2) {
				float distance = Math.Abs (absoluteDifference - AppDelegate.ScreenWidth / 2);
				float alpha = (100 - distance) / 100;

				TopBanner.Alpha = alpha;
			} else {
				TopBanner.Alpha = 1f;
			}*/

			// manages infobox 'teleportation'
			if (rightScreenNumber != LastScreen) {
				
				LastScreen = rightScreenNumber;
				infoBox.Center = new CGPoint ((ScrollViewWidthSize * rightScreenNumber) + infoBox.Frame.Width + UIInfoBox.XMargin, infoBox.Center.Y);
			}
		}

		public void DrawWidgets(List<Widget> widgetsToDraw, float virtualLeftBound, float virtualRightBound, float leftScreenNumber, float rightScreenNumber) {
			
			List<Widget> WidgetsToRemove = DrawnWidgets.FindAll (item => !widgetsToDraw.Contains (item));

			foreach (var wid in WidgetsToRemove) {
				wid.RemoveFromSuperview ();
				DrawnWidgets.Remove (wid);
			}

			foreach (var wid in widgetsToDraw) {
				// if the widget hasnt been drawn
				if (wid.Superview != ScrollView) {

					if (rightScreenNumber != leftScreenNumber) {

						if (virtualLeftBound < wid.content.Center.X + wid.Frame.Width / 2) {
							wid.SetTransforms (ScrollViewWidthSize * leftScreenNumber);
						} else if (wid.content.Center.X - wid.Frame.Width / 2 < virtualRightBound) {
							wid.SetTransforms (ScrollViewWidthSize * rightScreenNumber);
						}
							
					} else {
						// da igual, imprimo en la pantalla
						wid.SetTransforms (ScrollViewWidthSize * rightScreenNumber);
					}

					//wid.SetTransforms (2600);
					ScrollView.AddSubview (wid);
					DrawnWidgets.Add (wid);
				}
			}
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

