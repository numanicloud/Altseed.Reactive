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
		private Subject<Unit> onUpdatedForTheFirstTimeEvent_ = new Subject<Unit>();

		/// <summary>
		/// 更新されたときに発行されるイベント。他のシーンに移るときに完了します。
		/// </summary>
		public IObservable<float> OnUpdateEvent => onUpdatedEvent_;
		/// <summary>
		/// 最初に更新されたときに発行されるイベント。発行と同時に完了します。
		/// </summary>
		public IObservable<Unit> OnUpdatedForTheFirstTimeEvent => onUpdatedForTheFirstTimeEvent_;

		protected override void OnUpdateForTheFirstTime()
		{
			onUpdatedForTheFirstTimeEvent_.OnNext(Unit.Default);
			onUpdatedForTheFirstTimeEvent_.OnCompleted();
		}

		protected override void OnUpdated()
		{
			onUpdatedEvent_.OnNext(Engine.DeltaTime);
		}

		protected override void OnChanging()
		{
			onUpdatedEvent_.OnCompleted();
		}
	}
}
