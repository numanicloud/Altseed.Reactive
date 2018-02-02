using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altseed.Reactive.Object
{
	public static class Helper
	{
		/// <summary>
		/// この IDisposable を、指定したインスタンスが破棄されるときに破棄するよう設定します。
		/// </summary>
		/// <param name="disposable">破棄する IDisposable のインスタンス。</param>
		/// <param name="disposer">IDisposable のインスタンスを紐付ける IDisposer。</param>
		public static void AddTo(this IDisposable disposable, IDisposer disposer)
		{
			disposer.AddDisposable(disposable);
		}
	}
}
