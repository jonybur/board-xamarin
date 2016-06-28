using Board.Screens.Controls;
using Board.Utilities;
using UIKit;

namespace Board.Screens
{
	public class InviteScreen : UIViewController
	{
		UIMenuBanner Banner;
		const int hborder = 65;

		public override void ViewDidLoad ()
		{
			LoadBanner ();

			var content = new UIContactView (UIContactView.ScreenContact.InviteScreen);

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
			Banner = new UIMenuBanner ("Invite", "menu_left");

			var tap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					AppDelegate.containerScreen.BringSideMenuUp("invite");
				}
			});

			Banner.AddTap (tap);
			Banner.SuscribeToEvents ();
			View.AddSubview (Banner);
		}
	}
}