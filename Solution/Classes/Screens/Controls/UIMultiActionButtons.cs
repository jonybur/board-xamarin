using System;
using System.Collections.Generic;
using CoreGraphics;
using Haneke;
using UIKit;

namespace Board.Screens.Controls
{
	public sealed class UIMultiActionButtons : UIView
	{
		public const int Height = 50;
		List<UIMultiActionButton> ListButtons;

		public UIMultiActionButtons ()
		{
			Frame = new CGRect(0, AppDelegate.ScreenHeight - Height, AppDelegate.ScreenWidth, Height);
			BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
			var backgroundView = new UIImageView (new CGRect(0,0,Frame.Width, Frame.Height));
			backgroundView.BackgroundColor = UIColor.FromRGB(249, 249, 249);
			backgroundView.Alpha = .95f;
			AddSubview (backgroundView);

			ListButtons = new List<UIMultiActionButton> ();
			UserInteractionEnabled = true;

			CreateTimelineButton ();
			CreateFeaturedButton ();
			CreateDirectoryButton ();
			CreateMapButton ();

			foreach (var button in ListButtons) {
				AddSubview (button);
			}

			float xposition = AppDelegate.ScreenWidth / 8;

			if (ListButtons.Count == 4) {
				ListButtons [0].SetFullImage ();
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
				SwitchScreen(0, "Board", AppDelegate.Narwhal26);
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
				SwitchScreen(1, "Weekly Features", UIFont.SystemFontOfSize(20, UIFontWeight.Medium));
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
				SwitchScreen(2, "Directory", UIFont.SystemFontOfSize(20, UIFontWeight.Medium));
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

				var containerScreen = AppDelegate.NavigationController.TopViewController as ContainerScreen;
				var mainMenuScreen = containerScreen.CurrentScreenViewController as MainMenuScreen;
				mainMenuScreen.ShowMap();
			};

			ListButtons.Add (mapButton);
		}

		private void UnselectAllButtons(){
			foreach (var button in ListButtons){
				button.SetEmptyImage();
			}
		}

		private void SwitchScreen(int indexOfCurrentViewController, string screenName, UIFont newFont, UIColor newColor){
			var containerScreen = AppDelegate.NavigationController.TopViewController as ContainerScreen;
			var mainMenuScreen = containerScreen.CurrentScreenViewController as MainMenuScreen;

			mainMenuScreen.PlaceNewScreen (UIMagazineServices.Pages[indexOfCurrentViewController].ContentDisplay, screenName, newFont, newColor);
		}

		private void SwitchScreen(int indexOfCurrentViewController, string screenName, UIFont newFont){
			SwitchScreen (indexOfCurrentViewController, screenName, newFont, AppDelegate.BoardBlack);
		}

		private void ScrollsUp(){
			var containerScreen = AppDelegate.NavigationController.TopViewController as ContainerScreen;
			var mainMenuScreen = containerScreen.CurrentScreenViewController as MainMenuScreen;

			mainMenuScreen.ScrollsUp ();
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
			lowerLine.Frame = new CGRect (0, Frame.Bottom - 2, Frame.Width, 2);
			lowerLine.BackgroundColor = AppDelegate.BoardBlack;
			lowerLine.Alpha = 0f;

			imageView = new UIImageView();
			imageView.Frame = new CGRect(0, 0, Frame.Size.Width * .5f, Frame.Size.Height * .5f);
			imageView.Center = new CGPoint(Frame.Width / 2, Frame.Height / 2);
			SetEmptyImage();
			imageView.TintColor = AppDelegate.BoardBlack;

			this.AddSubviews(imageView, lowerLine);
		}

		public void SetEmptyImage(){
			imageView.SetImage(UIImage.FromFile("./screens/main/buttons/"+EmptyImage+".png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), EmptyImage);
			lowerLine.Alpha = 0f;
			isOn = false;
		}

		public void SetFullImage(){
			imageView.SetImage(UIImage.FromFile("./screens/main/buttons/"+FullImage+".png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), FullImage);
			lowerLine.Alpha = 1f;
			isOn = true;
		}
	}
}

