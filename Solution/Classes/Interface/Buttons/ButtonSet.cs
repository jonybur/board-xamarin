namespace Board.Interface.Buttons
{
	public class ButtonSet
	{
		public BIButton[] arrayButtons;

		public ButtonSet ()
		{
		}

		public void DisableAllButtons()
		{
			foreach (var b in arrayButtons) {
				b.DisableButton ();
			}
		}

		public void EnableAllButtons()
		{
			foreach (var b in arrayButtons){
				b.EnableButton();
			}
		}

	}
}

