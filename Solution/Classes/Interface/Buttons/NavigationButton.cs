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
		private static Widget highLighted;
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

			UITapGestureRecognizer tapGesture= new UITapGestureRecognizer  (tg => {
				tg.NumberOfTapsRequired = 1;

				if (BoardInterface.DictionaryWidgets == null)
				{ return; }

				if (BoardInterface.DictionaryWidgets.Count == 0)
				{ return; }


				if (highlitedContent >= BoardInterface.DictionaryWidgets.Count) {
					highlitedContent = 0;
				}

				List<Widget> NavigationList = BoardInterface.DictionaryWidgets.Values.OrderBy(o=>o.EyeOpen).ToList();
				if(!NavigationList[0].EyeOpen)
				{
					highlitedContent = 0;
				}

				PointF position = new PointF(0,0);

				Widget widget = NavigationList[highlitedContent];
				if (!widget.EyeOpen)
				{
					widget.OpenEye();
					SubtractNavigationButtonText();
				}

				position = new PointF ((float)(widget.View.Frame.X - AppDelegate.ScreenWidth/2 + widget.View.Frame.Width/2), 0f);

				if (highLighted != null)
				{
					highLighted.InstantUnhighlight();
					highLighted = null;
				}

				widget.Highlight();
				highLighted = widget;

				AppDelegate.boardInterface.BoardScroll.ScrollView.SetContentOffset (position, true);
				highlitedContent++;
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

			UIFont font = UIFont.SystemFontOfSize (12);
			numberLabel = new UILabel (new CGRect (0, ButtonSize / 2 - 14, ButtonSize, 14));
			numberLabel.Font = font;
			numberLabel.TextAlignment = UITextAlignment.Center;
			numberLabel.Text = contentAmmount.ToString ();
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

			UIFont font = UIFont.SystemFontOfSize (12);
			contentAmmount = content;
			numberLabel = new UILabel (new CGRect (0, ButtonSize / 2 - 14, ButtonSize, 14));
			numberLabel.Font = font;
			numberLabel.TextAlignment = UITextAlignment.Center;
			numberLabel.Text = content.ToString ();
			numberLabel.TextColor = UIColor.Black;

			uiButton.AddSubview (numberLabel);
		}
	}
}

