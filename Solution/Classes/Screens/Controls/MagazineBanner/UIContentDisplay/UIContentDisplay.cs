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

		public void SelectiveRendering(CGPoint contentOffset){
			foreach (var view in ListViews) {

				if (view.Frame.Y > (contentOffset.Y - view.Frame.Height) &&
					view.Frame.Y < (contentOffset.Y + AppDelegate.ScreenHeight)) {

					AddSubview (view);

				} else {

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

