using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Object2DComponents
{
	public class EasingComponent2D<TObject> : Object2DComponent where TObject : Object2D
	{
		private TObject typedOwner;
		private Func<TObject, Vector2DF> getter;
		private Action<TObject, Vector2DF> setter;
		private int count, maxCount;
		private Vector2DF lastValue;
		private float[] easing;
		private Action callback;

		public EasingComponent2D(
			Func<TObject, Vector2DF> getter,
			Action<TObject, Vector2DF> setter,
			Vector2DF goal,
			EasingStart start,
			EasingEnd end,
			int count,
			Action callback = null)
		{
			easing = Easing.GetEasingFunction(start, end);
			lastValue = goal;
			maxCount = count;
			count = 0;
			this.callback = callback;
			this.getter = getter;
			this.setter = setter;
		}

		protected override void OnUpdate()
		{
			if(typedOwner == null)
			{
				typedOwner = (TObject)Owner;
			}

			++count;
			var v = Easing.GetNextValue(getter(typedOwner), lastValue, count, maxCount, easing);
			setter(typedOwner, v);

			if(count >= maxCount)
			{
				callback?.Invoke();
				Vanish();
			}
		}
	}
}
