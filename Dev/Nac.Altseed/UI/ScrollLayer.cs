using System;
using System.Linq;
using System.Reactive.Linq;
using asd;
using Nac.Altseed.Linq;

namespace Nac.Altseed.UI
{
	public class ScrollLayer : Layer2D
	{
		private Vector2DF starting_, ending_;
		private Vector2DF position_, cameraSize_;
		private RectF bindingAreaRange_;

		private CameraObject2D camera { get; set; }
		private RectF seeingArea { get; set; }
		private IDisposable disposableForSeeingArea { get; set; }
		private IDisposable scrollDisposable { get; set; }
		private Vector2DF cameraTargetPosition { get; set; }
		private Func<Vector2DF, IObservable<RectF>> getCameraMoving { get; set; }

		public Vector2DF Starting
		{
			get { return starting_; }
			set
			{
				starting_ = value;
				ReviseCamera(seeingArea);
			}
		}
		public Vector2DF Ending
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
			getCameraMoving = p => Observable.Return(camera.Src.ToFloat().WithPosition(p));
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
			getCameraMoving = target => UpdateManager.Instance.FrameUpdate
				.Select(t => camera.Src.Position.To2DF())
				.EasingVector2DF(target, EasingStart.StartRapidly2, EasingEnd.EndSlowly3, time)
				.Select(p => camera.Src.ToFloat().WithPosition(p));
        }


		private void ReviseCamera(RectF rect)
		{
			var offset = new Vector2DF();

			var innerBindingRect = new RectF(
				cameraTargetPosition.X + BindingAreaRange.X,
				cameraTargetPosition.Y + BindingAreaRange.Y,
				BindingAreaRange.Width,
				BindingAreaRange.Height);
			offset += GetJut(rect, innerBindingRect);

			var outerBindingRect = new RectF(
				Starting.X,
				Starting.Y,
				Ending.X - Starting.X,
				Ending.Y - Starting.Y);
			offset -= GetJut(camera.Src.ToFloat().WithPosition(cameraTargetPosition + offset), outerBindingRect);

			if(offset != new Vector2DF(0, 0))
			{
				cameraTargetPosition = cameraTargetPosition + offset;
				scrollDisposable?.Dispose();
				scrollDisposable = getCameraMoving(cameraTargetPosition)
					.Subscribe(r => camera.Src = r.ToInt());
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
			camera.Src = new RectF(camera.Src.X, camera.Src.Y, CameraSize.X, CameraSize.Y).ToInt();
			camera.Dst = new RectF(Position.X, Position.Y, CameraSize.X, CameraSize.Y).ToInt();
			ReviseCamera(seeingArea);
		}
	}
}
