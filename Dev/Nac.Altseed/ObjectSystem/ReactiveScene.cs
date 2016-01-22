using System;
using System.Reactive;
using System.Reactive.Subjects;
using asd;

namespace Nac.Altseed.ObjectSystem
{
	/// <summary>
	/// 特定のタイミングでイベントを発行するシーン。
	/// </summary>
	public class ReactiveScene : Scene, INotifyUpdated
	{
		private Subject<float> onUpdatedEvent_ = new Subject<float>();
		private Subject<Unit> onRegisteredEvent_ = new Subject<Unit>();

		/// <summary>
		/// 更新されたときに発行されるイベント。このシーンが破棄されると完了します。
		/// </summary>
		public IObservable<float> OnUpdateEvent => onUpdatedEvent_;
		/// このシーンがエンジンに登録されたときに発行されるイベント。このシーンが破棄されると完了します。
		/// </summary>
		public IObservable<Unit> OnRegisteredEvent => onRegisteredEvent_;

		protected override void OnRegistered()
		{
			onRegisteredEvent_.OnNext(Unit.Default);
		}

		protected override void OnUpdated()
		{
			onUpdatedEvent_.OnNext(Engine.DeltaTime);
		}

		protected override void OnDispose()
		{
			onUpdatedEvent_.OnCompleted();
			onRegisteredEvent_.OnCompleted();
		}
	}
}
