using System;
using UIKit;
using Board.Utilities;
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
			this.Board = board;
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
			this.Frame = new CGRect (0, 0, autosize, autosize);
			this.Center = new CGPoint (imgx, imgy);

			UIImageView boardImage = new UIImageView (new CGRect (0, 0, imgw * .8f, imgh * .8f));
			boardImage.Center = new CGPoint (autosize / 2, autosize / 2);

			UIImage img = CommonUtils.ResizeImage (board.ImageView.Image, this.Frame.Size);
			boardImage.Image = img;

			this.AddSubview (boardImage);

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
		private UIImage CreateThumbImage(UIImage logo)
		{
			UIGraphics.BeginImageContext (new CGSize(66, 96));

			using (UIImage circle = UIImage.FromFile ("./screens/home/map/marker_blue.png")) {
				circle.Draw (new CGRect (0, 0, 66, 96));
			}

			float imgw, imgh;

			float scale = (float)(logo.Size.Height/logo.Size.Width);
			imgw = 40;
			imgh = imgw * scale;

			logo.Draw (new CGRect (33 - imgw / 2, 33 - imgh / 2, imgw, imgh));

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

