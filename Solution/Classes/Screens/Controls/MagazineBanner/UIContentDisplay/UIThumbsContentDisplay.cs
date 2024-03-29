﻿using System;
using System.Collections.Generic;
using System.Linq;
using Board.Utilities;
using CoreGraphics;
using UIKit;

namespace Board.Screens.Controls
{
	public class UIThumbsContentDisplay : UIContentDisplay
	{
		private interface IBoardComparer : IComparer<Board.Schema.Board> {
			string GetComparisonPropertyDescription(Board.Schema.Board target);

			string Description { get; }
		}

		private class AlphabeticComparer : IBoardComparer
		{
			public string Description { 
				get { return "Alphabetical Order"; }
			}

			public int Compare (Board.Schema.Board x, Board.Schema.Board y)
			{
				return String.Compare(x.Name[0].ToString().ToLower(), y.Name[0].ToString().ToLower());
			}

			public string GetComparisonPropertyDescription(Board.Schema.Board target) {
				return target.Name [0].ToString ().ToLower();
			}
		}

		private class NeighbourhoodComparer : IBoardComparer
		{
			public string Description { 
				get { return "Neighbourhood"; }
			}

			public int Compare (Board.Schema.Board x, Board.Schema.Board y)
			{
				return String.Compare(x.GeolocatorObject.Neighborhood, y.GeolocatorObject.Neighborhood);
			}

			public string GetComparisonPropertyDescription(Board.Schema.Board target) {
				return target.GeolocatorObject.Neighborhood;
			}
		}

		private class CategoryComparer : IBoardComparer
		{
			public string Description {
				get { return "Category"; }
			}

			public int Compare (Board.Schema.Board x, Board.Schema.Board y)
			{
				return String.Compare (x.Category, y.Category);
			}

			public string GetComparisonPropertyDescription (Board.Schema.Board target)
			{
				return target.Category;
			}
		}

		private class DistanceComparer : IBoardComparer
		{
			public string Description { 
				get { return "Distance"; }
			}

			public int Compare (Board.Schema.Board x, Board.Schema.Board y)
			{
				return (int)Math.Floor(x.Distance*100.0) - (int)Math.Floor(y.Distance*100.0);
			}
		
			public string GetComparisonPropertyDescription(Board.Schema.Board target) {
				return string.Empty;
			}
		}

		public readonly float ThumbSize;
		public enum OrderMode { Category = 0, /*Neighborhood,*/ Alphabetic, Distance }
		public const int TopAndBottomSeparation = 20;

		public List<UIBoardThumbComponent> ListThumbComponents;
		List<OrderMode> Modes;

		private IBoardComparer _boardComparer;
		private Dictionary<OrderMode, IBoardComparer> _boardComparersByMode; 
		private List<Board.Schema.Board> BoardList;

		public UIThumbsContentDisplay (List<Board.Schema.Board> boardList, OrderMode mode,
			float extraTopMargin = 0, float extraLowMargin = 0)
		{
			BoardList = boardList;

			if (boardList.Count == 0) {
				return;
			} 

			ThumbSize = AppDelegate.ScreenWidth / 3.5f;

			this._boardComparersByMode = new Dictionary<OrderMode, IBoardComparer> ();
			this._boardComparersByMode.Add (OrderMode.Category, new CategoryComparer ());
			//this._boardComparersByMode.Add (OrderMode.Neighborhood, new NeighbourhoodComparer ());
			this._boardComparersByMode.Add (OrderMode.Alphabetic, new AlphabeticComparer ());
			this._boardComparersByMode.Add (OrderMode.Distance, new DistanceComparer ());

			Modes = new List<OrderMode> ();
			Modes.Add (OrderMode.Category);
			//Modes.Add (OrderMode.Neighborhood);
			Modes.Add (OrderMode.Alphabetic);
			Modes.Add (OrderMode.Distance);

			GenerateList (mode, extraTopMargin, extraLowMargin);

			UserInteractionEnabled = true;
		}
		bool didHaveFirstLoad = false;
		public void GenerateList(){
			if (!didHaveFirstLoad) {
				MemoryUtility.ReleaseSubviews(Subviews);
				GenerateList (OrderMode.Distance);
				didHaveFirstLoad = true;
			}
		}

		private void GenerateList(OrderMode mode, float extraTopMargin = 0, float extraLowMargin = 0){

			ListThumbs = new List<UIContentThumb> ();
			ListThumbComponents = new List<UIBoardThumbComponent> ();

			this._boardComparer = this._boardComparersByMode [mode];

			BoardList = BoardList.OrderBy(x => x, this._boardComparer).ToList();

			var comparer = BoardList[0];

			// starting point
			int linecounter = 1, sectionNumber = 0, i = 0;

			float yposition = TopAndBottomSeparation + UIMenuBanner.Height + extraTopMargin;

			var tap = new UITapGestureRecognizer (tg => {
				MemoryUtility.ReleaseSubviews(Subviews);
				int nextMode = ((int)mode + 1) % Modes.Count;
				GenerateList (Modes[nextMode], extraTopMargin, extraLowMargin);
				SelectiveThumbsRendering(new CGPoint(0,0));
			});

			var filterSelector = new UIFilterSelector (yposition - 20, _boardComparer.Description, tap);
			AddSubview (filterSelector);
			yposition += (float)filterSelector.Frame.Height;

			int locationSeparation = 30, minSeparation = 20, separationBetweenThumbs = 10;
			if (AppDelegate.PhoneVersion == AppDelegate.PhoneVersions.iPhone5 || AppDelegate.PhoneVersion == AppDelegate.PhoneVersions.iPhone4) {
				locationSeparation = 40; minSeparation = 30; separationBetweenThumbs = 20;
			}

			foreach (var b in BoardList) {
				if (this._boardComparer.Compare(comparer, b) != 0 || i == 0) {

					string header = _boardComparer.GetComparisonPropertyDescription (b);

					if (header != string.Empty) {
						
						if (sectionNumber > 0) {
							yposition += ThumbSize / 2 + UIBoardThumbComponent.TextSpace + minSeparation;
						}

						// draw new location string
						var locationLabel = new UILocationLabel (header, 
							                    new CGPoint (UICarouselController.ItemSeparation, yposition), UITextAlignment.Center);

						yposition += (float)locationLabel.Frame.Height + ThumbSize / 2 + minSeparation;
						comparer = b;
						AddSubview (locationLabel);

						linecounter = 1;
						sectionNumber++;
					} else {
						if (i == 0) {
							yposition += UILocationLabel.Height + locationSeparation;
						}
					}
				}

				if (linecounter >= 4) {
					linecounter = 1;
					// nueva linea de thumbs
					yposition += ThumbSize + UIBoardThumbComponent.TextSpace + separationBetweenThumbs;
				}

				var btComponent = new UIBoardThumbComponent (b, new CGPoint ((AppDelegate.ScreenWidth/ 4) * linecounter, yposition), ThumbSize);
				ListThumbs.Add (btComponent.BoardThumb);
				ListThumbComponents.Add (btComponent);
				//AddSubview (btComponent);
				linecounter++;
				i++;
			}

			Frame = new CGRect (0, 0, AppDelegate.ScreenWidth, yposition + ThumbSize / 2 + TopAndBottomSeparation + extraLowMargin + UIBoardThumbComponent.TextSpace + UIActionButton.Height);

			var superView = Superview as UIScrollView;
			if (superView != null) {
				superView.ContentSize = Frame.Size;
			}

			SuscribeToEvents ();
		}

		public void SelectiveThumbsRendering(CGPoint contentOffset){
			foreach (var thumb in ListThumbComponents) {

				if (thumb.Frame.Y > (contentOffset.Y - thumb.Frame.Height) &&
					thumb.Frame.Y < (contentOffset.Y + AppDelegate.ScreenHeight)) {

					AddSubview (thumb);

				} else {
					
					thumb.RemoveFromSuperview ();

				}

			}
		}

		class UIFilterSelector : UILabel{
			UITapGestureRecognizer _tap;

			public UIFilterSelector(float yposition, string text, UITapGestureRecognizer tap){
				Frame = new CGRect(20, yposition, AppDelegate.ScreenWidth - 40, 50);
				Font = UIFont.SystemFontOfSize (16, UIFontWeight.Medium);
				TextColor = AppDelegate.BoardOrange;
				AdjustsFontSizeToFitWidth = true;
				Text = "Sorted by " + text;
				TextAlignment = UITextAlignment.Center;

				UserInteractionEnabled = true;
				AddGestureRecognizer(tap);
				_tap = tap;
			}

			protected override void Dispose (bool disposing)
			{
				RemoveGestureRecognizer (_tap);
			}
		}
	}
}

