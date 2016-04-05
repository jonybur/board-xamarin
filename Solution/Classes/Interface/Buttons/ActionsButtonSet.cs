namespace Board.Interface.Buttons
{
	public class ActionsButtonSet : ButtonSet
	{
		public static int CantButtons = 4;

		public ActionsButtonSet ()
		{
			arrayButtons = new Button[CantButtons];

			arrayButtons[0] = new BackButton ();
			arrayButtons[1] = new CameraButton ();
			arrayButtons[2] = new CardButton ();
			arrayButtons[3] = new SettingsButton ();
			//arrayButtons[4] = new InfoButton ();
		}
	}
}