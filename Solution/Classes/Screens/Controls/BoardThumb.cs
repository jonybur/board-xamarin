using System;
using UIKit;
using MGImageUtilitiesBinding;
using Board.Interface;
using CoreGraphics;

namespace Board.Screens.Controls
{
	public sealed class BoardThumb : UIButton
	{
		public static float Size;
		private EventHandler TouchEvent;
		public Board.Schema.Board Board;

		public BoardThumb (Board.Schema.Board board, CGPoint contentOffset)
		{ 
			
			Board = board;
			float imgx, imgy, imgw, imgh;

			float autosize = Size;
			float scale = (float)(board.ImageView.Frame.Height / board.ImageView.Frame.Width);

			if (scale > 1) {
				scale = (float)(board.ImageView.Frame.Width / board.ImageView.Frame.Height);
				imgh = autosize;
				imgw = autosize * scale;
			} else {
				imgw = autosize;
				imgh = autosize * scale;	
			}

			imgx = (float)(contentOffset.X);

			if (imgx < AppDelegate.ScreenWidth / 2) {
				imgx -= autosize / 4;
			} else if (AppDelegate.ScreenWidth / 2 < imgx) {
				imgx += autosize / 4;
			}

			imgy = (float)(contentOffset.Y);

			// launches the image preview
			Frame = new CGRect (0, 0, autosize, autosize);
			Center = new CGPoint (imgx, imgy);

			CGSize iconsize = new CGSize (autosize * .75f, autosize * .75f);

			UIImage img = board.ImageView.Image.ImageScaledToFitSize (iconsize);//CreateThumbImage (board.ImageView.Image, new CGSize(autosize, autosize));
			img = CreateThumbImage(img, Frame.Size);
			SetImage(img, UIControlState.Normal);

			TouchEvent = (sender, e) => {
				if (AppDelegate.boardInterface == null)
				{
					AppDelegate.boardInterface = new BoardInterface (board, false);
					AppDelegate.NavigationController.PushViewController (AppDelegate.boardInterface, true);
				}
			};

			this.UserInteractionEnabled = true;
		}

		// TODO: implement this
		private UIImage CreateThumbImage(UIImage logo, CGSize size)
		{
			UIGraphics.BeginImageContext (size);

			CGContext current = UIGraphics.GetCurrentContext ();

			current.SetFillColor (UIColor.White.CGColor);
			current.FillEllipseInRect (new CGRect(0,0,size.Width, size.Height));

			logo.Draw (new CGRect (size.Width / 2 - logo.Size.Width / 4, size.Height / 2 - logo.Size.Height / 4, logo.Size.Width / 2, logo.Size.Height/2));

			return UIGraphics.GetImageFromCurrentImageContext ();
		}

		public void SuscribeToEvent()
		{
			TouchUpInside += TouchEvent;	
		}

		public void UnsuscribeToEvent()
		{
			TouchUpInside -= TouchEvent;
		}
	}
}

