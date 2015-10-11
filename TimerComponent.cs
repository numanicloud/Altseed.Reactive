using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed
{
	public class TimerComponent : Object2DComponent
	{ 
		private float time;
		private Action callback;

		public TimerComponent(float time, Action callback)
		{
			this.time = time;
			this.callback = callback;
		}

		protected override void OnUpdate()
		{
			time -= Engine.DeltaTime;
			if(time <= 0)
			{
				callback?.Invoke();
				Vanish();
			}
		}
	}
}
