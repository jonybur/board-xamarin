using System.Collections.Generic;
using BigTed;
using Board.Facebook;
using Board.Screens.Controls;
using Board.Utilities;
using CoreGraphics;
using UIKit;

namespace Board.Interface.FacebookImport
{
	public class AlbumsScreen : UIViewController
	{
		UIMenuBanner Banner;
		UIScrollView ScrollView;
		List<UIMenuButton> Buttons;
		bool pressed;
		float yPosition;

		private void AlbumsCompletion(List<FacebookElement> elementList) {
			LoadAlbums (elementList);

			BTProgressHUD.Dismiss ();
		}

		public override void ViewDidLoad(){
			View.BackgroundColor = UIColor.White;

			LoadBanner ();

			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));

			Buttons = new List<UIMenuButton> ();

			BTProgressHUD.Show ();
			FacebookUtils.MakeGraphRequest (UIBoardInterface.board.FBPage.Id, "albums", AlbumsCompletion);

			View.AddSubviews (ScrollView, Banner);
		}

		public override void ViewDidAppear(bool animated) {
			Banner.SuscribeToEvents ();

			yPosition = (float)Banner.Frame.Bottom - 21;

			pressed = false;

			foreach (var but in Buttons) {
				but.SetUnpressedColors ();
				but.SuscribeToEvent ();
			}
		}

		public override void ViewDidDisappear(bool animated) {
			Banner.UnsuscribeToEvents ();

			foreach (var but in Buttons) {
				but.UnsuscribeToEvent ();
			}

			// TODO: release with MemoryUtility
		}

		private void LoadAlbums(List<FacebookElement> elementList)
		{
			int i = 0;
			float buttonheight = 0;
				
			foreach (FacebookElement album in elementList) {
				var albumButton = AlbumButton (yPosition, (FacebookAlbum)album);
				ScrollView.AddSubview (albumButton);
				Buttons.Add (albumButton);
				albumButton.SuscribeToEvent ();
				i++;
				buttonheight = (float)albumButton.Frame.Height;
				yPosition += buttonheight + 1;
			}
			var videosButton = VideoButton ();
			videosButton.SuscribeToEvent ();
			Buttons.Add (videosButton);
			ScrollView.AddSubview (videosButton);
					
			ScrollView.ContentSize = new CGSize(AppDelegate.ScreenWidth, (elementList.Count + 1) * (buttonheight + 1) + Banner.Frame.Height);
		}

		private UIOneLineMenuButton VideoButton(){
			var videosButton = new UIOneLineMenuButton (yPosition);
			videosButton.SetLabel ("Videos >");
			videosButton.SetUnpressedColors ();

			videosButton.TapEvent = new System.EventHandler (delegate {
				if (!pressed){
					var videosScreen = new VideosScreen();
					AppDelegate.NavigationController.PushViewController(videosScreen, true);

					pressed = true;
					videosButton.SetPressedColors();
				}
			});

			return videosButton;
		}

		private UIOneLineMenuButton AlbumButton(float yPosition, FacebookAlbum album)
		{
			UIOneLineMenuButton albumButton = new UIOneLineMenuButton (yPosition);
			albumButton.SetLabel (album.Name + " >");
			albumButton.SetUnpressedColors ();

			albumButton.TapEvent += (sender, e) => {
				if (!pressed){
					var photosScreen = new PhotosScreen(album.Id);
					AppDelegate.NavigationController.PushViewController(photosScreen, true);

					pressed = true;
					albumButton.SetPressedColors();
				}
			};

			return albumButton;
		}

		private void LoadBanner()
		{
			Banner = new UIMenuBanner ("ALBUMS", "cross_left");

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					AppDelegate.PopViewControllerLikeDismissView();
					Banner.UnsuscribeToEvents ();
					MemoryUtility.ReleaseUIViewWithChildren (View);
				}
			});

			Banner.AddTap (tap);
		}
	}
}