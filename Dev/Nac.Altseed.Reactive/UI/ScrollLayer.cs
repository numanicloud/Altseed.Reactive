using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Reactive.UI
{
	public class ScrollLayer : Layer2D
	{
		private Vector2DF starting_, ending_, position_, cameraSize_;
		private RectF bindingAreaRange_;
		private IDisposable scrollDisposable;

		private CameraObject2D camera { get; set; }
		private RectF seeingArea { get; set; }
		private IDisposable disposableForSeeingArea { get; set; }
		private Vector2DF seeingAreaRangeRightBottom { get; set; }

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
				seeingAreaRangeRightBottom = BindingAreaRange.Vertexes[2];
				ReviseCamera(seeingArea);
			}
		}
		public RectI CameraSrc => camera.Src;


		public ScrollLayer()
		{
			camera = new CameraObject2D();
			AddObject(camera);
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


		private void ReviseCamera(RectF rect)
		{
			var innerBindingRect = new RectF(
				camera.Src.X + BindingAreaRange.X,
				camera.Src.Y + BindingAreaRange.Y,
				BindingAreaRange.Width,
				BindingAreaRange.Height);
			var outerBindingRect = new RectF(
				Starting.X,
				Starting.Y,
				Ending.X - Starting.X,
				Ending.Y - Starting.Y);

			var offset = new Vector2DF();
			offset += GetJut(rect, innerBindingRect);
			offset -= GetJut(AddPosition(camera.Src.ToFloat(), offset), outerBindingRect);

			if(offset != new Vector2DF(0, 0))
			{
				scrollDisposable?.Dispose();
				scrollDisposable = UpdateManager.Instance.FrameUpdate
					.Select(t => camera.Src.Position.To2DF())
					.EasingVector2DF(camera.Src.Position.To2DF() + offset, EasingStart.StartRapidly2, EasingEnd.EndSlowly3, 10)
					.Select(p => p.To2DI())
					.Subscribe(p => camera.Src = new RectI(p.X, p.Y, camera.Src.Width, camera.Src.Height));
			}
		}

		private Vector2DF GetJut(RectF rect, RectF bound)
		{
			Vector2DF result = new Vector2DF();

			if(rect.X < bound.X)
			{
				result.X = rect.X - bound.X;
			}
			else if(rect.Width < bound.Width && rect.X + rect.Width > bound.X + bound.Width)
			{
				result.X = rect.X + rect.Width - (bound.X + bound.Width);
			}

			if(rect.Y < bound.Y)
			{
				result.Y = rect.Y - bound.Y;
			}
			else if(rect.Height < bound.Height && rect.Y + rect.Height > bound.Y + bound.Height)
			{
				result.Y = rect.Y + rect.Height - (bound.Y + bound.Height);
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
