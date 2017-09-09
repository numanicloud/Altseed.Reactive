using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altseed.Reactive.Easings.Library
{
	/// <summary>
	/// 線形にアニメーションするイージングを提供します。
	/// </summary>
	public class LinearEasing : IEasing
	{
		/// <summary>
		/// 指定した時間における0~1のイージングの値を取得します。
		/// </summary>
		/// <param name="time">イージング中の地点を表す時間。0~1である必要があります。</param>
		/// <returns></returns>
		public float GetNormalValue(float time)
		{
			return time;
		}
	}
}
