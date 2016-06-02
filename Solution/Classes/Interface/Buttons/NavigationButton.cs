using System.Drawing;
using Board.Interface.Widgets;
using CoreGraphics;
using System;
using System.Linq;
using System.Collections.Generic;
using UIKit;

namespace Board.Interface.Buttons
{
	public class NavigationButton : BIButton
	{
		private static UILabel numberLabel;
		public static Widget HighlightedWidget;
		private static int contentAmmount;

		private void SetImage (string buttonName)
		{
			using (UIImage image = UIImage.FromFile ("./boardinterface/strokebuttons/" + buttonName + ".png")) {
				SetImage (image, UIControlState.Normal);
			}
		}

		public NavigationButton ()
		{
			int highlitedContent = 0;

			SetImage ("closedeye");

			Frame = new CGRect (0, 0, ButtonSize, ButtonSize);
			Center = new CGPoint (AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight - ButtonSize / 2);

			var tapGesture= new UITapGestureRecognizer (tg => {
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
				var widgetPosition = widget.content.Center.X - AppDelegate.ScreenWidth / 2;

				int rightScreenNumber, leftScreenNumber, rightCurrentScreenNumber, leftCurrentScreenNumber;
				leftScreenNumber = boardScroll.LeftScreenNumber;
				rightScreenNumber = boardScroll.RightScreenNumber;
				rightCurrentScreenNumber = boardScroll.CurrentRightScreenNumber;
				leftCurrentScreenNumber = boardScroll.CurrentLeftScreenNumber;

				var scrollViewOffsetToPan = UIBoardScroll.ScrollViewWidthSize * leftCurrentScreenNumber;

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
			var longPressGesture = new UILongPressGestureRecognizer ((tg) => {
				/*if (tg.State == UIGestureRecognizerState.Began) {
					BoardInterface.UnzoomScrollview();
				}
				else if (tg.State == UIGestureRecognizerState.Ended) {
					BoardInterface.ZoomScrollview();
				}*/
			});

			gestureRecognizers.Add (tapGesture);
			//gestureRecognizers.Add (longPressGesture);

			UserInteractionEnabled = true;
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
			numberLabel.Text = contentAmmount + " NEW";
			numberLabel.AdjustsFontSizeToFitWidth = true;
			numberLabel.TextColor = UIColor.Black;

			AddSubview (numberLabel);
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
			numberLabel.Text = content + " NEW";
			numberLabel.AdjustsFontSizeToFitWidth = true;
			numberLabel.TextColor = UIColor.Black;

			AddSubview (numberLabel);
		}
	}
}

