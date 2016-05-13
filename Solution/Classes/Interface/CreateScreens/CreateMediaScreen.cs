using System;
using Board.Schema;
using Board.Utilities;
using CoreGraphics;
using MediaPlayer;
using Foundation;
using UIKit;

namespace Board.Interface.CreateScreens
{
	public class CreateMediaScreen : CreateScreen
	{
		PlaceholderTextView textview;

		UITapGestureRecognizer scrollViewTap;
		EventHandler nextButtonTap;
		UIImageView thumbView;

		float positionY;

		public CreateMediaScreen (NSUrl videoURL){
			content = new Video ();
			((Video)content).Url = videoURL;
		}

		public CreateMediaScreen (){
			content = new Picture ();
			((Picture)content).SetImageFromUIImage(AppDelegate.CameraPhoto);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			LoadContent ();

			LoadBanner (null, null, "SHARE", "cross_left");
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
			thumbView.Image = null;

			MemoryUtility.ReleaseUIViewWithChildren (View);
		}

		private void CreateGestures()
		{
			scrollViewTap = new UITapGestureRecognizer (obj => textview.ResignFirstResponder ());

			nextButtonTap += (sender, e) => {
				if (content is Picture){
					((Picture)content).Description = textview.Text;
				} else if (content is Video){
					((Video)content).Description = textview.Text;
				}

				Preview.Initialize (content);

				content.SocialChannel = ShareButtons.GetActiveSocialChannels ();

				var mightBeBoardInterface = AppDelegate.NavigationController.ViewControllers[AppDelegate.NavigationController.ViewControllers.Length - 3];
				if (mightBeBoardInterface is UIBoardInterface) {
					AppDelegate.NavigationController.PopToViewController (mightBeBoardInterface, false);
				} else {
					AppDelegate.NavigationController.PopToViewController(AppDelegate.NavigationController.ViewControllers[AppDelegate.NavigationController.ViewControllers.Length - 2], false);
				}
			};
		}

		private void LoadTextView()
		{
			const float autosize = 50;
			float imgw, imgh;

			UIImage image;
			if (content is Picture) {
				image = ((Picture)content).Image;
			} else if (content is Video) {
				MPMoviePlayerController moviePlayer = new MPMoviePlayerController (((Video)content).Url);
				image = moviePlayer.ThumbnailImageAt (0, MPMovieTimeOption.Exact);
				moviePlayer.Pause ();
				moviePlayer.Dispose ();
			} else {
				image = new UIImage ();
			}

			float scale = (float)(image.Size.Height / image.Size.Width);
			imgw = autosize;
			imgh = autosize * scale;

			thumbView = new UIImageView (new CGRect (10, Banner.Frame.Bottom + 10, imgw, imgh));
			thumbView.Image = image;

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