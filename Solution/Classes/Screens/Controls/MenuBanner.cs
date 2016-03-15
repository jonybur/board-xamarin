using CoreGraphics;
using UIKit;
using System.Collections.Generic;

namespace Board.Screens.Controls
{
	public class MenuBanner : UIImageView
	{
		List<UITapGestureRecognizer> taps;

		public MenuBanner (string imagePath)
		{
			taps = new List<UITapGestureRecognizer> ();

			using (UIImage bannerImage = UIImage.FromFile (imagePath)) {
				Frame = new CGRect (0, 0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2);
				Image = bannerImage;
			}

			UserInteractionEnabled = true;
			Alpha = .95f;
		}

		public void AddTap(UITapGestureRecognizer tap)
		{
			taps.Add (tap);
		}

		public void SuscribeToEvents()
		{
			foreach (UITapGestureRecognizer tap in taps) {
				AddGestureRecognizer (tap);
			}
		}

		public void UnsuscribeToEvents()
		{
			foreach (UITapGestureRecognizer tap in taps) {
				RemoveGestureRecognizer (tap);
			}
		}
	}
}

