using System;
using Board.Schema;
using Board.Utilities;
using CoreGraphics;
using AVFoundation;
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

		public CreateMediaScreen (NSUrl videoURL, bool isLocalUrl){
			content = new Video ();
			if (isLocalUrl) {
				((Video)content).LocalNSUrl = videoURL;
			} else {
				((Video)content).AmazonNSUrl = videoURL;
			}
		}

		public CreateMediaScreen (){
			content = new Picture ();
			((Picture)content).SetImageFromUIImage(AppDelegate.CameraPhoto);
		}

		public CreateMediaScreen (Picture picture){
			content = picture;
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

				AppDelegate.NavigationController.PopToViewController(AppDelegate.BoardInterface, false);
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
				var video = (Video)content;

				AVAsset asset;
				if (video.LocalNSUrl != null) {
					asset = AVAsset.FromUrl (video.LocalNSUrl);
				} else {
					asset = AVAsset.FromUrl (video.AmazonNSUrl);
				}

				var generator = new AVAssetImageGenerator (asset);
				var requestedTime = new CoreMedia.CMTime (1, 60);
				var outTime = new CoreMedia.CMTime ();
				var outError = new NSError ();
				var imgRef = generator.CopyCGImageAtTime (requestedTime, out outTime, out outError);
				image = new UIImage (imgRef);

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