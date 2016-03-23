using UIKit;
using Board.Utilities;
using Board.Screens.Controls;

namespace Board.Interface
{
	public class SettingsScreen : UIViewController
	{
		MenuBanner Banner;
		OneLineMenuButton SyncButton, AnalyticsButton;

		public override void ViewDidLoad ()
		{
			LoadBanner ();

			CreateAnalyticsButton ((float)Banner.Frame.Bottom);
			CreateSyncButton ((float)AnalyticsButton.Frame.Bottom + 1);

			View.AddSubviews (SyncButton, AnalyticsButton);

			View.BackgroundColor = UIColor.White;
		}

		public override void ViewDidAppear(bool animated)
		{
			if (BoardInterface.board.FBPage == null) {
				SyncButton.SetLabel("Connect to Facebook Page >");
			} else {
				SyncButton.SetLabel(string.Format ("Connected to {0}", BoardInterface.board.FBPage.Name));
			}

			AnalyticsButton.SetUnpressedColors ();
			SyncButton.SetUnpressedColors ();
			Banner.SuscribeToEvents ();
		}

		private void CreateAnalyticsButton(float yPosition)
		{
			AnalyticsButton = new OneLineMenuButton (yPosition);
			AnalyticsButton.SetLabel ("Get Analytics >");

			AnalyticsButton.TouchUpInside += (sender, e) => {
				AnalyticsButton.SetPressedColors();
				AnalyticsScreen analScreen = new AnalyticsScreen();
				Banner.UnsuscribeToEvents ();
				AppDelegate.NavigationController.PushViewController(analScreen, true);
			};
		}


		private void CreateSyncButton(float yPosition)
		{
			SyncButton = new OneLineMenuButton (yPosition);

			SyncButton.TouchUpInside += (sender, e) => {
				SyncButton.SetPressedColors();
				PageSelectorScreen pgScreen = new PageSelectorScreen();
				Banner.UnsuscribeToEvents ();
				AppDelegate.NavigationController.PushViewController(pgScreen, true);
			};
		}

		private void LoadBanner()
		{
			Banner = new MenuBanner ("./boardinterface/screens/settings/banner/" + AppDelegate.PhoneVersion + ".jpg");

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					AppDelegate.PopViewLikeDismissView();
					Banner.UnsuscribeToEvents ();
					MemoryUtility.ReleaseUIViewWithChildren (View);
				}
			});

			Banner.AddTap (tap);

			View.AddSubview (Banner);
		}
	}
}

