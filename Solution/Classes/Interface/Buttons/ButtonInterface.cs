using System;
using CoreGraphics;
using UIKit;
using System.Collections.Generic;

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

		public static List<UIView> GetUserButtons(bool facebookPage)
		{
			List<UIView> views = new List<UIView>();
			views.Add(actionsButtonSet.arrayButtons [0].uiButton);
			views.Add(navigationButton.uiButton);

			return views;
		}

		public static List<UIView> GetCreatorButtons()
		{
			List<UIView> views = new List<UIView> ();
			views.Add(actionsButtonSet.arrayButtons [0].uiButton);
			views.Add(actionsButtonSet.arrayButtons [1].uiButton);
			views.Add(actionsButtonSet.arrayButtons [2].uiButton);
			views.Add(actionsButtonSet.arrayButtons [3].uiButton);
			views.Add(confirmationButtonSet.arrayButtons [0].uiButton);
			views.Add(confirmationButtonSet.arrayButtons [1].uiButton);
			views.Add(navigationButton.uiButton);
			return views;
		}

		public static void DisableAllLayouts()
		{
			actionsButtonSet.DisableAllButtons ();
			confirmationButtonSet.DisableAllButtons ();
			navigationButton.DisableButton ();
		}

		public static void SwitchButtonLayout(int newLayout)
		{
			DisableAllLayouts ();

			switch (newLayout) {
			case (int)ButtonLayout.ConfirmationBar:
					confirmationButtonSet.EnableAllButtons ();
					break;

			case (int)ButtonLayout.NavigationBar:
					actionsButtonSet.EnableAllButtons ();
					break;
			}

			navigationButton.EnableButton ();
		}

	}
}