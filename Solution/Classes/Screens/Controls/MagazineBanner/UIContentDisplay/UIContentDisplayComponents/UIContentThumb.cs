using UIKit;
using System;
using Clubby.Schema;

namespace Clubby.Screens.Controls
{
	public class UIContentThumb : UIButton
	{
		public EventHandler TouchEvent;
		public Venue Board;

		public void SuscribeToEvent()
		{
			TouchUpInside += TouchEvent;	
		}

		public void UnsuscribeToEvent()
		{
			TouchUpInside -= TouchEvent;
		}	
	}
}

