using CoreGraphics;
using UIKit;

namespace Board.Interface.Buttons
{
	public class InfoButton : Button
	{
		public InfoButton ()
		{
			uiButton = new UIButton (UIButtonType.Custom);

			using (UIImage uiImage = UIImage.FromFile ("./boardinterface/strokebuttons/info_3px3.png")) {
				uiButton.SetImage (uiImage, UIControlState.Normal);
			}
			uiButton.Frame = new CGRect (0,0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint ((AppDelegate.ScreenWidth + ButtonSize) / 2 + 
				(AppDelegate.ScreenWidth - ButtonSize) / 8 * 3, AppDelegate.ScreenHeight - ButtonSize / 2);

			eventHandlers.Add ((sender, e) => {
				
			});

		}
	}
}
