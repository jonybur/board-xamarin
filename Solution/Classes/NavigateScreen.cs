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
	public partial class NavigateScreen : UIViewController
	{
		public static float ScreenWidth, ScreenHeight;
	
		public NavigateScreen () : base ("Board", null){}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ScreenWidth = (float)UIScreen.MainScreen.Bounds.Width;
			ScreenHeight = (float)UIScreen.MainScreen.Bounds.Height;

			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;

			LoadButtons ();
		}

		private void LoadButtons()
		{
			UIButton uiButton = new UIButton(UIButtonType.Custom);
			UIImage baseImage = UIImage.FromFile ("./mainmenu/mapscreen.jpg");
			uiButton.SetImage (baseImage, UIControlState.Normal);
			uiButton.Frame = new CGRect (0, 0, ScreenWidth, ScreenHeight);
			uiButton.TouchUpInside += (sender, e) => {
				AppDelegate.NavigationController.PopViewController (false);
			};
			View.AddSubview (uiButton);
		}
	}
}

