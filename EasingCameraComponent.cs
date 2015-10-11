using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed
{
	public class EasingCameraComponent : Object2DComponent
	{
		int count, maxCount;
		Vector2DF lastValue;
		float[] easing;
		private Action callback;
		private CameraObject2D owner;

		public EasingCameraComponent(Vector2DF goal, EasingStart start, EasingEnd end, int count, Action callback)
		{
			easing = Easing.GetEasingFunction(start, end);
			lastValue = goal;
			maxCount = count;
			count = 0;
			this.callback = callback;
		}

		protected override void OnUpdate()
		{
			if(owner == null)
			{
				owner = Owner as CameraObject2D;
			}

			++count;
			var pos = Easing.GetNextValue(owner.Src.Position.ToFloat(), lastValue, count, maxCount, easing);
			owner.Src = new RectI((int)pos.X, (int)pos.Y, owner.Src.Width, owner.Src.Height);

			if(count >= maxCount)
			{
				callback?.Invoke();
				Vanish();
			}
		}
	}
}
