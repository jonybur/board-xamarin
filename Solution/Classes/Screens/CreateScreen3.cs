﻿using System;
using System.Drawing;

using Foundation;
using UIKit;
using Facebook.CoreKit;
using CoreGraphics;

using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace Solution
{
	public class CreateScreen3 : UIViewController
	{
		const int hborder = 65;

		// hborder is navbar + completionbar height
		SuscriptionButton[] numberButtons;
		Board board;

		UIImageView orangeRectangle;
		UIImageView banner;

		int? selectedIndex;

		bool nextEnabled;

		public CreateScreen3 (Board _board){
			board = _board;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			selectedIndex = null;

			LoadInterface ();
		}

		private void LoadInterface()
		{
			View.BackgroundColor = UIColor.FromRGB (150, 150, 150);

			LoadBanner ();

			LoadControls ();
		}

		private void LoadBanner()
		{
			UIImage bannerImage = UIImage.FromFile ("./createscreens/screen3/banner.jpg");

			banner = new UIImageView(new CGRect(0,0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
			banner.Image = bannerImage;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {

				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					NavigationController.PopViewController(false);
				} else if (tg.LocationInView(this.View).X > (AppDelegate.ScreenWidth / 4) * 3 && nextEnabled){
					NavigationController.PopToViewController(NavigationController.ViewControllers[NavigationController.ViewControllers.Length - 4], false);
				}
			});


			orangeRectangle = CreateColorSquare (new CGSize (75, 60), 
				new CGPoint ((AppDelegate.ScreenWidth / 4) * 3 + 60, 25),
				AppDelegate.CityboardOrange.CGColor);

			NextButtonEnabled(false);

			banner.AddSubview (orangeRectangle);
			banner.UserInteractionEnabled = true;
			banner.AddGestureRecognizer (tap);
			banner.Alpha = .95f;
			View.AddSubview (banner);
		}

		private void NextButtonEnabled(bool enabled)
		{
			nextEnabled = enabled;

			if (nextEnabled) {
				orangeRectangle.Alpha = 0f;
			} else {
				orangeRectangle.Alpha = .5f;
			}
		}

		private UIImageView CreateColorSquare(CGSize size, CGPoint center, CGColor startcolor)
		{
			CGRect frame = new CGRect (0, 0, size.Width, size.Height);

			UIGraphics.BeginImageContext (new CGSize(frame.Size.Width, frame.Size.Height));
			CGContext context = UIGraphics.GetCurrentContext ();

			context.SetFillColor(startcolor);
			context.FillRect(frame);

			UIImage orange = UIGraphics.GetImageFromCurrentImageContext ();
			UIImageView uiv = new UIImageView (orange);
			uiv.Center = center;

			return uiv;
		}

		private void LoadControls()
		{
			const int cantSuscriptionButtons = 2;

			float yposition = (float)(hborder + banner.Frame.Size.Height) + 20;
	
			yposition = (float)banner.Frame.Bottom;

			float heightButton = (float)(AppDelegate.ScreenHeight - banner.Frame.Height) / cantSuscriptionButtons;
			float widthButton = AppDelegate.ScreenWidth;

			numberButtons = new SuscriptionButton[cantSuscriptionButtons];
			float xposition = 0;

			for (int i = 0; i < cantSuscriptionButtons; i++) {
				SuscriptionButton but;
				CGRect frame = new CGRect (xposition, yposition, widthButton, heightButton);

				switch (i) {
				case 0:
					but = CreateSuscriptionButton (i, "Basic", "· Engage with your audience\n· Post content to all of your\nsocial media sites\n· Acquire new customers", "Free", frame);
					break;	
				case 1:
					but = CreateSuscriptionButton (i, "Premium", "· Target to a specific audience\n· Get daily analytics\n· Broaden your CityBoard’s\narea range", "TBA", frame);
					but.AddSubview(CreateTopLayer (but.Frame, UIColor.Black.CGColor, UIImage.FromFile("./createscreens/screen3/lock.png")));
					but.Enabled = false;
					break;
				case 2:
					but = CreateSuscriptionButton (i, string.Empty, string.Empty, string.Empty, frame);
					but.AddSubview(CreateTopLayer (but.Frame, UIColor.Black.CGColor, UIImage.FromFile("./createscreens/screen3/lock.png")));
					but.Enabled = false;
					break;
				default:
					but = CreateSuscriptionButton (i, string.Empty, string.Empty, string.Empty, frame);
					break;
				}

				yposition += heightButton + 1;
				numberButtons [i] = but;
			}

			View.AddSubviews (numberButtons);
		}

		private UIImageView CreateTopLayer(CGRect frame, CGColor color, UIImage image)
		{ 
			UIGraphics.BeginImageContextWithOptions (new SizeF((float)frame.Width, (float)frame.Height), false, 0);
			CGContext context = UIGraphics.GetCurrentContext ();
			context.SetFillColor(color);
			context.FillRect(new RectangleF(0,0,(float)frame.Width, (float)frame.Height));

			UIImage black = UIGraphics.GetImageFromCurrentImageContext ();
			UIImageView blackLocked = new UIImageView(black);
			blackLocked.Alpha = .5f;

			UIImage lockImage = image;
			UIImageView lockIV = new UIImageView();
			lockIV.Frame = new CGRect (0, 0, lockImage.Size.Width / 2, lockImage.Size.Height / 2);
			lockIV.Image = lockImage;
			lockIV.Center = new CGPoint (frame.Width / 2, frame.Height / 2);

			blackLocked.AddSubview (lockIV);

			return blackLocked;
		}

		class SuscriptionButton : UIButton{
			public int Index;
			public string Name;
			public string Description;
			public UILabel NameLabel;
			public UILabel PriceLabel;
			public UITextView DescriptionView;
		}

		private SuscriptionButton CreateSuscriptionButton(int index, string name, string description, string price, CGRect frame)
		{
			SuscriptionButton button = new SuscriptionButton ();
			button.Frame = frame;

			UIColor unselectedColor = UIColor.FromRGB (230, 230, 230);

			button.BackgroundColor = unselectedColor;
			button.Index = index;
			button.Name = name;
			button.Description = description;

			UIFont nameFont = UIFont.SystemFontOfSize (24);
			CGSize labelSize = name.StringSize (nameFont);

			button.NameLabel = new UILabel (new CGRect(0, 0, button.Frame.Width, labelSize.Height));
			button.NameLabel.Center = new CGPoint (button.Frame.Width / 2 + 30, button.Frame.Height / 2 - labelSize.Height - 30);
			button.NameLabel.Font = nameFont;
			button.NameLabel.Text = name;
			button.NameLabel.UserInteractionEnabled = false;
			button.NameLabel.TextColor = AppDelegate.CityboardBlue;
			button.NameLabel.TextAlignment = UITextAlignment.Left;
			button.AddSubview (button.NameLabel);

			UIFont descriptionFont = UIFont.SystemFontOfSize (18);
			button.DescriptionView = new UITextView(new CGRect(button.NameLabel.Frame.Left, button.NameLabel.Frame.Bottom + 10, (button.Frame.Width / 3) * 2, button.Frame.Height));
			button.DescriptionView.Font = descriptionFont;
			button.DescriptionView.Text = description;
			button.DescriptionView.UserInteractionEnabled = false;
			button.DescriptionView.TextColor = AppDelegate.CityboardBlue;
			button.DescriptionView.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
			button.DescriptionView.TextAlignment = UITextAlignment.Left;
			button.AddSubview (button.DescriptionView);

			UIFont priceFont = UIFont.SystemFontOfSize (28);
			CGSize priceSize = price.StringSize (priceFont);
			button.PriceLabel = new UILabel (new CGRect(button.DescriptionView.Frame.Right, 0, priceSize.Width, priceSize.Height));
			button.PriceLabel.Center = new CGPoint (button.DescriptionView.Frame.Right + priceSize.Width - 10, button.Frame.Height / 2);
			button.PriceLabel.Font = priceFont;
			button.PriceLabel.Text = price;
			button.PriceLabel.UserInteractionEnabled = false;
			button.PriceLabel.TextColor = AppDelegate.CityboardBlue;
			button.PriceLabel.TextAlignment = UITextAlignment.Left;
			button.AddSubview (button.PriceLabel);

			button.TouchUpInside += (sender, e) => {
				if (selectedIndex != null)
				{
					// there's one selected
					if (selectedIndex != button.Index)
					{
						// if the one selected is different than the one that has been pressed

						// unselect the selected one

						numberButtons[(int)selectedIndex].BackgroundColor = unselectedColor;
						numberButtons[(int)selectedIndex].NameLabel.TextColor = AppDelegate.CityboardBlue;
						numberButtons[(int)selectedIndex].DescriptionView.TextColor = AppDelegate.CityboardBlue;
						numberButtons[(int)selectedIndex].PriceLabel.TextColor = AppDelegate.CityboardBlue;

						// select the pressed one

						numberButtons[button.Index].BackgroundColor = AppDelegate.CityboardBlue;
						numberButtons[button.Index].NameLabel.TextColor = UIColor.White;
						numberButtons[button.Index].DescriptionView.TextColor = UIColor.White;
						numberButtons[button.Index].PriceLabel.TextColor = UIColor.White;

						selectedIndex = button.Index;
					}
					else {
						// unselect the selected one

						numberButtons[(int)selectedIndex].BackgroundColor = unselectedColor;
						numberButtons[(int)selectedIndex].NameLabel.TextColor = AppDelegate.CityboardBlue;
						numberButtons[(int)selectedIndex].DescriptionView.TextColor = AppDelegate.CityboardBlue;
						numberButtons[(int)selectedIndex].PriceLabel.TextColor = AppDelegate.CityboardBlue;

						selectedIndex = null;
					}

				}
				else {
					// nothing has been selected yet

					// select the one that has been pressed
					numberButtons[button.Index].BackgroundColor = AppDelegate.CityboardBlue;
					numberButtons[button.Index].NameLabel.TextColor = UIColor.White;
					numberButtons[button.Index].DescriptionView.TextColor = UIColor.White;
					numberButtons[button.Index].PriceLabel.TextColor = UIColor.White;

					selectedIndex = button.Index;
				}

				if (selectedIndex != null)
				{
					NextButtonEnabled(true);
				}
				else {
					NextButtonEnabled(false);
				}
			};

			return button;

		}

	}
}

