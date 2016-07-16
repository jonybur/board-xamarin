using UIKit;
using System;
using Clubby.Schema;

namespace Clubby.Screens.Controls
{
	public class UIContentThumb : UIButton
	{
		public EventHandler TouchEvent;
		public Venue Board;

		bool suscribed = false;

		public void SuscribeToEvent()
		{
			if (!suscribed) {
				TouchUpInside += TouchEvent;	
				suscribed = true;
			}
		}

		public void UnsuscribeToEvent()
		{
			if (suscribed) {
				TouchUpInside -= TouchEvent;
				suscribed = false;
			}
		}	
	}
}

