using CoreGraphics;
using UIKit;

using Board.Interface;
using Board.Interface.Buttons;
using Board.Utilities;
using Board.Schema;

namespace Board.Interface.CreateScreens
{
	public class CreateScreen : UIViewController
	{
		public UIImageView Banner;
		public UIScrollView ScrollView;
		public UIButton NextButton;
		public PostToButtons ShareButtons;

		public Content content;

		UITapGestureRecognizer bannerTap;

		public override void ViewDidLoad()
		{
			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;
		}

		public override void ViewDidAppear (bool animated)
		{
			Banner.AddGestureRecognizer (bannerTap);
		}

		public override void ViewDidDisappear (bool animated)
		{
			Banner.RemoveGestureRecognizer (bannerTap);
		}

		protected void LoadContent()
		{
			ScrollView = new UIScrollView(new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			ScrollView.BackgroundColor = UIColor.FromRGB(250, 250, 250);
			ScrollView.UserInteractionEnabled = true;

			View.AddSubview (ScrollView);
		}

		protected void LoadBanner(string imagePath)
		{
			using (UIImage bannerImage = UIImage.FromFile (imagePath)) {
				Banner = new UIImageView(new CGRect(0, 0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
				Banner.Image = bannerImage;	
			}

			bannerTap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					NavigationController.PopViewController(true);
				}
			});

			Banner.UserInteractionEnabled = true;
			Banner.Alpha = .95f;
			View.AddSubview (Banner);
		}

		protected void LoadNextButton()
		{
			using (UIImage mapImage = UIImage.FromFile ("./screens/share/next/" + AppDelegate.PhoneVersion + ".jpg")) {
				NextButton = new UIButton(new CGRect(0,AppDelegate.ScreenHeight - (mapImage.Size.Height / 2),
					mapImage.Size.Width / 2, mapImage.Size.Height / 2));
				NextButton.SetImage(mapImage, UIControlState.Normal);	
			}

			NextButton.Alpha = .95f;
			View.AddSubview (NextButton);
		}

		protected void LoadPostToButtons(float positionY)
		{
			ShareButtons = new PostToButtons(positionY, this);
			ScrollView.AddSubview (ShareButtons.View);
		}

	}
}

