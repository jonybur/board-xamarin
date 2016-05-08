using System;
using UIKit;
using MGImageUtilitiesBinding;
using System.Linq;
using System.Collections.Generic;
using Board.Interface;
using CoreGraphics;
using Board.Utilities;

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
				get { return "ALPHABETIC"; }
			}

			public int Compare (Board.Schema.Board x, Board.Schema.Board y)
			{
				return String.Compare(x.Name[0].ToString(), y.Name[0].ToString());
			}

			public string GetComparisonPropertyDescription(Board.Schema.Board target) {
				return target.Name [0].ToString ();
			}
		}

		private class NeighbourhoodComparer : IBoardComparer
		{
			public string Description { 
				get { return "NEIGHBOURHOOD"; }
			}

			public int Compare (Board.Schema.Board x, Board.Schema.Board y)
			{
				return String.Compare(x.GeolocatorObject.Neighborhood, y.GeolocatorObject.Neighborhood);
			}

			public string GetComparisonPropertyDescription(Board.Schema.Board target) {
				return target.GeolocatorObject.Neighborhood;
			}
		}

		private class DistanceComparer : IBoardComparer
		{
			public string Description { 
				get { return "DISTANCE"; }
			}

			public int Compare (Board.Schema.Board x, Board.Schema.Board y)
			{
				return (int)Math.Floor(x.Distance*10.0) - (int)Math.Floor(y.Distance*10.0);
			}
		
			public string GetComparisonPropertyDescription(Board.Schema.Board target) {
				return string.Empty;
			}
		}

		public readonly float ThumbSize;
		public enum OrderMode { Neighborhood = 0, Alphabetic, Distance }
		public const int TopAndBottomSeparation = 20;

		public List<UIBoardThumbComponent> ListThumbComponents;
		List<OrderMode> Modes;

		private IBoardComparer _boardComparer;
		private Dictionary<OrderMode, IBoardComparer> _boardComparersByMode; 

		public UIThumbsContentDisplay (List<Board.Schema.Board> boardList, OrderMode mode,
			float extraTopMargin = 0, float extraLowMargin = 0)
		{
			if (boardList.Count == 0) {
				return;
			} 

			ThumbSize = AppDelegate.ScreenWidth / 3.5f;

			this._boardComparersByMode = new Dictionary<OrderMode, IBoardComparer> ();
			this._boardComparersByMode.Add (OrderMode.Alphabetic, new AlphabeticComparer ());
			this._boardComparersByMode.Add (OrderMode.Neighborhood, new NeighbourhoodComparer ());
			this._boardComparersByMode.Add (OrderMode.Distance, new DistanceComparer ());

			Modes = new List<OrderMode> ();
			Modes.Add (OrderMode.Neighborhood);
			Modes.Add (OrderMode.Alphabetic);
			Modes.Add (OrderMode.Distance);

			GenerateList (boardList, mode, extraTopMargin, extraLowMargin);

			UserInteractionEnabled = true;
		}

		private void GenerateList(List<Board.Schema.Board> boardList, OrderMode mode,
			float extraTopMargin = 0, float extraLowMargin = 0){

			ListThumbs = new List<UIContentThumb> ();
			ListThumbComponents = new List<UIBoardThumbComponent> ();

			this._boardComparer = this._boardComparersByMode [mode];

			boardList = boardList.OrderBy(x => x, this._boardComparer).ToList();

			var comparer = boardList[0];

			// starting point
			int linecounter = 1, sectionNumber = 0, i = 0;

			float yposition = TopAndBottomSeparation + UIMenuBanner.Height + extraTopMargin;

			var tap = new UITapGestureRecognizer(tg => {
				MemoryUtility.ReleaseSubviews(Subviews);
				int nextMode = ((int)mode + 1) % Modes.Count;
				GenerateList (boardList, Modes[nextMode], extraTopMargin, extraLowMargin);
			});

			var filterSelector = new UIFilterSelector (yposition - 20, _boardComparer.Description, tap);
			AddSubview (filterSelector);
			yposition += (float)filterSelector.Frame.Height;

			foreach (Board.Schema.Board b in boardList) {
				if (this._boardComparer.Compare(comparer, b) != 0 || i == 0) {

					string header = _boardComparer.GetComparisonPropertyDescription (b);

					if (header != string.Empty) {
						
						if (sectionNumber > 0) {
							yposition += ThumbSize / 2 + UIBoardThumbComponent.TextSpace + 10;
						}

						// draw new location string
						var locationLabel = new UILocationLabel (header, 
							                    new CGPoint (UICarouselController.ItemSeparation, yposition), UITextAlignment.Center);

						yposition += (float)locationLabel.Frame.Height + ThumbSize / 2 + 10;
						comparer = b;
						AddSubview (locationLabel);

						linecounter = 1;
						sectionNumber++;
					} else {
						if (i == 0) {
							yposition += UILocationLabel.Height + 30;
						}
					}
				}

				if (linecounter >= 4) {
					linecounter = 1;
					// nueva linea de thumbs
					yposition += ThumbSize + UIBoardThumbComponent.TextSpace;
				}

				var btComponent = new UIBoardThumbComponent (b, new CGPoint ((AppDelegate.ScreenWidth/ 4) * linecounter, yposition), ThumbSize);
				ListThumbs.Add (btComponent.BoardThumb);
				ListThumbComponents.Add (btComponent);
				AddSubview (btComponent);
				linecounter++;
				i++;
			}

			Frame = new CGRect (0, 0, AppDelegate.ScreenWidth, yposition + ThumbSize / 2 + TopAndBottomSeparation + extraLowMargin + UIBoardThumbComponent.TextSpace);

			var superView = Superview as UIScrollView;
			if (superView != null) {
				superView.ContentSize = Frame.Size;
			}

			SuscribeToEvents ();
		}

		class UIFilterSelector : UILabel{
			UITapGestureRecognizer _tap;

			public UIFilterSelector(float yposition, string text, UITapGestureRecognizer tap){
				Frame = new CGRect(20, yposition, AppDelegate.ScreenWidth - 40, 50);
				Font = AppDelegate.Narwhal16;
				TextColor = AppDelegate.BoardOrange;
				AdjustsFontSizeToFitWidth = true;
				Text = "SORT: " + text;
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

