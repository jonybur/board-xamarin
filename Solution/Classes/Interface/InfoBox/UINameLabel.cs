using CoreGraphics;
using UIKit;

namespace Clubby.Interface
{
	public class UITitleLabel : UILabel
	{
		public UITitleLabel(float yposition, float infoboxwidth, UIFont font, string text, UIColor color){
			var size = text.StringSize (font);
			Frame = new CGRect (UIInfoBox.XContentMargin, yposition, infoboxwidth - UIInfoBox.XContentMargin * 2, size.Height);
			Font = font;
			TextColor = color;
			Text = text;
			TextAlignment = UITextAlignment.Center;
			AdjustsFontSizeToFitWidth = true;
		}

		public UITitleLabel(float yposition, float infoboxwidth, UIFont font, string text){
			var size = text.StringSize (font);
			Frame = new CGRect (UIInfoBox.XContentMargin, yposition, infoboxwidth - UIInfoBox.XContentMargin * 2, size.Height);
			Font = font;
			Text = text;
			TextColor = UIColor.White;
			TextAlignment = UITextAlignment.Center;
			AdjustsFontSizeToFitWidth = true;
		}
	}
}

