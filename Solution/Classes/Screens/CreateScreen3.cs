using System.Drawing;
using Board.Utilities;
using CoreGraphics;
using UIKit;
using Board.Screens.Controls;

namespace Board.Screens
{
	public class CreateScreen3 : UIViewController
	{
		const int hborder = 65;

		// hborder is navbar + completionbar height
		SuscriptionButton[] numberButtons;
		Board.Schema.Board board;

		UIImageView orangeRectangle;
		MenuBanner Banner;

		int? selectedIndex;

		bool nextEnabled;

		public CreateScreen3 (Board.Schema.Board _board){
			board = _board;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			selectedIndex = null;

			LoadInterface ();
		}

		public override void ViewDidAppear(bool animated)
		{
			Banner.SuscribeToEvents ();
		}

		public override void ViewDidDisappear(bool animated)
		{
			Banner.UnsuscribeToEvents ();
		}

		private void LoadInterface()
		{
			View.BackgroundColor = UIColor.White;

			LoadBanner ();

			LoadControls ();
		}

		private void LoadBanner()
		{
			Banner = new MenuBanner ("./screens/create/3/banner/" + AppDelegate.PhoneVersion + ".jpg");

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {

				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					AppDelegate.NavigationController.PopViewController(false);
				} else if (tg.LocationInView(this.View).X > (AppDelegate.ScreenWidth / 4) * 3 && nextEnabled){

					if (AppDelegate.ServerActive)
					{
						string json = "{ \"name\": \"" + board.Name + "\", " +
							"\"address\": \"" + board.Location  + "\", " +
							"\"logoURL\": \"" + "http://www.getonboard.us/wp-content/uploads/2016/02/orange_60.png" + "\", " +
							"\"mainColorCode\": \"" + CommonUtils.UIColorToHex(board.MainColor)  + "\", " +
							"\"secondaryColorCode\": \"" + CommonUtils.UIColorToHex(board.SecondaryColor)   +"\" }";

						string result = CommonUtils.JsonPOSTRequest ("http://192.168.1.101:5000/api/board?authToken=" + AppDelegate.EncodedBoardToken, json);
					}

					AppDelegate.ListNewBoards.Add(board);

					var containerScreen = AppDelegate.NavigationController.ViewControllers[AppDelegate.NavigationController.ViewControllers.Length - 4] as ContainerScreen;
					if (containerScreen!= null)
					{
						containerScreen.LoadBusinessScreen();
					}

					AppDelegate.NavigationController.PopToViewController (containerScreen, false);
				}
			});


			orangeRectangle = CreateColorSquare (new CGSize (75, 60), 
				new CGPoint ((AppDelegate.ScreenWidth / 4) * 3 + 60, 25),
				AppDelegate.BoardOrange.CGColor);

			NextButtonEnabled(false);

			Banner.AddSubview (orangeRectangle);

			Banner.AddTap (tap);

			View.AddSubview (Banner);
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

			UIImageView uiv;
			using (UIImage img = UIGraphics.GetImageFromCurrentImageContext ()) {
				uiv = new UIImageView (img);
			}
			uiv.Center = center;

			return uiv;
		}

		private void LoadControls()
		{
			const int cantSuscriptionButtons = 2;

			float yposition = (float)(hborder + Banner.Frame.Size.Height) + 20;
	
			yposition = (float)Banner.Frame.Bottom;

			float heightButton = (float)(AppDelegate.ScreenHeight - Banner.Frame.Height) / cantSuscriptionButtons;
			float widthButton = AppDelegate.ScreenWidth;

			numberButtons = new SuscriptionButton[cantSuscriptionButtons];

			for (int i = 0; i < cantSuscriptionButtons; i++) {
				SuscriptionButton but;
				CGRect frame = new CGRect (0, yposition, widthButton, heightButton);

				switch (i) {
				case 0:
					but = CreateSuscriptionButton (i, "Basic", "· Create a Board\n· Engage with your audience\n· Post content to all of your\nsocial media sites", "Free", frame);
					break;	
				case 1:
					but = CreateSuscriptionButton (i, "Premium", "· Target to a specific audience\n· Get daily analytics\n· Broaden your Board’s\narea range", "TBA", frame);
					using (UIImage lockImage = UIImage.FromFile("./screens/create/3/lock.png"))
					{
						but.AddSubview(CreateTopLayer (but.Frame, UIColor.Black.CGColor, lockImage));
					}
					but.Enabled = false;
					break;
				case 2:
					but = CreateSuscriptionButton (i, string.Empty, string.Empty, string.Empty, frame);
					using (UIImage lockImage = UIImage.FromFile ("./screens/create/3/lock.png")) {
						but.AddSubview (CreateTopLayer (but.Frame, UIColor.Black.CGColor, lockImage));
					}
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

			UIImageView blackLocked;
			using (UIImage img = UIGraphics.GetImageFromCurrentImageContext ()) {
				blackLocked = new UIImageView(img);
			}
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

			UIColor unselectedColor = UIColor.White;

			button.BackgroundColor = unselectedColor;
			button.Index = index;
			button.Name = name;
			button.Description = description;

			UIFont nameFont = AppDelegate.Narwhal24;
			CGSize labelSize = name.StringSize (nameFont);

			button.NameLabel = new UILabel (new CGRect(0, 0, button.Frame.Width, labelSize.Height));
			button.NameLabel.Center = new CGPoint (button.Frame.Width / 2 + 30, button.Frame.Height / 2 - labelSize.Height - 30);
			button.NameLabel.Font = nameFont;
			button.NameLabel.Text = name;
			button.NameLabel.UserInteractionEnabled = false;
			button.NameLabel.TextColor = AppDelegate.BoardOrange;
			button.NameLabel.TextAlignment = UITextAlignment.Left;
			button.AddSubview (button.NameLabel);

			UIFont descriptionFont = AppDelegate.SystemFontOfSize18;
			button.DescriptionView = new UITextView(new CGRect(button.NameLabel.Frame.Left, button.NameLabel.Frame.Bottom + 10, (button.Frame.Width / 3) * 2, button.Frame.Height));
			button.DescriptionView.Font = descriptionFont;
			button.DescriptionView.Text = description;
			button.DescriptionView.UserInteractionEnabled = false;
			button.DescriptionView.TextColor = AppDelegate.BoardBlue;
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
			button.PriceLabel.TextColor = AppDelegate.BoardBlue;
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
						numberButtons[(int)selectedIndex].NameLabel.TextColor = AppDelegate.BoardOrange;
						numberButtons[(int)selectedIndex].DescriptionView.TextColor = AppDelegate.BoardBlue;
						numberButtons[(int)selectedIndex].PriceLabel.TextColor = AppDelegate.BoardBlue;

						// select the pressed one

						numberButtons[button.Index].BackgroundColor = AppDelegate.BoardLightBlue;
						numberButtons[button.Index].NameLabel.TextColor = UIColor.White;
						numberButtons[button.Index].DescriptionView.TextColor = UIColor.White;
						numberButtons[button.Index].PriceLabel.TextColor = UIColor.White;

						selectedIndex = button.Index;
					}
					else {
						// unselect the selected one

						numberButtons[(int)selectedIndex].BackgroundColor = unselectedColor;
						numberButtons[(int)selectedIndex].NameLabel.TextColor = AppDelegate.BoardOrange;
						numberButtons[(int)selectedIndex].DescriptionView.TextColor = AppDelegate.BoardBlue;
						numberButtons[(int)selectedIndex].PriceLabel.TextColor = AppDelegate.BoardBlue;

						selectedIndex = null;
					}

				}
				else {
					// nothing has been selected yet

					// select the one that has been pressed
					numberButtons[button.Index].BackgroundColor = AppDelegate.BoardLightBlue;
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

