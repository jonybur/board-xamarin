using System;
using System.Collections.Generic;
using System.Linq;
using BigTed;
using Foundation;
using Board.Interface;
using System.Threading;
using Board.Interface.Buttons;
using Board.Interface.Widgets;
using Board.Utilities;
using Board.Schema;
using MGImageUtilitiesBinding;
using CoreGraphics;
using Facebook.CoreKit;
using UIKit;

namespace Board
{
	public class UIBoardScroll : UIScrollView
	{
		public UIScrollView ScrollView;
		private UIImageView CenterLogo, TopBanner;

		public static int BannerHeight = 66;
		public static int ButtonBarHeight = 45;
		private float TempContentOffset; 

		public enum Tags : byte { Background = 1, Content };

		public static int ScrollViewWidthSize = 2600;
		List<Widget> DrawnWidgets;
		EventHandler ScrolledEvent;

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

			ScrolledEvent = (sender, e) => {
				// call from here "open eye" function
				//InfiniteScroll();

				//GetSpeed();

				SelectiveRendering();
			};

			Frame = new CGRect (0, 0, ScrollViewWidthSize, AppDelegate.ScreenHeight);

			LoadMainLogo ();

			AddSubview (ScrollView); 
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

		private void SuscribeToEvents()
		{
			ScrollView.Scrolled += ScrolledEvent;
			ViewForZoomingInScrollView += sv => Subviews [0];
		}

		private void UnsuscribeToEvents()
		{
			ScrollView.Scrolled -= ScrolledEvent;
			ViewForZoomingInScrollView -= sv => Subviews [0];
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

		private void OpenEye(Widget widget)
		{
			widget.OpenEye();
			ButtonInterface.navigationButton.SubtractNavigationButtonText();
		}

		public void DrawWidgets(List<Widget> WidgetsToDraw)
		{
			List<Widget> WidgetsToRemove = DrawnWidgets.FindAll (item => !WidgetsToDraw.Contains (item));

			foreach (var wid in WidgetsToRemove) {
				wid.View.RemoveFromSuperview ();
				DrawnWidgets.Remove (wid);
			}

			foreach (var wid in WidgetsToDraw) {
				// if the widget hasnt been drawn
				if (wid.View.Superview != ScrollView) {
					// draw it!
					ScrollView.AddSubview (wid.View);
					DrawnWidgets.Add (wid);
				}
			}
		}

	}
}

