using CoreGraphics;
using UIKit;

using Board.Utilities;
using Board.Schema;

using MediaPlayer;
using System;
using Board.Interface.Buttons;

namespace Board.Interface.CreateScreens
{
	public class CreateMediaScreen : CreateScreen
	{
		PlaceholderTextView textview;

		UITapGestureRecognizer scrollViewTap;
		EventHandler nextButtonTap;

		float positionY;

		public CreateMediaScreen (Content _content){
			content = _content;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			LoadContent ();

			string imagePath = "./boardinterface/screens/share/banner/" + AppDelegate.PhoneVersion + ".jpg";

			LoadBanner (imagePath, null, null);
			LoadNextButton (false);
			LoadTextView ();

			positionY = (float)textview.Frame.Bottom + 60;

			LoadPostToButtons (positionY);

			CreateGestures ();
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			ScrollView.AddGestureRecognizer (scrollViewTap);
			NextButton.TouchUpInside += nextButtonTap;
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
			ScrollView.RemoveGestureRecognizer (scrollViewTap);
			NextButton.TouchUpInside -= nextButtonTap;
			MemoryUtility.ReleaseUIViewWithChildren (View, true);
		}

		private void CreateGestures()
		{
			scrollViewTap = new UITapGestureRecognizer (obj => textview.ResignFirstResponder ());

			nextButtonTap += (sender, e) => {
				AppDelegate.NavigationController.PopViewController (false);

				content.SocialChannel = ShareButtons.GetActiveSocialChannels ();

				// shows the image preview so that the user can position the image
				BoardInterface.scrollView.AddSubview (Preview.View);

				// switches to confbar
				ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.ConfirmationBar);
			};
		}

		private void LoadTextView()
		{
			float autosize = 50;
			float imgw, imgh;

			UIImageView imageView;
			if (content is Picture) {
				imageView = ((Picture)content).ImageView;
			} else if (content is Video) {
				MPMoviePlayerController moviePlayer = new MPMoviePlayerController (((Video)content).Url);
				imageView = new UIImageView (moviePlayer.ThumbnailImageAt (0, MPMovieTimeOption.Exact));
				moviePlayer.Pause ();
				moviePlayer.Dispose ();
			} else {
				imageView = new UIImageView ();
			}

			float scale = (float)(imageView.Frame.Height / imageView.Frame.Width);
			imgw = autosize;
			imgh = autosize * scale;

			UIImageView thumbView = new UIImageView (new CGRect (10, Banner.Frame.Bottom + 10, imgw, imgh));
			thumbView.Image = imageView.Image;

			var frame = new CGRect(70, Banner.Frame.Bottom, 
				AppDelegate.ScreenWidth - 50 - 23,
				140);

			textview = new PlaceholderTextView(frame, "Write a caption...");

			textview.KeyboardType = UIKeyboardType.Default;
			textview.ReturnKeyType = UIReturnKeyType.Default;
			textview.EnablesReturnKeyAutomatically = true;
			textview.BackgroundColor = UIColor.White;
			textview.TextColor = AppDelegate.BoardBlue;
			textview.Font = AppDelegate.SystemFontOfSize18;

			UIImageView colorWhite = new UIImageView(new CGRect (0, 0, AppDelegate.ScreenWidth, frame.Bottom));
			colorWhite.BackgroundColor = UIColor.White;

			ScrollView.AddSubviews (colorWhite, textview, thumbView);
		}
	}
}