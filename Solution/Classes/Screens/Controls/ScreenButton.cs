using System;
using System.Collections.Generic;
using UIKit;

namespace Board.Screens.Controls
{
	public class ScreenButton : UIButton
	{
		public EventHandler TapEvent;
		public List<UILabel> ListLabels;

		public ScreenButton()
		{
			ListLabels = new List<UILabel> ();
		}

		public void SuscribeToEvent()
		{
			TouchUpInside += TapEvent;
		}

		public void UnsuscribeToEvent()
		{
			TouchUpInside -= TapEvent;
		}

		public void SetPressedColors()
		{
			BackgroundColor = AppDelegate.BoardLightBlue;
			foreach (UILabel Label in ListLabels) {
				Label.TextColor = UIColor.White;
			}
		}

		public void SetUnpressedColors()
		{
			BackgroundColor = UIColor.FromRGB (250, 250, 250);	
 			foreach (UILabel Label in ListLabels) {
				Label.TextColor = AppDelegate.BoardBlue;
			}
		}
	}
}

