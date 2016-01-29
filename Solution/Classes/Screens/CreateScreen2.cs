using System;
using System.Drawing;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using AdvancedColorPicker;
using CoreText;

using System.Net.Http;

using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Facebook.CoreKit;
using Facebook.LoginKit;

namespace Solution
{
	public class CreateScreen2 : UIViewController
	{
		UIImageView banner;
		Board board;

		CGSize ColorSquareSize;
		CGPoint ColorSquarePosition1;
		CGPoint ColorSquarePosition2;

		UITextField hexView1;
		UITextField hexView2;

		public CreateScreen2 (Board _board) : base ("Board", null){
			board = _board;
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;

			ColorSquareSize = new CGSize (230, 40);
			ColorSquarePosition1 = new CGPoint (145, 350);
			ColorSquarePosition2 = new CGPoint (145, 461);

			InitializeInterface ();
		}

		private void InitializeInterface()
		{
			// loads center button
			LoadBanner ();
			LoadContent ();
		}

		private void LoadContent()
		{
			UIImage contentImage = UIImage.FromFile ("./createscreens/screen2/content2.jpg");
			UIImageView contentImageView = new UIImageView (new CGRect(0, banner.Frame.Bottom, contentImage.Size.Width / 2, contentImage.Size.Height / 2));
			contentImageView.Image = contentImage;

			View.AddSubviews (contentImageView);

			UIImageView color1 = CreateColorSquare (ColorSquareSize, 
				ColorSquarePosition1,
				AppDelegate.CityboardOrange.CGColor, 1);
			
			UITextField hash1 = new UITextField(new CGRect(color1.Frame.Right + 10, color1.Frame.Y + 10, 10, 20));
			hash1.Font = UIFont.SystemFontOfSize (20);
			hash1.BackgroundColor = UIColor.White;
			hash1.TextColor = AppDelegate.CityboardBlue;
			hash1.Text = "#";
			hash1.UserInteractionEnabled = false;

			hexView1 = new UITextField(new CGRect(hash1.Frame.Right + 3, hash1.Frame.Y, 100, hash1.Frame.Height));
			hexView1.Font = UIFont.SystemFontOfSize (20);
			hexView1.BackgroundColor = UIColor.White;
			hexView1.TextColor = AppDelegate.CityboardBlue;
			hexView1.KeyboardType = UIKeyboardType.Default;
			hexView1.ReturnKeyType = UIReturnKeyType.Done;
			hexView1.Text = CommonUtils.UIColorToHex(AppDelegate.CityboardOrange);
			hexView1.AutocapitalizationType = UITextAutocapitalizationType.None;

			hexView1.ShouldChangeCharacters = (textField, range, replacementString) => {
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return newLength <= 6;
			};

			hexView1.ShouldReturn += (textField) => {
				UIColor color = CommonUtils.HexToUIColor(hexView1.Text);

				color1.RemoveFromSuperview();

				color1 = CreateColorSquare (ColorSquareSize, 
					ColorSquarePosition1,
					color.CGColor, 1);

				View.AddSubview(color1);

				textField.ResignFirstResponder();

				return true;
			};

			UIImageView color2 = CreateColorSquare (ColorSquareSize, 
				ColorSquarePosition2,
				AppDelegate.CityboardBlue.CGColor, 2);

			UITextField hash2 = new UITextField(new CGRect(color2.Frame.Right + 10, color2.Frame.Y + 10, 10, 20));
			hash2.Font = UIFont.SystemFontOfSize (20);
			hash2.BackgroundColor = UIColor.White;
			hash2.TextColor = AppDelegate.CityboardBlue;
			hash2.Text = "#";
			hash2.UserInteractionEnabled = false;

			hexView2 = new UITextField(new CGRect(hash2.Frame.Right + 3, hash2.Frame.Y, 100, hash2.Frame.Height));
			hexView2.Font = UIFont.SystemFontOfSize (20);
			hexView2.BackgroundColor = UIColor.White;
			hexView2.TextColor = AppDelegate.CityboardBlue;
			hexView2.KeyboardType = UIKeyboardType.Default;
			hexView2.ReturnKeyType = UIReturnKeyType.Done;
			hexView2.Text = CommonUtils.UIColorToHex(AppDelegate.CityboardBlue);
			hexView2.AutocapitalizationType = UITextAutocapitalizationType.None;

			hexView2.ShouldChangeCharacters = (textField, range, replacementString) => {
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return newLength <= 6;
			};

			hexView2.ShouldReturn += (textField) => {
				textField.ResignFirstResponder();
				return true;
			};


			View.AddSubviews (color1, color2, hash1, hexView1, hash2, hexView2);
		}

		private UIImageView CreateColorSquare(CGSize size, CGPoint center, CGColor startcolor, int numberOfView)
		{
			CGRect frame = new CGRect (0, 0, size.Width, size.Height);

			UIGraphics.BeginImageContext (new CGSize(frame.Size.Width, frame.Size.Height));
			CGContext context = UIGraphics.GetCurrentContext ();

			context.SetFillColor(startcolor);
			context.FillRect(frame);

			UIImage orange = UIGraphics.GetImageFromCurrentImageContext ();
			UIImageView uiv = new UIImageView (orange);
			uiv.Center = center;

			UITapGestureRecognizer tap = new UITapGestureRecognizer (async (tg) => {

				UIColor color = await ColorPickerViewController.PresentAsync (
					NavigationController, 
					"Pick a Color",
					View.BackgroundColor);

				UIGraphics.BeginImageContextWithOptions (new CGSize(BoardInterface.ScreenWidth, BoardInterface.ScreenHeight), false, 0);
				CGContext ctx = UIGraphics.GetCurrentContext ();

				ctx.SetFillColor(color.CGColor);
				ctx.FillRect(frame);

				UIImage img = UIGraphics.GetImageFromCurrentImageContext ();
				uiv.Image = img;

				switch(numberOfView)
				{
				case 1:
					hexView1.Text = CommonUtils.UIColorToHex(color);
					break;
				case 2:
					hexView2.Text = CommonUtils.UIColorToHex(color);
					break;
				}

			});

			uiv.AddGestureRecognizer (tap);
			uiv.UserInteractionEnabled = true;

			return uiv;
		}


		private void LoadBanner()
		{
			UIImage bannerImage = UIImage.FromFile ("./createscreens/screen2/banner.jpg");

			banner = new UIImageView(new CGRect(0,0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
			banner.Image = bannerImage;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 3){
					NavigationController.PopViewController(false);
				} else if (tg.LocationInView(this.View).X > (AppDelegate.ScreenWidth / 3) * 2){
					CreateScreen3 createScreen3 = new CreateScreen3();
					NavigationController.PushViewController(createScreen3, false);
				}
			});

			banner.UserInteractionEnabled = true;
			banner.AddGestureRecognizer (tap);
			banner.Alpha = .95f;
			View.AddSubview (banner);
		}
	}
}