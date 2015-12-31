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

namespace Solution
{
	public class ActionsButtonSet : ButtonSet
	{
		public static int CantButtons = 4;

		public ActionsButtonSet (UINavigationController navigationController, //Action<UIViewController, bool, NSAction> presentViewController, 
			UIScrollView scrollView, Action refreshContent)
		{
			arrayButtons = new Button[CantButtons];

			arrayButtons[0] = new BackButton ();
			arrayButtons[1] = new ImageButton (navigationController, scrollView);
			arrayButtons[2] = new TextButton (navigationController, scrollView, refreshContent);
			arrayButtons[3] = new GalleryButton ();
		}
	}
}

