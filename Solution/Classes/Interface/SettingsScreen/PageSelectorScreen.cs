using System.Collections.Generic;
using Board.Utilities;
using System.Threading;
using Board.Facebook;
using CoreGraphics;
using UIKit;
using Board.Screens.Controls;
using BigTed;

namespace Board.Interface
{
	public class PageSelectorScreen : UIViewController
	{
		UIMenuBanner Banner;
		UIScrollView ScrollView;
		List<UIMenuButton> Buttons;
		bool pressed;

		public override async void ViewDidLoad ()
		{
			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;

			LoadBanner ();
			View.BackgroundColor = UIColor.White;

			Buttons = new List<UIMenuButton> ();

			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			View.AddSubview (ScrollView);
			View.AddSubview (Banner);

			await FacebookUtils.GetReadPermission (this, "pages_show_list");

			BTProgressHUD.Show ();
			FacebookUtils.MakeGraphRequest ("me", "accounts", Completion);
		}

		public override void ViewDidDisappear (bool animated)
		{
			BTProgressHUD.Dismiss ();
		}

		private void SuscribeToEvents()
		{
			foreach (UIMenuButton sb in Buttons) {
				sb.SuscribeToEvent ();
			}
		}

		private void UnsuscribeToEvents()
		{
			foreach (UIMenuButton sb in Buttons) {
				sb.UnsuscribeToEvent ();
			}
			Banner.UnsuscribeToEvents ();
		}

		private void Completion(List<FacebookElement> ElementList)
		{
			LoadPages (ElementList);

			SuscribeToEvents ();

			BTProgressHUD.Dismiss();
		}

		private void LoadPages(List<FacebookElement> ElementList)
		{
			ScrollView.ContentSize = new CGSize (AppDelegate.ScreenWidth, 80 * ElementList.Count + Banner.Frame.Height + ElementList.Count + 1);
			float yPosition = (float)Banner.Frame.Bottom - 21 ;

			int i = 0;
			foreach (FacebookElement page in ElementList) {
				UITwoLinesMenuButton pageButton = PageButton (yPosition, (FacebookPage)page);
				ScrollView.AddSubview (pageButton);
				Buttons.Add (pageButton);
				i++;
				yPosition += (float)pageButton.Frame.Height + 1;
			}

			UIOneLineMenuButton unsyncButton = CreateUnsyncButton (yPosition);
			if (UIBoardInterface.board.FacebookId == null) {
				unsyncButton.Alpha = 0f;
			}
			ScrollView.AddSubview (unsyncButton);
			Buttons.Add (unsyncButton);
		}

		private UITwoLinesMenuButton PageButton(float yPosition, FacebookPage page)
		{
			UITwoLinesMenuButton pageButton = new UITwoLinesMenuButton (yPosition);
			pageButton.SetLabels (page.Name, page.Category);

			pageButton.SetTapEvent (delegate {
				if (!pressed)
				{
					pressed = true;

					Thread thread = new Thread(new ThreadStart(PopOut));
					thread.Start();

					UIBoardInterface.board.FacebookId = page.Id;
				}
			});

			return pageButton;
		}

		private UIOneLineMenuButton CreateUnsyncButton(float yPosition)
		{
			UIOneLineMenuButton unsyncButton = new UIOneLineMenuButton (yPosition);
			unsyncButton.SetLabel("Disconnect");

			unsyncButton.SetTapEvent (delegate {
				if (!pressed)
				{
					pressed = true;

					Thread thread = new Thread(new ThreadStart(PopOut));
					thread.Start();

					UIBoardInterface.board.FacebookId = null;
				}
			});

			return unsyncButton;
		}

		private void PopOut()
		{
			Thread.Sleep (300);
			InvokeOnMainThread(() => NavigationController.PopViewController(true));
		}

		private void LoadBanner()
		{
			Banner = new UIMenuBanner ("Select", "arrow_left");

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					UnsuscribeToEvents ();
					AppDelegate.PopViewControllerWithCallback(delegate{
						MemoryUtility.ReleaseUIViewWithChildren (View);
					});
				}
			});

			Banner.AddTap (tap);

			Banner.SuscribeToEvents (); 
		}
	}
}

