using Board.Schema;
using Board.Facebook;

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
			LoadImportButton ("posts", LoadFromFacebookEvent);
			LoadNextButton ();

			positionY = (float)Banner.Frame.Bottom;

			LoadPostToButtons (positionY);

			CreateGestures ();
		}

		private void LoadFromFacebookEvent(FacebookElement FBElement)
		{
			FacebookEvent FBEvent = (FacebookEvent)FBElement;
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

