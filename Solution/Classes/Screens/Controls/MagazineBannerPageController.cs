using System;
using CoreGraphics;
using UIKit;

namespace Board.Screens.Controls
{
	public class MagazineBannerPageController : UIPageViewController
	{
		public static UIViewController[] _viewControllers;
		private static UIPageControl pageControl;

		public MagazineBannerPageController (UIPageViewControllerTransitionStyle transitionStyle, UIPageViewControllerNavigationOrientation navigationOrientation, CGSize size)
			: base (transitionStyle, navigationOrientation)
		{
			this.Delegate = new CustomDelegate ();
			this.DataSource = new CustomDataSource ();

			View.Frame = new CGRect(0,0,size.Width, size.Height);
			View.BackgroundColor = UIColor.FromRGBA(0,0,0,0);
		}

		public override void ViewDidLoad ()
		{
			GenerateControllers ("EDITOR'S CHOICE", "ALL");

			GeneratePageControl ();

			SetViewControllers (new []{ _viewControllers[0] }, UIPageViewControllerNavigationDirection.Forward, true, null);
		}

		private void GeneratePageControl(){
			pageControl = new UIPageControl (new CGRect (0, MagazineBannerPage.Height - 20, View.Frame.Width, 20));
			pageControl.Pages = _viewControllers.Length;
			//pageControl.CurrentPageIndicatorTintColor = AppDelegate.BoardOrange;
			//pageControl.PageIndicatorTintColor = AppDelegate.
			pageControl.UserInteractionEnabled = false;
			pageControl.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 100);
			View.AddSubview (pageControl);
		}

		// generates the magazine headers
		private void GenerateControllers(params string[] subtitles){
			var controllers = new UIViewController[subtitles.Length];

			for (int i = 0; i < subtitles.Length; i++) {
				var controller = new UIViewController ();
				var banner = new MagazineBannerPage (subtitles[i]);
				controller.Add (banner);
				controller.View.Frame = banner.Frame;
				controllers [i] = controller;
			}
			 
			_viewControllers = controllers;
		}

		private class CustomDelegate : UIPageViewControllerDelegate{
			public override void DidFinishAnimating (UIPageViewController pageViewController, bool finished, UIViewController[] previousViewControllers, bool completed)
			{
				if (!completed) {
					return;
				}	

				var newViewController = pageViewController.ViewControllers[0];
				int indexOfCurrentViewController = Array.IndexOf (MagazineBannerPageController._viewControllers, newViewController);
				MagazineBannerPageController.pageControl.CurrentPage = indexOfCurrentViewController;
			}	
		}

		private class CustomDataSource : UIPageViewControllerDataSource{
			
			public override UIViewController GetNextViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
			{
				int indexOfCurrentViewController = Array.IndexOf (MagazineBannerPageController._viewControllers, referenceViewController);
				return indexOfCurrentViewController < MagazineBannerPageController._viewControllers.Length - 1 ? MagazineBannerPageController._viewControllers [indexOfCurrentViewController + 1] : null;
			}

			public override UIViewController GetPreviousViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
			{
				int indexOfCurrentViewController = Array.IndexOf (MagazineBannerPageController._viewControllers, referenceViewController);
				return indexOfCurrentViewController > 0 ? MagazineBannerPageController._viewControllers [indexOfCurrentViewController - 1] : null;				
			}
		}

		 
	}


}

