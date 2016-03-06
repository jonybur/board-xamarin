using CoreGraphics;
using UIKit;

using Board.Utilities;
using Board.Schema;

using System;
using Board.Interface.Buttons;

namespace Board.Interface.CreateScreens
{
	public class CreateMediaScreen : CreateScreen
	{
		PlaceholderTextView textview;
		UIImageView imageView;

		UITapGestureRecognizer scrollViewTap;
		EventHandler nextButtonTap;

		float positionY;

		public CreateMediaScreen (UIImageView _imageView, Content _content){
			imageView = _imageView; content = _content;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			LoadContent ();

			string imagePath = "./screens/share/banner/" + AppDelegate.PhoneVersion + ".jpg";

			LoadBanner (imagePath);
			LoadNextButton ();
			LoadTextView ();

			positionY = (float)textview.Frame.Bottom + 50;

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
		}

		private void CreateGestures()
		{
			scrollViewTap = new UITapGestureRecognizer (obj => textview.ResignFirstResponder ());

			nextButtonTap += (sender, e) => {
				NavigationController.PopViewController (false);

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
			textview.Font = UIFont.SystemFontOfSize (18);

			UIImageView colorWhite = new UIImageView(new CGRect (0, 0, AppDelegate.ScreenWidth, frame.Bottom));
			colorWhite.BackgroundColor = UIColor.White;

			ScrollView.AddSubviews (colorWhite, textview, thumbView);
		}
	}
}