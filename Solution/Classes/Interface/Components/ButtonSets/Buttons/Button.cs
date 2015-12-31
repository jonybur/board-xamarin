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
	// father class to all buttons
	public class Button
	{
		public UIButton uiButton;

		// TODO: this value must be variable depending on the screen
		public static float ButtonSize = 64;

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

