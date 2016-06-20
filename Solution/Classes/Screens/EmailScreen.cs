using System.Drawing;
using Board.Infrastructure;
using Board.Screens.Controls;
using CoreGraphics;
using Facebook.LoginKit;
using System.Net;
using Foundation;
using System;
using System.Text.RegularExpressions;
using CoreAnimation;
using UIKit;

namespace Board.Screens
{
	public class EmailScreen : UIViewController
	{
		UIMenuBanner Banner;
		UITextField UserField, PasswordField;
		UIActionButton NextButton;
		UIWindow window;

		private void LoadBanner()
		{
			Banner = new UIMenuBanner ("LOG IN", "cross_left");

			var tap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					AppDelegate.NavigationController.DismissViewController(true, null);
				}
			});

			Banner.AddTap (tap);
			Banner.SuscribeToEvents ();
		}

		public override void ViewDidLoad ()
		{
			View.BackgroundColor = UIColor.White;

			LoadBanner ();

			UserField = new UITextField (new CGRect(15, Banner.Frame.Bottom + 20, AppDelegate.ScreenWidth - 15, 40));
			UserField.Placeholder = "EMAIL ADDRESS";
			UserField.Font = UIFont.SystemFontOfSize (16);
			UserField.TextColor = AppDelegate.BoardBlack;
			UserField.AutocapitalizationType = UITextAutocapitalizationType.None;
			UserField.KeyboardType = UIKeyboardType.EmailAddress;
			UserField.ReturnKeyType = UIReturnKeyType.Next;

			UserField.ShouldReturn = delegate(UITextField textField) {
				textField.ResignFirstResponder();
				PasswordField.BecomeFirstResponder();
				return false;
			};

			PasswordField = new UITextField (new CGRect(15, UserField.Frame.Bottom + 10, AppDelegate.ScreenWidth - 15, 40));
			PasswordField.Placeholder = "PASSWORD";
			PasswordField.Font = UIFont.SystemFontOfSize (16);
			PasswordField.SecureTextEntry = true;
			PasswordField.TextColor = AppDelegate.BoardBlack;
			PasswordField.AutocapitalizationType = UITextAutocapitalizationType.None;
			PasswordField.ReturnKeyType = UIReturnKeyType.Done;

			PasswordField.ShouldReturn = delegate {
				PasswordField.ResignFirstResponder();
				return false;
			};

			NextButton = new UIActionButton ("NEXT");

			bool nextTapped = false;
			NextButton.TouchUpInside += delegate {
				if (!nextTapped){
					nextTapped = true;

					string errorList = string.Empty;

					if (!VerifyEmail()){
						errorList += "Please write a valid email address";
					}

					if (PasswordField.Text.Length < 6){
						if (errorList.Length > 0){
							errorList += "\n\n";
						}

						errorList += "Password must be at least 6 characters long";
					}

					if (errorList.Length > 0){

						var alert = UIAlertController.Create("Invalid information", errorList, UIAlertControllerStyle.Alert);
						alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Cancel, HideWindow));

						window = new UIWindow(new CGRect(0,0,AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
						window.RootViewController = new UIViewController();
						window.WindowLevel = UIWindowLevel.Alert + 1;

						window.MakeKeyAndVisible();
						window.RootViewController.PresentViewController(alert, true, null);

					} else {

						bool couldLogIn = CloudController.LogInEmail(UserField.Text, PasswordField.Text);

						if (couldLogIn){

							AppDelegate.containerScreen = new ContainerScreen ();
							AppDelegate.NavigationController.DismissViewController(true, delegate {
								AppDelegate.NavigationController.PushViewController(AppDelegate.containerScreen, true);
							});

						} else {

							var alert = UIAlertController.Create("Error", "Incorrect password", UIAlertControllerStyle.Alert);
							alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Cancel, HideWindow));

							window = new UIWindow(new CGRect(0,0,AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
							window.RootViewController = new UIViewController();
							window.WindowLevel = UIWindowLevel.Alert + 1;

							window.MakeKeyAndVisible();
							window.RootViewController.PresentViewController(alert, true, null);

						}
					}

					nextTapped = false;
				}
			};

			LoadWarning ();

			View.AddSubviews (UserField, PasswordField, Banner, NextButton);
		}

		private void HideWindow(UIAlertAction action)
		{
			window.Hidden = true;
		}

		private bool VerifyEmail(){
			return Regex.IsMatch(UserField.Text, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
		}

		private void LoadWarning (){
			var label = new UITextView ();
			label.Frame = new CGRect (5, PasswordField.Frame.Bottom + 20, AppDelegate.ScreenWidth - 10, 0);
			label.Font = UIFont.SystemFontOfSize (11, UIFontWeight.Light);
			label.TextColor = AppDelegate.BoardBlack;
			label.Text = "This will create a new account in case\nthe email address is not already registered\n\n" +
				"Passwords must be at least 6 characters long";
			label.TextAlignment = UITextAlignment.Center;
			label.ScrollEnabled = false;
			label.Editable = false;
			label.Selectable = false;
			label.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);

			var size = label.SizeThatFits (label.Frame.Size);
			label.Frame = new CGRect (label.Frame.X, label.Frame.Y, label.Frame.Width, size.Height);

			View.AddSubview (label);
		}
	}
}

