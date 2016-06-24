using System;
using System.Reactive.Linq;
using asd;
using Nac.Altseed.Linq;
using Nac.Altseed.ObjectSystem;

namespace Nac.Altseed.UI
{
	public class ScrollLayer : ReactiveLayer2D
	{
		private Vector2DF actualCameraPosition_;
		private RectF bindingAreaRange_;

		private readonly CameraObject2D camera_;
		private IDisposable disposableForSeeingArea_;
		private Func<Vector2DF, IObservable<RectF>> getCameraMoving_;
		private Vector2DF position_, cameraSize_;
		private IDisposable scrollDisposable_;
		private Vector2DF starting_, ending_;

		public Vector2DF BoundaryStartingPosition
		{
			get { return starting_; }
			set
			{
				starting_ = value;
				ReviseCamera(SeeingArea);
			}
		}

		public Vector2DF BoundaryEndingPosition
		{
			get { return ending_; }
			set
			{
				ending_ = value;
				ReviseCamera(SeeingArea);
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
				ReviseCamera(SeeingArea);
			}
		}

		public RectI CameraSrc => camera_.Src;
		public RectF SeeingArea { get; private set; }


		/// <summary>
		/// ScrollLayer クラスを初期化します。
		/// </summary>
		public ScrollLayer()
		{
			camera_ = new CameraObject2D();
			AddObject(camera_);
			getCameraMoving_ = p => Observable.Return(camera_.Src.ToF().WithPosition(p));
		}

		/// <summary>
		/// カメラで映す範囲を強制的に変更します。
		/// </summary>
		/// <param name="initialCameraSrc">カメラが映す範囲。</param>
		/// <param name="initialSeeingArea">注目する範囲。</param>
		public void SetScrollPosition(RectI initialCameraSrc, RectF initialSeeingArea)
		{
			actualCameraPosition_ = initialCameraSrc.Position.To2DF();
			camera_.Src = initialCameraSrc;
			ReviseCamera(initialSeeingArea);
		}

		/// <summary>
		/// 注目している範囲が移動したときに通知するイベントを登録します。
		/// </summary>
		/// <param name="onSeeingAreaChanged">注目している範囲が移動したときに通知するイベント。</param>
		public void SubscribeSeeingArea(IObservable<RectF> onSeeingAreaChanged)
		{
			disposableForSeeingArea_?.Dispose();
			disposableForSeeingArea_ = onSeeingAreaChanged.Subscribe(rect =>
			{
				SeeingArea = rect;
				ReviseCamera(rect);
			});
		}

		/// <summary>
		/// スクロールを滑らかに行うように準備します。
		/// </summary>
		/// <param name="start">アニメーションの開始速度。</param>
		/// <param name="end">アニメーションの終了速度。</param>
		/// <param name="time">アニメーションにかける時間。</param>
		public void SetEasingBehaviorUp(EasingStart start, EasingEnd end, int time)
		{
			getCameraMoving_ = target => OnUpdateEvent
				.Select(t => camera_.Src.Position.To2DF())
				.EasingVector2DF(target, EasingStart.StartRapidly2, EasingEnd.EndSlowly3, time)
				.Select(p => camera_.Src.ToF().WithPosition(p));
		}


		private void ReviseCamera(RectF rect)
		{
			var offset = new Vector2DF();

			var innerBindingRect = new RectF(
				actualCameraPosition_.X + BindingAreaRange.X,
				actualCameraPosition_.Y + BindingAreaRange.Y,
				BindingAreaRange.Width,
				BindingAreaRange.Height);
			offset += GetJut(rect, innerBindingRect);

			var outerBindingRect = new RectF(
				BoundaryStartingPosition.X,
				BoundaryStartingPosition.Y,
				BoundaryEndingPosition.X - BoundaryStartingPosition.X,
				BoundaryEndingPosition.Y - BoundaryStartingPosition.Y);
			offset -= GetJut(camera_.Src.ToF().WithPosition(actualCameraPosition_ + offset), outerBindingRect);

			if (offset != new Vector2DF(0, 0))
			{
				actualCameraPosition_ = actualCameraPosition_ + offset;
				scrollDisposable_?.Dispose();
				scrollDisposable_ = getCameraMoving_(actualCameraPosition_)
					.Subscribe(r => camera_.Src = r.ToI());
			}
		}

		private Vector2DF GetJut(RectF rect, RectF bound)
		{
			var result = new Vector2DF();
			if (rect.X + rect.Width > bound.X + bound.Width)
			{
				result.X = rect.X + rect.Width - (bound.X + bound.Width);
			}
			if (rect.X - result.X < bound.X)
			{
				result.X += rect.X - result.X - bound.X;
			}

			if (rect.Y + rect.Height > bound.Y + bound.Height)
			{
				result.Y = rect.Y + rect.Height - (bound.Y + bound.Height);
			}
			if (rect.Y - result.Y < bound.Y)
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
			camera_.Src = new RectF(camera_.Src.X, camera_.Src.Y, CameraSize.X, CameraSize.Y).ToI();
			camera_.Dst = new RectF(Position.X, Position.Y, CameraSize.X, CameraSize.Y).ToI();
			ReviseCamera(SeeingArea);
		}
	}
}