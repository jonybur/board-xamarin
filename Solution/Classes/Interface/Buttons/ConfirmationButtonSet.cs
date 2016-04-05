
using System;

namespace Board.Interface.Buttons
{
	public class ConfirmationButtonSet : ButtonSet
	{
		public static int CantButtons = 2;

		public ConfirmationButtonSet ()
		{
			arrayButtons = new Button[CantButtons];

			arrayButtons [0] = new CancelButton ();
			arrayButtons [1] = new AcceptButton ();
		}
	}
}

