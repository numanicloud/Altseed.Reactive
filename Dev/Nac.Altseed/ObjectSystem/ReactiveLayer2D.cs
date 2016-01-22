using System;
using System.Reactive;
using System.Reactive.Subjects;
using asd;

namespace Nac.Altseed.ObjectSystem
{
	/// <summary>
	/// 特定のタイミングでイベントを発行する2Dレイヤー。
	/// </summary>
	public class ReactiveLayer2D : Layer2D, INotifyUpdated
	{
		private Subject<float> onUpdatedEvent_ = new Subject<float>();
		private Subject<Unit> onVanisEvent_ = new Subject<Unit>();

		/// <summary>
		/// 更新されたときに発行されるイベント。破棄されたとき完了します。
		/// </summary>
		public IObservable<float> OnUpdateEvent => onUpdatedEvent_;
		/// <summary>
		/// 破棄されたときに発行されるイベント。発行されると同時に完了します。
		/// </summary>
		public IObservable<Unit> OnVanishEvent => onVanisEvent_;

		protected override void OnUpdated()
		{
			onUpdatedEvent_.OnNext(Engine.DeltaTime);
		}

		protected override void OnDispose()
		{
			onVanisEvent_.OnNext(Unit.Default);
			onUpdatedEvent_.OnCompleted();
			onVanisEvent_.OnCompleted();
		}
	}
}
