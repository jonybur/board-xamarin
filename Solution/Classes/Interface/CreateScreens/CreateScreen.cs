using System;
using Board.Facebook;
using Board.Schema;
using Board.Screens.Controls;
using CoreGraphics;
using UIKit;

namespace Board.Interface.CreateScreens
{
	public class CreateScreen : UIViewController
	{
		public UIMenuBanner Banner;
		public UIScrollView ScrollView;
		public UIButton NextButton;
		public UIButton SaveButton;
		public PostToButtons ShareButtons;
		public Content content;

		public override void ViewDidLoad()
		{
			AppDelegate.NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			AppDelegate.NavigationController.NavigationBarHidden = true;
		}

		public override void ViewDidAppear (bool animated)
		{
			Banner.SuscribeToEvents ();
		}

		protected void LoadContent()
		{
			ScrollView = new UIScrollView(new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			ScrollView.BackgroundColor = UIColor.FromRGB(250, 250, 250);
			ScrollView.UserInteractionEnabled = true;

			View.AddSubview (ScrollView);
		}

		protected void LoadBanner(string toImport, Action<FacebookElement> onReturn, string title, string left_button = null, string right_button = null)
		{
			Banner = new UIMenuBanner (title, left_button, right_button);

			var leftTap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4) {
					AppDelegate.PopToViewControllerLikeDismissView(AppDelegate.BoardInterface);
				} else if (AppDelegate.ScreenWidth * 3 / 4 < tg.LocationInView(this.View).X && toImport != null) {
					
					if (UIBoardInterface.board.FacebookId != null)
					{
						ImportScreen importScreen = new ImportScreen(toImport, onReturn);
						AppDelegate.NavigationController.PushViewController(importScreen, true);
					} else { 
						UIAlertController alert = UIAlertController.Create("Board not connected to a page", "Do you wish to go to settings to connect to a Facebook page?", UIAlertControllerStyle.Alert);
						alert.AddAction (UIAlertAction.Create ("Later", UIAlertActionStyle.Cancel, null));
						alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, delegate {
							SettingsScreen settingsScreen = new SettingsScreen();
							AppDelegate.PushViewLikePresentView(settingsScreen);
						}));
						AppDelegate.NavigationController.PresentViewController (alert, true, null);
					}
				}
			});

			Banner.AddTap (leftTap);
			View.AddSubview (Banner);
		}

		protected void LoadNextButton(bool isEditing)
		{
			if (isEditing) {
				NextButton = new UIActionButton ("SAVE");
			} else {
				NextButton = new UIActionButton ("NEXT");
			}

			NextButton.Alpha = .95f;
			View.AddSubview (NextButton);
		}

		protected void LoadPostToButtons(float positionY)
		{
			ShareButtons = new PostToButtons(positionY);
			ScrollView.AddSubview (ShareButtons.View);
		}

	}
}

