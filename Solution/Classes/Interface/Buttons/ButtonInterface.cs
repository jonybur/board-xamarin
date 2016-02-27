using System;
using CoreGraphics;
using UIKit;

namespace Board.Interface.Buttons
{
	public static class ButtonInterface
	{
		// buttons are square-shaped and must be the same height all-around
		// its height is hardcoded
		public enum ButtonLayout : byte {NavigationBar=1, ConfirmationBar, Disable};

		static ActionsButtonSet actionsButtonSet;
		static ConfirmationButtonSet confirmationButtonSet;
		public static NavigationButton navigationButton;

		public static void Initialize(Action refreshContent, UIColor color)
		{
			actionsButtonSet = new ActionsButtonSet (color);
			confirmationButtonSet = new ConfirmationButtonSet (refreshContent);
			navigationButton = new NavigationButton (color);			
		}

		public static UIView[] GetUserButtons()
		{
			const int cantViews = 2;
			UIView[] views = new UIView[cantViews];
			views [0] = actionsButtonSet.arrayButtons [0].uiButton;
			views [1] = navigationButton.uiButton;
			return views;
		}

		public static UIView[] GetCreatorButtons()
		{
			int cantViews = 1 + ActionsButtonSet.CantButtons + ConfirmationButtonSet.CantButtons;
			UIView[] views = new UIView[cantViews];
			views [0] = actionsButtonSet.arrayButtons [0].uiButton;
			views [1] = actionsButtonSet.arrayButtons [1].uiButton;
			views [2] = actionsButtonSet.arrayButtons [2].uiButton;
			views [3] = actionsButtonSet.arrayButtons [3].uiButton;
			views [4] = confirmationButtonSet.arrayButtons [0].uiButton;
			views [5] = confirmationButtonSet.arrayButtons [1].uiButton;
			views [6] = navigationButton.uiButton;
			return views;
		}

		public static void DisableAllLayouts()
		{
			actionsButtonSet.DisableAllButtons ();
			confirmationButtonSet.DisableAllButtons ();
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
		}

	}
}