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
		private int cycle;
		private int highlitedContent, contentToSee;

		public NavigationButton (UIColor color)
		{
			cycle = 0;
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

				if (BoardInterface.ListPictureWidgets == null && BoardInterface.ListAnnouncementWidgets == null && BoardInterface.ListVideoWidgets == null)
					return;

				if (BoardInterface.ListPictureWidgets.Count == 0 && BoardInterface.ListAnnouncementWidgets.Count == 0 && BoardInterface.ListVideoWidgets == null)
					return;

				// llega al tope
				if (highlitedContent >= BoardInterface.ListPictureWidgets.Count && cycle == 0) 
				{
					highlitedContent = 0;
					cycle++;
				}

				if (highlitedContent >= BoardInterface.ListVideoWidgets.Count && cycle == 1)
				{
					highlitedContent = 0;
					cycle++;
				}

				if (highlitedContent >= BoardInterface.ListAnnouncementWidgets.Count && cycle == 2)
				{
					highlitedContent = 0;
					cycle = 0;
				}

				Content content;
				PointF position = new PointF(0,0);
				UIView uivComponent;

				switch (cycle)
				{
				case 0:
					PictureWidget pictureWidget = BoardInterface.ListPictureWidgets[highlitedContent];
					uivComponent = pictureWidget.View;
					position = new PointF ((float)(uivComponent.Frame.X - AppDelegate.ScreenWidth/2 + uivComponent.Frame.Width/2), 0f);
					break;
				case 1:
					VideoWidget videoWidget = BoardInterface.ListVideoWidgets[highlitedContent];
					uivComponent = videoWidget.View;
					position = new PointF ((float)(uivComponent.Frame.X - AppDelegate.ScreenWidth/2 + uivComponent.Frame.Width/2), 0f);
					break;
				case 2:
					AnnouncementWidget announcementWidget = BoardInterface.ListAnnouncementWidgets[highlitedContent];
					uivComponent = announcementWidget.View;
					position = new PointF ((float)(uivComponent.Frame.X - AppDelegate.ScreenWidth/2 + uivComponent.Frame.Width/2), 0f);
					break;
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

