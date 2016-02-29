using UIKit;
using System;
using System.Collections.Generic;

namespace Board.Interface.Widgets
{
	// father class to all buttons
	public class Widget
	{
		public static UIImage ClosedEyeImage;
		public static UIImage OpenEyeImage;

		public UIView View;
		public List<UIGestureRecognizer> gestureRecognizers;

		protected UIImageView eye;

		public bool EyeOpen;

		public Widget()
		{
			ClosedEyeImage = UIImage.FromFile ("./boardinterface/widget/closedeye.png");
			OpenEyeImage = UIImage.FromFile ("./boardinterface/widget/openeye.png");
			ClosedEyeImage = ClosedEyeImage.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
			OpenEyeImage = OpenEyeImage.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);

			View = new UIButton ();
			gestureRecognizers = new List<UIGestureRecognizer> ();
		}

		public void SuscribeToEvents ()
		{
			foreach (UIGestureRecognizer gr in gestureRecognizers) {
				View.AddGestureRecognizer(gr);
			}
		}

		public void UnsuscribeToEvents()
		{
			foreach (UIGestureRecognizer gr in gestureRecognizers) {
				View.RemoveGestureRecognizer(gr);
			}
		}

		public void OpenEye()
		{
			eye.Image = OpenEyeImage;
			eye.TintColor = BoardInterface.board.MainColor;
			EyeOpen = true;
		}
	}
}

