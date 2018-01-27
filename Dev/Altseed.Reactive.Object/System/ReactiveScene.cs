using System;
using System.Reactive;
using System.Reactive.Subjects;
using asd;
using Altseed.Reactive.Helper;
using System.Reactive.Disposables;

namespace Altseed.Reactive.Object
{
	/// <summary>
	/// 特定のタイミングでイベントを発行するシーン。
	/// </summary>
	public class ReactiveScene : Scene, INotifyUpdated, IDisposer
	{
		private Subject<Unit> onRegisteredEvent_ = new Subject<Unit>();
		private Subject<Unit> onStartUpdating_ = new Subject<Unit>();
		private Subject<Unit> onTransitionFinished_ = new Subject<Unit>();
		private Subject<Unit> onTransitionBegin_ = new Subject<Unit>();
		private Subject<Unit> onStopUpdating_ = new Subject<Unit>();
		private Subject<Unit> onUnregistered_ = new Subject<Unit>();
		private Subject<long> onUpdatedEvent_ = new Subject<long>();
		private Subject<long> onUpdatingEvent_ = new Subject<long>();
		private Subject<Unit> onDisposeEvent_ = new Subject<Unit>();
		private CompositeDisposable disposable = new CompositeDisposable();

		/// <summary>
		/// シーン遷移にて、このシーンがエンジンに登録されたタイミングで発行されるイベント。このシーンが破棄されると完了します。
		/// </summary>
		public IObservable<Unit> OnRegisteredEvent => onRegisteredEvent_;
		/// <summary>
		/// シーン遷移にて、このシーンの更新が始まるタイミングで発行されるイベント。このシーンが破棄されると完了します。
		/// </summary>
		public IObservable<Unit> OnStartUpdatingEvent => onStartUpdating_;
		/// <summary>
		/// シーン遷移にて、このシーンへの遷移が完了したタイミングで発行されるイベント。このシーンが破棄されると完了します。
		/// </summary>
		public IObservable<Unit> OnTransitionFinishedEvent => onTransitionFinished_;
		/// <summary>
		/// シーン遷移にて、このシーンからの遷移が開始したタイミングで発行されるイベント。このシーンが破棄されると完了します。
		/// </summary>
		public IObservable<Unit> OnTransitionBeginEvent => onTransitionBegin_;
		/// <summary>
		/// シーン遷移にて、このシーンの更新が止まるタイミングで発行されるイベント。このシーンが破棄されると完了します。
		/// </summary>
		public IObservable<Unit> OnStopUpdatingEvent => onStopUpdating_;
		/// <summary>
		/// シーン遷移にて、このシーンがエンジンから登録解除されたタイミングで発行されるイベント。このシーンが破棄されると完了します。
		/// </summary>
		public IObservable<Unit> OnUnregisteredEvent => onUnregistered_;
		/// <summary>
		/// 更新された直後に発行されるイベント。このシーンが破棄されると完了します。
		/// 値として何個目のイベントかのインデックスが流れます。
		/// </summary>
		public IObservable<long> OnUpdateEvent => onUpdatedEvent_.Total();
		/// <summary>
		/// 更新される直前に発行されるイベント。このシーンが破棄されると完了します。
		/// 値として何個目のイベントかのインデックスが流れます。
		/// </summary>
		public IObservable<long> OnUpdatingEvent => onUpdatingEvent_.Total();
		/// <summary>
		/// このシーンが破棄されたときに発行されるイベント。発行されると同時に完了します。
		/// </summary>
		public IObservable<Unit> OnDisposeEvent => onDisposeEvent_;

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
			onUpdatedEvent_.OnNext(1);
		}

		protected override void OnUpdating()
		{
			onUpdatingEvent_.OnNext(1);
		}

		protected override void OnDispose()
		{
			onDisposeEvent_.OnNext(Unit.Default);
			onRegisteredEvent_.OnCompleted();
			onStartUpdating_.OnCompleted();
			onTransitionFinished_.OnCompleted();
			onTransitionBegin_.OnCompleted();
			onStopUpdating_.OnCompleted();
			onUnregistered_.OnCompleted();
			onUpdatedEvent_.OnCompleted();
			onUpdatingEvent_.OnCompleted();
			onDisposeEvent_.OnCompleted();
		}

		/// <summary>
		/// このオブジェクトが破棄されるときに一緒に破棄されるインスタンスを設定します
		/// </summary>
		/// <param name="resource">このオブジェクトが破棄されるときに一緒に破棄されるインスタンス。</param>
		public void AddDisposable(IDisposable resource)
		{
			disposable.Add(resource);
		}
	}
}
