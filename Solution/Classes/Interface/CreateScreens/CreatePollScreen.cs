using UIKit;
using Board.Schema;
using System;
using Board.Interface.Buttons;

namespace Board.Interface.CreateScreens
{
	public class CreatePollScreen : CreateScreen
	{
		EventHandler nextButtonTap;

		float positionY;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			content = new Poll();

			LoadContent ();

			string imagePath = "./screens/poll/banner/" + AppDelegate.PhoneVersion + ".jpg";

			LoadBanner (imagePath, null, null);
			LoadNextButton ();

			positionY = (float)Banner.Frame.Bottom;

			LoadPostToButtons (positionY);

			CreateGestures ();
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
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

