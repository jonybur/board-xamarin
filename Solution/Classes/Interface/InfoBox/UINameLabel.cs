using CoreGraphics;
using UIKit;

namespace Clubby.Interface
{
	public class UITitleLabel : UILabel
	{
		public UITitleLabel(float yposition, float infoboxwidth, UIFont font, string text){
			Frame = new CGRect (UIInfoBox.XMargin, yposition, infoboxwidth - UIInfoBox.XMargin *2, 24);
			Font = font;
			TextColor = UIColor.Black;
			Text = text;
			TextAlignment = UITextAlignment.Center;
			AdjustsFontSizeToFitWidth = true;
		}
	}
}

