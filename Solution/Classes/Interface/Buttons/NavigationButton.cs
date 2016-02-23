using System.Drawing;
using Board.Interface.Widgets;
using Board.Schema;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Board.Interface.Buttons
{
	public class NavigationButton : Button
	{
		private static UILabel numberLabel;

		private void SetImage (string buttonName)
		{
			UIImage image = UIImage.FromFile ("./boardinterface/strokebuttons/" + buttonName + ".png");
			uiButton.SetImage (image, UIControlState.Normal);
		}

		public NavigationButton (UIColor color)
		{
			int content = 3;
			int highlitedContent = 0, cycle = 0;

			uiButton = new UIButton (UIButtonType.Custom);

			SetImage ("closedeye");

			uiButton.Frame = new CGRect (0, 0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint (AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight - ButtonSize / 2);
			RefreshNavigationButtonText(content);

			// deprecated, not used
			UITapGestureRecognizer doubletap = new UITapGestureRecognizer ((tg) => {
				tg.NumberOfTapsRequired = 2;

				BoardInterface.scrollView.SetContentOffset(new CGPoint(BoardInterface.ScrollViewWidthSize / 2 - AppDelegate.ScreenWidth / 2, 0), true);
			});

			// TODO: moves from content to content, improve code
			UITapGestureRecognizer tapGesture= new UITapGestureRecognizer  (tg => {

				content--;
				RefreshNavigationButtonText(content);	

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

				PointF position = new PointF(0,0);
				UIView uivComponent;

				switch (cycle)
				{
				case 0:
					if (highlitedContent < BoardInterface.ListPictureWidgets.Count)
					{
						PictureWidget pictureWidget = BoardInterface.ListPictureWidgets[highlitedContent];
						uivComponent = pictureWidget.View;
						position = new PointF ((float)(uivComponent.Frame.X - AppDelegate.ScreenWidth/2 + uivComponent.Frame.Width/2), 0f);
					}
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

				if (highlitedContent >= BoardInterface.ListAnnouncementWidgets.Count && cycle == 2)
				{
					highlitedContent = 0;
					cycle = 0;
				}
			});

			// unzooms
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

			numberLabel = new UILabel (new CGRect (0, ButtonSize / 2 - 14, ButtonSize, 14));
			numberLabel.Font = font;
			numberLabel.TextAlignment = UITextAlignment.Center;
			numberLabel.Text = content.ToString ();
			numberLabel.TextColor = UIColor.Black;

			uiButton.AddSubview (numberLabel);
		}
	}
}

