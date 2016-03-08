using CoreGraphics;
using UIKit;
using Board.Screens.Controls;

namespace Board.Interface
{
	public class SettingsScreen : UIViewController
	{
		UIImageView Banner;
		OneLineScreenButton SyncButton;

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
				SyncButton.SetLabel("Sync to Facebook Page >");
			} else {
				SyncButton.SetLabel(string.Format ("Synced to {0}", BoardInterface.board.FBPage.Name));
			}

			SyncButton.SetUnpressedColors ();
		}

		private void CreateSyncButton(float yPosition)
		{
			SyncButton = new OneLineScreenButton (yPosition);

			SyncButton.TouchUpInside += (sender, e) => {
				SyncButton.SetPressedColors();
				PageSelectorScreen pgScreen = new PageSelectorScreen();
				NavigationController.PushViewController(pgScreen, true);
			};
		}

		private void LoadBanner()
		{
			using (UIImage bannerImage = UIImage.FromFile ("./screens/settings/banner/" + AppDelegate.PhoneVersion + ".jpg")) {
				Banner = new UIImageView(new CGRect(0,0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
				Banner.Image = bannerImage;	
			}

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					NavigationController.PopViewController(true);
				}
			});

			Banner.UserInteractionEnabled = true;
			Banner.AddGestureRecognizer (tap);
			Banner.Alpha = .95f;
			View.AddSubview (Banner);
		}
	}
}

