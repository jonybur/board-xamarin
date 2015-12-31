using System;
using System.Timers;
using Newtonsoft.Json;
using System.Collections.Generic;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;

namespace Solution
{
	public class NavigationButton : Button
	{
		public static UIImageView NavigationText;
		private bool pictureCycle;
		private static UIImage regular, alert;
		private int highlitedContent, contentToSee;

		public NavigationButton ()
		{
			pictureCycle = true;
			highlitedContent = 0;
			contentToSee = 0;

			regular = UIImage.FromFile ("./boardscreen/buttons/navigation.png");
			alert = UIImage.FromFile ("./boardscreen/buttons/alert.png");

			uiButton = new UIButton (UIButtonType.Custom);
			uiButton.SetImage (regular, UIControlState.Normal);

			//uiButton.Frame = new RectangleF (ButtonSize*2, BoardInterface.ScreenHeight - ButtonSize, ButtonSize, ButtonSize);
			uiButton.Frame = new CGRect (0, 0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint (BoardInterface.ScreenWidth / 2, BoardInterface.ScreenHeight - ButtonSize / 2);

			float i = 0;

			UITapGestureRecognizer tapGesture= new UITapGestureRecognizer  ((tg) => {
				// OVERRIDE FOR PRESENTATION

				CGPoint position;
				position = new CGPoint (BoardInterface.ScreenWidth * i,0);
				BoardInterface.scrollView.SetContentOffset (position, true);
				i+= .8f;

				if (i > 5.8f) {
					i = 0;
				}
				/*
				if (BoardInterface.zoomingScrollView.ZoomScale < 1)
				{
					BoardInterface.ZoomScrollview();
					return;
				}

				if (BoardInterface.ListPictures == null && BoardInterface.ListTextboxes == null)
					return;

				if (BoardInterface.ListPictures.Count == 0 && BoardInterface.ListTextboxes.Count == 0)
					return;

				if (highlitedContent >= BoardInterface.ListPictures.Count && pictureCycle) {
					highlitedContent = 0;

					if (BoardInterface.ListTextboxes.Count > 0)
					{
						pictureCycle = false;
					}
				}

				else if (highlitedContent >= BoardInterface.ListTextboxes.Count && !pictureCycle)
				{
					highlitedContent = 0;

					if (BoardInterface.ListPictures.Count > 0)
					{
						pictureCycle = true;
					}
				}

				Content content;
				PointF position;
				if (pictureCycle)
				{
					content = BoardInterface.ListPictures[highlitedContent];
					position = new PointF (content.ImgX - BoardInterface.ScreenWidth/2,
											content.ImgY - BoardInterface.ScreenHeight/2);
				} else {
					content = BoardInterface.ListTextboxes[highlitedContent];
					position = new PointF (content.ImgX - BoardInterface.ScreenWidth/2 + content.ImgW/2,
											content.ImgY - BoardInterface.ScreenHeight/2 + content.ImgH / 2);
				}


				BoardInterface.scrollView.SetContentOffset (position, true);
				highlitedContent++;*/
			});

			UILongPressGestureRecognizer longPressGesture = new UILongPressGestureRecognizer ((tg) => {
				if (tg.State == UIGestureRecognizerState.Began)
				{
					BoardInterface.UnzoomScrollview();
				}
				else if (tg.State == UIGestureRecognizerState.Ended)
				{
					BoardInterface.ZoomScrollview();
				}
			});


			uiButton.UserInteractionEnabled = true;
			uiButton.AddGestureRecognizer (tapGesture);
			uiButton.AddGestureRecognizer (longPressGesture);
		}

		public override void DisableButton()
		{
			base.DisableButton ();

			if (NavigationText != null) {
				NavigationText.Alpha = 0f;
			}
		}

		public override void EnableButton()
		{
			base.EnableButton ();

			if (NavigationText != null) {
				NavigationText.Alpha = 1f;
			}
		}

		// changes the number on the navigation button
		// remember to add subview after refreshing value
		public bool RefreshNavigationButtonText(int content)
		{
			// kills the current navText
			if (NavigationText != null) {
				NavigationText.RemoveFromSuperview ();
				NavigationText.Dispose ();
			}	

			// if content is 0 then do not draw anything
			if (content <= 0) {
				uiButton.SetImage(regular, UIControlState.Normal);
				return false;
			}

			// otherwise, instanciate new text

			uiButton.SetImage(alert, UIControlState.Normal);

			UIGraphics.BeginImageContextWithOptions (new CGSize(BoardInterface.ScreenWidth, BoardInterface.ScreenHeight), false, 0);
			CGContext context = UIGraphics.GetCurrentContext ();
			string contentString = content.ToString();

			NSString contentNSString = new NSString (contentString);
			UIFont font = UIFont.SystemFontOfSize (18);
			CGSize contentSize = contentNSString.StringSize (font);

			//TODO: get x and y from navigation button
			CGRect rect = new CGRect (BoardInterface.ScreenWidth/2-32, ((64 - contentSize.Height) / 2) + 
				BoardInterface.ScreenHeight-64, 64, contentSize.Height);

			context.SetFillColor (UIColor.Black.CGColor);
			new NSString (contentString).DrawString (rect, font, UILineBreakMode.WordWrap, UITextAlignment.Center);

			NavigationText = new UIImageView (UIGraphics.GetImageFromCurrentImageContext ());

			return true;
		}
	}
}

