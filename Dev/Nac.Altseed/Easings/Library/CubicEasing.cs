using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Altseed.Easings.Library
{
	/// <summary>
	/// 三次曲線によるイージングを提供します。
	/// </summary>
	public class CubicEasing : IEasing
	{
		/// <summary>
		/// イージングの開始・終了地点での速さを指定するための列挙体。
		/// </summary>
		public enum Speed
		{
			Slowly4 = 5,
			Slowly3 = 15,
			Slowly2 = 25,
			Slowly1 = 35,
			Linear = 45,
			Rapidly1 = 55,
			Rapidly2 = 65,
			Rapidly3 = 75,
			Rapidly4 = 85
		}

		// 三次曲線の係数
		private float a, b, c;

		/// <summary>
		/// 始点と終点の速度を指定して、三次曲線によるイージングを初期化します。
		/// </summary>
		/// <param name="startSpeed">アニメーション始点の速度。</param>
		/// <param name="endSpeed">アニメーション終点の速度。</param>
		public CubicEasing(Speed startSpeed, Speed endSpeed)
		{
			float Tan(Speed speed) => (float)Math.Tan((double)speed / 180 * Math.PI);
			var tan1 = Tan(startSpeed);
			var tan2 = Tan(endSpeed);

			a = tan1 + tan2 - 2;
			b = 1 - tan1 - a;
			c = tan1;
		}

		/// <summary>
		/// 指定した時間における0~1のイージングの値を取得します。
		/// </summary>
		/// <param name="time">イージング中の地点を表す時間。</param>
		/// <returns></returns>
		public float GetNormalValue(float time)
		{
			var t2 = time * time;
			var t3 = time * t2;
			return a * t3 + b * t2 + c * time;
		}
	}
}
