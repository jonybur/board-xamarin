using Haneke;
using CoreGraphics;
using UIKit;
using System.Collections.Generic;

namespace Board.Screens.Controls
{
	public sealed class UIMultiActionButtons : UIView
	{
		public const int Height = 50;
		List<UIMultiActionButton> ListButtons;

		public UIMultiActionButtons ()
		{
			Frame = new CGRect(0, AppDelegate.ScreenHeight - Height, AppDelegate.ScreenWidth, Height);
			BackgroundColor = UIColor.FromRGB(249, 249, 249);
			ListButtons = new List<UIMultiActionButton> ();
			UserInteractionEnabled = true;

			CreateTimelineButton ();
			CreateFeaturedButton ();
			CreateDirectoryButton ();
			CreateMapButton ();

			foreach (var button in ListButtons) {
				AddSubview (button);
			}

			float xposition = AppDelegate.ScreenWidth / 8;

			if (ListButtons.Count == 4) {
				ListButtons [0].Center = new CGPoint (xposition * 1, ListButtons [0].Center.Y);
				ListButtons [1].Center = new CGPoint (xposition * 3, ListButtons [1].Center.Y);
				ListButtons [2].Center = new CGPoint (xposition * 5, ListButtons [2].Center.Y);
				ListButtons [3].Center = new CGPoint (xposition * 7, ListButtons [3].Center.Y);
			}
		}

		private void CreateTimelineButton(){
			var timelineButton = new UIMultiActionButton ("empty_house", "full_house");

			timelineButton.TouchUpInside += delegate {

				if (timelineButton.IsOn){
					return;
				}

				UnselectAllButtons();
				timelineButton.SetFullImage();

			};

			ListButtons.Add (timelineButton);	
		}

		private void CreateFeaturedButton(){
			var featuredButton = new UIMultiActionButton ("empty_star", "full_star");

			featuredButton.TouchUpInside += delegate {

				if (featuredButton.IsOn){
					return;
				}

				UnselectAllButtons();
				featuredButton.SetFullImage();

			};

			ListButtons.Add (featuredButton);
		}

		private void CreateDirectoryButton(){
			var directoryButton = new UIMultiActionButton ("empty_directory", "full_directory");

			directoryButton.TouchUpInside += delegate {

				if (directoryButton.IsOn){
					return;
				}

				UnselectAllButtons();
				directoryButton.SetFullImage();

			};

			ListButtons.Add (directoryButton);
		}

		private void CreateMapButton(){
			var mapButton = new UIMultiActionButton ("empty_map", "full_map");

			mapButton.TouchUpInside += delegate {

				if (mapButton.IsOn){
					return;
				}

				UnselectAllButtons();
				mapButton.SetFullImage();

			};

			ListButtons.Add (mapButton);
		}

		private void UnselectAllButtons(){
			foreach (var button in ListButtons){
				button.SetEmptyImage();
			}
		}
	}

	public sealed class UIMultiActionButton : UIButton{
		UIImageView imageView;

		public bool IsOn;

		string EmptyImage;
		string FullImage;

		public UIMultiActionButton(string emptyImage, string fullImage){
			EmptyImage = emptyImage;
			FullImage = fullImage;

			Frame = new CGRect (0, 0, 50, 50);

			imageView = new UIImageView();
			imageView.Frame = new CGRect(0, 0, Frame.Size.Width * .5f, Frame.Size.Height * .5f);
			imageView.Center = new CGPoint(Frame.Width / 2, Frame.Height / 2);
			SetEmptyImage();

			this.AddSubview(imageView);
		}

		public void SetEmptyImage(){
			imageView.SetImage("./screens/main/buttons/"+EmptyImage+".png");
			IsOn = false;
		}

		public void SetFullImage(){
			imageView.SetImage("./screens/main/buttons/"+FullImage+".png");
			IsOn = true;
		}
	}
}

