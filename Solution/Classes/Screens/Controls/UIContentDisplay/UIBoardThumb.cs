using UIKit;
using MGImageUtilitiesBinding;
using Board.Interface;
using CoreGraphics;
using Foundation;
using Board.Utilities;

namespace Board.Screens.Controls
{
	public class UIBoardThumbComponent : UIView{
		public UIBoardThumb BoardThumb;
		public UILabel NameLabel;
		public const int TextSpace = 60;
		public readonly float Size;

		public UIBoardThumbComponent(Board.Schema.Board board, CGPoint contentOffset, float size){
			Size = size;
			BoardThumb = new UIBoardThumb (board, contentOffset, size);

			Frame = new CGRect(BoardThumb.Frame.X, BoardThumb.Frame.Y, BoardThumb.Frame.Width, BoardThumb.Frame.Height + TextSpace);
			BoardThumb.Frame = new CGRect (0, 0, BoardThumb.Frame.Width, BoardThumb.Frame.Height);
			UserInteractionEnabled = true;

			NameLabel = CreateNameLabel (board.Name, GetDistance(board), size);

			AddSubviews (BoardThumb, NameLabel);
		}

		private double GetDistance(Board.Schema.Board board){
			/*
			var containerScreen = AppDelegate.NavigationController.TopViewController as ContainerScreen;
			var mainMenuScreen = containerScreen.CurrentScreen as MainMenuScreen;
			*/

			var location = AppDelegate.UserLocation;
			double distance = 0;

			if (location.IsValid()) {
				distance = CommonUtils.DistanceBetweenCoordinates (board.GeolocatorObject.Coordinate, location, 'M');
			}

			return distance;
		}

		public void UpdateDistanceLabel(){
			NameLabel.RemoveFromSuperview ();
			NameLabel = CreateNameLabel (BoardThumb.Board.Name, GetDistance(BoardThumb.Board), Size);
			AddSubview (NameLabel);
			
		}

		private UILabel CreateNameLabel(string nameString, double distance, float width)
		{
			UILabel label = new UILabel ();

			label.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);

			string farAway;
			if (distance != 1) {
				farAway = " miles away";
			} else {
				farAway = " mile away";
			}

			string distanceString = distance.ToString ("F1");
			if (distanceString == "0.0") {
				distanceString = "0";
			}

			string distanceTotalString = distanceString + farAway;
			string compositeString = nameString + "\n" + distanceTotalString;

			System.Console.WriteLine (compositeString);

			var nameAttributes = new UIStringAttributes {
				Font = UIFont.SystemFontOfSize(14),
				ForegroundColor = UIColor.Black
			};

			var distanceAttributes = new UIStringAttributes {
				Font = UIFont.SystemFontOfSize(14),
				ForegroundColor = UIColor.FromRGB(100,100,100)
			};

			var attributedString = new NSMutableAttributedString (compositeString);
			attributedString.SetAttributes (nameAttributes.Dictionary, new NSRange (0, nameString.Length));
			attributedString.SetAttributes (distanceAttributes, new NSRange (nameString.Length, distanceTotalString.Length + 1));

			label.TextColor = AppDelegate.BoardBlack;
			label.Lines = 0;
			label.AttributedText = attributedString;
			label.AdjustsFontSizeToFitWidth = false;
			label.Font = UIFont.SystemFontOfSize(14);
			label.Frame = new CGRect (5, width, width - 10, TextSpace);
			label.SizeToFit ();

			return label;
		}
	}

	public class UIBoardThumb : UIContentThumb
	{
		public UIBoardThumb (Board.Schema.Board board, CGPoint contentOffset, float size)
		{ 
			Board = board;

			float autosize = size;
			float imgx, imgy;

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

			CGSize iconsize = new CGSize (autosize * .7f, autosize * .7f);

			UIImage img = board.ImageView.Image.ImageScaledToFitSize (iconsize);
			UIImage circle = CreateThumbImage(Frame.Size);

			SetBackgroundImage (circle, UIControlState.Normal);
			SetImage(img, UIControlState.Normal);

			TouchEvent = (sender, e) => {
				if (AppDelegate.boardInterface == null)
				{
					AppDelegate.boardInterface = new BoardInterface (board);
					AppDelegate.NavigationController.PushViewController (AppDelegate.boardInterface, true);
				}
			};

			UserInteractionEnabled = true;
		}

		private UIImage CreateThumbImage(CGSize size)
		{
			UIGraphics.BeginImageContextWithOptions (size, false, 2f);

			CGContext current = UIGraphics.GetCurrentContext ();

			current.SetFillColor (UIColor.White.CGColor);
			current.FillEllipseInRect (new CGRect(0, 0, size.Width, size.Height));

			return UIGraphics.GetImageFromCurrentImageContext ();
		}
	}


}

