using UIKit;
using System.Collections.Generic;

namespace Board.Screens.Controls
{
	public class UIContentDisplay : UIView
	{
		public List<UIContentThumb> ListThumbs;

		public void SuscribeToEvents(){
			foreach (var thumb in ListThumbs) {
				thumb.SuscribeToEvent ();
			}
		}

		public void UnsuscribeToEvents(){
			foreach (var thumb in ListThumbs) {
				thumb.UnsuscribeToEvent ();
			}
		}
	}
}

