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
		protected float Time { get; set; }
		protected Action Callback { get; private set; }

		public TimerComponent(float time, Action callback)
		{
			this.Time = time;
			this.Callback = callback;
		}

		protected override void OnUpdate()
		{
			Time -= Engine.DeltaTime;
			if(Time <= 0)
			{
				Callback?.Invoke();
				Vanish();
			}
		}
	}
}
