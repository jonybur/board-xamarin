using System;
using Clubby.Schema;
using Clubby.Screens.Controls;
using UIKit;

namespace Clubby.Interface
{
	public class SingleWidgetScreen : UIViewController
	{
		UIMenuBanner Banner;
		UITimelineWidget Widget;
		UIScrollView ScrollView;
		Content content;

		public SingleWidgetScreen (Content _content)
		{
			content = _content;
		}

		public override void ViewDidLoad ()
		{
			Widget = new UITimelineWidget (UIVenueInterface.venue, content);


			View.AddSubview (Widget);
		}

		public override void ViewDidAppear (bool animated)
		{
			
		}


		private void LoadBanner()
		{
			Banner = new UIMenuBanner ("");
			Banner.ChangeTitle ("Clubby", AppDelegate.Narwhal26);
		}
	}
}

