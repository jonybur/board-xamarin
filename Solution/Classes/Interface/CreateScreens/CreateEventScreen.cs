using Board.Schema;
using Board.Facebook;
using UIKit;
using CoreGraphics;
using System;
using Board.Interface.Buttons;
using Board.Picker;

namespace Board.Interface.CreateScreens
{
	public class CreateEventScreen : CreateScreen
	{
		EventHandler nextButtonTap;
		UIImageView PictureBox;
		UIWindow window;
		float positionY;


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			content = new BoardEvent ();

			LoadContent ();

			string imagePath = "./screens/event/banner/" + AppDelegate.PhoneVersion + ".jpg";

			LoadBanner (imagePath, "posts", LoadFromFacebookEvent);
			LoadNextButton ();
			LoadPictureBox ();

			positionY = (float)PictureBox.Frame.Bottom + 50;

			LoadPostToButtons (positionY);

			CreateGestures ();
		}
			
		private void LoadPictureBox()
		{
			PictureBox = new UIImageView (new CGRect (0, Banner.Frame.Bottom,
				AppDelegate.ScreenWidth, 200));
			PictureBox.BackgroundColor = UIColor.FromRGB (100, 100, 100);

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				ImagePicker ip = new ImagePicker(PictureBox, HideWindow);

				window = new UIWindow(new CGRect(0,0,AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
				window.RootViewController = new UIViewController();
				window.MakeKeyAndVisible();
				
				window.RootViewController.PresentViewController(ip.UIImagePicker, true, null);
			});
			PictureBox.UserInteractionEnabled = true;
			PictureBox.AddGestureRecognizer (tap);

			ScrollView.AddSubview (PictureBox);
		}

		private void HideWindow()
		{
			window.Hidden = true;
		}

		private void LoadFromFacebookEvent(FacebookElement FBElement)
		{
			FacebookEvent FBEvent = (FacebookEvent)FBElement;

			content.FacebookId = FBEvent.Id;
		}

		private void CreateGestures()
		{
			nextButtonTap += (sender, e) => {
				NavigationController.PopViewController (false);

				content.SocialChannel = ShareButtons.GetActiveSocialChannels ();

				// shows the image preview so that the user can position the image
				BoardInterface.scrollView.AddSubview (Preview.View);

				// switches to confbar
				ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.ConfirmationBar);
			};


		}
	}
}

