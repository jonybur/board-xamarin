using UIKit;
using System;
using System.Collections.Generic;

namespace Board.Interface.Buttons
{
	// father class to all buttons
	public class BIButton : UIButton
	{
		public List<EventHandler> eventHandlers;
		public List<UIGestureRecognizer> gestureRecognizers;

		public const float ButtonSize = 45;

		public BIButton() : base (UIButtonType.Custom)
		{
			eventHandlers = new List<EventHandler> ();
			gestureRecognizers = new List<UIGestureRecognizer> ();
		}

		public virtual void DisableButton()
		{
			Alpha = 0f;
			Enabled = false;
			UnsuscribeToEvents ();
		}

		public virtual void EnableButton()
		{
			Alpha = 1f;
			Enabled = true;
			SuscribeToEvents ();
		}

		public void SuscribeToEvents ()
		{
			foreach (EventHandler e in eventHandlers) {
				TouchUpInside += e;
			}
			foreach (UIGestureRecognizer gr in gestureRecognizers) {
				AddGestureRecognizer(gr);
			}
		}

		public void UnsuscribeToEvents()
		{
			foreach (EventHandler e in eventHandlers) {
				TouchUpInside -= e;
			}
			foreach (UIGestureRecognizer gr in gestureRecognizers) {
				RemoveGestureRecognizer(gr);
			}
		}
	}
}

