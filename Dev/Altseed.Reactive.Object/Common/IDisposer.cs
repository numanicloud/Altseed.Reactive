using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altseed.Reactive.Object
{
	public interface IDisposer
	{
		/// <summary>
		/// このオブジェクトが破棄されるときに一緒に破棄されるインスタンスを設定します
		/// </summary>
		/// <param name="resource">このオブジェクトが破棄されるときに一緒に破棄されるインスタンス。</param>
		void AddDisposable(IDisposable resource);
	}
}
