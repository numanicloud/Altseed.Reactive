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
		private Subject<Unit> onRegisteredEvent_ = new Subject<Unit>();
		private Subject<Unit> onStartUpdating_ = new Subject<Unit>();
		private Subject<Unit> onTransitionFinished_ = new Subject<Unit>();
		private Subject<Unit> onTransitionBegin_ = new Subject<Unit>();
		private Subject<Unit> onStopUpdating_ = new Subject<Unit>();
		private Subject<Unit> onUnregistered_ = new Subject<Unit>();
		private Subject<float> onUpdatedEvent_ = new Subject<float>();

		/// <summary>
		/// このシーンがエンジンに登録されたときに発行されるイベント。このシーンが破棄されると完了します。
		/// </summary>
		public IObservable<Unit> OnRegisteredEvent => onRegisteredEvent_;
		public IObservable<Unit> OnStartUpdatingEvent => onStartUpdating_;
		public IObservable<Unit> OnTransitionFinishedEvent => onTransitionFinished_;
		public IObservable<Unit> OnTransitionBeginEvent => onTransitionBegin_;
		public IObservable<Unit> OnStopUpdatingEvent => onStopUpdating_;
		public IObservable<Unit> OnUnregisteredEvent => onUnregistered_;
		/// <summary>
		/// 更新されたときに発行されるイベント。このシーンが破棄されると完了します。
		/// </summary>
		public IObservable<float> OnUpdateEvent => onUpdatedEvent_;

		protected override void OnRegistered()
		{
			onRegisteredEvent_.OnNext(Unit.Default);
		}

		protected override void OnStartUpdating()
		{
			onStartUpdating_.OnNext(Unit.Default);
		}

		protected override void OnTransitionFinished()
		{
			onTransitionFinished_.OnNext(Unit.Default);
		}

		protected override void OnTransitionBegin()
		{
			onTransitionBegin_.OnNext(Unit.Default);
		}

		protected override void OnStopUpdating()
		{
			onStopUpdating_.OnNext(Unit.Default);
		}

		protected override void OnUnregistered()
		{
			onUnregistered_.OnNext(Unit.Default);
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
