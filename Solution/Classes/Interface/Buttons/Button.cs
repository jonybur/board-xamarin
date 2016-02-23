using UIKit;

namespace Board.Interface.Buttons
{
	// father class to all buttons
	public class Button
	{
		public UIButton uiButton;

		// TODO: this value must be variable depending on the screen
		public const float ButtonSize = 45;

		public Button()
		{
			uiButton = new UIButton ();
		}

		public virtual void DisableButton()
		{
			uiButton.Alpha = 0f;
			uiButton.Enabled = false;
		}

		public virtual void EnableButton()
		{
			uiButton.Alpha = 1f;
			uiButton.Enabled = true;
		}
	}
}

