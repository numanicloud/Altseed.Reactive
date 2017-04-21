using asd;
using Nac.Altseed.Easings;
using Nac.Altseed.Easings.Library;
using Nac.Altseed.Linq;
using Nac.Altseed.ObjectSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Altseed.Test.Easing
{
	class EasingVisual : GeometryObject2D
	{
		int counter;
		int trackingTime;
		Func<int, Vector2DF> frameToPosition;
		Subject<float> updateTimeSubject;

		public IObservable<float> UpdateTime => updateTimeSubject;

		public EasingVisual(Func<int, Vector2DF> frameToPosition, int trackingTime)
		{
			Shape = new CircleShape
			{
				OuterDiameter = 25,
				NumberOfCorners = 36,
			};
			counter = 0;
			updateTimeSubject = new Subject<float>();
			this.trackingTime = trackingTime;
			this.frameToPosition = frameToPosition;
		}

		protected override void OnUpdate()
		{
			Position = frameToPosition?.Invoke(counter) ?? Position;
			++counter;
			updateTimeSubject.OnNext(counter);

			if (counter <= trackingTime && counter % 2 == 0)
			{
				var child = new GeometryObject2D
				{
					Shape = new CircleShape
					{
						OuterDiameter = 4,
						NumberOfCorners = 24,
					},
					Position = Position,
					Color = new Color(0, 128, 255),
				};
				Layer.AddObject(child);
			}
		}
	}

	class CubicEasingTest : AltseedTest
	{
		protected override void OnStart()
		{
			int easingTime = 30;
			int waitTime = 30;

			int GetTimeInEasing(int frame)
			{
				var t = frame % (easingTime + waitTime);
				return Math.Min(easingTime, t);
			}

			{
				var linear = new LinearEasing();

				Vector2DF LinearPosition(int frame)
				{
					var t = GetTimeInEasing(frame);
					return new Vector2DF(linear.GetGeneralValue(t, easingTime, 100, 500), 30);
				}

				Engine.AddObject2D(new EasingVisual(LinearPosition, easingTime));
			}

			{
				var obj = new EasingVisual(null, easingTime);
				Engine.AddObject2D(obj);
				obj.UpdateTime.LinearEasingNormalValue()
					.Generalize(easingTime, 100, 500)
					.Subscribe(x => obj.Position = new Vector2DF(x, 80));
			}

			{
				var cubic = new CubicEasing(CubicEasing.Speed.Rapidly2, CubicEasing.Speed.Slowly3);

				Vector2DF CubicPosition(int frame)
				{
					var t = GetTimeInEasing(frame);
					return new Vector2DF(cubic.GetGeneralValue(t, easingTime, 100, 500), 130);
				}

				Engine.AddObject2D(new EasingVisual(CubicPosition, easingTime));
			}

			{
				var obj = new EasingVisual(null, easingTime);
				Engine.AddObject2D(obj);
				obj.UpdateTime.CubicEasingsNormalValue(CubicEasing.Speed.Rapidly2, CubicEasing.Speed.Slowly3)
					.Generalize(easingTime, 100, 500)
					.Subscribe(x => obj.Position = new Vector2DF(x, 180));
			}
			
			{
				var bounce = new BounceEasing(10, 0.5f);

				Vector2DF BouncePosition(int frame)
				{
					var t = GetTimeInEasing(frame);
					return new Vector2DF(bounce.GetGeneralValue(t, easingTime, 100, 500), 230);
				}

				Engine.AddObject2D(new EasingVisual(BouncePosition, easingTime));
			}

			{
				var bounce = new BounceEasing(10, 0.2f);

				Vector2DF BouncePosition(int frame)
				{
					var t = GetTimeInEasing(frame);
					return new Vector2DF(bounce.GetGeneralValue(t, easingTime, 100, 500), 280);
				}

				Engine.AddObject2D(new EasingVisual(BouncePosition, easingTime));
			}
		}
	}
}
