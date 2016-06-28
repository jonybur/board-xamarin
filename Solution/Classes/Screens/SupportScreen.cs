using Board.Screens.Controls;
using Board.Utilities;
using UIKit;

namespace Board.Screens
{
	public class SupportScreen : UIViewController
	{
		UIMenuBanner Banner;
		const int hborder = 65;

		public override void ViewDidLoad ()
		{
			LoadBanner ();

			var content = new UIContactView (UIContactView.ScreenContact.SupportScreen);

			View.AddSubview (content);

			View.BackgroundColor = UIColor.White;
		}

		public override void ViewDidDisappear(bool animated)
		{
			Banner.UnsuscribeToEvents ();
			MemoryUtility.ReleaseUIViewWithChildren (View);
		}

		private void LoadBanner()
		{
			Banner = new UIMenuBanner ("Support", "menu_left");

			var tap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					AppDelegate.containerScreen.BringSideMenuUp("support");
				}
			});

			Banner.AddTap (tap);
			Banner.SuscribeToEvents ();
			View.AddSubview (Banner);
		}
	}
}

