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
		MenuBanner Banner;
		UIScrollView ScrollView;
		List<MenuButton> Buttons;
		bool pressed;

		public override async void ViewDidLoad ()
		{
			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;

			LoadBanner ();
			View.BackgroundColor = UIColor.White;

			Buttons = new List<MenuButton> ();

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
			foreach (MenuButton sb in Buttons) {
				sb.SuscribeToEvent ();
			}
		}

		private void UnsuscribeToEvents()
		{
			foreach (MenuButton sb in Buttons) {
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
				TwoLinesMenuButton pageButton = PageButton (yPosition, (FacebookPage)page);
				ScrollView.AddSubview (pageButton);
				Buttons.Add (pageButton);
				i++;
				yPosition += (float)pageButton.Frame.Height + 1;
			}

			OneLineMenuButton unsyncButton = CreateUnsyncButton (yPosition);
			if (BoardInterface.board.FBPage == null) {
				unsyncButton.Alpha = 0f;
			}
			ScrollView.AddSubview (unsyncButton);
			Buttons.Add (unsyncButton);
		}

		private TwoLinesMenuButton PageButton(float yPosition, FacebookPage page)
		{
			TwoLinesMenuButton pageButton = new TwoLinesMenuButton (yPosition);
			pageButton.SetLabels (page.Name, page.Category);
			pageButton.SetUnpressedColors ();

			pageButton.TapEvent += (sender, e) => {
				if (!pressed)
				{
					pressed = true;
					pageButton.SetPressedColors();

					Thread thread = new Thread(new ThreadStart(PopOut));
					thread.Start();

					BoardInterface.board.FBPage = page;
				}
			};

			return pageButton;
		}

		private OneLineMenuButton CreateUnsyncButton(float yPosition)
		{
			OneLineMenuButton unsyncButton = new OneLineMenuButton (yPosition);
			unsyncButton.SetLabel("Unsync");
			unsyncButton.SetUnpressedColors ();

			unsyncButton.TapEvent += (sender, e) => {
				if (!pressed)
				{
					pressed = true;
					unsyncButton.SetPressedColors();

					Thread thread = new Thread(new ThreadStart(PopOut));
					thread.Start();

					BoardInterface.board.FBPage = null;
				}
			};

			return unsyncButton;
		}

		private void PopOut()
		{
			Thread.Sleep (300);
			InvokeOnMainThread(() => NavigationController.PopViewController(true));
		}

		private void LoadBanner()
		{
			Banner = new MenuBanner ("./screens/pageselector/banner/" + AppDelegate.PhoneVersion + ".jpg");

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					UnsuscribeToEvents ();
					NavigationController.PopViewController(true);
					MemoryUtility.ReleaseUIViewWithChildren (View);
				}
			});

			Banner.AddTap (tap);

			Banner.SuscribeToEvents (); 
		}
	}
}

