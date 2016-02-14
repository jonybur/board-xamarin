using System;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;

using System.Net.Http;

using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace Board.Buttons
{
	public class ActionsButtonSet : ButtonSet
	{
		public static int CantButtons = 4;

		public ActionsButtonSet (UINavigationController navigationController, UIScrollView scrollView, Action refreshContent)
		{
			arrayButtons = new Button[CantButtons];

			arrayButtons[0] = new BackButton ();
			arrayButtons[1] = new ImageButton (navigationController, scrollView);
			arrayButtons[2] = new CameraButton (navigationController, scrollView);
			arrayButtons[3] = new CardButton (navigationController);

			//arrayButtons[1] = new TextButton (navigationController, scrollView, refreshContent);
		}
	}
}