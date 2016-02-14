using System;

namespace Board.Interface.Buttons
{
	public class ButtonSet
	{
		public Button[] arrayButtons;

		public ButtonSet ()
		{
		}

		public void DisableAllButtons()
		{
			foreach (Button b in arrayButtons) {
				b.DisableButton ();
			}
		}

		public void EnableAllButtons()
		{
			foreach (Button b in arrayButtons){
				b.EnableButton();
			}
		}

	}
}

