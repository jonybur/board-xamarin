using System;
using CoreGraphics;
using MGImageUtilitiesBinding;
using UIKit;

namespace Board.Screens.Controls
{
	public class MagazineBanner : UIViewController
	{
		UIPageViewController pageViewController;

		public MagazineBanner ()
		{
			pageViewController = new UIPageViewController (UIPageViewControllerTransitionStyle.Scroll,
				UIPageViewControllerNavigationOrientation.Horizontal);

			/*var controllers = new UIViewController[2];
			var imageView = new MagazineBannerPage ();

			var controller = new UIViewController ();
			var controller2 = new UIViewController ();

			controller.Add (imageView);
			controller2.Add (imageView);

			controllers [0] = controller;
			controllers [1] = controller2;*/

			pageViewController.DataSource = new PageViewControllerDataSource (this);

			//SetViewControllers (controllers, UIPageViewControllerNavigationDirection.Forward, true, null);
		}

		class PageViewControllerDataSource : UIPageViewControllerDataSource{
			private MagazineBanner _parentViewController;
			private int cantPaginas = 3;

			public PageViewControllerDataSource (UIViewController parentViewController)
			{
				_parentViewController = parentViewController as MagazineBanner;
			}

			public override UIViewController GetPreviousViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
			{
				var imageView = new MagazineBannerPage ();
				var controller = new UIViewController ();
				controller.Add (imageView);
				return controller;
			}

			public override UIViewController GetNextViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
			{
				var imageView = new MagazineBannerPage ();
				var controller = new UIViewController ();
				controller.Add (imageView);
				return controller;
			}

			public override nint GetPresentationCount (UIPageViewController pageViewController)
			{
				return cantPaginas;
			}

			public override nint GetPresentationIndex (UIPageViewController pageViewController)
			{
				return 0;
			}	
		}
	}


}

