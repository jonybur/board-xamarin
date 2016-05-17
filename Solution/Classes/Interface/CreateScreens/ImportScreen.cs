using System;
using System.Collections.Generic;
using System.Threading;
using Board.Facebook;
using Board.Screens.Controls;
using Board.Utilities;
using CoreGraphics;
using UIKit;

namespace Board.Interface.CreateScreens
{
	public class ImportScreen : UIViewController
	{
		UIMenuBanner Banner;
		UIScrollView ScrollView;
		Action<FacebookElement> OnReturn;
		List<UIMenuButton> Buttons;
		string TypeOfImport;
		bool pressed;

		public ImportScreen(string typeOfImport, Action<FacebookElement> onReturn)
		{
			TypeOfImport = typeOfImport;
			OnReturn = onReturn;
		}

		public override void ViewDidLoad()
		{
			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;

			LoadBanner ();
			View.BackgroundColor = UIColor.White;

			Buttons = new List<UIMenuButton> ();

			FacebookUtils.MakeGraphRequest (UIBoardInterface.board.FBPage.Id, TypeOfImport, Completion);
		}

		public override void ViewDidAppear(bool animated)
		{
			Banner.SuscribeToEvents ();
		}

		public override void ViewDidDisappear (bool animated)
		{
			UnsuscribeToEvents ();
			MemoryUtility.ReleaseUIViewWithChildren (View);
		}

		private void Completion(List<FacebookElement> ElementList)
		{
			LoadEvents (ElementList);
			SuscribeToEvents ();
		}
			
		private void LoadEvents(List<FacebookElement> ElementList)
		{
			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			ScrollView.ContentSize = new CGSize (AppDelegate.ScreenWidth, 80 * ElementList.Count + Banner.Frame.Height + ElementList.Count + 1);
			float yPosition = (float)Banner.Frame.Height;

			int i = 0;
			foreach (FacebookElement fbelement in ElementList) {
				UIOneLineMenuButton button;

				// for importing events
				if (fbelement is FacebookEvent) {
					FacebookEvent fbevent = (FacebookEvent)fbelement;
					button = CreateButton (yPosition, fbevent.Name, fbevent);
				}

				// for importing posts
				else if (fbelement is FacebookPost) {
					FacebookPost fbpost = (FacebookPost)fbelement;
					string text = string.Empty;
					if (fbpost.Message != "<null>") {
						// ignores stories
						text = fbpost.Message;
					} else {
						continue;
					}
					button = CreateButton (yPosition, text, fbpost);

				// in case of an error
				} else {
					button = new UIOneLineMenuButton ();
				}

				yPosition += (float)button.Frame.Height + 1;
				ScrollView.AddSubview (button);
				Buttons.Add (button);
				i++;
			}

			View.AddSubview (ScrollView);
			View.AddSubview (Banner);
		}

		private UIOneLineMenuButton CreateButton(float yPosition, string content, FacebookElement fbelement)
		{
			UIOneLineMenuButton fbeventButton = new UIOneLineMenuButton (yPosition);

			if (content.Length > 35) {
				content = content.Substring (0, 35) + "...";
			}

			fbeventButton.SetLabel (content);
			fbeventButton.SetUnpressedColors ();

			fbeventButton.TapEvent += (sender, e) => {
				if (!pressed)
				{
					pressed = true;
					fbeventButton.SetPressedColors();

					Thread thread = new Thread(new ThreadStart(PopOut));
					thread.Start();

					OnReturn(fbelement);
				}
			};

			return fbeventButton;
		}

		protected void LoadBanner()
		{
			Banner = new UIMenuBanner ("IMPORT", "arrow_left");

			var tap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4) {
					NavigationController.PopViewController(true);
				}
			});

			Banner.AddTap (tap);

			View.AddSubview (Banner);
		}

		private void PopOut()
		{
			Thread.Sleep (300);
			InvokeOnMainThread(() => NavigationController.PopViewController(true));
		}

		private void SuscribeToEvents()
		{
			foreach (UIMenuButton sb in Buttons) {
				sb.SuscribeToEvent ();
			}
		}

		private void UnsuscribeToEvents()
		{
			foreach (UIMenuButton sb in Buttons) {
				sb.UnsuscribeToEvent ();
			}

			Banner.UnsuscribeToEvents ();
		}

	}
}

