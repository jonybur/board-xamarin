using UIKit;
using Board.Screens.Controls;

namespace Board.Interface
{
	public class SettingsScreen : UIViewController
	{
		MenuBanner Banner;
		OneLineMenuButton SyncButton;

		public override void ViewDidLoad ()
		{
			LoadBanner ();

			CreateSyncButton ((float)Banner.Frame.Bottom);

			View.AddSubview (SyncButton);

			View.BackgroundColor = UIColor.White;
		}

		public override void ViewDidAppear(bool animated)
		{
			if (BoardInterface.board.FBPage == null) {
				SyncButton.SetLabel("Connect to Facebook Page >");
			} else {
				SyncButton.SetLabel(string.Format ("Connected to {0}", BoardInterface.board.FBPage.Name));
			}

			SyncButton.SetUnpressedColors ();
			Banner.SuscribeToEvents ();
		}

		public override void ViewDidDisappear(bool animated)
		{
			Banner.UnsuscribeToEvents ();
		}

		private void CreateSyncButton(float yPosition)
		{
			SyncButton = new OneLineMenuButton (yPosition);

			SyncButton.TouchUpInside += (sender, e) => {
				SyncButton.SetPressedColors();
				PageSelectorScreen pgScreen = new PageSelectorScreen();
				AppDelegate.NavigationController.PushViewController(pgScreen, true);
			};
		}

		private void LoadBanner()
		{
			Banner = new MenuBanner ("./boardinterface/screens/settings/banner/" + AppDelegate.PhoneVersion + ".jpg");

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					AppDelegate.PopViewLikeDismissView();
				}
			});

			Banner.AddTap (tap);

			View.AddSubview (Banner);
		}
	}
}

