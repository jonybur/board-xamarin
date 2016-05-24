using System;
using Board.Interface.Buttons;
using Board.Interface.LookUp;
using Board.Schema;
using CoreGraphics;
using UIKit;

namespace Board.Interface.Widgets
{
	public partial class Widget
	{
		private UIPanGestureRecognizer PanGestureRecognizer;
		private UIRotationGestureRecognizer RotationGestureRecognizer;
		private UIPinchGestureRecognizer PinchGestureRecognizer;
		private UITapGestureRecognizer TapGestureRecognizer, DoubleTapGestureRecognizer;
		private UILongPressGestureRecognizer LongPressGestureRecognizer;

		public override UIView HitTest (CGPoint point, UIEvent uievent)
		{
			if (Alpha > 0) {
				foreach (var subview in Subviews) {
					var subPoint = subview.ConvertPointFromView (point, this);
					var result = subview.HitTest (subPoint, uievent);
					if (result != null) {
						return result;
					}
				}
			}
			return base.HitTest(point, uievent);
		}

		protected void CreateGestures()
		{
			DoubleTapGestureRecognizer = SetNewDoubleTapGestureRecognizer ();
			TapGestureRecognizer = SetNewTapGestureRecognizer ();
			LongPressGestureRecognizer = SetNewLongPressGestureRecognizer ();

			TapGestureRecognizer.RequireGestureRecognizerToFail (DoubleTapGestureRecognizer);

			PanGestureRecognizer = SetNewPanGestureRecognizer ();
			RotationGestureRecognizer = SetNewRotationGestureRecognizer ();
			PinchGestureRecognizer = SetNewPinchGestureRecognizer ();
		}

		private UIPanGestureRecognizer SetNewPanGestureRecognizer()
		{
			float dx = 0;
			float dy = 0;

			var panGesture = new UIPanGestureRecognizer (pg => {
				if ((pg.State == UIGestureRecognizerState.Began || pg.State == UIGestureRecognizerState.Changed)) {
					var p0 = pg.LocationInView(Superview);

					if (dx == 0)
						dx = (float)(p0.X - Center.X);

					if (dy == 0)
						dy = (float)(p0.Y - Center.Y);

					var p1 = new CGPoint (p0.X - dx, p0.Y - dy);

					Center = p1;

				} else if (pg.State == UIGestureRecognizerState.Ended) {
					dx = 0;
					dy = 0;
				}

			});

			panGesture.Delegate = new CustomGestureRecognizerDelegate ();

			return panGesture;
		}

		private UIRotationGestureRecognizer SetNewRotationGestureRecognizer()
		{
			var rotateGesture = new UIRotationGestureRecognizer (rg => {
				if ((rg.State == UIGestureRecognizerState.Began || rg.State == UIGestureRecognizerState.Changed) && (rg.NumberOfTouches == 2)) {

					Transform = CGAffineTransform.Rotate(Transform, rg.Rotation);

					rg.Rotation = 0;

				}
			});

			rotateGesture.Delegate = new CustomGestureRecognizerDelegate ();

			return rotateGesture;
		}

		private UIPinchGestureRecognizer SetNewPinchGestureRecognizer(){

			var panGesture = new UIPinchGestureRecognizer (pinch => {
				if ((pinch.State == UIGestureRecognizerState.Began || pinch.State == UIGestureRecognizerState.Changed) && (pinch.NumberOfTouches == 2)) {

					var bounds = Bounds;
					var pinchCenter = pinch.LocationInView(this);
					pinchCenter.X -= bounds.GetMidX();
					pinchCenter.Y -= bounds.GetMidY();
					var transform = Transform;
					transform = CGAffineTransform.Translate(transform, pinchCenter.X, pinchCenter.Y);
					var scale = pinch.Scale;
					transform = CGAffineTransform.Scale(transform, scale, scale);
					transform = CGAffineTransform.Translate(transform, -pinchCenter.X, -pinchCenter.Y);

					if (transform.xx < 1.5f && transform.xx > .75f && transform.yx < 1.5f && transform.yy > .75f) {
						Transform = transform;
					}

					pinch.Scale = 1f;
				} 

			});

			panGesture.Delegate = new CustomGestureRecognizerDelegate ();

			return panGesture;
		}

		private UITapGestureRecognizer SetNewDoubleTapGestureRecognizer(){
			var doubletap = new UITapGestureRecognizer (tg => {
				if (Preview.IsAlive) { return; }

				Like();
			});

			doubletap.NumberOfTapsRequired = 2;
			doubletap.DelaysTouchesBegan = true;

			return doubletap;
		}

		private UITapGestureRecognizer SetNewTapGestureRecognizer(){
			var tapgesture = new UITapGestureRecognizer (tg => {
				if (Preview.IsAlive) { return;	}

				if (LikeComponent.Frame.Left < tg.LocationInView(this).X &&
					LikeComponent.Frame.Top < tg.LocationInView(this).Y)
				{
					Like();
				}
				else
				{

					UILookUp lookUp;

					if (content is Video)
					{
						lookUp = new VideoLookUp((Video)content);
					} 
					else if (content is Picture)
					{
						lookUp = new PictureLookUp((Picture)content);
					}
					else if (content is Announcement)
					{
						lookUp = new AnnouncementLookUp((Announcement)content);
					}
					else if (content is BoardEvent)
					{
						lookUp = new EventLookUp((BoardEvent)content);
					}
					else if (content is Map)
					{
						lookUp = new MapLookUp(UIBoardInterface.board.GeolocatorObject);
					}
					else if (content is Poll)
					{
						return;
					}
					else {
						lookUp = new UILookUp();
					}

					AppDelegate.PushViewLikePresentView(lookUp);
				}
			});

			tapgesture.DelaysTouchesBegan = true;
			tapgesture.NumberOfTapsRequired = 1;

			return tapgesture;
		}

		private UILongPressGestureRecognizer SetNewLongPressGestureRecognizer(){
			// to edit the widget location
			var longPress = new UILongPressGestureRecognizer (tg => {

				UnsuscribeFromUsabilityEvents();
				SuscribeToEditingEvents();

				CreateDeleteButton ();
				AddSubview(DeleteButton);

				IdToUpdate = this.content.Id;
				AuxCenter = Center;

				ButtonInterface.SwitchButtonLayout(ButtonInterface.ButtonLayout.MoveWidgetBar);

			});

			return longPress;
		}

		class CustomGestureRecognizerDelegate : UIGestureRecognizerDelegate{
			public override bool ShouldRecognizeSimultaneously (UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
			{
				if (gestureRecognizer.View != otherGestureRecognizer.View) {
					return false;
				}
				return true;
			}
		}

		public void SuscribeToUsabilityEvents ()
		{
			AddGestureRecognizer (LongPressGestureRecognizer);
			AddGestureRecognizer (TapGestureRecognizer);
			AddGestureRecognizer (DoubleTapGestureRecognizer);
			if (UIBoardInterface.UserCanEditBoard) {
				AddGestureRecognizer (LongPressGestureRecognizer);
			}
		}

		public void UnsuscribeFromUsabilityEvents()
		{
			RemoveGestureRecognizer (LongPressGestureRecognizer);
			RemoveGestureRecognizer (TapGestureRecognizer);
			RemoveGestureRecognizer (DoubleTapGestureRecognizer);			
			if (UIBoardInterface.UserCanEditBoard) {
				RemoveGestureRecognizer (LongPressGestureRecognizer);
			}
		}

		public void SuscribeToEditingEvents(){
			AddGestureRecognizer(PanGestureRecognizer);
			AddGestureRecognizer(RotationGestureRecognizer);
			AddGestureRecognizer(PinchGestureRecognizer);
		}

		public void UnsuscribeFromEditingEvents(){
			RemoveGestureRecognizer(PanGestureRecognizer);
			RemoveGestureRecognizer(RotationGestureRecognizer);
			RemoveGestureRecognizer(PinchGestureRecognizer);
		}

		public void DisableEditing(){
			DeleteButton.RemoveFromSuperview ();
			UnsuscribeFromEditingEvents ();
			SuscribeToUsabilityEvents ();
		}
	}
}

