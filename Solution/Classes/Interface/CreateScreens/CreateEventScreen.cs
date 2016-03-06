using CoreGraphics;
using UIKit;

using Board.Utilities;
using Board.Schema;

using System;
using Board.Interface.Buttons;

namespace Board.Interface.CreateScreens
{
	public class CreateEventScreen : CreateScreen
	{
		EventHandler nextButtonTap;

		float positionY;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			content = new BoardEvent ();

			LoadContent ();

			string imagePath = "./screens/event/banner/" + AppDelegate.PhoneVersion + ".jpg";

			LoadBanner (imagePath);
			LoadNextButton ();

			positionY = (float)Banner.Frame.Bottom;

			LoadPostToButtons (positionY);

			CreateGestures ();
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			NextButton.TouchUpInside += nextButtonTap;
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
			NextButton.TouchUpInside -= nextButtonTap;
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

