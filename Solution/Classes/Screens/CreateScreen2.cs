using System;
using AdvancedColorPicker;
using Board.Interface;
using Board.Picker;
using Board.Utilities;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Board.Screens
{
	public class CreateScreen2 : UIViewController
	{
		UIImageView banner;
		Board.Schema.Board board;

		CGSize ColorSquareSize;
		CGPoint ColorSquarePosition1;
		CGPoint ColorSquarePosition2;

		UITextField hexView1;
		UITextField hexView2;

		float pushRight;
		float boardHeight;
		float topBarHeight;
		float bottomBarHeight;

		UIScrollView scrollView;

		// previewboard
		UIImageView boardView, circleTop, circleLower, logoBackground;
		UIImageView preview_mainLogo;

		public CreateScreen2 (Board.Schema.Board _board) : base ("Board", null){
			board = _board;
		}

		public override void ViewDidAppear(bool animated)
		{
			SuscribeToEvents ();
		}

		public override void ViewDidDisappear(bool animated)
		{
			UnsuscribeToEvents ();
		}

		private void SuscribeToEvents()
		{
			// Keyboard popup
			NSNotificationCenter.DefaultCenter.AddObserver
			(UIKeyboard.DidShowNotification,KeyBoardUpNotification);

			// Keyboard Down
			NSNotificationCenter.DefaultCenter.AddObserver
			(UIKeyboard.WillHideNotification,KeyBoardDownNotification);
		}

		private void UnsuscribeToEvents()
		{
			// Keyboard popup
			NSNotificationCenter.DefaultCenter.RemoveObserver (UIKeyboard.DidShowNotification);

			// Keyboard Down
			NSNotificationCenter.DefaultCenter.RemoveObserver (UIKeyboard.WillHideNotification);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.AutomaticallyAdjustsScrollViewInsets = false;

			ColorSquareSize = new CGSize (230, 40);
			ColorSquarePosition1 = new CGPoint (145, 350);
			ColorSquarePosition2 = new CGPoint (145, 461);

			InitializeInterface ();
		}

		private UIImageView GeneratePreviewBoard()
		{
			boardHeight = ((AppDelegate.ScreenWidth * AppDelegate.ScreenHeight) / BoardInterface.ScrollViewWidthSize);
			topBarHeight = ((50 * boardHeight) / AppDelegate.ScreenHeight);
			bottomBarHeight = ((100 * boardHeight) / AppDelegate.ScreenHeight);

			boardView = new UIImageView (new CGRect(0, AppDelegate.ScreenHeight - boardHeight, AppDelegate.ScreenWidth, boardHeight));

			using (UIImage img = UIImage.FromFile ("./boardinterface/backgrounds/intern.png")){
				UIImage tmp = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				circleTop = new UIImageView (tmp);
			}

			using (UIImage img = UIImage.FromFile ("./boardinterface/backgrounds/outer.png")) {
				UIImage tmp = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				circleLower = new UIImageView (tmp);
			}

			using (UIImage img = UIImage.FromFile ("./boardinterface/backgrounds/logobackground.png")) {
				UIImage tmp = img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				logoBackground = new UIImageView (tmp);
			}

			circleTop.Frame = new CGRect (0, 0, AppDelegate.ScreenWidth, boardHeight);
			circleLower.Frame = circleTop.Frame;
			logoBackground.Frame = circleTop.Frame;

			boardView.BackgroundColor = board.MainColor;
			circleTop.TintColor = board.MainColor;
			circleLower.TintColor = board.SecondaryColor;
			logoBackground.TintColor = UIColor.White;

			preview_mainLogo = GenerateBoardThumb (board.ImageView.Image, new CGPoint (boardView.Frame.Width / 2, boardView.Frame.Height / 2), false);

			boardView.AddSubviews (logoBackground, preview_mainLogo, circleTop, circleLower);

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				AppDelegate.boardInterface = new BoardInterface(board, true);
				NavigationController.PushViewController(AppDelegate.boardInterface, true);
			});

			boardView.AddGestureRecognizer (tap);
			boardView.UserInteractionEnabled = true;

			return boardView;
		}

		private void InitializeInterface()
		{
			scrollView = new UIScrollView(new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));

			// loads center button
			LoadBanner ();

			LoadContent ();

			View.AddSubview (scrollView);
		}

		private void LoadContent()
		{
			UIImageView contentImageView;
			using (UIImage contentImage = UIImage.FromFile ("./screens/create/2/content/" + AppDelegate.PhoneVersion + ".jpg")) {
				contentImageView = new UIImageView (new CGRect(0, banner.Frame.Bottom, contentImage.Size.Width / 2, contentImage.Size.Height / 2));
				contentImageView.Image = contentImage;
			}
			scrollView.AddSubviews (contentImageView);

			// top image
			UIImageView boardThumb;
			using (UIImage image = UIImage.FromFile  ("./screens/create/2/icon.png"))
			{
				boardThumb = GenerateBoardThumb (image, new CGPoint (AppDelegate.ScreenWidth / 2, 220), true);
			}
	
			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				ImagePicker ip = new ImagePicker (boardThumb.Subviews[0] as UIImageView, preview_mainLogo, board);

				NavigationController.PresentViewController (ip.UIImagePicker, true, null);
			});

			boardThumb.AddGestureRecognizer (tap);
			boardThumb.UserInteractionEnabled = true;

			using (UIImage image = UIImage.FromFile ("./screens/create/2/icon.png")) {
				board.ImageView = new UIImageView (image);
			}

			scrollView.AddSubview (boardThumb);

			// color selectors + hex

			GenerateColorSelectors ();

			// bottom preview

			UIImageView previewBoard = GeneratePreviewBoard ();
			scrollView.AddSubview (previewBoard); 
		}

		private void GenerateColorSelectors()
		{
			pushRight = 0f;

			if (AppDelegate.PhoneVersion == "6plus") {
				pushRight = 23;
			}

			board.MainColor = AppDelegate.BoardOrange;
			board.SecondaryColor = AppDelegate.BoardBlue;

			UIImageView color1 = CreateColorSquare (ColorSquareSize, 
				ColorSquarePosition1,
				AppDelegate.BoardOrange.CGColor, 1);

			// creates first colorsquare + first hash + first hexstring

			UITextField hash1 = new UITextField(new CGRect(color1.Frame.Right + pushRight + 10, color1.Frame.Y + 10, 10, 20));
			hash1.Font = UIFont.SystemFontOfSize (20);
			hash1.BackgroundColor = UIColor.White;
			hash1.TextColor =AppDelegate.BoardBlue;
			hash1.Text = "#";
			hash1.UserInteractionEnabled = false;

			hexView1 = new UITextField(new CGRect(hash1.Frame.Right + 3, hash1.Frame.Y, 100, hash1.Frame.Height));
			hexView1.Font = UIFont.SystemFontOfSize (20);
			hexView1.BackgroundColor = UIColor.White;
			hexView1.TextColor = AppDelegate.BoardBlue;
			hexView1.KeyboardType = UIKeyboardType.Default;
			hexView1.ReturnKeyType = UIReturnKeyType.Done;
			hexView1.Text = CommonUtils.UIColorToHex(AppDelegate.BoardOrange);
			hexView1.AutocapitalizationType = UITextAutocapitalizationType.None;

			hexView1.ShouldChangeCharacters = (textField, range, replacementString) => {
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return newLength <= 6;
			};

			hexView1.ShouldReturn += (textField) => {
				// when user changes colorcode

				UIColor color = CommonUtils.HexToUIColor(hexView1.Text);

				color1.RemoveFromSuperview();

				color1 = CreateColorSquare (ColorSquareSize, 
					ColorSquarePosition1,
					color.CGColor, 1);

				scrollView.AddSubview(color1);

				board.MainColor = color;
				boardView.BackgroundColor = color;

				circleTop.TintColor = board.MainColor;

				textField.ResignFirstResponder();

				return true;
			};

			// creates second colorsquare + second hash + second hexstring

			UIImageView color2 = CreateColorSquare (ColorSquareSize, 
				ColorSquarePosition2,
				AppDelegate.BoardBlue.CGColor, 2);

			UITextField hash2 = new UITextField(new CGRect(color2.Frame.Right + pushRight + 10, color2.Frame.Y + 10, 10, 20));
			hash2.Font = UIFont.SystemFontOfSize (20);
			hash2.BackgroundColor = UIColor.White;
			hash2.TextColor = AppDelegate.BoardBlue;
			hash2.Text = "#";
			hash2.UserInteractionEnabled = false;

			hexView2 = new UITextField(new CGRect(hash2.Frame.Right + 3, hash2.Frame.Y, 100, hash2.Frame.Height));
			hexView2.Font = UIFont.SystemFontOfSize (20);
			hexView2.BackgroundColor = UIColor.White;
			hexView2.TextColor = AppDelegate.BoardBlue;
			hexView2.KeyboardType = UIKeyboardType.Default;
			hexView2.ReturnKeyType = UIReturnKeyType.Done;
			hexView2.Text = CommonUtils.UIColorToHex(AppDelegate.BoardBlue);
			hexView2.AutocapitalizationType = UITextAutocapitalizationType.None;

			hexView2.ShouldChangeCharacters = (textField, range, replacementString) => {
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return newLength <= 6;
			};

			hexView2.ShouldReturn += (textField) => {
				// when user changes colorcode

				UIColor color = CommonUtils.HexToUIColor(hexView2.Text);

				color2.RemoveFromSuperview();

				color2 = CreateColorSquare (ColorSquareSize, 
					ColorSquarePosition2,
					color.CGColor, 2);

				scrollView.AddSubview(color2);

				board.SecondaryColor = color;

				circleLower.TintColor = board.SecondaryColor;

				textField.ResignFirstResponder();

				return true;
			};

			scrollView.AddSubviews (color1, color2, hash1, hexView1, hash2, hexView2);
		}

		// this one just creates a color square
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
			uiv.Frame = frame;

			return uiv;
		}

		// this one changes the hex text and the preview color bar
		private UIImageView CreateColorSquare(CGSize size, CGPoint center, CGColor startcolor, int numberOfView)
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

			UITapGestureRecognizer tap = new UITapGestureRecognizer (async (tg) => {

				UIColor color = await ColorPickerViewController.PresentAsync (
					NavigationController, 
					"Pick a Color",
					View.BackgroundColor);

				uiv.RemoveFromSuperview();

				uiv = CreateColorSquare (size, 
					center,
					color.CGColor, numberOfView);

				scrollView.AddSubview(uiv);


				switch(numberOfView)
				{
				case 1:
					board.MainColor = color;

					hexView1.Text = CommonUtils.UIColorToHex(color);

					boardView.BackgroundColor = board.MainColor;
					circleTop.TintColor = board.MainColor;
					break;
				case 2:
					board.SecondaryColor = color;

					hexView2.Text = CommonUtils.UIColorToHex(color);

					circleLower.TintColor = board.SecondaryColor;
					break;
				}

			});

			uiv.AddGestureRecognizer (tap);
			uiv.UserInteractionEnabled = true;

			return uiv;
		}

		private void LoadBanner()
		{
			using (UIImage bannerImage = UIImage.FromFile ("./screens/create/2/banner/" + AppDelegate.PhoneVersion + ".jpg")) {
				banner = new UIImageView(new CGRect(0,0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
				banner.Image = bannerImage;
			}

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					
					NavigationController.PopViewController(false);


					// Keyboard popup
					NSNotificationCenter.DefaultCenter.RemoveObserver
					(UIKeyboard.DidShowNotification);

					// Keyboard Down
					NSNotificationCenter.DefaultCenter.RemoveObserver
					(UIKeyboard.WillHideNotification);

				} else if (tg.LocationInView(this.View).X > (AppDelegate.ScreenWidth / 4) * 3){
					
					CreateScreen3 createScreen3 = new CreateScreen3(board);
					NavigationController.PushViewController(createScreen3, false);


					// Keyboard popup
					NSNotificationCenter.DefaultCenter.RemoveObserver
					(UIKeyboard.DidShowNotification);

					// Keyboard Down
					NSNotificationCenter.DefaultCenter.RemoveObserver
					(UIKeyboard.WillHideNotification);

				} 
			});

			banner.UserInteractionEnabled = true;
			banner.AddGestureRecognizer (tap);
			banner.Alpha = .95f;
			scrollView.AddSubview (banner);
		}


		private UIImageView GenerateBoardThumb(UIImage image, CGPoint ContentOffset, bool SizeForThumb)
		{
			float imgx, imgy, imgw, imgh;

			float autosize;

			if (SizeForThumb) {
				autosize = AppDelegate.ScreenWidth / 3;
			} else {
				autosize = AppDelegate.ScreenWidth / 5.8f;
			}

			float scale = (float)(image.Size.Height/image.Size.Width);

			if (scale > 1) {
				scale = (float)(image.Size.Width/image.Size.Height);
				imgh = autosize;
				imgw = autosize * scale;
			}
			else {
				imgw = autosize;
				imgh = autosize * scale;	
			}

			imgx = (float)(ContentOffset.X);

			if (imgx < AppDelegate.ScreenWidth / 2) {
				imgx -= autosize / 4;
			} else if (AppDelegate.ScreenWidth / 2 < imgx) {
				imgx += autosize / 4;
			}

			imgy = (float)(ContentOffset.Y);

			// launches the image preview
			UIImageView boardIcon = new UIImageView (new CGRect (0, 0, autosize, autosize));
			boardIcon.Center = new CGPoint (imgx, imgy);
			//boardIcon.BackgroundColor = UIColor.Black;

			UIImageView boardImage = new UIImageView(new CGRect (0, 0, imgw * .8f, imgh * .8f));
			boardImage.Center = new CGPoint (autosize/2, autosize/2);
			UIImage img = CommonUtils.ResizeImage (image, boardIcon.Frame.Size);
			boardImage.Image = img;

			boardIcon.AddSubview (boardImage);

			return boardIcon;
		}

		#region Keyboard

		private void ScrollTheView(bool move)
		{
			// scroll the view up or down
			UIView.BeginAnimations (string.Empty, System.IntPtr.Zero);
			UIView.SetAnimationDuration (0.3);
			UIView.CommitAnimations();

			if (move) {
				OpenKeyboard (hexView2.Frame);
			} else {
				CloseKeyboard (hexView2.Frame);
			}

		}

		private void OpenKeyboard(CGRect writingFieldFrame)
		{
			scrollView.SetContentOffset (new CGPoint (0, 85), true);

			hexView2.Frame = writingFieldFrame;
		}

		private void CloseKeyboard(CGRect writingFieldFrame)
		{
			scrollView.SetContentOffset (new CGPoint (0, 0), true);

			hexView2.Frame = writingFieldFrame;
		}

		private void KeyBoardUpNotification(NSNotification notification)
		{
			// get the keyboard size
			ScrollTheView (true);
		}

		private void KeyBoardDownNotification(NSNotification notification)
		{
			// Calculate how far we need to scroll
			ScrollTheView(false);
		}

		#endregion
	}
}