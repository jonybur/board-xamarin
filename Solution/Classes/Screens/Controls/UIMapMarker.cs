﻿using Google.Maps;
using UIKit;
using MGImageUtilitiesBinding;
using Foundation;
using CoreLocation;
using CoreGraphics;

namespace Board.Screens.Controls
{
	public class UIMapMarker : Marker
	{
		public static UIImage Image;
		private SizeMode sizeMode;

		public enum SizeMode { Small = 0, Normal }

		public UIMapMarker (CLLocationCoordinate2D position, MapView map)
		{
			AppearAnimation = MarkerAnimation.Pop;
			Position = position;
			Map = map;
			Draggable = false;
			InfoWindowAnchor = new CGPoint (.5, .5);
		}

		public UIMapMarker (Board.Schema.Board board, MapView map)
		{
			AppearAnimation = MarkerAnimation.Pop;
			Position = board.GeolocatorObject.Coordinate;
			Map = map;
			Draggable = false;
			Title = board.Name;
			Snippet = board.GeolocatorObject.Address;
			InfoWindowAnchor = new CGPoint (.5, .5);
			Tappable = true;
			UserData = new NSString(board.Id);
		}

		public UIMapMarker (Board.Schema.Board board, MapView map, SizeMode mode)
		{
			sizeMode = mode;
			AppearAnimation = MarkerAnimation.Pop;
			Position = board.GeolocatorObject.Coordinate;
			Map = map;
			Icon = CreateMarkerImage (board.Image);
			Draggable = false;
			Title = board.Name;
			Snippet = board.GeolocatorObject.Address;
			InfoWindowAnchor = new CGPoint (.5, .5);
			Tappable = true;
			UserData = new NSString(board.Id);
		}

		private UIImage CreateMarkerImage(UIImage logo)
		{
			CGSize size;
			int position;
			float autosize;
			if (sizeMode == SizeMode.Normal) {
				size = new CGSize (66, 96);
				position = 33;
				autosize = 40;
			} else {
				size = new CGSize (44, 64);
				position = 22;
				autosize = 25;
			}

			UIGraphics.BeginImageContextWithOptions (size, false, 2f);

			if (UIMapMarker.Image == null) {
				UIMapMarker.Image = UIImage.FromFile ("./screens/main/map/markercontainer_black.png");
			}

			UIMapMarker.Image.Draw (new CGRect (0, 0, size.Width, size.Height));

			logo = logo.ImageScaledToFitSize (new CGSize(autosize,autosize));

			logo.Draw (new CGRect (position - logo.Size.Width / 2, position - logo.Size.Height / 2, logo.Size.Width, logo.Size.Height));

			return UIGraphics.GetImageFromCurrentImageContext ();
		}
	}
}

