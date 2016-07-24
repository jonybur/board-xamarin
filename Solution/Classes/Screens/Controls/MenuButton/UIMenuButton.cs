using System;
using CoreGraphics;
using System.Collections.Generic;
using UIKit;

namespace Clubby.Screens.Controls
{
	public class UIMenuButton : UIButton
	{
		private EventHandler TapEvent;
		public List<UILabel> ListLabels;

		public void SetTapEvent(Action tapAction){
			TapEvent = delegate {
				tapAction.Invoke();
				SetUnpressedColors ();
			};
		}

		public UIMenuButton()
		{
			ListLabels = new List<UILabel> ();
		}

		public void SuscribeToEvent()
		{
			TouchUpInside += TapEvent;

			TouchDown += (sender, e) => {
				SetPressedColors();
			};
			TouchUpOutside += (sender, e) => {
				SetUnpressedColors();
			};
			TouchCancel += (sender, e) => {
				SetUnpressedColors();
			};
			TouchDragExit += (sender, e) => {
				SetUnpressedColors();
			};
		}

		public void UnsuscribeToEvent()
		{
			TouchUpInside -= TapEvent;
		}

		protected void SetPressedColors()
		{
			BackgroundColor = AppDelegate.ClubbyBlue;
		}

		protected void SetUnpressedColors()
		{
			BackgroundColor = UIColor.Black;
		}
	}
}

