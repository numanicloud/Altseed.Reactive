using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed
{
	public class EasingComponent2<TObject> : Object2DComponent where TObject : Object2D
	{
		int count, maxCount;
		float lastValue;
		float[] easing;
		Func<TObject, float> getter;
		Action<TObject, float> setter;
		Action callback;

		public EasingComponent2(
			Func<TObject, float> getter,
			Action<TObject, float> setter,
			float goal,
			EasingStart start,
			EasingEnd end,
			int count,
			Action callback = null)
		{
			easing = Easing.GetEasingFunction(start, end);
			lastValue = goal;
			maxCount = count;

			this.getter = getter;
			this.setter = setter;
			this.callback = callback;
		}

		protected override void OnUpdate()
		{
			var o = Owner as TObject;
            ++count;
			var v = Easing.GetNextValue(getter(o), lastValue, count, maxCount, easing);
			setter(o, v);
			if(count >= maxCount)
			{
				if(callback != null)
				{
					callback();
				}
				Vanish();
			}
		}

		public static EasingComponent2<Object2D> CreateXEasing(float goal, EasingStart start, EasingEnd end, int count, Action callback = null)
		{
			return new EasingComponent2<Object2D>(
				o => o.Position.X,
				(o, v) => o.Position = new Vector2DF(v, o.Position.Y),
				goal,
				start,
				end,
				count,
				callback);
		}
	}
}
