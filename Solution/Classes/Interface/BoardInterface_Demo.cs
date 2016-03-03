using System;
using System;
using CoreGraphics;
using System.Linq;

using Foundation;
using UIKit;

using System.Collections.Generic;

using MediaPlayer;

using Facebook.CoreKit;
using Board.Interface.Buttons;
using Board.Interface.Widgets;
using Board.Interface;
using Board.Schema;

using BigTed;

namespace Board.Interface
{
	public partial class BoardInterface
	{

		private void GenerateTestPictures()
		{
			using (UIImage img = UIImage.FromFile ("./demo/pictures/0.jpg")) {
				AddTestPicture (img, 70, 20, -.03f);
			}

			AddTestVideo ("./demo/videos/0.mp4", 45, 220, -.01f);

			using (UIImage img = UIImage.FromFile ("./demo/pictures/2.jpg")) {
				AddTestPicture (img, 340, 20, 0f);
			}
			using (UIImage img = UIImage.FromFile ("./demo/pictures/1.jpg")) {
				AddTestPicture (img, 360, 220, -.04f);
			}

			AddTestVideo ("./demo/videos/1.mp4", 610, 25, -.02f);

			using (UIImage img = UIImage.FromFile ("./demo/pictures/3.jpg")) {
				AddTestPicture (img, 655, 225, .01f);
			}

			using (UIImage img = UIImage.FromFile ("./demo/events/0.jpg")) {
				AddTestEvent ("La Roxtar", img, new DateTime (2016, 6, 16, 22, 30, 0), new CGRect (1650, 29, 0, 0), -.03f);
			}

			using (UIImage img = UIImage.FromFile ("./demo/events/1.jpg")) {
				AddTestEvent ("RIVERS in the Alley", img, new DateTime (2016, 11, 4, 18, 0, 0), new CGRect (1910, 30, 0, 0), .02f);
			}

			using (UIImage img = UIImage.FromFile ("./demo/events/2.jpg")) {
				AddTestEvent ("Retirement Block Party", img, new DateTime (2016, 2, 28, 11, 30, 0), new CGRect (2170, 27, 0, 0), .02f);
			}

			using (UIImage img = UIImage.FromFile ("./demo/pictures/4.jpg")) {
				AddTestPicture (img, 50, 420, .03f);
			}

			AddTestVideo ("./demo/videos/2.mp4", 330, 415, -.02f);

			AddTestVideo ("./demo/videos/3.mp4", 635, 420, .0f);

		}

		private void AddTestAnnouncement(CGRect frame, float rotation)
		{
			// need to have JSON of the format dictionaries

			Announcement ann = new Announcement ();
			ann.Frame = frame;
			ann.Rotation = rotation;
			DictionaryContent.Add (ann.Id, ann);
		}

		private void AddTestEvent(string name, UIImage img, DateTime date, CGRect frame, float rotation)
		{
			BoardEvent bevent = new BoardEvent (name, img, date, rotation, frame, null);
			bevent.Rotation = rotation;
			DictionaryContent.Add (bevent.Id, bevent);
		}

		private void AddTestPicture(UIImage image, float imgx, float imgy, float rotation)
		{
			Picture pic = new Picture ();
			pic.ImageView = new UIImageView(image);
			pic.Frame = new CGRect(imgx, imgy, 0, 0);
			pic.Rotation = rotation;
			DictionaryContent.Add (pic.Id, pic);
		}

		private void AddTestVideo(string url, float imgx, float imgy, float rotation)
		{
			Video vid = new Video ();

			using (MPMoviePlayerController moviePlayer = new MPMoviePlayerController (NSUrl.FromFilename (url))) {
				vid.ThumbnailView = new UIImageView(moviePlayer.ThumbnailImageAt (0, MPMovieTimeOption.Exact));
				moviePlayer.Pause ();
				moviePlayer.Dispose ();	
			}

			vid.Url = NSUrl.FromFilename (url);
			vid.Frame = new CGRect(imgx, imgy, 0, 0);
			vid.Rotation = rotation;

			DictionaryContent.Add (vid.Id, vid);
		}

			
	}
}

