
using System;

namespace Board.Interface.Buttons
{
	public class ConfirmationButtonSet : ButtonSet
	{
		public static int CantButtons = 2;

		public ConfirmationButtonSet (EventHandler acceptTapEvent, EventHandler cancelTapEvent)
		{
			arrayButtons = new BIButton[CantButtons];

			arrayButtons [0] = new CancelButton (cancelTapEvent);
			arrayButtons [1] = new AcceptButton (acceptTapEvent);
		}
	}
}

