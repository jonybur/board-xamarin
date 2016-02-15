using System;
using CoreGraphics;
using System.Collections.Generic;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;

using Board.Schema;

namespace Board.Interface
{
	public class Gallery
	{
		private const int maxrows = 2;
		private int rows = 1;
		private const int separation = 1;
		private float[] yPositions;
		private static UIScrollView scrollView;
		private float rotation;
		private CGAffineTransform defaultOrientation;

		public UIScrollView GetScrollView()
		{
			return scrollView;
		}

		public void SetAlpha(float alpha)
		{
			scrollView.Alpha = alpha;
		}

		public Gallery ()
		{
			yPositions = new float[rows];
			rotation = (float)(Math.PI * 90 / 180.0);

			scrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			scrollView.BackgroundColor = UIColor.Black;
			defaultOrientation = scrollView.Transform;

			scrollView.Alpha = 0f;
		}

		public void RemoveAllPictures()
		{
			foreach (UIImageView iv in scrollView) { 
				iv.RemoveFromSuperview ();
				iv.Dispose ();
			}
		}

		public bool LoadGallery()
		{
			RemoveAllPictures ();

			// TODO: make this work with our own server
			List<Picture> thumbnails = new List<Picture> (); //StorageController.ReturnAllStoredPictures (true);

			if (thumbnails.Count > 0) {
				if (thumbnails.Count > 1) {
					rows = maxrows;
					yPositions = new float[maxrows];
				}

				GenerateThumbnails (thumbnails);
				scrollView.ContentSize = new CGSize(AppDelegate.ScreenWidth, FindLength());
				return true;
			}

			return false;
		}

		private void GenerateThumbnails(List<Picture> thumbnails)
		{
			foreach (Picture p in thumbnails) {
				UIImage image = p.Thumbnail;
				UIImageView thumb = CreateImageFrame (image);

				scrollView.AddSubview (thumb);
			}
		}

		private int FindShortestRow ()
		{
			int row = rows-1; float min = yPositions[rows-1];

			for (int i = rows - 1; i >= 0; i--) {
				if (yPositions[i] < min) {
					min = yPositions [i];
					row = i;
				}
			}

			return row;
		}

		private float FindLength ()
		{
			float max = yPositions[0];

			for (int i = 1; i < yPositions.Length; i++) {
				if (yPositions[i] > max) {
					max = yPositions [i];
				}
			}

			return max;
		}

		private UIImageView CreateImageFrame(UIImage image)
		{
			// first it chooses the shortest row, this is where the new image will be located

			int row = FindShortestRow ();

			float autosize = (AppDelegate.ScreenWidth / rows) - separation;

			float imgw, imgh;
			float scale = (float)(image.Size.Width/image.Size.Height);

			imgh = autosize;
			imgw = autosize * scale;
	
			UIImageView imageView = new UIImageView (new CGRect(0,0,imgw,imgh));
			imageView.Image = image;
			imageView.Transform = CGAffineTransform.MakeRotation(rotation);
			imageView.Center = new CGPoint (imgh / 2 + row * (AppDelegate.ScreenWidth / rows), 
				yPositions[row] + imgw / 2);
			yPositions [row] += imgw + separation;

			row++;

			return imageView;
		}

		public void SetOrientation(bool rightOrientation)
		{			
			scrollView.Transform = defaultOrientation;
			if (rightOrientation) {
				scrollView.Transform = CGAffineTransform.MakeRotation (rotation * 2);
			}
		}
	}
}

