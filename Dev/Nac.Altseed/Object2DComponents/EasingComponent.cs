using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Object2DComponents
{
	public class EasingComponent<TObject> : Object2DComponent where TObject : Object2D
	{
		private int count, maxCount;
		private float lastValue;
		private float[] easing;
		private Func<TObject, float> getter;
		private Action<TObject, float> setter;
		private Action callback;

		public EasingComponent(
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
	}
}
