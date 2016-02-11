using System;
using System.Timers;
using System.Drawing;

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
		private static UIImageView alertStroke;
		private bool pictureCycle;
		private int highlitedContent, contentToSee;

		public NavigationButton (UIColor color)
		{
			pictureCycle = true;
			highlitedContent = 0;
			contentToSee = 0;

			alertStroke = new UIImageView (new CGRect (0, 0, ButtonSize, ButtonSize));
			UIImage stroke = UIImage.FromFile ("./boardscreen/buttons/navigationfront.png");
			alertStroke.Image = stroke.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
			alertStroke.TintColor = color;

			uiButton = new UIButton (UIButtonType.Custom);
			UIImage image = UIImage.FromFile ("./boardscreen/buttons/navigationback.png");
			uiButton.SetImage (image, UIControlState.Normal);
			uiButton.Frame = new CGRect (0, 0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint (AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight - ButtonSize / 2 - 10);

			uiButton.AddSubview (alertStroke);

			float i = 0;

			UITapGestureRecognizer doubletap = new UITapGestureRecognizer ((tg) => {
				tg.NumberOfTapsRequired = 2;

				BoardInterface.scrollView.SetContentOffset(new CGPoint(BoardInterface.ScrollViewWidthSize / 2 - AppDelegate.ScreenWidth / 2, 0), true);
			});

			UITapGestureRecognizer tapGesture= new UITapGestureRecognizer  ((tg) => {
				
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

					PictureComponent pictureComponent = BoardInterface.ListPictureComponents[highlitedContent];
					UIView uivComponent = pictureComponent.View;
					position = new PointF ((float)(uivComponent.Frame.X - AppDelegate.ScreenWidth/2 + uivComponent.Frame.Width/2), 0f);

				} else {
					content = BoardInterface.ListTextboxes[highlitedContent];
					position = new PointF (content.ImgX - AppDelegate.ScreenWidth/2 + content.ImgW/2,
											content.ImgY - AppDelegate.ScreenHeight/2 + content.ImgH / 2);
				}


				BoardInterface.scrollView.SetContentOffset (position, true);
				highlitedContent++;
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
			//uiButton.AddGestureRecognizer (doubletap);
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

		public void SetColor(UIColor color)
		{
			alertStroke.TintColor = color;
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
				return false;
			}

			// otherwise, instanciate new text

			UIGraphics.BeginImageContextWithOptions (new CGSize(AppDelegate.ScreenWidth, AppDelegate.ScreenHeight), false, 0);
			CGContext context = UIGraphics.GetCurrentContext ();
			string contentString = content.ToString();

			NSString contentNSString = new NSString (contentString);
			UIFont font = UIFont.SystemFontOfSize (18);
			CGSize contentSize = contentNSString.StringSize (font);

			//TODO: get x and y from navigation button
			CGRect rect = new CGRect (AppDelegate.ScreenWidth/2-32, ((64 - contentSize.Height) / 2) + 
				AppDelegate.ScreenHeight-64, 64, contentSize.Height);

			context.SetFillColor (UIColor.Black.CGColor);
			new NSString (contentString).DrawString (rect, font, UILineBreakMode.WordWrap, UITextAlignment.Center);

			NavigationText = new UIImageView (UIGraphics.GetImageFromCurrentImageContext ());

			return true;
		}
	}
}

