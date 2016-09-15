using UIKit;
using CoreGraphics;
using System.Collections.Generic;

namespace Board.Screens.Controls
{
	public class UIContentDisplay : UIView
	{
		public List<UIContentThumb> ListThumbs;
		public List<UIView> ListViews;

		public UIContentDisplay(){
			ListViews = new List<UIView>();
		}

		bool even;

		public void ForceSelectiveRendering(CGPoint contentOffset){
			even = false;
			SelectiveRendering (contentOffset);
		}

		public void SelectiveRendering(CGPoint contentOffset){
			if (even) {
				even = false;
				return;
			}
			even = true;

			foreach (var view in ListViews) {

				// if its on a screenheight * 2 range...
				if (view.Frame.Y > contentOffset.Y - view.Frame.Height &&
					view.Frame.Y < contentOffset.Y + AppDelegate.ScreenHeight) {

					if (view is UITimelineWidget) {
						if (view.Frame.Y < contentOffset.Y + AppDelegate.ScreenHeight * 3) {

							var timelineWidget = (UITimelineWidget)view;
							timelineWidget.ActivateImage ();

						}
					} else if (view is UICarouselController) {
						if (view.Frame.Y < contentOffset.Y + AppDelegate.ScreenHeight * 2) {

							var carouselController = (UICarouselController)view;
							carouselController.ActivateImage ();

						}
					}

					// if its on a screenheight range
					AddSubview (view);

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
				//thumb.UnsuscribeToEvent ();
			}
		}
	}
}

