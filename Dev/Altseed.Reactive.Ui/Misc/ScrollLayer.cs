using System;
using System.Reactive.Linq;
using Altseed.Reactive;
using Altseed.Reactive.Easings;
using Altseed.Reactive.Object;
using asd;

namespace Altseed.Reactive.Ui
{
	/// <summary>
	/// 注目している範囲が特定の範囲に含まれるように、レイヤーのスクロールを制御するクラス。
	/// </summary>
	public class ScrollLayer : ReactiveLayer2D
	{
		private readonly CameraObject2D camera_;

		private Vector2DF actualCameraPosition_;
		private RectF bindingAreaRange_;
		private Vector2DF starting_, ending_;
		private Vector2DF position_, cameraSize_;

		private IEasing easing_;
		private Func<Vector2DF, IObservable<RectF>> getCameraMoving_;
		private IDisposable disposableForSeeingArea_;
		private IDisposable scrollDisposable_;

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
		public void SetEasingBehaviorUp(IEasing easing, int time)
		{
			getCameraMoving_ = target =>
			{
				var initial = camera_.Src.Position.To2DF();
				return OnUpdateEvent.Select((x, i) => easing.GetGeneralValue(i, time, 0, 1))
					.Select(t => initial * (1 - t) + target * t)
					.Select(x => camera_.Src.ToF().WithPosition(x));
			};
		}


		private void ReviseCamera(RectF rect)
		{
			var offset = new Vector2DF();

			//　包含範囲が注目範囲を追いかけるように補正を設定する。
			var innerBindingRect = new RectF(
				actualCameraPosition_.X + BindingAreaRange.X,
				actualCameraPosition_.Y + BindingAreaRange.Y,
				BindingAreaRange.Width,
				BindingAreaRange.Height);
			offset += GetJut(rect, innerBindingRect);

			// カメラが見ても良い範囲を飛び出さないようにする。
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

		/// <summary>
		/// 矩形のはみだしを計算します。
		/// </summary>
		/// <param name="inner">内側の矩形。</param>
		/// <param name="outer">外側の矩形。</param>
		/// <returns>はみ出しが最小になるような補正値。</returns>
		private Vector2DF GetJut(RectF inner, RectF outer)
		{
			var result = new Vector2DF();
			if (inner.X + inner.Width > outer.X + outer.Width)
			{
				result.X = inner.X + inner.Width - (outer.X + outer.Width);
			}
			if (inner.X - result.X < outer.X)
			{
				result.X += inner.X - result.X - outer.X;
			}

			if (inner.Y + inner.Height > outer.Y + outer.Height)
			{
				result.Y = inner.Y + inner.Height - (outer.Y + outer.Height);
			}
			if (inner.Y - result.Y < outer.Y)
			{
				result.Y += inner.Y - result.Y - outer.Y;
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