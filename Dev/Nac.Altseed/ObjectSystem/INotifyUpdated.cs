using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Altseed.ObjectSystem
{
	/// <summary>
	/// 更新されたときにイベントを発行するインターフェース。
	/// </summary>
	public interface INotifyUpdated
	{
		/// <summary>
		/// このオブジェクトシステムのインスタンスが更新されたときに値が流れるイベント。
		/// </summary>
		IObservable<float> OnUpdateEvent { get; }
	}
}
