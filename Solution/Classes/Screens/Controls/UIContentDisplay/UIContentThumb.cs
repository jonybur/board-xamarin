using UIKit;
using System;

namespace Board.Screens.Controls
{
	public class UIContentThumb : UIButton
	{
		public EventHandler TouchEvent;
		public Board.Schema.Board Board;

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

