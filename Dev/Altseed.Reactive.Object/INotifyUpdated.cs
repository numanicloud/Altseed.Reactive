using System;

namespace Altseed.Reactive.Object
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
