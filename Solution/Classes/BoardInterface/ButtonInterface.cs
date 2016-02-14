using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;
using Board.Interface.Buttons;

namespace Solution
{
	public class ButtonInterface
	{
		// buttons are square-shaped and must be the same height all-around
		// its height is hardcoded
		public enum ButtonLayout : byte {NavigationBar=1, ConfirmationBar, Disable};

		static ActionsButtonSet actionsButtonSet;
		static ConfirmationButtonSet confirmationButtonSet;
		static NavigationButton navigationButton;

		public ButtonInterface(Action refreshContent, 
			UIScrollView scrollView, UINavigationController navigationController, UIColor color)
		{
			actionsButtonSet = new ActionsButtonSet (navigationController, scrollView, refreshContent);
			confirmationButtonSet = new ConfirmationButtonSet (refreshContent);
			navigationButton = new NavigationButton (color);
		}

		public UIView[] GetUserButtons()
		{
			int cantViews = 2;
			UIView[] views = new UIView[cantViews];
			CGRect frame = new CGRect (0, AppDelegate.ScreenHeight - Button.ButtonSize, AppDelegate.ScreenWidth, Button.ButtonSize);
			views [0] = actionsButtonSet.arrayButtons [0].uiButton;
			views [1] = navigationButton.uiButton;
			return views;
		}

		public UIView[] GetCreatorButtons()
		{
			int cantViews = 1 + ActionsButtonSet.CantButtons + ConfirmationButtonSet.CantButtons;
			UIView[] views = new UIView[cantViews];
			CGRect frame = new CGRect (0, AppDelegate.ScreenHeight - Button.ButtonSize, AppDelegate.ScreenWidth, Button.ButtonSize);
			views [0] = actionsButtonSet.arrayButtons [0].uiButton;
			views [1] = actionsButtonSet.arrayButtons [1].uiButton;
			views [2] = actionsButtonSet.arrayButtons [2].uiButton;
			views [3] = actionsButtonSet.arrayButtons [3].uiButton;
			views [4] = confirmationButtonSet.arrayButtons [0].uiButton;
			views [5] = confirmationButtonSet.arrayButtons [1].uiButton;
			views [6] = navigationButton.uiButton;
			return views;
		}

		private static void DisableAllLayouts()
		{
			actionsButtonSet.DisableAllButtons ();
			confirmationButtonSet.DisableAllButtons ();
		}

		public static void SwitchButtonLayout(int NewLayout)
		{
			DisableAllLayouts ();

			switch (NewLayout) {
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