using System;
using UIKit;
using Board.Screens.Controls;
using Board.Utilities;
using CoreGraphics;
using System.Collections.Generic;

namespace Board.Screens
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
				new License ("Share Plugin", "The MIT License (MIT)\n\nCopyright (c) 2015 Jakob Gürtl & James Montemagno/Refractored LLC\n\nPermission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:\n\nThe above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE."),
				new License ("BTProgressHUD", "Copyright 2013 Nic Wise / Big Ted Ltd and Sam Vermette\n\nBTProgressHUD:\n\nLicensed under the Apache License, Version 2.0 (the \"License\"); you may not use this file except in compliance with the License. You may obtain a copy of the License at\n\nhttp://www.apache.org/licenses/LICENSE-2.0\n\nUnless required by applicable law or agreed to in writing, software distributed under the License is distributed on an \"AS IS\" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License."),
				new License ("SQLite.NET", "Copyright (c) 2015 Krueger Systems, Inc.\n\nPermission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:\n\nThe above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE."),
				new License ("MGImageUtilities", "License Agreement for Source Code provided by Matt Legend Gemmell\n\nThis software is supplied to you by Matt Legend Gemmell in consideration of your agreement to the following terms, and your use, installation, modification or redistribution of this software constitutes acceptance of these terms. If you do not agree with these terms, please do not use, install, modify or redistribute this software.\n\nIn consideration of your agreement to abide by the following terms, and subject to these terms, Matt Legend Gemmell grants you a personal, non-exclusive license, to use, reproduce, modify and redistribute the software, with or without modifications, in source and/or binary forms; provided that if you redistribute the software in its entirety and without modifications, you must retain this notice and the following text and disclaimers in all such redistributions of the software, and that in all cases attribution of Matt Legend Gemmell as the original author of the source code shall be included in all such resulting software products or distributions.\u2028\nNeither the name, trademarks, service marks or logos of Matt Legend Gemmell or Instinctive Code may be used to endorse or promote products derived from the software without specific prior written permission from Matt Legend Gemmell. Except as expressly stated in this notice, no other rights or licenses, express or implied, are granted by Matt Legend Gemmell herein, including but not limited to any patent rights that may be infringed by your derivative works or by other works in which the software may be incorporated.\n\nThe software is provided by Matt Legend Gemmell on an \"AS IS\" basis. MATT LEGEND GEMMELL AND INSTINCTIVE CODE MAKE NO WARRANTIES, EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION THE IMPLIED WARRANTIES OF NON-INFRINGEMENT, MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE, REGARDING THE SOFTWARE OR ITS USE AND OPERATION ALONE OR IN COMBINATION WITH YOUR PRODUCTS.\n\nIN NO EVENT SHALL MATT LEGEND GEMMELL OR INSTINCTIVE CODE BE LIABLE FOR ANY SPECIAL, INDIRECT, INCIDENTAL OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) ARISING IN ANY WAY OUT OF THE USE, REPRODUCTION, MODIFICATION AND/OR DISTRIBUTION OF THE SOFTWARE, HOWEVER CAUSED AND WHETHER UNDER THEORY OF CONTRACT, TORT (INCLUDING NEGLIGENCE), STRICT LIABILITY OR OTHERWISE, EVEN IF MATT LEGEND GEMMELL OR INSTINCTIVE CODE HAVE BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGE."),
				new License ("PBJVision", "The MIT License (MIT)\n\nCopyright (c) 2013-present Patrick Piemonte\n\nPermission is hereby granted, free of charge, to any person obtaining a copy of\nthis software and associated documentation files (the \"Software\"), to deal in\nthe Software without restriction, including without limitation the rights to\nuse, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of\nthe Software, and to permit persons to whom the Software is furnished to do so,\nsubject to the following conditions:\n\nThe above copyright notice and this permission notice shall be included in all\ncopies or substantial portions of the Software.\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR\nIMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS\nFOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR\nCOPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER\nIN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN\nCONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE."),
				new License ("PNChart", "The MIT License (MIT)\n\nCopyright (c) 2013 Kevin\n\nPermission is hereby granted, free of charge, to any person obtaining a copy of\nthis software and associated documentation files (the \"Software\"), to deal in\nthe Software without restriction, including without limitation the rights to\nuse, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of\nthe Software, and to permit persons to whom the Software is furnished to do so,\nsubject to the following conditions:\n\nThe above copyright notice and this permission notice shall be included in all\ncopies or substantial portions of the Software.\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR\nIMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS\nFOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR\nCOPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER\nIN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN\nCONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE."),
				new License ("Freepik Contents", "designed by Freepik"));
			

			float yposition = UIMenuBanner.Height - 20;

			foreach (var license in LicenseList) {
				
				var licenseButton = new UIOneLineMenuButton(yposition + 1);
				licenseButton.SetLabel(license.ProductName + " >");
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
			View.BackgroundColor = UIColor.White;
		}

		public override void ViewDidAppear(bool animated){
			Banner.SuscribeToEvents ();
		}

		public override void ViewWillDisappear (bool animated) {
			Banner.UnsuscribeToEvents ();
		}

		private void LoadBanner()
		{
			Banner = new UIMenuBanner ("LICENSES", "arrow_left");

			bool taps = false;

			var tap = new UITapGestureRecognizer (tg => {
				if (taps){
					return;
				}

				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					taps = true;

					var containerScreen = AppDelegate.NavigationController.ViewControllers[AppDelegate.NavigationController.ViewControllers.Length - 2] as ContainerScreen;
					if (containerScreen!= null)
					{
						containerScreen.LoadSettingsScreen();
					}
					AppDelegate.PopViewControllerWithCallback (delegate{
						MemoryUtility.ReleaseUIViewWithChildren (View);
					});
				}
			});

			Banner.AddTap (tap);
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

