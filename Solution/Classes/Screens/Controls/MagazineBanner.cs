using System;
using CoreGraphics;
using MGImageUtilitiesBinding;
using UIKit;

namespace Board.Screens.Controls
{
	public class MagazineBanner : UIPageViewController
	{
		public static UIViewController[] _viewControllers;

		public MagazineBanner ()
		{
		}

		public override void ViewDidLoad ()
		{
			_viewControllers = GenerateControllers ();

			/*
			this.Delegate = new CustomDelegate ();
			this.DataSource = new CustomDataSource ();
			*/

			SetupPageViewController ();

			Console.WriteLine ("didload");
		}

		private void SetupPageViewController(){
			SetViewControllers (new [] { _viewControllers [0] }, UIPageViewControllerNavigationDirection.Forward, true, null);
		}

		private UIViewController[] GenerateControllers(){

			var controller = new UIViewController ();
			var controller2 = new UIViewController ();
			var imageView = new MagazineBannerPage ();

			controller.Add (imageView);
			controller2.Add (imageView);

			var controllers = new UIViewController[2];

			controllers [0] = controller;
			controllers [1] = controller2;

			return controllers;
		}

		private class CustomDelegate : UIPageViewControllerDelegate{
			public override void DidFinishAnimating (UIPageViewController pageViewController, bool finished, UIViewController[] previousViewControllers, bool completed)
			{
				if (!completed) {
					return;
				}	

				var newViewController = MagazineBanner._viewControllers[0];
				// page control shiet
			}	
		}

		private class CustomDataSource : UIPageViewControllerDataSource{
			
			public override UIViewController GetNextViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
			{
				int indexOfCurrentViewController = Array.IndexOf (MagazineBanner._viewControllers, pageViewController);
				return indexOfCurrentViewController > 0 ? MagazineBanner._viewControllers [indexOfCurrentViewController - 1] : null;
			}

			public override UIViewController GetPreviousViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
			{
				int indexOfCurrentViewController = Array.IndexOf (MagazineBanner._viewControllers, pageViewController);
				return indexOfCurrentViewController < indexOfCurrentViewController - 1 ? MagazineBanner._viewControllers [indexOfCurrentViewController + 1] : null;				
			}
		}

		 
	}


}

