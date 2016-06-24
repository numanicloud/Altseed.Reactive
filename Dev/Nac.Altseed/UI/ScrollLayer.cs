﻿using System;
using System.Linq;
using System.Reactive.Linq;
using asd;
using Nac.Altseed.Linq;
using Nac.Altseed.ObjectSystem;

namespace Nac.Altseed.UI
{
	public class ScrollLayer : ReactiveLayer2D
	{
		private Vector2DF starting_, ending_;
		private Vector2DF position_, cameraSize_;
		private RectF bindingAreaRange_;

		private CameraObject2D camera;
		private RectF seeingArea;
		private IDisposable disposableForSeeingArea;
		private IDisposable scrollDisposable;
		private Vector2DF cameraPositionGoalOfAnimation;
		private Func<Vector2DF, IObservable<RectF>> getCameraMoving;

		public Vector2DF BoundaryStartingPosition
		{
			get { return starting_; }
			set
			{
				starting_ = value;
				ReviseCamera(seeingArea);
			}
		}
		public Vector2DF BoundaryEndingPosition
		{
			get { return ending_; }
			set
			{
				ending_ = value;
				ReviseCamera(seeingArea);
			}
		}
		public Vector2DF Position
		{
			get { return position_; }
			set
			{
				position_ = value;
				ResetCamera();
			}
		}
		public Vector2DF CameraSize
		{
			get { return cameraSize_; }
			set
			{
				cameraSize_ = value;
				ResetCamera();
			}
		}
		public RectF BindingAreaRange
		{
			get { return bindingAreaRange_; }
			set
			{
				bindingAreaRange_ = value;
				ReviseCamera(seeingArea);
			}
		}
		public RectI CameraSrc => camera.Src;
        public RectF SeeingArea => seeingArea;


		public ScrollLayer()
		{
			camera = new CameraObject2D();
			AddObject(camera);
			getCameraMoving = p => Observable.Return(camera.Src.ToF().WithPosition(p));
		}

		public void SubscribeSeeingArea(IObservable<RectF> onSeeingAreaChanged)
		{
			disposableForSeeingArea?.Dispose();
			disposableForSeeingArea = onSeeingAreaChanged.Subscribe(rect =>
			{
				seeingArea = rect;
				ReviseCamera(rect);
			});
		}

		public void SetEasingBehaviorUp(EasingStart start, EasingEnd end, int time)
		{
			getCameraMoving = target => OnUpdateEvent
				.Select(t => camera.Src.Position.To2DF())
				.EasingVector2DF(target, EasingStart.StartRapidly2, EasingEnd.EndSlowly3, time)
				.Select(p => camera.Src.ToF().WithPosition(p));
        }


		private void ReviseCamera(RectF rect)
		{
			var offset = new Vector2DF();

			var innerBindingRect = new RectF(
				cameraPositionGoalOfAnimation.X + BindingAreaRange.X,
				cameraPositionGoalOfAnimation.Y + BindingAreaRange.Y,
				BindingAreaRange.Width,
				BindingAreaRange.Height);
			offset += GetJut(rect, innerBindingRect);

			var outerBindingRect = new RectF(
				BoundaryStartingPosition.X,
				BoundaryStartingPosition.Y,
				BoundaryEndingPosition.X - BoundaryStartingPosition.X,
				BoundaryEndingPosition.Y - BoundaryStartingPosition.Y);
			offset -= GetJut(camera.Src.ToF().WithPosition(cameraPositionGoalOfAnimation + offset), outerBindingRect);

			if(offset != new Vector2DF(0, 0))
			{
				cameraPositionGoalOfAnimation = cameraPositionGoalOfAnimation + offset;
				scrollDisposable?.Dispose();
				scrollDisposable = getCameraMoving(cameraPositionGoalOfAnimation)
					.Subscribe(r => camera.Src = r.ToI());
			}
		}

		private Vector2DF GetJut(RectF rect, RectF bound)
		{
			Vector2DF result = new Vector2DF();
			if(rect.X + rect.Width > bound.X + bound.Width)
			{
				result.X = rect.X + rect.Width - (bound.X + bound.Width);
			}
			if(rect.X - result.X < bound.X)
			{
				result.X += rect.X - result.X - bound.X;
			}

			if(rect.Y + rect.Height > bound.Y + bound.Height)
			{
				result.Y = rect.Y + rect.Height - (bound.Y + bound.Height);
			}
			if(rect.Y - result.Y < bound.Y)
			{
				result.Y += rect.Y - result.Y - bound.Y;
			}

			return result;
		}

		private RectF AddPosition(RectF source, Vector2DF d)
		{
			return new RectF(source.X + d.X, source.Y + d.Y, source.Width, source.Height);
		}

		private void ResetCamera()
		{
			camera.Src = new RectF(camera.Src.X, camera.Src.Y, CameraSize.X, CameraSize.Y).ToI();
			camera.Dst = new RectF(Position.X, Position.Y, CameraSize.X, CameraSize.Y).ToI();
			ReviseCamera(seeingArea);
		}
	}
}
