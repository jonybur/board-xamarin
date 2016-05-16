using AVFoundation;
using AVKit;
using Board.Interface.CreateScreens;
using Board.Interface.LookUp;
using Board.Schema;
using Board.Utilities;
using CoreGraphics;
using UIKit;

namespace Board.Interface.FacebookImport
{
	public class VideoImportLookUp : UILookUp
	{
		AVPlayerViewController playerView;
		UIImageView NextButton;
		UITapGestureRecognizer NextTap;

		public VideoImportLookUp(Video video)
		{
			content = video;

			View.BackgroundColor = UIColor.Black;

			CreateButtons (UIColor.White);

			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			ScrollView.UserInteractionEnabled = true;

			playerView = new AVPlayerViewController ();
			playerView.ShowsPlaybackControls = true;
			playerView.View.Frame = new CGRect (0, TrashButton.Frame.Bottom, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight - TrashButton.Frame.Bottom - LikeButton.Frame.Height);
			playerView.Player = LoadPlayer (video);

			ScrollView.AddSubview (playerView.View);

			CreateNextButton (UIColor.White);

			View.AddSubviews (ScrollView, BackButton, NextButton);
		}

		private void CreateNextButton(UIColor buttonColor)
		{
			using (UIImage img = UIImage.FromFile ("./camera/nextbutton.png")) {
				NextButton = new UIImageView(new CGRect(0,0,img.Size.Width * 2,img.Size.Height * 2));

				UIImageView subView = new UIImageView (new CGRect (0, 0, img.Size.Width / 2, img.Size.Height / 2));
				subView.Image = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				subView.Center = new CGPoint (NextButton.Frame.Width / 2, NextButton.Frame.Height / 2);
				subView.TintColor = buttonColor;

				NextButton.AddSubview (subView);
				NextButton.Center = new CGPoint (AppDelegate.ScreenWidth - img.Size.Width / 2 - 5, AppDelegate.ScreenHeight - img.Size.Height / 2 - 5);
			}

			NextButton.UserInteractionEnabled = true;

			NextTap = new UITapGestureRecognizer (tg => {
				var createScreen = new CreateMediaScreen(((Video)content).AmazonNSUrl, false);
				AppDelegate.NavigationController.PushViewController(createScreen, true);
			});
		}

		private AVPlayer LoadPlayer(Video video)
		{	
			AVPlayerItem playerItem;

			using (AVAsset _asset = AVAsset.FromUrl (video.AmazonNSUrl)) {
				playerItem = new AVPlayerItem (_asset);
			}

			playerItem.AudioMix = new AVAudioMix ();
			var player = new AVPlayer (playerItem);
			player.Seek (new CoreMedia.CMTime (0, 1000000000));
			player.Play ();

			return player;
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear (animated);
			NextButton.AddGestureRecognizer (NextTap);
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear (animated);
			NextButton.RemoveGestureRecognizer (NextTap);

			playerView.Player.Pause ();
			playerView.Player.ReplaceCurrentItemWithPlayerItem (null);

			ScrollView = null;

			MemoryUtility.ReleaseUIViewWithChildren (View);
		}
	}
}