using UIKit;
using System;
using System.Collections.Generic;

namespace Board.Interface.Buttons
{
	// father class to all buttons
	public class Button
	{
		public UIButton uiButton;
		public List<EventHandler> eventHandlers;
		public List<UIGestureRecognizer> gestureRecognizers;

		// TODO: this value must be variable depending on the screen
		public const float ButtonSize = 45;

		public Button()
		{
			uiButton = new UIButton ();
			eventHandlers = new List<EventHandler> ();
			gestureRecognizers = new List<UIGestureRecognizer> ();
		}

		public virtual void DisableButton()
		{
			uiButton.Alpha = 0f;
			uiButton.Enabled = false;
			UnsuscribeToEvents ();
		}

		public virtual void EnableButton()
		{
			uiButton.Alpha = 1f;
			uiButton.Enabled = true;
			SuscribeToEvents ();
		}

		public void SuscribeToEvents ()
		{
			foreach (EventHandler e in eventHandlers) {
				uiButton.TouchUpInside += e;
			}
			foreach (UIGestureRecognizer gr in gestureRecognizers) {
				uiButton.AddGestureRecognizer(gr);
			}
		}

		public void UnsuscribeToEvents()
		{
			foreach (EventHandler e in eventHandlers) {
				uiButton.TouchUpInside -= e;
			}
			foreach (UIGestureRecognizer gr in gestureRecognizers) {
				uiButton.RemoveGestureRecognizer(gr);
			}
		}
	}
}

