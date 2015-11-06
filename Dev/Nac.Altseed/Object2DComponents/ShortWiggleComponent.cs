using asd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Altseed.Object2DComponents
{
	public class ShortWiggleComponent : Object2DComponent
	{
		private Vector2DF amplitude;
		private Vector2DF frequency;
		private float time;
		private float timeCount;
		private Action callback;

		public Vector2DF Center;

		/// <summary>
		/// <see cref="ShortWiggleComponent"/>のインスタンスを生成します。
		/// </summary>
		/// <param name="center">オブジェクトの振動の中心点。</param>
		/// <param name="amplitude">振幅</param>
		/// <param name="time">振動が終わるまでの時間。</param>
		/// <param name="frequency">１秒ごとの周波数。</param>
		/// <param name="callback">振動が終わったときに呼ばれるコールバック。</param>
		public ShortWiggleComponent(Vector2DF center, Vector2DF amplitude, Vector2DF frequency, float time, Action callback = null)
		{
			this.Center = center;
			this.amplitude = amplitude;
			this.time = time;
			this.frequency = frequency;
			this.callback = callback;
			timeCount = 0;
		}

		protected override void OnUpdate()
		{
			double phaseT = timeCount * Math.PI * 2;
            float amplitudeT = 1 - (timeCount / time);
			var x = (float)Math.Sin(phaseT * frequency.X) * amplitude.X * amplitudeT;
			var y = (float)Math.Sin(phaseT * frequency.Y) * amplitude.Y * amplitudeT;
			Owner.Position = Center + new Vector2DF(x, y);

			timeCount += Engine.DeltaTime;
			if (timeCount >= time)
			{
				Owner.Position = Center;
				callback?.Invoke();
				Vanish();
			}
		}
	}
}
