using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Altseed.Easings
{
	/// <summary>
	/// イージングによってアニメーションする値を取得する機能を公開します。
	/// </summary>
	public interface IEasing
	{
		/// <summary>
		/// 指定した時間における0~1のイージングの値を取得します。
		/// </summary>
		/// <param name="time">イージング中の地点を表す時間。0~1である必要があります。</param>
		/// <returns></returns>
		float GetNormalValue(float time);
	}
}
