using System;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;

using System.Net.Http;

using System.Threading.Tasks;
using System.Threading;

namespace Solution
{
	/*

	SNIPPET FOR SPLASHSCREEN
		UIImageView splashScreen = new UIImageView(new RectangleF(0,0, ScreenWidth, ScreenHeight));
		splashScreen.Image = UIImage.FromFile("./splash/mainsplash.png");;
		this.View.AddSubview(splashScreen);
		splashScreen.RemoveFromSuperview ();
		splashSCreen.Dispose();

	SNIPPET FOR ACTIVITY INDICATOR USAGE (THIS GOES TO ViewDidLoad)

		UIImage activityImage = UIImage.FromFile("./splash/justwheel.png");

		ActivityIndicator activityIndicator = new ActivityIndicator (activityImage);
		this.View.AddSubview (activityIndicator.ImageView);

		cloudController.BusyUpdate += (bool busy) => {
				if (busy && !activityIndicator.Started()){
					activityIndicator.StartAnimating ();
				}
				else {
					activityIndicator.ChangeRotation();
				}
			};

		activityIndicator.RemoveFromSuperview ();
		activityIndicator.Dispose();
		
	*/

	public class ActivityIndicator
	{
		public UIImageView ImageView;
		private bool rotates;
		private bool cw;
		private int sleepingTime;
		private float rotation;
		private Thread thread;
		private Random random;

		public bool Started()
		{
			return thread.IsAlive;
		}


		public ActivityIndicator()
		{}

		public ActivityIndicator (UIImage _image)
		{
			ImageView = new UIImageView (new CGRect(0,0,_image.Size.Width/2, _image.Size.Height/2));
			ImageView.Image = _image;
			ImageView.Center = new CGPoint (320/2, 522/2);
			rotation = 0f;
			sleepingTime = 10;
			thread = new Thread (new ThreadStart (Rotate));
			random = new Random ();
			cw = true;
		}

		public void StartAnimating()
		{
			thread.Start ();
		}

		public void Rotate()
		{
			rotates = true;
			while (rotates) {
				Thread.Sleep (sleepingTime);

				if (cw) {
					rotation -= 0.1f;
				} else {
					rotation += 0.1f;
				}

				using(var pool = new NSAutoreleasePool())
				{
					pool.InvokeOnMainThread (delegate{
						ImageView.Transform = CGAffineTransform.MakeRotation (rotation);
					});
				}
			}
		}

		public void RemoveFromSuperview()
		{
			ImageView.RemoveFromSuperview ();
			ImageView.Dispose ();
		}

		private void ChangeSpeed()
		{
			// sets new speed
			sleepingTime = random.Next (10, 15);
		}

		public void ChangeRotation()
		{
			cw = !cw;
			ChangeSpeed ();
		}
	}
}

