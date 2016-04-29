﻿using System.Collections.Generic;
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
		MenuBanner Banner;
		UIScrollView ScrollView;
		List<MenuButton> Buttons;

		private void Completion(List<FacebookElement> elementList) {
			LoadAlbums (elementList);
			BTProgressHUD.Dismiss();
		}

		public override void ViewDidLoad(){
			View.BackgroundColor = UIColor.White;

			LoadBanner ();

			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));

			Buttons = new List<MenuButton> ();

			BTProgressHUD.Show ();
			FacebookUtils.MakeGraphRequest (BoardInterface.board.FBPage.Id, "albums", Completion);

			View.AddSubviews (ScrollView, Banner);
		}

		public override void ViewDidAppear(bool animated) {
			Banner.SuscribeToEvents ();

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
			float yPosition = (float)Banner.Frame.Bottom - 21;

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
					
			ScrollView.ContentSize = new CGSize(AppDelegate.ScreenWidth, elementList.Count * (buttonheight + 1) + Banner.Frame.Height);
		}

		bool pressed;
		private OneLineMenuButton AlbumButton(float yPosition, FacebookAlbum album)
		{
			OneLineMenuButton albumButton = new OneLineMenuButton (yPosition);
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
			Banner = new MenuBanner ("./boardinterface/screens/albums/banner/" + AppDelegate.PhoneVersion + ".jpg");

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					AppDelegate.PopViewLikeDismissView();
					Banner.UnsuscribeToEvents ();
					MemoryUtility.ReleaseUIViewWithChildren (View);
				}
			});

			Banner.AddTap (tap);
		}
	}
}
