using UIKit;
using System;
using System.Collections.Generic;

namespace Board.Interface.Widgets
{
	// father class to all buttons
	public class Widget
	{
		public static UIImageView ClosedEyeImageView;
		public static UIImageView OpenEyeImageView;

		public UIView View;
		public List<UIGestureRecognizer> gestureRecognizers;

		protected UIImageView eye;

		public bool EyeOpen;

		public Widget()
		{
			using (UIImage image = UIImage.FromFile ("./boardinterface/widget/closedeye.png")) {
				ClosedEyeImageView = new UIImageView(image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate));
			}

			using (UIImage image = UIImage.FromFile ("./boardinterface/widget/openeye.png")) {
				OpenEyeImageView = new UIImageView(image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate));
			}
			
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
			eye.Image = OpenEyeImageView.Image;
			eye.TintColor = BoardInterface.board.MainColor;
			EyeOpen = true;
		}
	}
}

