using System;
using CoreGraphics;
using UIKit;

namespace Clubby.Screens.Controls
{
	public class UIMagazineBannerPageController : UIPageViewController
	{
		private static UIPageControl PageControl;

		public UIMagazineBannerPageController (UIPageViewControllerTransitionStyle transitionStyle, UIPageViewControllerNavigationOrientation navigationOrientation, CGSize size)
			: base (transitionStyle, navigationOrientation)
		{
			Delegate = new CustomDelegate ();
			DataSource = new CustomDataSource ();

			View.Frame = new CGRect(0,0,size.Width, size.Height);
			View.BackgroundColor = UIColor.FromRGBA(0,0,0,0);
		}

		public override void ViewDidLoad ()
		{
			GeneratePageControl ();
			SetViewControllers (new []{ UIMagazineServices.Pages[0].Banner }, UIPageViewControllerNavigationDirection.Forward, true, null);
		}

		private void GeneratePageControl(){
			PageControl = new UIPageControl (new CGRect (0, UIMagazineBannerPage.Height - 20, View.Frame.Width, 20));
			PageControl.Pages = UIMagazineServices.Pages.Length;
			PageControl.UserInteractionEnabled = false;
			PageControl.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 140);
			View.AddSubview (PageControl);
		}

		private class CustomDelegate : UIPageViewControllerDelegate{
			
			public override void DidFinishAnimating (UIPageViewController pageViewController, bool finished, UIViewController[] previousViewControllers, bool completed)
			{
				if (!completed) {
					return;
				}	

				// updates pagecontroller
				var newViewController = pageViewController.ViewControllers[0];
				int indexOfCurrentViewController = Array.IndexOf (UIMagazineServices.PageBanners, newViewController);
				PageControl.CurrentPage = indexOfCurrentViewController;

				var containerScreen = AppDelegate.NavigationController.TopViewController as ContainerScreen;
				var mainMenuScreen = containerScreen.CurrentScreenViewController as MainMenuScreen;

				mainMenuScreen.PlaceNewScreen (UIMagazineServices.Pages[indexOfCurrentViewController].ContentDisplay, "Board", AppDelegate.Narwhal26);
			}	
		}

		private class CustomDataSource : UIPageViewControllerDataSource{
			
			public override UIViewController GetNextViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
			{
				int indexOfCurrentViewController = Array.IndexOf (UIMagazineServices.PageBanners, referenceViewController);
				return indexOfCurrentViewController < UIMagazineServices.Pages.Length - 1 ? UIMagazineServices.Pages[indexOfCurrentViewController + 1].Banner : null;
			}

			public override UIViewController GetPreviousViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
			{
				int indexOfCurrentViewController = Array.IndexOf (UIMagazineServices.PageBanners, referenceViewController);
				return indexOfCurrentViewController > 0 ? UIMagazineServices.Pages[indexOfCurrentViewController - 1].Banner : null;				
			}
		}

		 
	}


}

