using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;

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
			UIScrollView scrollView, UINavigationController navigationController)
		{
			actionsButtonSet = new ActionsButtonSet (navigationController, scrollView, refreshContent);
			confirmationButtonSet = new ConfirmationButtonSet (refreshContent);
			navigationButton = new NavigationButton ();
		}


		// TODO: check if there's a way to properly tint the ButtonBar
		/*
		private UIImageView CreateOrangeBox(CGRect frame)
		{
			UIGraphics.BeginImageContextWithOptions (new CGSize(BoardInterface.ScreenWidth, BoardInterface.ScreenHeight), false, 0);
			CGContext context = UIGraphics.GetCurrentContext ();

			//this.View.BackgroundColor = UIColor.FromRGB(189,34,58);
			//UIColor.FromRGB(238,26,48);
			context.SetFillColor(BoardInterface.InterfaceColor.CGColor);
			//context.SetFillColorWithColor(UIColor.FromRGB(1,4,41).CGColor);
			//context.SetFillColorWithColor(UIColor.FromRGB(235,32,0).CGColor);
			//context.SetFillColorWithColor(UIColor.FromRGB(240,31,1).CGColor);
			context.FillRect(frame);
			UIImage orange = UIGraphics.GetImageFromCurrentImageContext ();
			UIImageView uiv = new UIImageView (orange);
			uiv.Alpha = .9f;
			return uiv;
		}*/

		public UIView[] GetAllViews()
		{
			int cantViews = 1 + ActionsButtonSet.CantButtons + ConfirmationButtonSet.CantButtons;
			UIView[] views = new UIView[cantViews];
			CGRect frame = new CGRect (0, BoardInterface.ScreenHeight - Button.ButtonSize, BoardInterface.ScreenWidth, Button.ButtonSize);
			//views [0] = CreateOrangeBox (frame);
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