using System;
using System.Collections.Generic;
using CoreGraphics;
using UIKit;

namespace Clubby.Screens.Controls
{
	public class UIContentDisplay : UIView
	{
		public List<UIContentThumb> ListThumbs;
		public List<UIView> ListViews;

		public UIContentDisplay(){
			ListViews = new List<UIView>();
		}

		public void SelectiveRendering(CGPoint contentOffset){

			foreach (var view in ListViews) {
				
				// if its on a screenheight * 2 range...
				if (view.Frame.Y > (contentOffset.Y - view.Frame.Height) &&
				    view.Frame.Y < (contentOffset.Y + AppDelegate.ScreenHeight * 2)) {

					if (view is UITimelineWidget) {
						var timelineWidget = (UITimelineWidget)view;
						timelineWidget.ActivateImage ();
					}

					// if its on a screenheight range
					if (view.Frame.Y > (contentOffset.Y - view.Frame.Height) &&
					    view.Frame.Y < (contentOffset.Y + AppDelegate.ScreenHeight)) {
						AddSubview (view);
					}

				} else if (view.Superview != null) {

					// if its not on a screenheight * 2 range and has been drawn, dissolve it
					view.RemoveFromSuperview ();

				}

			}
		}

		public void SuscribeToEvents(){
			if (ListThumbs == null) {
				return;
			}

			foreach (var thumb in ListThumbs) {
				thumb.SuscribeToEvent ();
			}
		}

		public void UnsuscribeToEvents(){
			if (ListThumbs == null) {
				return;
			}

			foreach (var thumb in ListThumbs) {
				thumb.UnsuscribeToEvent ();
			}
		}
	}
}

