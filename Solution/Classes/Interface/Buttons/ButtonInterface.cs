using System.Collections.Generic;
using UIKit;

namespace Board.Interface.Buttons
{
	public static class ButtonInterface
	{
		// buttons are square-shaped and must be the same height all-around
		// its height is hardcoded
		public enum ButtonLayout : byte { NavigationBar = 1, ConfirmationBar, Disable };

		static ActionsButtonSet actionsButtonSet;
		static ConfirmationButtonSet confirmationButtonSet;
		public static NavigationButton navigationButton;

		public static void Initialize()
		{
			actionsButtonSet = new ActionsButtonSet ();
			confirmationButtonSet = new ConfirmationButtonSet ();
			navigationButton = new NavigationButton ();			
		}

		public static List<UIView> GetUserButtons()
		{
			List<UIView> views = new List<UIView>();
			views.Add(actionsButtonSet.arrayButtons [0]);
			views.Add(navigationButton);

			return views;
		}

		public static List<UIView> GetCreatorButtons()
		{
			List<UIView> views = new List<UIView> ();
			views.Add(actionsButtonSet.arrayButtons [0]);
			views.Add(actionsButtonSet.arrayButtons [1]);
			views.Add(actionsButtonSet.arrayButtons [2]);
			views.Add(actionsButtonSet.arrayButtons [3]);
			views.Add(confirmationButtonSet.arrayButtons [0]);
			views.Add(confirmationButtonSet.arrayButtons [1]);
			views.Add(navigationButton);
			return views;
		}

		public static void DisableAllLayouts()
		{	
			actionsButtonSet.DisableAllButtons ();
			confirmationButtonSet.DisableAllButtons ();
			navigationButton.DisableButton ();
		}

		public static void SwitchButtonLayout(ButtonLayout newLayout)
		{
			DisableAllLayouts ();

			switch (newLayout) {
			case ButtonLayout.ConfirmationBar:
					confirmationButtonSet.EnableAllButtons ();
					break;

			case ButtonLayout.NavigationBar:
					actionsButtonSet.EnableAllButtons ();
					break;
			}

			navigationButton.EnableButton ();
		}

	}
}