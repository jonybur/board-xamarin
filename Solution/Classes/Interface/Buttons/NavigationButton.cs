using System.Drawing;
using Board.Interface.Widgets;
using CoreGraphics;
using System;
using System.Linq;
using System.Collections.Generic;
using UIKit;

namespace Board.Interface.Buttons
{
	public class NavigationButton : Button
	{
		private static UILabel numberLabel;
		public static Widget HighlightedWidget;
		private static int contentAmmount;

		private void SetImage (string buttonName)
		{
			using (UIImage image = UIImage.FromFile ("./boardinterface/strokebuttons/" + buttonName + ".png")) {
				uiButton.SetImage (image, UIControlState.Normal);
			}
		}

		public NavigationButton ()
		{
			int highlitedContent = 0;

			uiButton = new UIButton (UIButtonType.Custom);

			SetImage ("closedeye");

			uiButton.Frame = new CGRect (0, 0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint (AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight - ButtonSize / 2);

			// deprecated, not used
			UITapGestureRecognizer doubletap = new UITapGestureRecognizer (tg => {
				tg.NumberOfTapsRequired = 2;

				//AppDelegate.boardInterface.BoardScroll.SetContentOffset(new CGPoint(BoardInterface.ScrollViewWidthSize / 2 - AppDelegate.ScreenWidth / 2, 0), true);
			});

			UITapGestureRecognizer tapGesture= new UITapGestureRecognizer (tg => {
				tg.NumberOfTapsRequired = 1;

				if (UIBoardInterface.DictionaryWidgets == null)
				{ return; }

				if (UIBoardInterface.DictionaryWidgets.Count == 0)
				{ return; }


				if (highlitedContent >= UIBoardInterface.DictionaryWidgets.Count) {
					highlitedContent = 0;
				}

				List<Widget> NavigationList = UIBoardInterface.DictionaryWidgets.Values.OrderBy(o=>o.EyeOpen).ToList();
				if(!NavigationList[0].EyeOpen) {
					highlitedContent = 0;
				}

				PointF position = new PointF(0,0);

				Widget widget = NavigationList[highlitedContent];
				if (!widget.EyeOpen) {
					widget.OpenEye();
				}

				var boardScroll = AppDelegate.BoardInterface.BoardScroll;
				var scrollOffset = boardScroll.VirtualLeftBound + AppDelegate.ScreenWidth / 2;
				var widgetPosition = widget.content.Center.X - AppDelegate.ScreenWidth / 2;

				int rightScreenNumber, leftScreenNumber, rightCurrentScreenNumber, leftCurrentScreenNumber;
				leftScreenNumber = boardScroll.LeftScreenNumber;
				rightScreenNumber = boardScroll.RightScreenNumber;
				rightCurrentScreenNumber = boardScroll.CurrentRightScreenNumber;
				leftCurrentScreenNumber = boardScroll.CurrentLeftScreenNumber;

				float scrollViewOffsetToPan = UIBoardScroll.ScrollViewWidthSize;

				// el offset está en la mitad derecha del scroll
				if (scrollOffset > 1300){
					
					if (widgetPosition > 1300){
						// los dos a la derecha
						// mismo board
						scrollViewOffsetToPan = scrollViewOffsetToPan * leftCurrentScreenNumber;
					} else {
						// widget está del otro lado del infobox
						// board derecho
						scrollViewOffsetToPan = scrollViewOffsetToPan * rightScreenNumber;
					}

				} else {

					if (widgetPosition > 1300){
						// widget está del otro lado del infobox 
						// board izquierdo
						scrollViewOffsetToPan = scrollViewOffsetToPan * leftScreenNumber;
					} else {
						// los dos a la izquierda
						// mismo board
						scrollViewOffsetToPan = scrollViewOffsetToPan * rightCurrentScreenNumber;
					}

				}

				position = new PointF ((float)(widgetPosition + scrollViewOffsetToPan), 0f);

				// unhighlights the one highlighted
				if (HighlightedWidget != null) {
					HighlightedWidget.Unhighlight();
				}

				// highlights the new widget
				widget.Highlight();
				HighlightedWidget = widget;
				highlitedContent++;

				boardScroll.IsHighlighting = true;
				boardScroll.ScrollView.SetContentOffset (position, true); 
			});

			// unzooms
			UILongPressGestureRecognizer longPressGesture = new UILongPressGestureRecognizer ((tg) => {
				/*if (tg.State == UIGestureRecognizerState.Began) {
					BoardInterface.UnzoomScrollview();
				}
				else if (tg.State == UIGestureRecognizerState.Ended) {
					BoardInterface.ZoomScrollview();
				}*/
			});

			gestureRecognizers.Add (tapGesture);
			//gestureRecognizers.Add (longPressGesture);

			uiButton.UserInteractionEnabled = true;
		}

		public void SubtractNavigationButtonText()
		{
			// kills the current navText
			if (numberLabel != null) {
				numberLabel.RemoveFromSuperview ();
				numberLabel.Dispose ();
			}	

			contentAmmount--;

			// if content is 0 then open eye
			if (contentAmmount <= 0) {
				SetImage ("openeye");
				return;
			}

			// otherwise, instanciate new text

			UIFont font = AppDelegate.Narwhal12;
			font = UIFont.SystemFontOfSize (10);
			numberLabel = new UILabel (new CGRect (0, ButtonSize / 2 + 10, ButtonSize, 10));
			numberLabel.Font = font;
			numberLabel.TextAlignment = UITextAlignment.Center;
			numberLabel.Text = contentAmmount.ToString () + " NEW";
			numberLabel.AdjustsFontSizeToFitWidth = true;
			numberLabel.TextColor = UIColor.Black;

			uiButton.AddSubview (numberLabel);
		}

		public void RefreshNavigationButtonText(int content)
		{
			// kills the current navText
			if (numberLabel != null) {
				numberLabel.RemoveFromSuperview ();
				numberLabel.Dispose ();
			}	

			// if content is 0 then open eye
			if (content <= 0) {
				SetImage ("openeye");
				return;
			}

			// otherwise, instanciate new text

			UIFont font = AppDelegate.Narwhal12;
			font = UIFont.SystemFontOfSize (10);
			contentAmmount = content;			
			numberLabel = new UILabel (new CGRect (0, ButtonSize / 2 + 10, ButtonSize, 10));
			numberLabel.Font = font;
			numberLabel.TextAlignment = UITextAlignment.Center;
			numberLabel.Text = content.ToString () + " NEW";
			numberLabel.AdjustsFontSizeToFitWidth = true;
			numberLabel.TextColor = UIColor.Black;

			uiButton.AddSubview (numberLabel);
		}
	}
}

