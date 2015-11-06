using asd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Altseed.Components
{
	class ShortWiggleComponent : Object2DComponent
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
			var x = (float)Math.Sin(timeCount / Math.PI * frequency.X) * amplitude.X * (time - timeCount);
			var y = (float)Math.Sin(timeCount / Math.PI * frequency.Y) * amplitude.Y * (time - timeCount);
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
