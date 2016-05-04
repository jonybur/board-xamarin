using UIKit;
using System.Collections.Generic;

namespace Board.Screens.Controls
{
	public class UIContentDisplay : UIView
	{
		public List<UIContentThumb> ListThumbs;

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

