using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altseed.Reactive.Easings.Library
{
	/// <summary>
	/// 終点においてバウンドするアニメーションを提供します。
	/// </summary>
	public class BounceEasing : IEasing
	{
		private float speed;
		private float elasticity;

		/// <summary>
		/// パラメータを指定して、BounceEasing の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="speed"></param>
		/// <param name="elasticity"></param>
		public BounceEasing(float speed, float elasticity)
		{
			if (elasticity <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(elasticity), "弾性係数は0より大きい必要があります。");
			}
			this.speed = speed;
			this.elasticity = elasticity;
		}

		/// <summary>
		/// 指定した時間における0~1のイージングの値を取得します。
		/// </summary>
		/// <param name="time">イージング中の地点を表す時間。0~1である必要があります。</param>
		/// <returns>指定した時間における0~1のイージングの値。</returns>
		public float GetNormalValue(float time)
		{
			return 1 - (float)Math.Abs(Math.Cos(time * speed)) * (float)Math.Pow(1 - time, 1 / elasticity);
		}
	}
}
