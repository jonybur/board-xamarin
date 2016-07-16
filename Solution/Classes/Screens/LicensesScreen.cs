using System;
using UIKit;
using Clubby.Screens.Controls;
using Clubby.Utilities;
using CoreGraphics;
using System.Collections.Generic;

namespace Clubby.Screens
{
	public class LicensesScreen : UIViewController
	{
		UIMenuBanner Banner;
		UIScrollView ScrollView;
		List<License> LicenseList;

		public override void ViewDidLoad ()
		{
			LicenseList = new List<License> ();
			ScrollView = new UIScrollView ();
			ScrollView.Frame = new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);

			LoadBanner ();

			PopulateLicenseList (
				new License ("FBSDKLoginKit", "Copyright (c) 2014-present, Facebook, Inc. All rights reserved.\n\nYou are hereby granted a non-exclusive, worldwide, royalty-free license to use,\ncopy, modify, and distribute this software in source code or binary form for use\nin connection with the web services and APIs provided by Facebook.\n\nAs with any software that integrates with the Facebook platform, your use of\nthis software is subject to the Facebook Developer Principles and Policies\n[http://developers.facebook.com/policy/]. This copyright notice shall be\nincluded in all copies or substantial portions of the software.\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR\nIMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS\nFOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR\nCOPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER\nIN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN\nCONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.\n"), 
				new License ("FBSDKCoreKit", "Copyright (c) 2014-present, Facebook, Inc. All rights reserved.\n\nYou are hereby granted a non-exclusive, worldwide, royalty-free license to use,\ncopy, modify, and distribute this software in source code or binary form for use\nin connection with the web services and APIs provided by Facebook.\n\nAs with any software that integrates with the Facebook platform, your use of\nthis software is subject to the Facebook Developer Principles and Policies\n[http://developers.facebook.com/policy/]. This copyright notice shall be\nincluded in all copies or substantial portions of the software.\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR\nIMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS\nFOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR\nCOPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER\nIN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN\nCONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.\n"), 
				new License ("Google Maps", Google.Maps.MapServices.OpenSourceLicenseInfo),
				new License ("Haneke", "Copyright 2014 Alex Blount (@alexblount)\n\nLicensed under the Apache License, Version 2.0 (the \"License\"); you may not use this file except in compliance with the License. You may obtain a copy of the License at\n\nhttp://www.apache.org/licenses/LICENSE-2.0\n\nUnless required by applicable law or agreed to in writing, software distributed under the License is distributed on an \"AS IS\" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License."),
				new License ("DACircularProgress", "MIT License\n\nCopyright (c) 2015 Daniel Amitay (http://danielamitay.com)\n\nPermission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:\n\nThe above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE."),
				new License ("SQLite.NET", "Copyright (c) 2015 Krueger Systems, Inc.\n\nPermission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:\n\nThe above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE."),
				new License ("Freepik Contents", "designed by Freepik"),
				new License ("ModernHttpClient", "Copyright (c) 2013 Paul Betts\n\nPermission is hereby granted, free of charge, to any person obtaining a copy of\nthis software and associated documentation files (the \"Software\"), to deal in\nthe Software without restriction, including without limitation the rights to\nuse, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of\nthe Software, and to permit persons to whom the Software is furnished to do so,\nsubject to the following conditions:\n\nThe above copyright notice and this permission notice shall be included in all\ncopies or substantial portions of the Software.\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR\nIMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS\nFOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR\nCOPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER\nIN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN\nCONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE."),
				new License ("OkHttp", "Copyright 2013 Square, Inc.\n\nLicensed under the Apache License, Version 2.0 (the \"License\");\nyou may not use this file except in compliance with the License.\nYou may obtain a copy of the License at\n\n   http://www.apache.org/licenses/LICENSE-2.0\n\nUnless required by applicable law or agreed to in writing, software\ndistributed under the License is distributed on an \"AS IS\" BASIS,\nWITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.\nSee the License for the specific language governing permissions and\nlimitations under the License.")
			);
			

			float yposition = UIMenuBanner.Height - 20;

			foreach (var license in LicenseList) {
				
				var licenseButton = new UIOneLineMenuButton(yposition + 1);
				licenseButton.SetLabel(license.ProductName);
				licenseButton.SetTapEvent (delegate {
					var licenseScreen = new LicenseScreen(license);
					AppDelegate.NavigationController.PushViewController(licenseScreen, true);
				});
				licenseButton.SuscribeToEvent();
				yposition += (float)licenseButton.Frame.Height;

				ScrollView.AddSubview (licenseButton);

			}

			ScrollView.ContentSize = new CGSize (AppDelegate.ScreenWidth, LicenseList.Count * UIOneLineMenuButton.Height + UIActionButton.Height * 2);

			View.AddSubviews (ScrollView, Banner);
			View.BackgroundColor = AppDelegate.ClubbyBlack;
		}

		public override void ViewDidAppear(bool animated){
			NavigationController.InteractivePopGestureRecognizer.Enabled = true;
			NavigationController.InteractivePopGestureRecognizer.Delegate = null;
			Banner.SuscribeToEvents ();
		}

		public override void ViewWillDisappear (bool animated) {
			Banner.UnsuscribeToEvents ();
		}

		private void LoadBanner()
		{
			Banner = new UIMenuBanner ("Licenses", "arrow_left");

			Banner.AddLeftTap (delegate() {
				AppDelegate.NavigationController.PopViewController(true);
			});

			Banner.SuscribeToEvents ();
		}

		private void PopulateLicenseList(params License[] licenses){
			LicenseList.AddRange (licenses);
		}

	}


	public class License{
		public string ProductName;
		public string Description;

		public License(string productName, string description){
			ProductName = productName;
			Description = description;
		}
	}
}

