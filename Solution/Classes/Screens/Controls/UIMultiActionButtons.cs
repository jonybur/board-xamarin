using System;
using System.Collections.Generic;
using CoreGraphics;
using Haneke;
using UIKit;

namespace Clubby.Screens.Controls
{
	public sealed class UIMultiActionButtons : UIView
	{
		public const int Height = 50;
		public List<UIMultiActionButton> ListButtons;

		public UIMultiActionButtons ()
		{
			Frame = new CGRect(0, AppDelegate.ScreenHeight - Height, AppDelegate.ScreenWidth, Height);
			BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
			var backgroundView = new UIImageView (new CGRect(0,0,Frame.Width, Frame.Height));
			backgroundView.BackgroundColor = AppDelegate.ClubbyBlack;
			backgroundView.Alpha = .95f;
			AddSubview (backgroundView);

			ListButtons = new List<UIMultiActionButton> ();
			UserInteractionEnabled = true;

			if (UIMagazine.TheresTimeline) {
				CreateTimelineButton ();
			}

			if (UIMagazine.TheresMagazine) {
				CreateFeaturedButton ();
			}

			CreateDirectoryButton ();
			CreateMapButton ();

			foreach (var button in ListButtons) {
				AddSubview (button);
			}

			if (ListButtons.Count == 3) {
				float xposition = AppDelegate.ScreenWidth / 4;

				ListButtons [0].Center = new CGPoint (xposition * 1, ListButtons [0].Center.Y);
				ListButtons [1].Center = new CGPoint (xposition * 2, ListButtons [1].Center.Y);
				ListButtons [2].Center = new CGPoint (xposition * 3, ListButtons [2].Center.Y);
			}

			else if (ListButtons.Count == 4) {
				float xposition = AppDelegate.ScreenWidth / 8;

				ListButtons [0].Center = new CGPoint (xposition * 1, ListButtons [0].Center.Y);
				ListButtons [1].Center = new CGPoint (xposition * 3, ListButtons [1].Center.Y);
				ListButtons [2].Center = new CGPoint (xposition * 5, ListButtons [2].Center.Y);
				ListButtons [3].Center = new CGPoint (xposition * 7, ListButtons [3].Center.Y);
			}
		}

		private void CreateTimelineButton(){
			var timelineButton = new UIMultiActionButton ("empty_house", "full_house");

			timelineButton.TouchUpInside += delegate {

				if (timelineButton.IsOn){
					ScrollsUp();
					return;
				}

				UnselectAllButtons();
				timelineButton.SetFullImage();
				SwitchScreen(0);
			};

			ListButtons.Add (timelineButton);	
		}

		private void CreateFeaturedButton(){
			var featuredButton = new UIMultiActionButton ("empty_star", "full_star");

			featuredButton.TouchUpInside += delegate {

				if (featuredButton.IsOn){
					ScrollsUp();
					return;
				}

				UnselectAllButtons();
				featuredButton.SetFullImage();
				SwitchScreen(1, "Featured", UIFont.SystemFontOfSize(20, UIFontWeight.Medium), UIColor.White);
			};

			ListButtons.Add (featuredButton);
		}

		private void CreateDirectoryButton(){
			var directoryButton = new UIMultiActionButton ("empty_directory", "full_directory");

			directoryButton.TouchUpInside += delegate {

				if (directoryButton.IsOn){
					ScrollsUp();
					return;
				}

				UnselectAllButtons();
				directoryButton.SetFullImage();

				int screenNumber = 2;
				if (!UIMagazine.TheresMagazine)
				{
					screenNumber = 1;
				}
				SwitchScreen(screenNumber, "Directory", UIFont.SystemFontOfSize(20, UIFontWeight.Medium), UIColor.White);
			};

			ListButtons.Add (directoryButton);
		}

		private void CreateMapButton(){
			var mapButton = new UIMultiActionButton ("empty_map", "full_map");

			mapButton.TouchUpInside += delegate {

				if (mapButton.IsOn){
					return;
				}

				UnselectAllButtons();
				mapButton.SetFullImage();

				var mainMenuScreen = AppDelegate.NavigationController.TopViewController as MainMenuScreen;
				mainMenuScreen.ShowMap();
			};

			ListButtons.Add (mapButton);
		}

		private void UnselectAllButtons(){
			foreach (var button in ListButtons){
				button.SetEmptyImage();
			}
		}

		private void SwitchScreen(int indexOfCurrentViewController){
			var mainMenuScreen = AppDelegate.NavigationController.TopViewController as MainMenuScreen;
			mainMenuScreen.PlaceNewScreen (UIMagazine.Pages[indexOfCurrentViewController]);
		}

		private void SwitchScreen(int indexOfCurrentViewController, string screenName, UIFont newFont, UIColor newColor){
			var mainMenuScreen = AppDelegate.NavigationController.TopViewController as MainMenuScreen;
			mainMenuScreen.PlaceNewScreen (UIMagazine.Pages[indexOfCurrentViewController], screenName, newFont, newColor);
		}

		private void SwitchScreen(int indexOfCurrentViewController, string screenName, UIFont newFont){
			SwitchScreen (indexOfCurrentViewController, screenName, newFont, AppDelegate.ClubbyBlack);
		}

		private void ScrollsUp(){
			var mainMenuScreen = AppDelegate.NavigationController.TopViewController as MainMenuScreen;
			mainMenuScreen.ScrollsUp (true);
		}
	}

	public sealed class UIMultiActionButton : UIButton{
		UIImageView imageView;
		UIImageView lowerLine;

		private bool isOn;
		public bool IsOn{
			get{ return isOn; }
		}

		string EmptyImage;
		readonly string FullImage;

		public UIMultiActionButton(string emptyImage, string fullImage){
			EmptyImage = emptyImage;
			FullImage = fullImage;

			Frame = new CGRect (0, 0, 50, 50);
			lowerLine = new UIImageView ();
			lowerLine.Frame = new CGRect (0, Frame.Bottom - 3, Frame.Width, 3);
			lowerLine.Alpha = 0f;

			imageView = new UIImageView();
			imageView.Frame = new CGRect(0, 0, Frame.Size.Width * .5f, Frame.Size.Height * .5f);
			imageView.Center = new CGPoint(Frame.Width / 2, Frame.Height / 2);
			SetEmptyImage();

			this.AddSubviews(imageView, lowerLine);
		}

		public void SetEmptyImage(){
			imageView.SetImage("./screens/main/buttons/"+EmptyImage+".png");
			lowerLine.Alpha = 0f;
			imageView.TintColor = UIColor.White;
			this.TintColor = UIColor.White;
			isOn = false;
		}

		public void SetFullImage(){
			imageView.SetImage("./screens/main/buttons/"+FullImage+".png");
			lowerLine.Alpha = 1f;
			imageView.TintColor = AppDelegate.ClubbyBlue;
			this.TintColor = AppDelegate.ClubbyBlue;
			lowerLine.BackgroundColor = AppDelegate.ClubbyBlue;
			isOn = true;
		}
	}
}

