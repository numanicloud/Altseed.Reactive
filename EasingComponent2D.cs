using asd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Altseed
{
	public class EasingComponent2D : Object2DComponent
	{
		int count, maxCount;
		Vector2DF lastValue;
		float[] easing;
		private Action callback;

		public EasingComponent2D(Vector2DF goal, EasingStart start, EasingEnd end, int count, Action callback)
		{
			easing = Easing.GetEasingFunction(start, end);
			lastValue = goal;
			maxCount = count;
			count = 0;
			this.callback = callback;
		}

		protected override void OnUpdate()
		{
			++count;
			Owner.Position = Easing.GetNextValue(Owner.Position, lastValue, count, maxCount, easing);
			if (count >= maxCount)
			{
				callback?.Invoke();
				Vanish();
			}
		}
	}
}
