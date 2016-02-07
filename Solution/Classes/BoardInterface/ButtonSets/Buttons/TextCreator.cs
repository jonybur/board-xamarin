using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;

namespace Solution
{
	public class TextCreator : UIViewController
	{
		private const int sizePicture = 60;
		private const int fontSize = 18;

		const int wborder = 50;
		const int hborder = 65;

		private UIView uiView;
		private UITextView textView;

		public TextCreator (UINavigationController navigationController, Action refreshContent)
		{
			uiView = new UIView(new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));

			uiView.BackgroundColor = UIColor.White;

			textView = CreateTextView();
			uiView.Add (textView);

			View.Add (uiView);

			UITapGestureRecognizer tapGesture= new UITapGestureRecognizer ((tg) => {
				textView.ResignFirstResponder();
			});

			UINavigationBar uiNavigationBar = CreateNavigationBar (navigationController, refreshContent, textView);
			View.AddSubview (uiNavigationBar);


			View.AddGestureRecognizer(tapGesture);
		}

		private UINavigationBar CreateNavigationBar(UINavigationController navigationController, Action refreshContent, UITextView textview)
		{
			var frame = new CGRect (0,0,AppDelegate.ScreenWidth, hborder);
			UINavigationBar navBar = new UINavigationBar (frame);
			UINavigationItem[] item = new UINavigationItem[1];

			UIBarButtonItem rightBarButtonItem = new UIBarButtonItem ("Done", UIBarButtonItemStyle.Plain, null);
			UIBarButtonItem leftBarButtonItem = new UIBarButtonItem ("Cancel", UIBarButtonItemStyle.Plain, null);

			leftBarButtonItem.Clicked+= (sender, e) => {

				if (textview.Text.Length > 0)
				{
					var alert = new UIAlertView ("Discard Post?", "Your message will be discarded", null, "Keep", new string[] {"Discard"});
					alert.Clicked += (s, b) => {
						if (b.ButtonIndex == 0)
						{ return; }

						CloseTextCreator(navigationController);
					};
					alert.Show();
				} 
				else 
				{
					CloseTextCreator(navigationController);
				}

			};

			rightBarButtonItem.Enabled = false;

			textview.Changed += (sender, e) => {
				if (textview.Text.Length > 0)
				{ rightBarButtonItem.Enabled = true; } 
				else 
				{ rightBarButtonItem.Enabled = false; }
			};

			rightBarButtonItem.Clicked+= async (sender, e) => {
				TextBox tb = new TextBox (null, null, textView.Text, new CGRect(0,0, 200, 140), 0);

				await Preview.Initialize (tb, BoardInterface.scrollView.ContentOffset, refreshContent, navigationController);
				// shows the image preview so that the user can position the image
				BoardInterface.scrollView.AddSubview(Preview.GetUIView());

				// switches to confbar
				ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.ConfirmationBar);
				navigationController.DismissViewController(true, null);
				View.Dispose();
			};

			item [0] = new UINavigationItem ("Write");
			item [0].RightBarButtonItem = rightBarButtonItem;
			item [0].LeftBarButtonItem = leftBarButtonItem;

			navBar.SetItems (item,false);

			return navBar;
		}

		private void CloseTextCreator (UINavigationController navigationController)
		{				
			navigationController.DismissViewController(true, null);
			View.Dispose();
		}

		private UILabel CreateLabel()
		{
			UILabel label = new UILabel (new CGRect (0, 80, AppDelegate.ScreenWidth, fontSize + 10)) {
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.Clear,
				TextColor = UIColor.White,
				Font = UIFont.SystemFontOfSize (fontSize),
				Text = "Write Your Message",
				AdjustsFontSizeToFitWidth = true
			};

			return label;
		}

		private UITextView CreateTextView()
		{
			var frame = new CGRect(wborder + 20, hborder + 23, 
										AppDelegate.ScreenWidth - wborder - 23,
										AppDelegate.ScreenHeight - hborder - 20);

			UITextView textview = new UITextView(frame);

			textview.KeyboardType = UIKeyboardType.Default;
			textview.ReturnKeyType = UIReturnKeyType.Default;
			textview.EnablesReturnKeyAutomatically = true;
			textview.BackgroundColor = UIColor.White;
			textview.TextColor = UIColor.Black;
			textview.Font = UIFont.SystemFontOfSize (fontSize);;

			return textview;
		}


		private void LaunchTextBoxPreview()
		{
		}

	}
}

